using System.Text.RegularExpressions;

namespace D2RInstanceManager.Common;

public static partial class StringExtensions
{
    [GeneratedRegex("(?<!^)([A-Z])")]
    private static partial Regex WordBoundary();
    
    extension(string value)
    {
        public string SplitWords() => WordBoundary().Replace(value, " $1");

        public string ToSentenceCase()
        {
            if (string.IsNullOrEmpty(value))
                return value;

            var result = value.SplitWords();
        
            return char.ToUpperInvariant(result[0]) +
                   result[1..].ToLowerInvariant();
        }
    }
}