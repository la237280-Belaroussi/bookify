using System.Text.Json;

namespace Bookify.Services
{
    /// <summary>
    /// Client minimal pour récupérer l'ASIN Amazon via Open Library.
    /// </summary>
    public class OpenLibraryService
    {
        private readonly HttpClient _http;
        public OpenLibraryService(HttpClient http) => _http = http;

        public async Task<string?> GetAmazonAsinAsync(string isbn, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(isbn)) return null;

            // Nettoyer l'ISBN (garde chiffres et X)
            var cleaned = new string(isbn.Where(c => char.IsDigit(c) || c == 'X' || c == 'x').ToArray()).ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(cleaned)) return null;

            var url = $"https://openlibrary.org/isbn/{cleaned}.json";

            using var res = await _http.GetAsync(url, ct);
            if (!res.IsSuccessStatusCode) return null;

            await using var stream = await res.Content.ReadAsStreamAsync(ct);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

            if (!doc.RootElement.TryGetProperty("identifiers", out var ids)) return null;
            if (!ids.TryGetProperty("amazon", out var arr)) return null;
            if (arr.ValueKind != JsonValueKind.Array || arr.GetArrayLength() == 0) return null;

            return arr[0].GetString();
        }
    }
}
