namespace Pos.Domain.Model;

internal static class Text
{
    public static string Required(string value, string field)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"{field} не может быть пустым");
        }

        return value.Trim();
    }
}
