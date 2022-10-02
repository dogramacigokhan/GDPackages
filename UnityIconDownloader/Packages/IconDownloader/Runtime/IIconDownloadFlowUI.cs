using System;
using IconDownloader.IconApi;

namespace IconDownloader
{
	public interface IIconDownloadFlowUI
	{
		IObservable<IconSelectionResult> ShowIconSelection(
			IObservable<IconPreview> iconSource,
			string searchTerm,
			IconSearchPreferences searchPreferences,
			bool clearPreviousResult);

		IObservable<IconDownloadOptions> ShowDownloadOptions(
			IObservable<IconPreview> iconSource,
			string defaultSavePath);
	}
}