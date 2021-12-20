using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IconDownloader.IconApi.FlatIcon
{
	/// <summary>
	/// Converts default "images" response to a more C# friendly dictionary type.
	/// </summary>
	internal class FlatIconImageResponseConverter : JsonConverter
	{
		public override void WriteJson(
			JsonWriter writer,
			object value,
			JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object ReadJson(
			JsonReader reader,
			Type objectType,
			object existingValue,
			JsonSerializer serializer)
		{
			var jObject = JObject.Load(reader);
			var formats = new[] { "png", "svg" };

			return formats
				.Where(format => jObject.ContainsKey(format))
				.SelectMany(format => ((IEnumerable<KeyValuePair<string, JToken>>)jObject[format])
					.Select(kv => new FlatIconImage
					{
						Format = format,
						Size = int.Parse(kv.Key),
						PreviewUrl = kv.Value.ToString(),
					}))
				.ToList();
		}

		public override bool CanConvert(Type objectType) => true;
	}
}