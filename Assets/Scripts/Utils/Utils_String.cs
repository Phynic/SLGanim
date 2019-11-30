using System.Text.RegularExpressions;
public class Utils_String
{
    public static string ToUpperFirstLetter(string str)
    {
        return Regex.Replace(str, @"^\w", t => t.Value.ToUpper());
    }
}
