using System;

namespace D2RInstanceManager.Common;

public static partial class EnumExtensions
{
    public static string ToDisplayString<TEnum>(this TEnum value)
        where TEnum : Enum => value.ToString().SplitWords();

}