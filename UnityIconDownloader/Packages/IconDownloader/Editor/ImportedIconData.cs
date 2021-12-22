using IconDownloader.IconApi;

namespace IconDownloader.Editor
{
	public class ImportedIconData
	{
		public readonly IconData IconData;
		public readonly string AssetPath;

		public ImportedIconData(IconData iconData, string assetPath)
		{
			this.IconData = iconData;
			this.AssetPath = assetPath;
		}
	}
}