using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace IconDownloader.Editor.Extensions
{
	[CustomEditor(typeof(RawImage), true)]
	[CanEditMultipleObjects]
	public class RawImageEditor : UnityEditor.UI.RawImageEditor
	{
		private RawImage image;
		private string searchTerm;
		private IIconDownloaderSettings settings;
		
		private SerialDisposable iconDownloadDisposable;

		private void Awake()
		{
			this.image = this.target as RawImage;
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			this.settings = IconDownloaderSettings.FromResources;
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

			if (!this.settings.ShowOnRawImageEditor)
			{
				return;
			}

			EditorGUILayout.Space(5);
			EditorGUILayout.BeginHorizontal();
			
			this.searchTerm = EditorGUILayout.TextField("Search Icon", this.searchTerm);
			if (GUILayout.Button("Find"))
			{
				this.iconDownloadDisposable.Disposable = IconDownloadEditorFlow
					.DownloadAsTextureWithSelection(this.searchTerm, count: 100)
					.Subscribe(texture => this.image.texture = texture);
			}
			
			EditorGUILayout.EndHorizontal();
		}
	}
}