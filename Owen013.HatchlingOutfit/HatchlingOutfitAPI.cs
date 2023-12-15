namespace HatchlingOutfit
{
    public class HatchlingOutfitAPI
    {
        public bool GetPlayerHelmeted()
        {
            return HatchlingOutfit.s_instance.IsPlayerHelmeted();
        }
    }
}