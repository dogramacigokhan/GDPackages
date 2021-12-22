using System;
using System.Collections.Generic;
using System.Linq;
using IconDownloader.IconApi;
using UnityEngine;

namespace IconDownloader
{
	public class IconDownloaderSettings : ScriptableObject, IIconDownloaderSettings
	{
		private const string ApiKeyPrefPrefix = "IconDownloaderAPIKey_";

		private static IconDownloaderSettings settings;
		public static IIconDownloaderSettings FromResources =>
			settings ??= Resources.Load<IconDownloaderSettings>(nameof(IconDownloaderSettings));

		[SerializeField] internal string defaultSaveFolder;
		[SerializeField] internal ApiKeyStoringStrategy apiKeyStoringStrategy;
		[SerializeField] internal bool enableDownloadingAsPreview;
		[SerializeField] internal bool downloadLicenseData;
		[SerializeField] internal bool showOnImageEditor;
		[SerializeField] internal bool showOnRawImageEditor;
		[SerializeField] internal List<IconApiType> enabledApis = new List<IconApiType>();
		[SerializeField] internal List<ApiKey> serializedApiKeys = new List<ApiKey>();
		internal Dictionary<IconApiType, string> PlayerPrefsApiKeys { get; private set; }

		public string DefaultSaveFolder => this.defaultSaveFolder;
		public ApiKeyStoringStrategy ApiKeyStoringStrategy => this.apiKeyStoringStrategy;
		public bool EnableDownloadingAsPreview => this.enableDownloadingAsPreview;
		public bool DownloadLicenseData => this.downloadLicenseData;
		public bool ShowOnImageEditor => this.showOnImageEditor;
		public bool ShowOnRawImageEditor => this.showOnRawImageEditor;

		public IReadOnlyDictionary<IconApiType, bool> EnabledApis => Enum.GetValues(typeof(IconApiType))
			.Cast<IconApiType>()
			.ToDictionary(apiType => apiType, apiType => this.enabledApis?.Contains(apiType) ?? false);

		private void OnEnable()
		{
			this.PlayerPrefsApiKeys = Enum.GetValues(typeof(IconApiType))
				.Cast<IconApiType>()
				.ToDictionary(
					apiType => apiType,
					apiType => PlayerPrefs.GetString($"{ApiKeyPrefPrefix}{apiType}", string.Empty));
		}

		public string GetApiKey(IconApiType apiType) => this.apiKeyStoringStrategy switch
		{
			ApiKeyStoringStrategy.SharedAsset => this.serializedApiKeys?.FirstOrDefault(k => k.ApiType == apiType)?.Key ?? string.Empty,
			ApiKeyStoringStrategy.PlayerPrefs => this.PlayerPrefsApiKeys[apiType],
			_ => throw new ArgumentOutOfRangeException(),
		};

		public void UpdatePlayerPrefs()
		{
			if (this.ApiKeyStoringStrategy == ApiKeyStoringStrategy.PlayerPrefs)
			{
				foreach (var pair in this.PlayerPrefsApiKeys)
				{
					PlayerPrefs.SetString($"{ApiKeyPrefPrefix}{pair.Key}", pair.Value);
				}
				PlayerPrefs.Save();
			}
		}

		public void UpdateSerializedApiKeys()
		{
			if (this.ApiKeyStoringStrategy == ApiKeyStoringStrategy.PlayerPrefs)
			{
				this.serializedApiKeys = new List<ApiKey>();
			}
		}

		[Serializable]
		public class ApiKey
		{
			public IconApiType ApiType;
			public string Key;
		}
	}
}