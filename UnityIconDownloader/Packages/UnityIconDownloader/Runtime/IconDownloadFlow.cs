using System;
using System.Collections.Generic;
using System.Linq;
using IconDownloader.IconApi;
using IconDownloader.IconApi.FlatIcon;
using IconDownloader.IconApi.IconFinder;
using UniRx;
using UnityEngine;

namespace IconDownloader
{
	public class IconDownloadFlow
	{
		private readonly IIconDownloadUI iconDownloadUi;
		private readonly IconDownloaderSettings settings;
		private readonly IReadOnlyList<IIconAPI> iconApis;

		public IconDownloadFlow(IIconDownloadUI iconDownloadUI)
		{
			this.iconDownloadUi = iconDownloadUI;
			
			this.settings = Resources.Load<IconDownloaderSettings>(nameof(IconDownloaderSettings));
			if (this.settings == null)
			{
				Debug.LogError("Create a settings asset first!");
			}

			this.iconApis = new IIconAPI[]
			{
				new IconFinderAPI(this.settings),
				new FlatIconAPI(this.settings),
			};
		}

		public IObservable<IconDownloadOptions> DownloadSingleIcon(
			string searchTerm,
			IconSearchPreferences searchPreferences)
		{
			return this.iconApis
				.Select(iconApi => iconApi.SearchIcons(searchTerm, searchPreferences, count: 1))
				.Merge()
				.Take(1)
				.ContinueWith(iconPreview => iconPreview.LoadPreviewTexture().Select(_ => iconPreview))
				.ShowDownloadOptionsUI(this.iconDownloadUi, this.settings.defaultSaveFolder);
		}

		public IObservable<IconDownloadOptions> DownloadWithSelection(
			string searchTerm,
			int count,
			IconSearchPreferences searchPreferences = null)
		{
			var iconSearchPreferences = searchPreferences ?? IconSearchPreferences.FromCache;

			return this.iconApis
				.Select(iconApi => iconApi.SearchIcons(searchTerm, iconSearchPreferences, count))
				.Merge()
				.ShowIconSelectionUI(this.iconDownloadUi, searchTerm, iconSearchPreferences)
				.SelectMany(result =>
				{
					return result.Type switch
					{
						IconSelectionResult.ResultType.IconSelected => Observable.Return(result.SelectedIcon)
							.ShowDownloadOptionsUI(this.iconDownloadUi, this.settings.defaultSaveFolder),
						IconSelectionResult.ResultType.SearchRefreshed => this.DownloadWithSelection(
							result.SearchTerm,
							count,
							result.SearchPreferences),
						_ => throw new ArgumentOutOfRangeException(nameof(result.Type))
					};
				});
		}
	}
}