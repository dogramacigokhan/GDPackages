using System.Collections.Generic;

namespace IconDownloader.IconApi
{
	public static class ApiDetailsProvider
	{
		public static readonly Dictionary<IconApiType, ApiDetails> ApiDetailsMap =
			new Dictionary<IconApiType, ApiDetails>
			{
				[IconApiType.IconFinder] = new ApiDetails(
					apiWebsiteUrl: "https://www.iconfinder.com/api-solution",
					apiKeyName: "API Key",
					apiKeyDescription: "Create an app and use your API Key."),
				[IconApiType.FlatIcon] = new ApiDetails(
					apiWebsiteUrl: "https://api.flaticon.com/",
					apiKeyName: "API Key",
					apiKeyDescription: "Login to website and request an API key."),
			};

		public class ApiDetails
		{
			public readonly string ApiWebsiteUrl;
			public readonly string ApiKeyName;
			public readonly string ApiKeyDescription;

			public ApiDetails(
				string apiWebsiteUrl,
				string apiKeyName,
				string apiKeyDescription)
			{
				this.ApiWebsiteUrl = apiWebsiteUrl;
				this.ApiKeyName = apiKeyName;
				this.ApiKeyDescription = apiKeyDescription;
			}
		}
	}
}