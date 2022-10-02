using System;
using System.Collections.Generic;
using IconDownloader.Editor.Layout;
using IconDownloader.IconApi;
using UniRx;
using UnityEditor;
using UnityEngine;

namespace IconDownloader.Editor
{
    public class IconSelectionEditorWindow : EditorWindow
    {
        private const int ImageSize = 64;

        private readonly SerialDisposable iconPreviewDisposable = new();
        private List<IconPreview> iconPreviews = new();

        private string searchTerm;
        private IconSearchPreferences iconSearchPreferences;

        private Subject<IconSelectionResult> selectionResult;
        private Vector2 scrollPos;
        private Texture premiumIcon;
        private bool downloadInProgress;
        private SerialDisposable searchDisposable;

        public IObservable<IconSelectionResult> SetIconSource(
            IObservable<IconPreview> iconPreviewSource,
            string term,
            IconSearchPreferences searchPreferences,
            bool clearPreviousResult)
        {
            if (clearPreviousResult)
            {
                this.iconPreviews = new List<IconPreview>();
            }

            this.iconPreviewDisposable.Disposable = iconPreviewSource
                .DoOnSubscribe(() => this.downloadInProgress = true)
                .SelectMany(preview => preview.LoadPreviewTexture().Select(_ => preview))
                .DoOnCancel(() => this.downloadInProgress = false)
                .DoOnTerminate(() => this.downloadInProgress = false)
                .Subscribe(preview => this.iconPreviews.Add(preview));

            this.searchTerm = term;
            this.iconSearchPreferences = searchPreferences ?? IconSearchPreferences.FromCache;

            this.Show();

            this.selectionResult?.Dispose();
            this.selectionResult = new Subject<IconSelectionResult>();

            return this.selectionResult;
        }

        private void OnEnable()
        {
            this.premiumIcon = Resources.Load<Texture>("IconDownloaderPremiumIcon");
            this.searchDisposable = new SerialDisposable();
        }

        private void OnDisable()
        {
            this.iconSearchPreferences?.Cache();
            this.iconSearchPreferences = null;
            this.searchDisposable.Dispose();
        }

        public void OnInspectorUpdate()
        {
            if (this.downloadInProgress)
            {
                this.Repaint();
            }
        }

        private void OnGUI()
        {
            if (this.iconSearchPreferences == null)
            {
                return;
            }

            EditorGUILayout.Space(7);
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("Search Term:");
			this.searchTerm = EditorGUILayout.TextField(this.searchTerm);

            GUILayout.Label("Premium:");
			this.iconSearchPreferences.PremiumType = (IconPremiumType)EditorGUILayout.EnumPopup(this.iconSearchPreferences.PremiumType);

            GUILayout.Label("Stroke:");
			this.iconSearchPreferences.StrokeType = (IconStrokeType)EditorGUILayout.EnumPopup(this.iconSearchPreferences.StrokeType);

            GUILayout.Label("Color:");
            this.iconSearchPreferences.ColorType = (IconColorType)EditorGUILayout.EnumPopup(this.iconSearchPreferences.ColorType);

            GUILayout.Label("Limit:");
            this.iconSearchPreferences.Limit = EditorGUILayout.IntField(this.iconSearchPreferences.Limit);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(3);

            if (GUILayout.Button("Refresh", GUILayout.Height(24)))
            {
                this.iconSearchPreferences.Cache();
                this.selectionResult?.OnNext(IconSelectionResult.SearchRefreshed(this.searchTerm, this.iconSearchPreferences));
            }

            if (this.downloadInProgress)
            {
                EditorGUILayout.Space(3);
                EditorGuiLayoutHelpers.DrawCenteredLabel("Loading Icons...");
            }

            EditorGuiLayoutHelpers.DrawHorizontalLine();
            this.scrollPos = EditorGUILayout.BeginScrollView(this.scrollPos);

            const int nodeWidth = 70;
            const int columnSpacing = 20;
            const int padding = 30;
            var columnCount = Mathf.Max((int)((this.position.width - padding) / (nodeWidth + columnSpacing)), 1);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(10);

            var grid = new EditorGridLayout(columnCount, rowSpacing: 20, columnSpacing);

            foreach (var iconPreview in this.iconPreviews)
            {
                grid.AddElement(() =>
                {
                    EditorGUILayout.BeginVertical();
                    EditorGuiLayoutHelpers.DrawTexture(iconPreview.PreviewTexture.Value, ImageSize);

                    var rect = EditorGUILayout.GetControlRect(false, 24, GUILayout.Width(nodeWidth));
                    var selectContent = iconPreview.IsPremium
                        ? new GUIContent("Select", this.premiumIcon, "Premium Icon")
                        : new GUIContent("Select");

                    if (GUI.Button(rect, selectContent))
                    {
                        this.selectionResult?.OnNext(IconSelectionResult.IconSelected(iconPreview));
                    }
                    EditorGUILayout.EndVertical();
                });
            }
            grid.Draw();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(6);

            if (!this.downloadInProgress)
            {
                if (GUILayout.Button("Load More", GUILayout.Height(24)))
                {
                    this.selectionResult?.OnNext(IconSelectionResult.RequestedMoreResult(this.searchTerm, this.iconSearchPreferences));
                }

                EditorGUILayout.Space(6);
            }

            EditorGUILayout.EndScrollView();
        }

        private void OnDestroy()
        {
            if (this.iconPreviews != null)
            {
                foreach (var iconPreview in this.iconPreviews)
                {
                    iconPreview.Dispose();
                }
            }

            this.iconPreviewDisposable.Dispose();
            this.selectionResult?.OnCompleted();
            this.selectionResult?.Dispose();
        }
    }
}