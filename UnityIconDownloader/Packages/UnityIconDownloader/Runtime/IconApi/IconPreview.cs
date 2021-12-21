using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IconDownloader.IconApi
{
	public class IconPreview : IDisposable
	{
		public readonly IconApiType Source;
		public readonly string IconId;
		public readonly bool IsPremium;
		public readonly IObservable<IconData> IconData;
		public readonly Dictionary<int, IObservable<Texture2D>> TexturesBySize;
		private readonly string previewTextureUrl;

		private readonly ReactiveProperty<Texture2D> previewTexture = new ReactiveProperty<Texture2D>(null);
		public IReadOnlyReactiveProperty<Texture2D> PreviewTexture => this.previewTexture;

		public IconPreview(
			IconApiType source,
			string iconId,
			bool isPremium,
			string previewTextureUrl,
			IObservable<IconData> iconData,
			Dictionary<int, IObservable<Texture2D>> texturesBySize)
		{
			this.Source = source;
			this.IconId = iconId;
			this.IsPremium = isPremium;
			this.previewTextureUrl = previewTextureUrl;
			this.IconData = iconData;
			this.TexturesBySize = texturesBySize;
		}

		public IObservable<Texture2D> LoadPreviewTexture() => this.previewTexture.Value != null
			? this.previewTexture
			: ObservableWebRequest
				.GetTexture(this.previewTextureUrl)
				.Do(texture => this.previewTexture.Value = texture);

		public void Dispose()
		{
			if (this.previewTexture.Value != null)
			{
				Object.DestroyImmediate(this.previewTexture.Value);
			}

			this.previewTexture.Dispose();
		}
	}
}