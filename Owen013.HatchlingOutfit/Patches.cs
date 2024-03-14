using HarmonyLib;
using HatchlingOutfit.Components;

namespace HatchlingOutfit;

[HarmonyPatch]
public static class Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerCharacterController), nameof(PlayerCharacterController.Start))]
    private static void PlayerControllerStart(PlayerCharacterController __instance)
    {
        __instance.GetComponentInChildren<PlayerAnimController>().gameObject.AddComponent<PlayerModelSwapper>();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerCharacterController), nameof(PlayerCharacterController.OnSuitUp))]
    [HarmonyPatch(typeof(PlayerCharacterController), nameof(PlayerCharacterController.OnRemoveSuit))]
    //[HarmonyPatch(typeof(MapController), nameof(MapController.ExitMapView))] // why is this here???
    private static void SuitChanged()
    {
        PlayerModelSwapper.Instance?.UpdateOutfit();
    }
}