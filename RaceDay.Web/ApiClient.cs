using System.Net.Http.Json;
using System.Text.Json;
namespace RaceDay.Web;
public class ApiClient(IHttpClientFactory factory, IHttpContextAccessor context)
{
    private readonly JsonSerializerOptions options = new(JsonSerializerDefaults.Web);
    private HttpClient Create() {
        var client = factory.CreateClient("api");
        var cookie = context.HttpContext?.Session.GetString("ApiCookie");
        if (!string.IsNullOrEmpty(cookie)) client.DefaultRequestHeaders.TryAddWithoutValidation("Cookie", cookie);
        return client;
    }
    public async Task<(bool ok, string error, T? data)> Send<T>(HttpMethod method, string path, object? body = null) {
        try {
            using var client = Create(); using var req = new HttpRequestMessage(method, path);
            if (body != null) req.Content = JsonContent.Create(body);
            using var res = await client.SendAsync(req);
            if (path == "api/auth/login" && res.IsSuccessStatusCode) {
                var cookies = res.Headers.TryGetValues("Set-Cookie", out var values) ? values : Array.Empty<string>();
                var sessionCookie = cookies.Select(x => x.Split(';')[0]).FirstOrDefault(x => x.StartsWith(".RaceDay.Session="));
                if (sessionCookie != null) context.HttpContext!.Session.SetString("ApiCookie", sessionCookie);
            }
            var raw = await res.Content.ReadAsStringAsync();
            if (!res.IsSuccessStatusCode) return (false, $"API returned {(int)res.StatusCode}: {raw}", default);
            if (string.IsNullOrWhiteSpace(raw)) return (true, "", default);
            return (true, "", JsonSerializer.Deserialize<T>(raw, options));
        } catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException) {
            return (false, "Cannot reach the API. Start RaceDay.Part2 first on http://localhost:5100. " + ex.Message, default);
        }
    }
}
