namespace IconDownloader.IconApi
{
	public class IconData
	{
		public readonly string IconId;
		public readonly string IconName;
		public readonly bool IsPremium;
		public readonly string IconFormat;
		public readonly string LicenseName;
		public readonly string LicenseUrl;
		public readonly string Author;
		public readonly string AuthorWebsite;
		public readonly IconApiType IconApi;
		public readonly string IconApiWebsite;
		public readonly string IconUrl;
		public readonly string PreviewUrl;

		public IconData(
			string iconId,
			string iconName,
			bool isPremium,
			string iconFormat,
			string licenseName,
			string licenseUrl,
			string author,
			string authorWebsite,
			IconApiType iconApi,
			string iconApiWebsite,
			string iconUrl,
			string previewUrl)
		{
			this.IconId = iconId;
			this.IconName = iconName;
			this.IsPremium = isPremium;
			this.IconFormat = iconFormat;
			this.LicenseName = licenseName;
			this.LicenseUrl = licenseUrl;
			this.Author = author;
			this.AuthorWebsite = authorWebsite;
			this.IconApi = iconApi;
			this.IconApiWebsite = iconApiWebsite;
			this.IconUrl = iconUrl;
			this.PreviewUrl = previewUrl;
		}
	}
}