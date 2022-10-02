using System;
using IconDownloader.IconApi;

namespace IconDownloader
{
	public static class ObservableUIExtensions
	{
		public static IObservable<IconSelectionResult> ShowIconSelectionUI(
			this IObservable<IconPreview> iconSource,
			IIconDownloadFlowUI iconDownloadFlowUI,
			string searchTerm,
			IconSearchPreferences searchPreferences,
			bool clearPreviousResult)
		{
			return iconDownloadFlowUI.ShowIconSelection(iconSource, searchTerm, searchPreferences, clearPreviousResult);
		}

		public static IObservable<IconDownloadOptions> ShowDownloadOptionsUI(
			this IObservable<IconPreview> iconSource,
			IIconDownloadFlowUI iconDownloadFlowUI,
			string defaultSavePath)
		{
			return iconDownloadFlowUI.ShowDownloadOptions(iconSource, defaultSavePath);
		}
	}
}