using IconDownloader.IconApi;
using UnityEngine;

namespace IconDownloader
{
	public class IconSearchPreferences
	{
		private const string PremiumTypePrefKey = "IconDownloader_PremiumType";
		private const string StrokeTypePrefKey = "IconDownloader_StrokeType";
		private const string ColorTypePrefKey = "IconDownloader_ColorType";

		public IconPremiumType PremiumType { get; set; }
		public IconStrokeType StrokeType { get; set; }
		public IconColorType ColorType { get; set; }

		public IconSearchPreferences(IconPremiumType premiumType, IconStrokeType strokeType, IconColorType colorType)
		{
			this.PremiumType = premiumType;
			this.StrokeType = strokeType;
			this.ColorType = colorType;
		}

		public static IconSearchPreferences Default => new IconSearchPreferences(
			IconPremiumType.All,
			IconStrokeType.All,
			IconColorType.All);

		public static IconSearchPreferences FromCache => new IconSearchPreferences(
			(IconPremiumType) PlayerPrefs.GetInt(PremiumTypePrefKey, (int) IconPremiumType.All),
			(IconStrokeType) PlayerPrefs.GetInt(StrokeTypePrefKey, (int) IconStrokeType.All),
			(IconColorType) PlayerPrefs.GetInt(ColorTypePrefKey, (int) IconColorType.All));

		public void Cache()
		{
            PlayerPrefs.SetInt(PremiumTypePrefKey, (int) this.PremiumType);
            PlayerPrefs.SetInt(StrokeTypePrefKey, (int) this.StrokeType);
            PlayerPrefs.SetInt(ColorTypePrefKey, (int) this.ColorType);
		}
	}
}