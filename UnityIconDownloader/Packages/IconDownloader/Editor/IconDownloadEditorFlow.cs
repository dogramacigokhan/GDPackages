using System;
using UniRx;
using UnityEditor;
using UnityEngine;

namespace IconDownloader.Editor
{
	public static class IconDownloadEditorFlow
	{
		private static readonly IconDownloadFlow DownloadFlow = new IconDownloadFlow(new IconDownloadEditorFlowUI());

		public static IObservable<ImportedIconData> DownloadSingle(
			string searchTerm,
			IconSearchPreferences searchPref = null)
		{
			return DownloadFlow
				.DownloadSingleIcon(searchTerm, searchPref)
				.SelectMany(iconData =>
					IconImporter.ImportToProject(iconData, IconDownloaderSettings.FromResources));
		}

		public static IObservable<ImportedIconData> DownloadWithSelection(
			string searchTerm,
			IconSearchPreferences searchPref = null)
		{
			return DownloadFlow
				.DownloadWithSelection(searchTerm, searchPref)
				.SelectMany(iconData =>
					IconImporter.ImportToProject(iconData, IconDownloaderSettings.FromResources));
		}

		public static IObservable<Texture2D> DownloadAsTextureWithSelection(
			string searchTerm,
			IconSearchPreferences searchPref = null)
		{
			return DownloadWithSelection(searchTerm, searchPref)
				.Select(iconData => AssetDatabase.LoadAssetAtPath<Texture2D>(iconData.AssetPath));
		}

		public static IObservable<Sprite> DownloadAsSpriteWithSelection(
			string searchTerm,
			IconSearchPreferences searchPref = null)
		{
			return DownloadWithSelection(searchTerm, searchPref)
				.Select(iconData =>
				{
					var iconTextureImporter = (TextureImporter) AssetImporter.GetAtPath(iconData.AssetPath);
					if (iconTextureImporter.textureType != TextureImporterType.Sprite)
					{
						iconTextureImporter.textureType = TextureImporterType.Sprite;
						iconTextureImporter.SaveAndReimport();
					}

					return AssetDatabase.LoadAssetAtPath<Sprite>(iconData.AssetPath);
				});
		}
	}
}