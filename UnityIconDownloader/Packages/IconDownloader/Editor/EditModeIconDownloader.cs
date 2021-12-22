namespace IconDownloader.Editor
{
	public static class EditModeIconDownloader
	{
		private static readonly IconDownloadFlow instance;
		public static readonly IconDownloadFlow Instance = instance ??= new IconDownloadFlow(new IconDownloadEditorUI());
	}
}