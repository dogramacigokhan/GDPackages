using IconDownloader.Editor;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace IconDownloader.Editor.Extensions
{
	[CustomEditor(typeof(Image), true)]
	[CanEditMultipleObjects]
	public class ImageEditor : UnityEditor.UI.ImageEditor
	{
		private Image image;
		private string searchTerm;
		private IconDownloadFlow iconDownloadFlow;
		private IconDownloaderSettings settings;
		
		private SerialDisposable iconDownloadDisposable;

		private void Awake()
		{
			this.image = this.target as Image;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			
			if (this.iconDownloadFlow == null)
			{
				this.iconDownloadFlow = new IconDownloadFlow(new IconDownloadEditorUI());
			}

			if (this.settings == null)
			{
				this.settings = Resources.Load<IconDownloaderSettings>(nameof(IconDownloaderSettings));
			}

			this.iconDownloadDisposable = new SerialDisposable();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			this.iconDownloadDisposable.Dispose();
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (!this.settings.ShowOnImageEditor)
			{
				return;
			}

			EditorGUILayout.Space(5);
			EditorGUILayout.BeginHorizontal();
			
			this.searchTerm = EditorGUILayout.TextField("Search Icon", this.searchTerm);
			if (GUILayout.Button("Find"))
			{
				this.iconDownloadDisposable.Disposable = this.iconDownloadFlow
					.DownloadWithSelection(this.searchTerm, count: 100)
					.SelectMany(iconData => IconImporter.ImportToProject(iconData, this.settings))
					.Select(iconData => AssetDatabase.LoadAssetAtPath<Sprite>(iconData.AssetPath))
					.Subscribe(sprite => this.image.sprite = sprite);
			}
			
			EditorGUILayout.EndHorizontal();
		}
	}
}