using System;
using IconDownloader.IconApi;
using UniRx;
using UnityEditor;
using UnityEngine;

namespace IconDownloader.Editor
{
	internal class IconDownloaderEditorWindow : EditorWindow
	{
		private const int MaxImageSize = 256;

		private SerialDisposable downloadDisposable;
		private IconDownloadFlow iconDownloadFlow;
		private IIconDownloaderSettings settings;

		private string searchTerm;
		private Texture2D iconTexture;
		private IconSearchPreferences searchPref;

		[MenuItem("Tools/Icon Downloader/Search", isValidateFunction: false, priority: 1)]
		private static void Init() => GetWindow<IconDownloaderEditorWindow>().Show();

		private void OnEnable()
		{
			this.downloadDisposable = new SerialDisposable();
			this.iconDownloadFlow = EditModeIconDownloader.Instance;
			this.settings = IconDownloaderSettings.FromResources;
            this.searchPref = IconSearchPreferences.FromCache;
		}

		private void OnDisable()
		{
			this.downloadDisposable.Dispose();
		}

		public void OnInspectorUpdate()
        {
            this.Repaint();
        }

		private void OnGUI()
		{
			this.searchTerm = EditorGUILayout.TextField("Search Term:", this.searchTerm);
			this.searchPref.PremiumType = (IconPremiumType)EditorGUILayout.EnumPopup("Premium", this.searchPref.PremiumType);
			this.searchPref.StrokeType = (IconStrokeType)EditorGUILayout.EnumPopup("Stroke", this.searchPref.StrokeType);
			this.searchPref.ColorType = (IconColorType)EditorGUILayout.EnumPopup("Color", this.searchPref.ColorType);

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Download Single Icon"))
			{
				this.downloadDisposable.Disposable = null;
				this.downloadDisposable.Disposable = this.iconDownloadFlow
					.DownloadSingleIcon(this.searchTerm, this.searchPref)
					.SelectMany(iconData => IconImporter.ImportToProject(iconData, this.settings))
					.SelectMany(icon => ObservableWebRequest.GetTexture(icon.IconData.PreviewUrl))
					.Subscribe(this.HandleDownloadResult, HandleDownloadError);
			}

			if (GUILayout.Button("Download With Selection"))
			{
				this.downloadDisposable.Disposable = null;
				this.downloadDisposable.Disposable = this.iconDownloadFlow
					.DownloadWithSelection(this.searchTerm, count: 100, this.searchPref)
					.SelectMany(iconData => IconImporter.ImportToProject(iconData, this.settings))
					.SelectMany(icon => ObservableWebRequest.GetTexture(icon.IconData.PreviewUrl))
					.Subscribe(this.HandleDownloadResult, HandleDownloadError);
			}
			EditorGUILayout.EndHorizontal();

			if (this.iconTexture != null)
			{
				EditorGuiLayoutHelpers.DrawTexture(
					this.iconTexture,
					Math.Min(this.iconTexture.width, MaxImageSize));
			}
		}

		private static void HandleDownloadError(Exception error)
		{
			Debug.LogError(error);
		}

		private void HandleDownloadResult(Texture2D downloadedTexture)
		{
			if (this.iconTexture != null)
			{
				DestroyImmediate(this.iconTexture);
			}

			this.iconTexture = downloadedTexture;
		}
	}
}