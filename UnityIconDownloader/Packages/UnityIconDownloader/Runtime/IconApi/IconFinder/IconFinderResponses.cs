using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace IconDownloader.IconApi.IconFinder
{
    internal class License
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("license_id")]
        public int LicenseId;

        [JsonProperty("scope")]
        public string Scope;

        [JsonProperty("url")]
        public string Url;
    }

    internal class IconPrice
    {
        [JsonProperty("license")]
        public License License;

        [JsonProperty("price")]
        public double Price;

        [JsonProperty("currency")]
        public string Currency;
    }

    internal class IconFormat
    {
        [JsonProperty("format")]
        public string Format;

        [JsonProperty("download_url")]
        public string DownloadUrl;

        [JsonProperty("preview_url")]
        public string PreviewUrl;
    }

    internal class RasterSize
    {
        [JsonProperty("formats")]
        public List<IconFormat> Formats;

        [JsonProperty("size")]
        public int Size;

        [JsonProperty("size_width")]
        public int SizeWidth;

        [JsonProperty("size_height")]
        public int SizeHeight;
    }

    internal class VectorSize
    {
        [JsonProperty("formats")]
        public List<IconFormat> Formats;

        [JsonProperty("size")]
        public int Size;

        [JsonProperty("size_width")]
        public int SizeWidth;

        [JsonProperty("target_sizes")]
        public List<List<int>> TargetSizes;

        [JsonProperty("size_height")]
        public int SizeHeight;
    }

    internal class Style
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("identifier")]
        public string Identifier;
    }

    internal class Category
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("identifier")]
        public string Identifier;
    }

    internal class Container
    {
        [JsonProperty("format")]
        public string Format;

        [JsonProperty("download_url")]
        public string DownloadUrl;
    }

    internal class Icon
    {
        [JsonProperty("tags")]
        public List<string> Tags;

        [JsonProperty("prices")]
        public List<IconPrice> Prices;

        [JsonProperty("published_at")]
        public DateTime PublishedAt;

        [JsonProperty("raster_sizes")]
        public List<RasterSize> RasterSizes;

        [JsonProperty("vector_sizes")]
        public List<VectorSize> VectorSizes;

        [JsonProperty("styles")]
        public List<Style> Styles;

        [JsonProperty("is_purchased")]
        public bool IsPurchased;

        [JsonProperty("categories")]
        public List<Category> Categories;

        [JsonProperty("is_premium")]
        public bool IsPremium;

        [JsonProperty("containers")]
        public List<Container> Containers;

        [JsonProperty("type")]
        public string Type;

        [JsonProperty("icon_id")]
        public int IconId;

        [JsonProperty("is_icon_glyph")]
        public bool IsIconGlyph;
    }

    internal class IconFinderSearchResponse
    {
        [JsonProperty("icons")]
        public List<Icon> Icons;

        [JsonProperty("total_count")]
        public int TotalCount;
    }

    internal class Author
    {
        [JsonProperty("iconsets_count")]
        public int IconsetsCount;

        [JsonProperty("username")]
        public string Username;

        [JsonProperty("is_designer")]
        public bool IsDesigner;

        [JsonProperty("user_id")]
        public int UserId;

        [JsonProperty("company")]
        public string Company;

        [JsonProperty("name")]
        public string Name;
    }

    internal class Iconset
    {
        [JsonProperty("identifier")]
        public string Identifier;

        [JsonProperty("categories")]
        public List<Category> Categories;

        [JsonProperty("type")]
        public string Type;

        [JsonProperty("is_premium")]
        public bool IsPremium;

        [JsonProperty("are_all_icons_glyph")]
        public bool AreAllIconsGlyph;

        [JsonProperty("iconset_id")]
        public int IconsetId;

        [JsonProperty("published_at")]
        public DateTime PublishedAt;

        [JsonProperty("website_url")]
        public string WebsiteUrl;

        [JsonProperty("styles")]
        public List<Style> Styles;

        [JsonProperty("icons_count")]
        public int IconsCount;

        [JsonProperty("license")]
        public License License;

        [JsonProperty("author")]
        public Author Author;

        [JsonProperty("name")]
        public string Name;
    }

    internal class IconDetails
    {
        [JsonProperty("icon_id")]
        public int IconId;

        [JsonProperty("categories")]
        public List<Category> Categories;

        [JsonProperty("type")]
        public string Type;

        [JsonProperty("is_premium")]
        public bool IsPremium;

        [JsonProperty("styles")]
        public List<Style> Styles;

        [JsonProperty("iconset")]
        public Iconset Iconset;

        [JsonProperty("is_icon_glyph")]
        public bool IsIconGlyph;

        [JsonProperty("vector_sizes")]
        public List<VectorSize> VectorSizes;

        [JsonProperty("published_at")]
        public DateTime PublishedAt;

        [JsonProperty("containers")]
        public List<Container> Containers;

        [JsonProperty("raster_sizes")]
        public List<RasterSize> RasterSizes;

        [JsonProperty("tags")]
        public List<string> Tags;
    }
}