using System;

namespace IconDownloader.IconApi
{
	public interface IIconAPI
	{
		IObservable<IconPreview> SearchIcons(
			string searchTerm,
			IconSearchPreferences searchPreferences,
			int count);
	}
}