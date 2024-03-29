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
		private readonly IIconDownloadFlowUI iconDownloadFlowUI;
		private readonly IIconDownloaderSettings settings;
		private readonly IReadOnlyList<IIconAPI> iconApis;

		public IconDownloadFlow(IIconDownloadFlowUI iconDownloadFlowUI)
		{
			this.iconDownloadFlowUI = iconDownloadFlowUI;
			this.settings = IconDownloaderSettings.FromResources;

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
			IconSearchPreferences searchPreferences = null)
		{
			var iconSearchPreferences = searchPreferences ?? IconSearchPreferences.FromCache;

			return this.iconApis
				.Select(iconApi => iconApi.SearchIcons(searchTerm, iconSearchPreferences))
				.Merge()
				.Take(1)
				.ContinueWith(iconPreview => iconPreview
					.LoadPreviewTexture()
					.Select(_ => iconPreview)
					.ShowDownloadOptionsUI(this.iconDownloadFlowUI, this.settings.DefaultSaveFolder)
					.DoOnCompleted(iconPreview.Dispose));
		}

		public IObservable<IconDownloadOptions> DownloadWithSelection(
			string searchTerm,
			IconSearchPreferences searchPreferences = null,
			bool clearPreviousResult = true)
		{
			var iconSearchPreferences = searchPreferences ?? IconSearchPreferences.FromCache;

			return this.iconApis
				.Select(iconApi => iconApi.SearchIcons(searchTerm, iconSearchPreferences))
				.Merge()
				.ShowIconSelectionUI(this.iconDownloadFlowUI, searchTerm, iconSearchPreferences, clearPreviousResult)
				.SelectMany(result =>
				{
					return result.Type switch
					{
						IconSelectionResult.ResultType.IconSelected => Observable.Return(result.SelectedIcon)
							.ShowDownloadOptionsUI(this.iconDownloadFlowUI, this.settings.DefaultSaveFolder),
						IconSelectionResult.ResultType.SearchRefreshed => this.DownloadWithSelection(
							result.SearchTerm,
							result.SearchPreferences,
							clearPreviousResult: true),
						IconSelectionResult.ResultType.RequestedMoreResult => this.DownloadWithSelection(
							result.SearchTerm,
							result.SearchPreferences,
							clearPreviousResult: false),
						_ => throw new ArgumentOutOfRangeException(nameof(result.Type))
					};
				});
		}
	}
}