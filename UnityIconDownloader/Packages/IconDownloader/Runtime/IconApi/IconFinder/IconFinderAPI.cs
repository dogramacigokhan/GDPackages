using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UniRx;
using UnityEngine;

namespace IconDownloader.IconApi.IconFinder
{
	internal class IconFinderAPI : IIconAPI
	{
		private const string DesiredIconFormat = "png";

		private Dictionary<string,string> AuthenticationHeaders => new Dictionary<string, string>
		{
			["Authorization"] = $"Bearer {this.settings.GetApiKey(IconApiType.IconFinder)}",
		};

		private readonly IIconDownloaderSettings settings;

		public IconFinderAPI(IIconDownloaderSettings settings)
		{
			this.settings = settings;
		}

		public IObservable<IconPreview> SearchIcons(
			string searchTerm,
			IconSearchPreferences searchPreferences,
			int count)
		{
			if (!this.settings.EnabledApis[IconApiType.IconFinder])
			{
				return Observable.Empty<IconPreview>();
			}

			var encodedSearchTerm = WebUtility.UrlEncode(searchTerm);
			var styleFilter = searchPreferences.StrokeType switch
			{
				IconStrokeType.All => string.Empty,
				IconStrokeType.Filled => "&style=filled-outline",
				IconStrokeType.Linear => "&style=outline",
				_ => throw new ArgumentOutOfRangeException(nameof(searchPreferences.StrokeType), searchPreferences.StrokeType, null)
			};

			var premiumValue = searchPreferences.PremiumType switch
			{
				IconPremiumType.Free => "false",
				IconPremiumType.Premium => "true",
				IconPremiumType.All => "all",
				_ => throw new ArgumentOutOfRangeException(nameof(searchPreferences.PremiumType), searchPreferences.PremiumType, null)
			};

			var query = $"query={encodedSearchTerm}{styleFilter}&premium={premiumValue}&count={count}";
			var searchUrl = $"https://api.iconfinder.com/v4/icons/search?{query}";

			return ObservableWebRequest
				.GetObject<IconFinderSearchResponse>(searchUrl, this.AuthenticationHeaders)
				.CatchIgnore((Exception error) => Debug.LogWarning($"IconFinder search failed: {error}"))
				.SelectMany(searchResponse => searchResponse
					.Icons
					.Select(icon => this.GetIconPreview(searchTerm, icon)))
				.Merge();
		}

		private IObservable<IconPreview> GetIconPreview(string searchTerm, Icon icon)
		{
			 var formatsBySize = icon
				 .RasterSizes
				 .Select(rasterSize => (rasterSize.Size, format: rasterSize.Formats.FirstOrDefault(f => f.Format == DesiredIconFormat)))
				 .Where(pair => pair.format != null)
				 .OrderBy(pair => pair.Size)
				 .ToDictionary(pair => pair.Size, pair => pair.format);

			 var previewUrl = formatsBySize.Last().Value.PreviewUrl;
			 var texturesBySize = formatsBySize
				 .ToDictionary(
					  pair => pair.Key,
					  pair => ObservableWebRequest.GetTexture(
						  pair.Value.DownloadUrl,
						  this.AuthenticationHeaders));

			 var iconDetailsUrl = $"https://api.iconfinder.com/v4/icons/{icon.IconId}";
			 var iconDataObservable = ObservableWebRequest
				 .GetObject<IconDetails>(iconDetailsUrl, this.AuthenticationHeaders)
				 .Select(iconDetails =>
				 {
					  var author =
						  iconDetails.Iconset?.Author?.Company
						  ?? iconDetails.Iconset?.Author?.Name
						  ?? string.Empty;

					  var authorWebsiteFallback = !string.IsNullOrEmpty(iconDetails.Iconset?.Author?.Username)
						  ? $"https://www.iconfinder.com/{iconDetails.Iconset.Author.Username}"
						  : string.Empty;

					  var authorWebsite =
						  iconDetails.Iconset?.WebsiteUrl
						  ?? authorWebsiteFallback;

					  return new IconData(
						  iconId: iconDetails.IconId.ToString(),
						  iconName: $"{searchTerm.Replace(' ', '-')}",
						  isPremium: iconDetails.IsPremium,
						  iconFormat: DesiredIconFormat,
						  licenseName: iconDetails.Iconset?.License?.Name ?? string.Empty,
						  licenseUrl: iconDetails.Iconset?.License?.Url ?? string.Empty,
						  author: author,
						  authorWebsite: authorWebsite,
						  iconApi: IconApiType.IconFinder,
						  iconApiWebsite: "https://www.iconfinder.com/",
						  iconUrl: $"https://www.iconfinder.com/icons/{iconDetails.IconId}",
						  previewUrl: previewUrl);
				 });

			 return Observable.Return(new IconPreview(
				 source: IconApiType.IconFinder,
				 iconId: icon.IconId.ToString(),
				 isPremium: icon.IsPremium,
				 previewTextureUrl: previewUrl,
				 iconDataObservable,
				 texturesBySize));
		}
	}
}