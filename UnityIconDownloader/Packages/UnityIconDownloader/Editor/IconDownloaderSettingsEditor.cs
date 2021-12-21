using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IconDownloader.IconApi;
using UnityEditor;
using UnityEngine;

namespace IconDownloader.Editor
{
	[CustomEditor(typeof(IconDownloaderSettings))]
	internal class IconDownloaderSettingsEditor : UnityEditor.Editor
	{
		internal const string StorePreviewImagesToggleText = "Store Preview Images";
		private const string PreviewImageDisclaimer =
			"DISCLAIMER: These are the preview images (usually with a watermark) for premium icons. " +
			"Downloading and/or using preview images may violate the Terms of Service of the API(s) " +
			"regardless of private/personal usage.\n" +
			"By enabling this option, you are accepting to download them at your own risk." +
			"The maintainer(s) of this tool cannot be held liable for copyright infringements.";

		private IconDownloaderSettings settings;
		private SerializedProperty defaultSaveFolder;
		private SerializedProperty apiKeyStoringStrategy;
		private SerializedProperty enableDownloadingAsPreview;
		private SerializedProperty downloadLicenseData;
		private SerializedProperty showOnImageEditor;
		private List<IconApiType> iconApiTypes;
		private Dictionary<IconApiType, bool> iconApiFoldStates;

		private void OnEnable()
		{
			this.settings = (IconDownloaderSettings) this.target;
			this.defaultSaveFolder = this.serializedObject.FindProperty(nameof(this.settings.defaultSaveFolder));
			this.apiKeyStoringStrategy = this.serializedObject.FindProperty(nameof(this.settings.apiKeyStoringStrategy));
			this.enableDownloadingAsPreview = this.serializedObject.FindProperty(nameof(this.settings.enableDownloadingAsPreview));
			this.downloadLicenseData = this.serializedObject.FindProperty(nameof(this.settings.downloadLicenseData));
			this.showOnImageEditor = this.serializedObject.FindProperty(nameof(this.settings.showOnImageEditor));

			this.iconApiTypes = Enum.GetValues(typeof(IconApiType)).Cast<IconApiType>().ToList();
			this.iconApiFoldStates = this.iconApiTypes.ToDictionary(apiType => apiType, _ => false);
		}

		[MenuItem("Tools/Icon Downloader/Settings", isValidateFunction: false, priority: 2)]
		public static void ShowSettings()
		{
			var settingsAsset = (IconDownloaderSettings) IconDownloaderSettings.FromResources;
			if (settingsAsset == null)
			{
				settingsAsset = CreateInstance<IconDownloaderSettings>();

				const string defaultAssetsDirectory = "Resources";
				var directory = Path.GetFullPath(Path.Combine(Application.dataPath, defaultAssetsDirectory));

				Directory.CreateDirectory(directory);
				var targetPath = $"Assets/{defaultAssetsDirectory}/{nameof(IconDownloaderSettings)}.asset";

				// Default values
				settingsAsset.defaultSaveFolder = IconImporter.DefaultSaveDirectory;
				settingsAsset.downloadLicenseData = true;
				settingsAsset.showOnImageEditor = true;

				AssetDatabase.CreateAsset(settingsAsset, targetPath);
				AssetDatabase.Refresh();
			}

			EditorUtility.FocusProjectWindow();
			Selection.activeObject = settingsAsset;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			var wordWrapPreviousValue = EditorStyles.textField.wordWrap;
			EditorStyles.textField.wordWrap = true;

			EditorGUILayout.LabelField("Icon Downloader Settings", EditorStyles.boldLabel);
			EditorGuiLayoutHelpers.DrawHorizontalLine();

			// Icon Providers
			foreach (var apiType in this.iconApiTypes)
			{
				var apiDetails = ApiDetailsProvider.ApiDetailsMap[apiType];

				this.iconApiFoldStates[apiType] = EditorGUILayout.BeginFoldoutHeaderGroup(
					this.iconApiFoldStates[apiType],
					$"{apiType}");

				if (!this.iconApiFoldStates[apiType])
				{
					EditorGUILayout.EndFoldoutHeaderGroup();
					continue;
				}

				EditorGUILayout.BeginVertical(EditorStyles.helpBox);

				// API Name
				EditorGUILayout.BeginHorizontal();

				var wasApiEnabled = this.settings.enabledApis.Contains(apiType);

				EditorGUILayout.BeginHorizontal();
				var enable = EditorGUILayout.ToggleLeft("Enable", wasApiEnabled);
				if (GUILayout.Button($"Get {apiDetails.ApiKeyName}"))
				{
					Application.OpenURL(apiDetails.ApiWebsiteUrl);
				}

				EditorGUILayout.EndHorizontal();

				if (enable && !wasApiEnabled)
				{
					this.settings.enabledApis.Add(apiType);
				}
				else if (!enable)
				{
					this.settings.enabledApis.Remove(apiType);
				}

				EditorGUILayout.EndHorizontal();

				// API Key
				GUI.enabled = this.settings.EnabledApis[apiType];

				var apiKey = EditorGUILayout.TextArea(
					this.settings.GetApiKey(apiType),
					GUILayout.Height(35));

				if (this.settings.ApiKeyStoringStrategy == ApiKeyStoringStrategy.SharedAsset)
				{
					var apiKeyDefinition = this.settings.serializedApiKeys.FirstOrDefault(k => k.ApiType == apiType);
					if (apiKeyDefinition == null)
					{
						this.settings.serializedApiKeys.Add(new IconDownloaderSettings.ApiKey
						{
							ApiType = apiType,
							Key = apiKey,
						});
					}
					else
					{
						apiKeyDefinition.Key = apiKey;
					}
				}
				else if (this.settings.ApiKeyStoringStrategy == ApiKeyStoringStrategy.PlayerPrefs)
				{
					this.settings.PlayerPrefsApiKeys[apiType] = apiKey;
				}

				GUI.enabled = true;

				// API Details and "Get API Key" button
				EditorGUILayout.LabelField(apiDetails.ApiKeyDescription);
				EditorGUILayout.EndFoldoutHeaderGroup();

				EditorGUILayout.EndVertical();
			}

			EditorGUILayout.Space(5);

			// Default save location
			EditorGUILayout.BeginVertical();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Default Save Folder", EditorStyles.boldLabel);

			if (GUILayout.Button("Browse"))
			{
				this.defaultSaveFolder.stringValue = EditorUtility.OpenFolderPanel(
					title: "Save Location",
					folder: this.defaultSaveFolder.stringValue,
					defaultName: string.Empty);
			}

			EditorGUILayout.EndHorizontal();

			this.defaultSaveFolder.stringValue = EditorGUILayout.TextArea(
				this.defaultSaveFolder.stringValue,
				GUILayout.Height(35));

			EditorGUILayout.EndVertical();
			EditorGUILayout.Space(5);
			
			// Integrations
			EditorGUILayout.LabelField("Integrations", EditorStyles.boldLabel);
			this.showOnImageEditor.boolValue = EditorGUILayout.ToggleLeft(
				"Image Component",
				this.showOnImageEditor.boolValue);
			
			EditorGUILayout.Space(5);

			// Other settings
			EditorGUILayout.LabelField("Other Settings", EditorStyles.boldLabel);

			this.apiKeyStoringStrategy.enumValueIndex = (int) (ApiKeyStoringStrategy) EditorGUILayout.EnumPopup(
				"Store API Keys In",
				(ApiKeyStoringStrategy) this.apiKeyStoringStrategy.enumValueIndex);

			var selectedStrategy = (ApiKeyStoringStrategy) this.apiKeyStoringStrategy.enumValueIndex;
			var apiStorageHelpMessage = selectedStrategy switch
			{
				ApiKeyStoringStrategy.SharedAsset =>
					"API Keys will be stored in this asset and will be shared between project members. " +
					"This is recommended if your project is in a private repository.",
				ApiKeyStoringStrategy.PlayerPrefs =>
					"API Keys will be stored in player prefs and will not be shared between computers. " +
					"This is recommended if your project is in a public repository.",
				_ => string.Empty,
			};

			EditorGUILayout.Space(5);
			EditorGUILayout.HelpBox(apiStorageHelpMessage, MessageType.Info);
			EditorGUILayout.Space(5);

			// Downloading preview images
			this.downloadLicenseData.boolValue = EditorGUILayout.ToggleLeft(
				"Download License Data",
				this.downloadLicenseData.boolValue);

			// Downloading preview images
			this.enableDownloadingAsPreview.boolValue = EditorGUILayout.ToggleLeft(
				StorePreviewImagesToggleText,
				this.enableDownloadingAsPreview.boolValue);

			if (this.enableDownloadingAsPreview.boolValue)
			{
				EditorGUILayout.HelpBox(PreviewImageDisclaimer, MessageType.Warning);
			}

			this.serializedObject.ApplyModifiedProperties();
			EditorStyles.textField.wordWrap = wordWrapPreviousValue;

			EditorGUILayout.Space(5);

			if (GUILayout.Button("Apply Changes", GUILayout.Height(25)))
			{
				this.settings.UpdatePlayerPrefs();
				this.settings.UpdateSerializedApiKeys();

				EditorUtility.SetDirty(this.settings);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
		}
	}
}