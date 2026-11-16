using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Pos.Infrastructure;

internal static class JsonDefaults
{
    public static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
    };
}
