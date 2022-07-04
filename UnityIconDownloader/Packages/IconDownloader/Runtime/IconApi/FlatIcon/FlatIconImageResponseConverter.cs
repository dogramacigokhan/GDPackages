using System;
using System.Collections.Generic;
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
			var list = new List<FlatIconImage>();

			foreach (var pair in jObject)
			{
				if (int.TryParse(pair.Key, out var size))
				{
					list.Add(new FlatIconImage
					{
						Size = size,
						PreviewUrl = pair.Value?.ToString() ?? string.Empty,
					});
				}
			}

			return list;
		}

		public override bool CanConvert(Type objectType) => true;
	}
}