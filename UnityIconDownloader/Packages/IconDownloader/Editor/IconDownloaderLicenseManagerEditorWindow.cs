using System;
using System.IO;
using System.Linq;
using IconDownloader.IconApi;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace IconDownloader.Editor
{
	internal class IconDownloaderLicenseManagerEditorWindow : EditorWindow
	{
		private string licenseInfo;

		[MenuItem("Tools/Icon Downloader/Generate License Info", isValidateFunction: false, priority: 3)]
		private static void Init() => GetWindow<IconDownloaderLicenseManagerEditorWindow>().Show();

		private void OnEnable()
		{
			var rootFolder = Application.dataPath;
			var licenseFiles = Directory.GetFiles(
				rootFolder,
				"*-License.txt",
				SearchOption.AllDirectories);

			var licenseDetails = licenseFiles
				.Select(licenseFile =>
				{
					try
					{
						var fileContent = File.ReadAllText(licenseFile);
						return JsonConvert.DeserializeObject<IconData>(fileContent);
					}
					catch (Exception)
					{
						return null;
					}
				})
				.Where(data => data != null)
				.Select(data => string.Join(" | ", new[]
					{
						data.Author,
						data.AuthorWebsite,
						$"{data.IconApi} ({data.IconApiWebsite})",
					}
					.Where(val => !string.IsNullOrEmpty(val))))
				.Select(details => $"* {details}")
				.Distinct();

			var aggregatedLicense = string.Join("\n", licenseDetails);
			this.licenseInfo = $"Icons By:\n-------------\n{aggregatedLicense}";
		}

		private void OnGUI()
		{
			EditorGUILayout.LabelField("License info");
			EditorGUILayout.TextArea(this.licenseInfo, GUILayout.Height(300));

			if (GUILayout.Button("Copy To Clipboard"))
			{
				GUIUtility.systemCopyBuffer = this.licenseInfo;
			}
		}
	}
}