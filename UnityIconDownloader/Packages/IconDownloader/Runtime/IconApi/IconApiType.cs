using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IconDownloader.IconApi
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum IconApiType
	{
		IconFinder,
		FlatIcon,
	}
}