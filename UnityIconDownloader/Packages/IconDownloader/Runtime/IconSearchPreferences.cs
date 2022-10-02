using IconDownloader.IconApi;
using UnityEngine;

namespace IconDownloader
{
	public class IconSearchPreferences
	{
		private const int DefaultLimit = 100;

		private const string PremiumTypePrefKey = "IconDownloader_PremiumType";
		private const string StrokeTypePrefKey = "IconDownloader_StrokeType";
		private const string ColorTypePrefKey = "IconDownloader_ColorType";
		private const string LimitPrefKey = "IconDownloader_Limit";

		public IconPremiumType PremiumType { get; set; }
		public IconStrokeType StrokeType { get; set; }
		public IconColorType ColorType { get; set; }
		public int Limit { get; set; }
		public int Offset { get; set; }

		public IconSearchPreferences(
			IconPremiumType premiumType,
			IconStrokeType strokeType,
			IconColorType colorType,
			int limit,
			int offset = 0)
		{
			this.PremiumType = premiumType;
			this.StrokeType = strokeType;
			this.ColorType = colorType;
			this.Limit = limit;
			this.Offset = offset;
		}

		public static IconSearchPreferences Default => new(
			IconPremiumType.All,
			IconStrokeType.All,
			IconColorType.All,
			limit: DefaultLimit);

		public static IconSearchPreferences FromCache => new(
			(IconPremiumType) PlayerPrefs.GetInt(PremiumTypePrefKey, (int) IconPremiumType.All),
			(IconStrokeType) PlayerPrefs.GetInt(StrokeTypePrefKey, (int) IconStrokeType.All),
			(IconColorType) PlayerPrefs.GetInt(ColorTypePrefKey, (int) IconColorType.All),
			limit: PlayerPrefs.GetInt(LimitPrefKey, DefaultLimit));

		public void Cache()
		{
            PlayerPrefs.SetInt(PremiumTypePrefKey, (int) this.PremiumType);
            PlayerPrefs.SetInt(StrokeTypePrefKey, (int) this.StrokeType);
            PlayerPrefs.SetInt(ColorTypePrefKey, (int) this.ColorType);
            PlayerPrefs.SetInt(LimitPrefKey, this.Limit);
		}
	}
}