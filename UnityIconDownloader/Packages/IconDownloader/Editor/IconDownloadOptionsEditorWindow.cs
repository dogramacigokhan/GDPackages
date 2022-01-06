using System;
using System.Collections.Generic;
using System.Linq;
using IconDownloader.Editor.Layout;
using IconDownloader.IconApi;
using UniRx;
using UnityEditor;
using UnityEngine;

namespace IconDownloader.Editor
{
	public class IconDownloadOptionsEditorWindow : EditorWindow
	{
        private const int ImageHeight = 64;

        private Subject<IconDownloadOptions> optionsSubject;
        private IconPreview iconPreview;
        private string savePath;

        private Dictionary<int, bool> downloadSizeStatus;
        private IIconDownloaderSettings settings;

        private void OnEnable()
        {
	        this.settings = IconDownloaderSettings.FromResources;
	        if (this.settings == null)
	        {
		        Debug.LogError("Unable to load icon downloader settings!");
	        }
        }

        public IObservable<IconDownloadOptions> SetIcon(
	        IconPreview iconData,
	        string defaultSavePath)
        {
	        this.iconPreview = iconData;
	        this.savePath = defaultSavePath;

            this.Show();

            this.optionsSubject?.Dispose();
            this.optionsSubject = new Subject<IconDownloadOptions>();

            this.downloadSizeStatus = iconData
	            .TexturesBySize
	            .ToDictionary(pair => pair.Key, pair => false);

            return this.optionsSubject;
        }

        private void OnGUI()
        {
	        EditorGUILayout.Space(5);

	        // Icon preview
	        EditorGuiLayoutHelpers.DrawHorizontallyCentered(() =>
		        EditorGuiLayoutHelpers.DrawTexture(
			        this.iconPreview.PreviewTexture.Value,
			        ImageHeight));

	        // Source
	        EditorGuiLayoutHelpers.DrawCenteredLabel($"Source: {this.iconPreview.Source}");

	        // Premium Info
	        var premiumInfo = this.iconPreview.IsPremium ? "Premium" : "Free";
	        EditorGuiLayoutHelpers.DrawCenteredLabel($"Premium Icon: {premiumInfo}");

	        // Save location
	        EditorGUILayout.BeginHorizontal();
	        EditorGUILayout.LabelField("Save To:", GUILayout.Width(60));
	        this.savePath = EditorGUILayout.TextField(this.savePath);

	        if (GUILayout.Button("Browse", GUILayout.Width(60)))
	        {
		        this.savePath = EditorUtility.OpenFolderPanel(
			        title: "Save Location",
			        folder: this.savePath,
			        defaultName: string.Empty);
	        }
	        EditorGUILayout.EndHorizontal();

	        var downloadAsPreview = this.iconPreview.IsPremium;
	        if (!downloadAsPreview)
	        {
		        EditorGuiLayoutHelpers.DrawHorizontalLine();

		        // Size selection
		        EditorGuiLayoutHelpers.DrawCenteredLabel("Select Sizes");
		        var grid = new EditorGridLayout(columnCount: 4);
		        foreach (var downloadPair in this.iconPreview.TexturesBySize)
		        {
			        grid.AddElement(() => this.downloadSizeStatus[downloadPair.Key] = EditorGUILayout.ToggleLeft(
				        $"{downloadPair.Key}",
				        this.downloadSizeStatus[downloadPair.Key],
				        GUILayout.Width(50)));
		        }

		        grid.Draw(centered: true);
	        }

	        // Save button
	        EditorGuiLayoutHelpers.DrawHorizontalLine();
	        var buttonText = downloadAsPreview
		        ? "Download As Preview"
		        : "Download";

	        var canDownload = !downloadAsPreview || this.settings.EnableDownloadingAsPreview;
	        if (canDownload && GUILayout.Button(buttonText))
	        {
		        var desiredSizeDownloads = this.iconPreview
			        .TexturesBySize
			        .Where(pair => this.downloadSizeStatus[pair.Key])
			        .ToDictionary(pair => pair.Key, pair => pair.Value);

		        this.optionsSubject.OnNext(new IconDownloadOptions(
			        this.iconPreview,
			        this.savePath,
			        downloadAsPreview,
			        desiredSizeDownloads));
	        }

	        if (!canDownload)
	        {
		        EditorGUILayout.HelpBox(
			        $"This is a premium icon.\n{IconDownloaderSettingsEditor.StorePreviewImagesToggleText} option can be enabled to download the preview images.",
			        MessageType.Info);

		        if (GUILayout.Button("Open Settings"))
		        {
			        IconDownloaderSettingsEditor.ShowSettings();
			        this.Close();
		        }
	        }
        }

        private void OnDestroy()
        {
            this.optionsSubject?.OnCompleted();
            this.optionsSubject?.Dispose();
        }
	}
}