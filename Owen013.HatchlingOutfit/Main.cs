using HarmonyLib;
using OWML.ModHelper;
using OWML.Common;
using System.Reflection;

namespace HatchlingOutfit;

public class Main : ModBehaviour
{
    public static Main Instance;
    public delegate void ConfigureEvent();
    public event ConfigureEvent OnConfigure;

    public override object GetApi()
    {
        return new HatchlingOutfitAPI();
    }

    public override void Configure(IModConfig config)
    {
        base.Configure(config);
        Config.BodySetting = config.GetSettingsValue<string>("Body");
        Config.RightArmSetting = config.GetSettingsValue<string>("Right Arm");
        Config.LeftArmSetting = config.GetSettingsValue<string>("Left Arm");
        Config.HeadSetting = config.GetSettingsValue<string>("Head");
        Config.JetpackSetting = config.GetSettingsValue<string>("Jetpack");
        Config.IsMissingBody = config.GetSettingsValue<bool>("Missing Body");
        Config.IsMissingHead = config.GetSettingsValue<bool>("Missing Head");
        Config.IsMissingRightArm = config.GetSettingsValue<bool>("Missing Right Arm");
        Config.IsMissingLeftArm = config.GetSettingsValue<bool>("Missing Left Arm");

        OnConfigure?.Invoke();
    }

    private void Awake()
    {
        Instance = this;
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
    }
}