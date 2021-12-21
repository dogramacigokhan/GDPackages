namespace IconDownloader.Editor
{
	public class EditModeIconDownloader
	{
		private static readonly IconDownloadFlow instance;
		public static IconDownloadFlow Instance = instance ??= new IconDownloadFlow(new IconDownloadEditorUI());
	}
}