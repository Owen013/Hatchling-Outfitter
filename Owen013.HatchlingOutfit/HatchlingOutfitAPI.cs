using HatchlingOutfit.Components;

namespace HatchlingOutfit;

public class HatchlingOutfitAPI
{
    public bool GetPlayerHelmeted()
    {
        return Locator.GetPlayerBody().GetComponentInChildren<PlayerModelSwapper>().IsPlayerHelmeted();
    }
}