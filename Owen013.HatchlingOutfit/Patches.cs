using HarmonyLib;
using HatchlingOutfit.Components;

namespace HatchlingOutfit;

[HarmonyPatch]
public static class Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerAnimController), nameof(PlayerAnimController.Start))]
    private static void PlayerAnimControllerStart(PlayerAnimController __instance)
    {
        __instance.gameObject.AddComponent<PlayerModelSwapper>();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerCharacterController), nameof(PlayerCharacterController.OnSuitUp))]
    [HarmonyPatch(typeof(PlayerCharacterController), nameof(PlayerCharacterController.OnRemoveSuit))]
    //[HarmonyPatch(typeof(MapController), nameof(MapController.ExitMapView))] // why is this here???
    private static void SuitChanged(PlayerCharacterController __instance)
    {
        __instance.GetComponentInChildren<PlayerModelSwapper>().UpdateOutfit();
    }
}