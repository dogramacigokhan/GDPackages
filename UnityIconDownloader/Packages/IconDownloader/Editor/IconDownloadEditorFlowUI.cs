using System;
using IconDownloader.IconApi;
using UniRx;
using UnityEditor;

namespace IconDownloader.Editor
{
	public class IconDownloadEditorFlowUI : IIconDownloadFlowUI
	{
		public IObservable<IconSelectionResult> ShowIconSelection(
			IObservable<IconPreview> iconSource,
			string searchTerm,
			IconSearchPreferences searchPreferences)
		{
			return EditorWindow
				.GetWindow<IconSelectionEditorWindow>()
				.SetIconSource(iconSource, searchTerm, searchPreferences);
		}

		public IObservable<IconDownloadOptions> ShowDownloadOptions(
			IObservable<IconPreview> iconSource,
			string defaultSavePath)
		{
			return iconSource
				.Take(1)
				.ContinueWith(iconData => EditorWindow
					.GetWindow<IconDownloadOptionsEditorWindow>()
					.SetIcon(iconData, defaultSavePath));
		}
	}
}