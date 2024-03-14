using HarmonyLib;
using OWML.ModHelper;
using OWML.Common;
using System.Reflection;
using HatchlingOutfit.Components;

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
        Config.UpdateConfig(config);
        OnConfigure?.Invoke();
    }
    public void Log(string text, MessageType type = MessageType.Message)
    {
        ModHelper.Console.WriteLine(text, type);
    }

    private void Awake()
    {
        Instance = this;
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
    }
}