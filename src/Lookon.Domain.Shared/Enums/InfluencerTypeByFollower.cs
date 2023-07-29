using System.ComponentModel;

namespace LookOn.Enums;

public enum InfluencerTypeByFollower
{
    [Description("0-1,000 followers")]
    Normal,
    [Description("1,001-8,000 followers")]
    Nano,
    [Description("8,001-100,000 followers")]
    Micro,
    [Description("100,001-1,000,000 followers")]
    Macro,
    [Description(">1,000,001 followers")]
    Mega
}