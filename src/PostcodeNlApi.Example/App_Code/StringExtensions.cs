/// <summary>
/// Summary description for StringExtensions
/// </summary>
public static class StringExtensions
{
    public static string NullIfEmpty(this string value)
    {
        return !string.IsNullOrEmpty(value) ? value : null;
    }
}
