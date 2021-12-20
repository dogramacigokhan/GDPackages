using System;
using System.IO;
using System.Linq;
using IconDownloader.IconApi;
using Newtonsoft.Json;
using UniRx;
using UnityEditor;
using UnityEngine;

namespace IconDownloader.Editor
{
	public static class IconImporter
	{
		public static readonly string DefaultSaveDirectory = Path.GetFullPath(Path.Combine(Application.dataPath, "Icons"));

		public static IObservable<ImportedIconData> ImportToProject(
			IconDownloadOptions downloadOptions,
			IIconDownloaderSettings settings)
		{
			if (downloadOptions.DownloadAsPreview && !settings.EnableDownloadingAsPreview)
			{
				return Observable.Empty<ImportedIconData>();
			}

			var texturesToImport = Observable.Defer(() =>
				downloadOptions.DownloadAsPreview
					? downloadOptions.IconPreview.PreviewTexture
					: downloadOptions
						.DesiredSizeDownloads
						.Select(pair => pair.Value)
						.Merge());

			return downloadOptions
				.IconPreview
				.IconData
				.ContinueWith(iconData => texturesToImport.Select(texture => (texture, iconData)))
				.Select(pair =>
				{
					var (texture, iconData) = pair;
					var targetDirectory = new DirectoryInfo(downloadOptions.SavePath).ToString();
					Directory.CreateDirectory(targetDirectory);

					string ConstructTargetPath(string file) => Path.GetFullPath(Path.Combine(
						targetDirectory,
						$"{file}.{iconData.IconFormat}"));

					// Add _2, _3, etc. to the filename until there's no conflict
					var previewSuffix = downloadOptions.DownloadAsPreview ? "-Preview" : string.Empty;
					var iconName = $"{iconData.IconName}-{texture.width}{previewSuffix}";

					var targetPath = Enumerable.Range(1, 1000)
						.Select(index =>
						{
							iconName = index == 1 ? $"{iconName}" : $"{iconName}_{index}";
							return iconName;
						})
						.Select(_ => ConstructTargetPath(iconName))
						.SkipWhile(File.Exists)
						.First();

					// Write texture
					File.WriteAllBytes(
						targetPath,
						GetTextureBytes(iconData.IconFormat, texture));

					AssetDatabase.Refresh();
					
					var importedIcon = new IconData(
						iconData.IconId,
						iconName,
						iconData.IsPremium,
						iconData.IconFormat,
						iconData.LicenseName,
						iconData.LicenseUrl,
						iconData.Author,
						iconData.AuthorWebsite,
						iconData.IconApi,
						iconData.IconApiWebsite,
						iconData.IconUrl,
						iconData.PreviewUrl);

					if (settings.DownloadLicenseData)
					{
						WriteInfoFile(targetPath, importedIcon);
					}
					
					AssetDatabase.Refresh();
					
					// Convert the absolute path to a relative path starting from "Assets/"
					var assetPath = Path.Combine(
						"Assets",
						targetPath.Split(new[] { "Assets" }, StringSplitOptions.None)[1].Substring(1));
					
					return new ImportedIconData(importedIcon, assetPath);
				});
		}

		private static void WriteInfoFile(string imagePath, IconData importedIconData)
		{
			File.WriteAllText(
				imagePath.Replace($".{importedIconData.IconFormat}", "-License.txt"),
				JsonConvert.SerializeObject(importedIconData, Formatting.Indented));
		}

		private static byte[] GetTextureBytes(string iconFormat, Texture2D texture) =>
			iconFormat switch
			{
				"png" => texture.EncodeToPNG(),
				_ => throw new ArgumentOutOfRangeException(nameof(iconFormat), iconFormat, "Unexpected icon format"),
			};
	}
}