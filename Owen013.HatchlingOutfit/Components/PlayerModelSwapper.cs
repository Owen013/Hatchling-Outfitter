using UnityEngine;

namespace HatchlingOutfit.Components;

public class PlayerModelSwapper : MonoBehaviour
{
    private PlayerAnimController _animController;
    private GameObject _suitlessModel;
    private GameObject _suitlessBody;
    private GameObject _suitlessHead;
    private GameObject _suitlessRArm;
    private GameObject _suitlessLArm;
    private GameObject _suitlessHeadShader;
    private GameObject _suitlessRArmShader;
    private GameObject _suitModel;
    private GameObject _suitBody;
    private GameObject _suitHead;
    private GameObject _suitLArm;
    private GameObject _suitRArm;
    private GameObject _suitHeadShader;
    private GameObject _suitRArmShader;
    private GameObject _suitJetpack;
    private GameObject _suitJetpackFX;

    private void Awake()
    {
        _animController = FindObjectOfType<PlayerAnimController>();
        _suitlessModel = _animController.transform.Find("player_mesh_noSuit:Traveller_HEA_Player").gameObject;
        _suitModel = _animController.transform.Find("Traveller_Mesh_v01:Traveller_Geo").gameObject;
        _suitJetpackFX = GetComponentInParent<PlayerBody>().transform.Find("PlayerVFX").gameObject;

        // Get all individual parts for suitless
        _suitlessBody = _suitlessModel.transform.Find("player_mesh_noSuit:Player_Clothes").gameObject;
        _suitlessHead = _suitlessModel.transform.Find("player_mesh_noSuit:Player_Head").gameObject;
        _suitlessLArm = _suitlessModel.transform.Find("player_mesh_noSuit:Player_LeftArm").gameObject;
        _suitlessRArm = _suitlessModel.transform.Find("player_mesh_noSuit:Player_RightArm").gameObject;
        _suitlessHeadShader = _suitlessModel.transform.Find("player_mesh_noSuit:Player_Head_ShadowCaster").gameObject;
        _suitlessRArmShader = _suitlessModel.transform.Find("player_mesh_noSuit:Player_RightArm_ShadowCaster").gameObject;

        // Get all individual parts for suited
        _suitBody = _suitModel.transform.Find("Traveller_Mesh_v01:PlayerSuit_Body").gameObject;
        _suitHead = _suitModel.transform.Find("Traveller_Mesh_v01:PlayerSuit_Helmet").gameObject;
        _suitLArm = _suitModel.transform.Find("Traveller_Mesh_v01:PlayerSuit_LeftArm").gameObject;
        _suitRArm = _suitModel.transform.Find("Traveller_Mesh_v01:PlayerSuit_RightArm").gameObject;
        _suitHeadShader = _suitModel.transform.Find("Traveller_Mesh_v01:PlayerSuit_Helmet_ShadowCaster").gameObject;
        _suitRArmShader = _suitModel.transform.Find("Traveller_Mesh_v01:PlayerSuit_RightArm_ShadowCaster").gameObject;
        _suitJetpack = _suitModel.transform.Find("Traveller_Mesh_v01:Props_HEA_Jetpack").gameObject;

        Main.Instance.OnConfigure += UpdateOutfit;
        UpdateOutfit();
    }

    public void UpdateOutfit()
    {
        bool isSuited = PlayerState.IsWearingSuit();

        switch (Config.BodySetting)
        {
            case "Always Suitless":
                _suitBody.SetActive(false);
                break;
            case "Default":
                _suitBody.SetActive(isSuited);
                break;
            case "Always Suited":
                _suitBody.SetActive(true);
                break;
            case "Opposite":
                _suitBody.SetActive(!isSuited);
                break;
        }

        switch (Config.RightArmSetting)
        {
            case "Always Suitless":
                _suitRArm.SetActive(false);
                break;
            case "Default":
                _suitRArm.SetActive(isSuited);
                break;
            case "Always Suited":
                _suitRArm.SetActive(true);
                break;
            case "Opposite":
                _suitRArm.SetActive(!isSuited);
                break;
        }

        switch (Config.LeftArmSetting)
        {
            case "Always Suitless":
                _suitLArm.SetActive(false);
                break;
            case "Default":
                _suitLArm.SetActive(isSuited);
                break;
            case "Always Suited":
                _suitLArm.SetActive(true);
                break;
            case "Opposite":
                _suitLArm.SetActive(!isSuited);
                break;
        }

        // Helmet
        switch (Config.HeadSetting)
        {
            case "Always Suitless":
                _suitHead.SetActive(false);
                break;
            case "Default":
                _suitHead.SetActive(isSuited);
                break;
            case "Always Suited":
                _suitHead.SetActive(true);
                break;
            case "Opposite":
                _suitHead.SetActive(!isSuited);
                break;
        }

        // Jetpack
        switch (Config.JetpackSetting)
        {
            case "Always Off":
                _suitJetpack.SetActive(false);
                _suitJetpackFX.SetActive(false);
                ChangeAnimGroup("Suitless");
                break;
            case "Default":
                _suitJetpack.SetActive(isSuited);
                _suitJetpackFX.SetActive(isSuited);
                if (!isSuited) ChangeAnimGroup("Suitless");
                else ChangeAnimGroup("Suited");
                break;
            case "Always On":
                _suitJetpack.SetActive(true);
                _suitJetpackFX.SetActive(true);
                ChangeAnimGroup("Suited");
                break;
            case "Opposite":
                _suitJetpack.SetActive(!isSuited);
                _suitJetpackFX.SetActive(!isSuited);
                if (isSuited) ChangeAnimGroup("Suitless");
                else ChangeAnimGroup("Suited");
                break;
        }


        // Set both suitless and suited whole models as visible
        _suitlessModel.SetActive(true);
        _suitModel.SetActive(true);

        // Enable suitless body part if the cooresponding suited part is inactive
        _suitlessBody.SetActive(!_suitBody.activeSelf);
        _suitlessHead.SetActive(!_suitHead.activeSelf);
        _suitlessLArm.SetActive(!_suitLArm.activeSelf);
        _suitlessRArm.SetActive(!_suitRArm.activeSelf);

        // Remove chosen body parts
        if (Config.IsMissingBody)
        {
            _suitlessBody.SetActive(false);
            _suitBody.SetActive(false);
        }
        if (Config.IsMissingHead)
        {
            _suitlessHead.SetActive(false);
            _suitHead.SetActive(false);
        }
        if (Config.IsMissingRightArm)
        {
            _suitlessRArm.SetActive(false);
            _suitRArm.SetActive(false);
        }
        if (Config.IsMissingLeftArm)
        {
            _suitlessLArm.SetActive(false);
            _suitLArm.SetActive(false);
        }

        // Enable shaders for visible parts that have them
        _suitlessHeadShader.SetActive(_suitHeadShader.activeSelf);
        _suitlessRArmShader.SetActive(_suitRArmShader.activeSelf);
        _suitHeadShader.SetActive(_suitHead.activeSelf);
        _suitRArmShader.SetActive(_suitRArm.activeSelf);
    }

    private void ChangeAnimGroup(string animGroup)
    {
        // There are two anim groups for player: one for the suitless and one for suited. It looks weird
        // if the hatchling uses suit anims when the jetpack is off because they hold their left arm in
        // the air weirdly, so I switch anims, using suitless if no jetpack and suited if jetpack.
        switch (animGroup)
        {
            case "Suitless":
                _animController._animator.runtimeAnimatorController = _animController._unsuitedAnimOverride;
                _animController._unsuitedGroup.SetActive(!PlayerState.InMapView());
                _animController._suitedGroup.SetActive(false);
                break;
            case "Suited":
                _animController._animator.runtimeAnimatorController = _animController._baseAnimController;
                _animController._unsuitedGroup.SetActive(false);
                _animController._suitedGroup.SetActive(!PlayerState.InMapView());
                break;
        }
    }

    public bool IsPlayerHelmeted()
    {
        return _suitHead.activeSelf;
    }
}
