using System;
using IconDownloader.IconApi;

namespace IconDownloader
{
	public static class ObservableUIExtensions
	{
		public static IObservable<IconSelectionResult> ShowIconSelectionUI(
			this IObservable<IconPreview> iconSource,
			IIconDownloadUI iconDownloadUI,
			string searchTerm,
			IconSearchPreferences searchPreferences)
		{
			return iconDownloadUI.ShowIconSelection(iconSource, searchTerm, searchPreferences);
		}

		public static IObservable<IconDownloadOptions> ShowDownloadOptionsUI(
			this IObservable<IconPreview> iconSource,
			IIconDownloadUI iconDownloadUI,
			string defaultSavePath)
		{
			return iconDownloadUI.ShowDownloadOptions(iconSource, defaultSavePath);
		}
	}
}