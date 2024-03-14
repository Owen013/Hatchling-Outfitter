using OWML.Common;

namespace HatchlingOutfit;

public static class Config
{
    public static string BodySetting { get; private set; }
    public static string HeadSetting { get; private set; }
    public static string RightArmSetting { get; private set; }
    public static string LeftArmSetting { get; private set; }
    public static string JetpackSetting { get; private set; }
    public static bool IsMissingBody { get; private set; }
    public static bool IsMissingHead { get; private set; }
    public static bool IsMissingRightArm { get; private set; }
    public static bool IsMissingLeftArm { get; private set; }

    public static void UpdateConfig(IModConfig config)
    {
        BodySetting = config.GetSettingsValue<string>("Body");
        RightArmSetting = config.GetSettingsValue<string>("Right Arm");
        LeftArmSetting = config.GetSettingsValue<string>("Left Arm");
        HeadSetting = config.GetSettingsValue<string>("Head");
        JetpackSetting = config.GetSettingsValue<string>("Jetpack");
        IsMissingBody = config.GetSettingsValue<bool>("Missing Body");
        IsMissingHead = config.GetSettingsValue<bool>("Missing Head");
        IsMissingRightArm = config.GetSettingsValue<bool>("Missing Right Arm");
        IsMissingLeftArm = config.GetSettingsValue<bool>("Missing Left Arm");
    }
}