using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using UniRx;
using UnityEngine;

namespace IconDownloader.IconApi.FlatIcon
{
	internal class FlatIconAPI : IIconAPI
	{
		private const string DesiredIconFormat = "png";

		private static long CurrentTimestamp => ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();

		private readonly IIconDownloaderSettings settings;
		private AuthenticationResponseData cachedAuthenticationResponse;

		private static Dictionary<string, string> AuthenticationHeaders(string token) => new Dictionary<string, string>
		{
			["Authorization"] = $"Bearer {token}"
		};

		public FlatIconAPI(IIconDownloaderSettings settings)
		{
			this.settings = settings;
		}

		private IObservable<string> GetAuthenticationToken()
		{
			if (this.cachedAuthenticationResponse?.Token != null &&
			    this.cachedAuthenticationResponse.Expires > CurrentTimestamp)
			{
				return Observable.Return(this.cachedAuthenticationResponse.Token);
			}

			var authenticationData = JsonConvert
				.SerializeObject(new { apikey = this.settings.GetApiKey(IconApiType.FlatIcon) });

			return ObservableWebRequest.PostObject<FlatIconAuthenticationResponse>(
					"https://api.flaticon.com/v2/app/authentication",
					authenticationData)
				.Select(response =>
				{
					this.cachedAuthenticationResponse = response.Data;
					return this.cachedAuthenticationResponse.Token;
				});
		}

		public IObservable<IconPreview> SearchIcons(
			string searchTerm,
			IconSearchPreferences searchPreferences,
			int count)
		{
			if (!this.settings.EnabledApis[IconApiType.FlatIcon])
			{
				return Observable.Empty<IconPreview>();
			}

			var encodedSearchTerm = WebUtility.UrlEncode(searchTerm);
			var strokeFilter = searchPreferences.StrokeType switch
			{
				IconStrokeType.All => string.Empty,
				IconStrokeType.Filled => "&stroke=1",
				IconStrokeType.Linear => "&stroke=0",
				_ => throw new ArgumentOutOfRangeException(nameof(searchPreferences.StrokeType), searchPreferences.StrokeType, null)
			};

			var colorFilter = searchPreferences.ColorType switch
			{
				IconColorType.All => string.Empty,
				IconColorType.Colored => "&color=2",
				IconColorType.Monocolor => "&color=1",
				_ => throw new ArgumentOutOfRangeException(nameof(searchPreferences.ColorType), searchPreferences.ColorType, null)
			};

			// In FlatIcon API limit=1 is processed as limit=101 for some reason, but limit=2 works as expected.
			// That's why we are using limit=2 instead of limit=1 to decrease search request duration.
			var limit = count == 1 ? 2 : count;

			var query = $"q={encodedSearchTerm}{strokeFilter}{colorFilter}&limit={limit}";
			var searchUrl = $"https://api.flaticon.com/v2/search/icons/priority?{query}";

			return this.GetAuthenticationToken()
				.ContinueWith(token => ObservableWebRequest.GetObject<FlatIconSearchResponse>(
					searchUrl,
					AuthenticationHeaders(token)))
				.CatchIgnore((Exception error) => Debug.LogWarning($"Flaticon search failed: {error}"))
				.SelectMany(response => response
					.Data
					.Where(iconData =>
					{
						// Premium option is not available in the query,
						// so we are filtering out the response.
						return searchPreferences.PremiumType switch
						{
							IconPremiumType.All => true,
							IconPremiumType.Free => iconData.Premium == 0,
							IconPremiumType.Premium => iconData.Premium == 1,
							_ => throw new ArgumentOutOfRangeException(nameof(searchPreferences.PremiumType), searchPreferences.PremiumType, null),
						};
					})
					.Select(iconData => this.DownloadAsPreview(searchTerm, iconData)))
				.Merge();
		}

		private IObservable<IconPreview> DownloadAsPreview(string searchTerm, SearchResponseData iconData)
		{
			return Observable.Defer(() =>
			{
				var previewUrl = iconData.Images
					.OrderByDescending(image => image.Size)
					.First()
					.PreviewUrl;

				var texturesBySize = new[] { 16, 24, 32, 64, 128, 256, 512 }
					.Select(size => (size, url: $"https://api.flaticon.com/v2/item/icon/download/{iconData.Id}?size={size}"))
					.ToDictionary(
						pair => pair.size,
						pair => this.GetAuthenticationToken()
							.ContinueWith(token => ObservableWebRequest.GetTexture(pair.url, AuthenticationHeaders(token))));

				var rawIconName = !string.IsNullOrEmpty(iconData.Description)
					? iconData.Description
					: searchTerm;

				// Unfortunately there's no endpoint to get license details.
				// But currently, "2" means free-to-use.
				var licenseInfo = iconData.License == "2"
					? "Free"
					: iconData.License;

				return ObservableWebRequest.GetTexture(previewUrl)
					.CatchIgnore((Exception error) => Debug.LogWarning($"Flaticon downloading preview failed: {error}"))
					.Select(previewTexture =>
					{
						var icon = new IconData(
							iconId: iconData.Id.ToString(),
							iconName: $"{rawIconName.Replace(' ', '-')}",
							isPremium: iconData.Premium == 1,
							iconFormat: DesiredIconFormat,
							licenseName: licenseInfo,
							licenseUrl: $"https://www.flaticon.com/license/icon/{iconData.Id}",
							author: iconData.DesignerName ?? string.Empty,
							authorWebsite: iconData.DesignerWebsite ?? string.Empty,
							iconApi: IconApiType.FlatIcon,
							iconApiWebsite: "https://www.flaticon.com/",
							iconUrl: $"https://www.flaticon.com/{iconData.Slug}",
							previewUrl);

						return new IconPreview(
							source: IconApiType.FlatIcon,
							iconId: icon.IconId,
							isPremium: icon.IsPremium,
							previewTextureUrl: previewUrl,
							iconData: Observable.Return(icon),
							texturesBySize);
					});
			});
		}
	}
}