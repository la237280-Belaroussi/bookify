namespace Bookify.Services
{
    /// <summary>
    /// Construit les URLs Amazon (produit ou recherche).
    /// Configuré ici pour amazon.com par défaut.
    /// </summary>
    public class AmazonLinkBuilder
    {
        private readonly string _host;    // ex: "amazon.com"
        private readonly string? _tag;    // tag affilié éventuel, ex: "tonid-20"

        public AmazonLinkBuilder(string defaultMarket = "com", string? affiliateTag = null)
        {
            _host = defaultMarket.ToLower() switch
            {
                "com" => "amazon.com",
                "fr"  => "amazon.fr",
                "de"  => "amazon.de",
                "it"  => "amazon.it",
                "es"  => "amazon.es",
                "co.uk" => "amazon.co.uk",
                _ => "amazon.com"
            };
            _tag = string.IsNullOrWhiteSpace(affiliateTag) ? null : affiliateTag;
        }

        public Uri BuildProductOrSearchUrl(string isbn, string? asin = null)
        {
            if (!string.IsNullOrWhiteSpace(asin))
            {
                var url = $"https://{_host}/dp/{Uri.EscapeDataString(asin)}";
                if (!string.IsNullOrEmpty(_tag)) url += $"?tag={Uri.EscapeDataString(_tag)}";
                return new Uri(url);
            }

            var search = $"https://{_host}/s?k={Uri.EscapeDataString(isbn)}";
            if (!string.IsNullOrEmpty(_tag)) search += $"&tag={Uri.EscapeDataString(_tag)}";
            return new Uri(search);
        }
    }
}
