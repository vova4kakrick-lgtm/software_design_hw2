using System.Text.Json;

namespace Pos.Infrastructure.Persistence;

internal sealed class JsonFile<T>(string path)
    where T : class
{
    public string Path { get; } = path;

    public T? Read()
    {
        if (!File.Exists(Path))
        {
            return null;
        }

        try
        {
            using var stream = File.OpenRead(Path);
            return JsonSerializer.Deserialize<T>(stream, JsonDefaults.Options);
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException($"Файл «{Path}» повреждён: {ex.Message}", ex);
        }
    }

    public void Write(T value)
    {
        var directory = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(Path));
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(Path, JsonSerializer.Serialize(value, JsonDefaults.Options));
    }
}
