using IconDownloader.IconApi;

namespace IconDownloader
{
	public class IconSelectionResult
	{
		public readonly ResultType Type;
		public readonly IconPreview SelectedIcon;
		public readonly string SearchTerm;
		public readonly IconSearchPreferences SearchPreferences;

		public static IconSelectionResult IconSelected(IconPreview selectedIcon) =>
			new(
				ResultType.IconSelected,
				selectedIcon,
				searchTerm: string.Empty,
				searchPreferences: null);

		public static IconSelectionResult SearchRefreshed(string searchTerm, IconSearchPreferences searchPreferences) =>
			new(
				ResultType.SearchRefreshed,
				selectedIcon: null,
				searchTerm,
				searchPreferences);

		public static IconSelectionResult RequestedMoreResult(string searchTerm, IconSearchPreferences searchPreferences)
		{
			searchPreferences.Offset += searchPreferences.Limit;
			return new IconSelectionResult(
				ResultType.RequestedMoreResult,
				selectedIcon: null,
				searchTerm,
				searchPreferences);
		}

		private IconSelectionResult(
			ResultType type,
			IconPreview selectedIcon,
			string searchTerm,
			IconSearchPreferences searchPreferences)
		{
			this.Type = type;
			this.SelectedIcon = selectedIcon;
			this.SearchTerm = searchTerm;
			this.SearchPreferences = searchPreferences;
		}

		public enum ResultType
		{
			IconSelected,
			SearchRefreshed,
			RequestedMoreResult,
		}
	}
}