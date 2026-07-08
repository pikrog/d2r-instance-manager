using System;

namespace AvaloniaApplication1.Config;

public static class ConfigConstants
{
    public const string ConfigFileName = "config.json";
    public const string ConfigDirectoryName = "D2R Instance Manager";

    public static readonly Guid EuropeRegionId = new("eec68aa3-049a-4145-9474-6397cdc5a1cf");
    public const string EuropeRegionName = "Europe";
    public const string EuropeRegionAddress = "eu.actual.battle.net";
    
    public static readonly Guid UnitedStatesRegionId = new("1c7673c4-22e9-4cd7-afa9-d78d3b9c0116");
    public const string UnitedStatesRegionName = "United States";
    public const string UnitedStatesRegionAddress = "us.actual.battle.net";
    
    public static readonly Guid AsiaRegionId = new("66edc334-bf70-4557-8a75-cedaa25eb6da");
    public const string AsiaRegionName = "Asia";
    public const string AsiaRegionAddress = "kr.actual.battle.net";
}