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

		private readonly SerialDisposable downloadDisposable = new SerialDisposable();
		private IconDownloadFlow iconDownloadFlow;
		private IconDownloaderSettings settings;

		private string searchTerm;
		private Texture2D iconTexture;
		private bool searchInProgress;
		private IconSearchPreferences searchPref;

		[MenuItem("Tools/Icon Downloader/Search", isValidateFunction: false, priority: 1)]
		private static void Init() => GetWindow<IconDownloaderEditorWindow>().Show();

		private void OnEnable()
		{
			this.iconDownloadFlow = new IconDownloadFlow(new IconDownloadEditorUI());
			this.settings = Resources.Load<IconDownloaderSettings>(nameof(IconDownloaderSettings));
            this.searchPref = IconSearchPreferences.FromCache;
		}

		public void OnInspectorUpdate()
        {
            this.Repaint();
        }

		private void OnGUI()
		{
			GUI.enabled = !this.searchInProgress;

			this.searchTerm = EditorGUILayout.TextField("Search Term:", this.searchTerm);

			this.searchPref.PremiumType = (IconPremiumType)EditorGUILayout.EnumPopup("Premium", this.searchPref.PremiumType);
			this.searchPref.StrokeType = (IconStrokeType)EditorGUILayout.EnumPopup("Stroke", this.searchPref.StrokeType);
			this.searchPref.ColorType = (IconColorType)EditorGUILayout.EnumPopup("Color", this.searchPref.ColorType);

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Download Single Icon"))
			{
				this.downloadDisposable.Disposable = this.SetSearchInProgress(this.iconDownloadFlow
						.DownloadSingleIcon(this.searchTerm, this.searchPref))
					.SelectMany(iconData => IconImporter.ImportToProject(iconData, this.settings))
					.SelectMany(icon => ObservableWebRequest.GetTexture(icon.IconData.PreviewUrl))
					.Subscribe(this.HandleDownloadResult, HandleDownloadError);
			}

			if (GUILayout.Button("Download With Selection"))
			{
				this.downloadDisposable.Disposable = this.SetSearchInProgress(this.iconDownloadFlow
						.DownloadWithSelection(this.searchTerm, count: 100, this.searchPref))
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

		private IObservable<IconDownloadOptions> SetSearchInProgress(IObservable<IconDownloadOptions> downloadObservable)
		{
			return downloadObservable
				.DoOnSubscribe(() => this.searchInProgress = true)
				.DoOnCancel(() => this.searchInProgress = false)
				.DoOnTerminate(() => this.searchInProgress = false);
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