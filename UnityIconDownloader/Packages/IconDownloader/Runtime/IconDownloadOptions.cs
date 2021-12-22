using System;
using System.Collections.Generic;
using IconDownloader.IconApi;
using UnityEngine;

namespace IconDownloader
{
	public class IconDownloadOptions
	{
		public readonly IconPreview IconPreview;
		public readonly string SavePath;
		public readonly bool DownloadAsPreview;
		public readonly Dictionary<int, IObservable<Texture2D>> DesiredSizeDownloads;

		public IconDownloadOptions(
			IconPreview iconPreview,
			string savePath,
			bool downloadAsPreview,
			Dictionary<int, IObservable<Texture2D>> desiredSizeDownloads)
		{
			this.IconPreview = iconPreview;
			this.SavePath = savePath;
			this.DownloadAsPreview = downloadAsPreview;
			this.DesiredSizeDownloads = desiredSizeDownloads;
		}
	}
}