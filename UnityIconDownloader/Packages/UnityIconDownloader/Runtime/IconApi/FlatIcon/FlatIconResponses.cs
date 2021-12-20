using System.Collections.Generic;
using Newtonsoft.Json;

namespace IconDownloader.IconApi.FlatIcon
{

	internal class FlatIconAuthenticationResponse
	{
		[JsonProperty("data")]
		public AuthenticationResponseData Data;
	}

	internal class AuthenticationResponseData
	{
		[JsonProperty("token")]
		public string Token;

		[JsonProperty("expires")]
		public int Expires;
	}

	internal class FlatIconImage
	{
		public string Format;
		public int Size;
		public string PreviewUrl;
	}

	internal class SearchResponseData
    {
        [JsonProperty("id")]
        public int Id;

        [JsonProperty("type")]
        public int Type;

        [JsonProperty("priority")]
        public string Priority;

        [JsonProperty("selection")]
        public int Selection;

        [JsonProperty("premium")]
        public int Premium;

        [JsonProperty("state")]
        public int State;

        [JsonProperty("description")]
        public string Description;

        [JsonProperty("colors")]
        public string Colors;

        [JsonProperty("color")]
        public int Color;

        [JsonProperty("style")]
        public int Style;

        [JsonProperty("stroke")]
        public int Stroke;

        [JsonProperty("detail")]
        public int Detail;

        [JsonProperty("style_id")]
        public int StyleId;

        [JsonProperty("style_name")]
        public string StyleName;

        [JsonProperty("style_slug")]
        public string StyleSlug;

        [JsonProperty("style_class")]
        public string StyleClass;

        [JsonProperty("family_id")]
        public int FamilyId;

        [JsonProperty("family_name")]
        public string FamilyName;

        [JsonProperty("family_slug")]
        public string FamilySlug;

        [JsonProperty("priority_style")]
        public string PriorityStyle;

        [JsonProperty("slug")]
        public string Slug;

        [JsonProperty("license")]
        public string License;

        [JsonProperty("added")]
        public int Added;

        [JsonProperty("pack_id")]
        public int PackId;

        [JsonProperty("pack_name")]
        public string PackName;

        [JsonProperty("pack_slug")]
        public string PackSlug;

        [JsonProperty("pack_priority")]
        public string PackPriority;

        [JsonProperty("pack_items")]
        public int PackItems;

        [JsonProperty("designer_id")]
        public int DesignerId;

        [JsonProperty("designer_slug")]
        public string DesignerSlug;

        [JsonProperty("designer_name")]
        public string DesignerName;

        [JsonProperty("designer_website")]
        public string DesignerWebsite;

        [JsonProperty("downloads")]
        public string Downloads;

        [JsonProperty("views")]
        public string Views;

        [JsonProperty("tags")]
        public string Tags;

        [JsonProperty("category")]
        public string Category;

        [JsonProperty("item_equivalent")]
        public string ItemEquivalent;

        [JsonProperty("equivalents")]
        public int Equivalents;

        [JsonProperty("tags_id")]
        public string TagsId;

        [JsonProperty("category_id")]
        public string CategoryId;

        [JsonProperty("images")]
        [JsonConverter(typeof(FlatIconImageResponseConverter))]
        public List<FlatIconImage> Images;

        [JsonProperty("editor_choice")]
        public bool EditorChoice;

        [JsonProperty("style_color")]
        public string StyleColor;

        [JsonProperty("style_shape")]
        public string StyleShape;
    }

    internal class SearchResponseMetadata
    {
        [JsonProperty("page")]
        public int Page;

        [JsonProperty("count")]
        public int Count;

        [JsonProperty("total")]
        public int Total;
    }

    internal class FlatIconSearchResponse
    {
        [JsonProperty("data")]
        public List<SearchResponseData> Data;

        [JsonProperty("metadata")]
        public SearchResponseMetadata SearchResponseMetadata;
    }
}