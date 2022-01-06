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

        private readonly SerialDisposable iconPreviewDisposable = new SerialDisposable();
        private List<IconPreview> iconPreviews = new List<IconPreview>();
        
        private string searchTerm;
        private IconSearchPreferences iconSearchPreferences;

        private Subject<IconSelectionResult> selectionResult;
        private Vector2 scrollPos;
        private Texture premiumIcon;
        private bool downloadInProgress;
        private SerialDisposable searchDisposable;

        public IObservable<IconSelectionResult> SetIconSource(
            IObservable<IconPreview> iconPreview,
            string searchTerm, 
            IconSearchPreferences searchPreferences)
        {
            this.iconPreviews = new List<IconPreview>();

            this.iconPreviewDisposable.Disposable = iconPreview
                .DoOnSubscribe(() => this.downloadInProgress = true)
                .SelectMany(preview => preview.LoadPreviewTexture().Select(_ => preview))
                .DoOnCancel(() => this.downloadInProgress = false)
                .DoOnTerminate(() => this.downloadInProgress = false)
                .Subscribe(preview => this.iconPreviews.Add(preview));
            
            this.searchTerm = searchTerm;
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
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(3);

            if (GUILayout.Button("Refresh"))
            {
                this.iconSearchPreferences.Cache();
                this.selectionResult?.OnNext(IconSelectionResult.SearchRefreshed(this.searchTerm, this.iconSearchPreferences));
            }

            EditorGuiLayoutHelpers.DrawHorizontalLine();
            this.scrollPos = EditorGUILayout.BeginScrollView(this.scrollPos);

            var grid = new EditorGridLayout(columnCount: 7, rowSpacing: 20, columnSpacing: 15);
            foreach (var iconPreview in this.iconPreviews)
            {
                grid.AddElement(() =>
                {
                    EditorGUILayout.BeginVertical();
                    EditorGuiLayoutHelpers.DrawTexture(iconPreview.PreviewTexture.Value, ImageSize);

                    var rect = EditorGUILayout.GetControlRect(false, 24, GUILayout.Width(70));
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