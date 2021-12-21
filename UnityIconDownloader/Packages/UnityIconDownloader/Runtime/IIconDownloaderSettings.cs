using System.Collections.Generic;
using IconDownloader.IconApi;

namespace IconDownloader
{
	public interface IIconDownloaderSettings
	{
		string DefaultSaveFolder { get; }
		ApiKeyStoringStrategy ApiKeyStoringStrategy { get; }
		bool EnableDownloadingAsPreview { get; }
		bool DownloadLicenseData { get; }
		bool ShowOnImageEditor { get; }
		IReadOnlyDictionary<IconApiType, bool> EnabledApis { get; }

		string GetApiKey(IconApiType apiType);
	}
}