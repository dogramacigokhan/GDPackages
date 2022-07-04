using System.Collections.Generic;
using Newtonsoft.Json;

namespace IconDownloader.IconApi.FlatIcon
{

	internal class FlatIconAuthenticationResponse
	{
		[JsonProperty("data")] public AuthenticationResponseData Data;
	}

	internal class AuthenticationResponseData
	{
		[JsonProperty("token")] public string Token;
		[JsonProperty("expires")] public int Expires;
	}

	internal class FlatIconImage
	{
		public int Size;
		public string PreviewUrl;
	}

	internal class SearchResponseData
	{
		[JsonProperty("id")] public int Id;
		[JsonProperty("description")] public string Description;
		[JsonProperty("colors")] public string Colors;
		[JsonProperty("color")] public string Color;
		[JsonProperty("shape")] public string Shape;
		[JsonProperty("family_id")] public int FamilyID;
		[JsonProperty("family_name")] public string FamilyName;
		[JsonProperty("team_name")] public string TeamName;
		[JsonProperty("added")] public long Added;
		[JsonProperty("pack_id")] public int PackID;
		[JsonProperty("pack_name")] public string PackName;
		[JsonProperty("pack_items")] public int PackItems;
		[JsonProperty("tags")] public string Tags;
		[JsonProperty("equivalents")] public int Equivalents;
		[JsonProperty("images")] [JsonConverter(typeof(FlatIconImageResponseConverter))] public List<FlatIconImage> Images;
	}

	internal class SearchResponseMetadata
	{
		[JsonProperty("page")] public int Page;
		[JsonProperty("count")] public int Count;
		[JsonProperty("total")] public int Total;
	}

	internal class FlatIconSearchResponse
	{
		[JsonProperty("data")] public List<SearchResponseData> Data;
		[JsonProperty("metadata")] public SearchResponseMetadata SearchResponseMetadata;
	}
}