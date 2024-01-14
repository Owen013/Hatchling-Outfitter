namespace HatchlingOutfit;

public class HatchlingOutfitAPI
{
    public bool GetPlayerHelmeted()
    {
        return ModController.s_instance.IsPlayerHelmeted();
    }
}