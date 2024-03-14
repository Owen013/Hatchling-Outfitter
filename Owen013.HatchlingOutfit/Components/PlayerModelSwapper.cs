using UnityEngine;

namespace HatchlingOutfit.Components;

public class PlayerModelSwapper : MonoBehaviour
{
    public static PlayerModelSwapper Instance;
    private PlayerAnimController _animController;
    private PlayerSpacesuit _spacesuit;
    private GameObject _suitlessModel;
    private GameObject _suitlessBody;
    private GameObject _suitlessHead;
    private GameObject _suitlessRightArm;
    private GameObject _suitlessLeftArm;
    private GameObject _suitlessHeadShadow;
    private GameObject _suitlessRightArmShadow;
    private GameObject _suitModel;
    private GameObject _suitBody;
    private GameObject _suitHead;
    private GameObject _suitLeftArm;
    private GameObject _suitRightArm;
    private GameObject _suitHeadShadow;
    private GameObject _suitRightArmShadow;
    private GameObject _suitJetpack;
    private GameObject _suitJetpackFX;

    private void Awake()
    {
        Instance = this;

        _animController = GetComponent<PlayerAnimController>();
        _spacesuit = _animController.GetComponentInParent<PlayerSpacesuit>();
        _suitlessModel = _animController.transform.Find("player_mesh_noSuit:Traveller_HEA_Player").gameObject;
        _suitModel = _animController.transform.Find("Traveller_Mesh_v01:Traveller_Geo").gameObject;
        _suitJetpackFX = GetComponentInParent<PlayerBody>().transform.Find("PlayerVFX").gameObject;

        // Get all individual parts for suitless
        _suitlessBody = _suitlessModel.transform.Find("player_mesh_noSuit:Player_Clothes").gameObject;
        _suitlessHead = _suitlessModel.transform.Find("player_mesh_noSuit:Player_Head").gameObject;
        _suitlessLeftArm = _suitlessModel.transform.Find("player_mesh_noSuit:Player_LeftArm").gameObject;
        _suitlessRightArm = _suitlessModel.transform.Find("player_mesh_noSuit:Player_RightArm").gameObject;
        _suitlessHeadShadow = _suitlessModel.transform.Find("player_mesh_noSuit:Player_Head_ShadowCaster").gameObject;
        _suitlessRightArmShadow = _suitlessModel.transform.Find("player_mesh_noSuit:Player_RightArm_ShadowCaster").gameObject;

        // Get all individual parts for suited
        _suitBody = _suitModel.transform.Find("Traveller_Mesh_v01:PlayerSuit_Body").gameObject;
        _suitHead = _suitModel.transform.Find("Traveller_Mesh_v01:PlayerSuit_Helmet").gameObject;
        _suitLeftArm = _suitModel.transform.Find("Traveller_Mesh_v01:PlayerSuit_LeftArm").gameObject;
        _suitRightArm = _suitModel.transform.Find("Traveller_Mesh_v01:PlayerSuit_RightArm").gameObject;
        _suitHeadShadow = _suitModel.transform.Find("Traveller_Mesh_v01:PlayerSuit_Helmet_ShadowCaster").gameObject;
        _suitRightArmShadow = _suitModel.transform.Find("Traveller_Mesh_v01:PlayerSuit_RightArm_ShadowCaster").gameObject;
        _suitJetpack = _suitModel.transform.Find("Traveller_Mesh_v01:Props_HEA_Jetpack").gameObject;

        Main.Instance.OnConfigure += UpdateOutfit;
        UpdateOutfit();
    }

    private void LateUpdate()
    {
        // Helmet
        bool isWearingHelmet = _spacesuit.IsWearingHelmet();
        switch (Config.HeadSetting)
        {
            case "Always Suitless":
                _suitHead.SetActive(false);
                _suitHeadShadow.SetActive(false);
                break;
            case "Default":
                _suitHead.SetActive(isWearingHelmet);
                _suitHeadShadow.SetActive(isWearingHelmet);
                break;
            case "Always Suited":
                _suitHead.SetActive(true);
                _suitHeadShadow.SetActive(true);
                break;
            case "Opposite":
                _suitHead.SetActive(!isWearingHelmet);
                _suitHeadShadow.SetActive(!isWearingHelmet);
                break;
        }
        _suitlessHead.SetActive(!_suitHead.activeInHierarchy);
        _suitlessHeadShadow.SetActive(!_suitHeadShadow.activeInHierarchy);

        // Change rightarm shadow layers
        _suitlessRightArmShadow.layer = _suitlessRightArm.layer;
        _suitRightArmShadow.layer = _suitRightArm.layer;
    }

    private void OnDestroy()
    {
        Main.Instance.OnConfigure -= UpdateOutfit;
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
                _suitRightArm.SetActive(false);
                break;
            case "Default":
                _suitRightArm.SetActive(isSuited);
                break;
            case "Always Suited":
                _suitRightArm.SetActive(true);
                break;
            case "Opposite":
                _suitRightArm.SetActive(!isSuited);
                break;
        }

        switch (Config.LeftArmSetting)
        {
            case "Always Suitless":
                _suitLeftArm.SetActive(false);
                break;
            case "Default":
                _suitLeftArm.SetActive(isSuited);
                break;
            case "Always Suited":
                _suitLeftArm.SetActive(true);
                break;
            case "Opposite":
                _suitLeftArm.SetActive(!isSuited);
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
        _suitlessBody.SetActive(!_suitBody.activeInHierarchy);
        _suitlessLeftArm.SetActive(!_suitLeftArm.activeInHierarchy);
        _suitlessRightArm.SetActive(!_suitRightArm.activeInHierarchy);

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
            _suitlessRightArm.SetActive(false);
            _suitRightArm.SetActive(false);
        }
        if (Config.IsMissingLeftArm)
        {
            _suitlessLeftArm.SetActive(false);
            _suitLeftArm.SetActive(false);
        }

        // Enable shadows for visible parts that have them
        _suitlessRightArmShadow.SetActive(_suitlessRightArm.activeInHierarchy);
        _suitRightArmShadow.SetActive(_suitRightArm.activeInHierarchy);
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
        return _suitHead.activeInHierarchy;
    }
}