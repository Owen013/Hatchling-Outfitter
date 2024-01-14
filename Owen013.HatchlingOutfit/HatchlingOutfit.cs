using HarmonyLib;
using OWML.ModHelper;
using OWML.Common;
using UnityEngine;

namespace HatchlingOutfit;

public class ModController : ModBehaviour
{
    // Config vars
    private string _bodySetting;
    private string _headSetting;
    private string _rArmSetting;
    private string _lArmSetting;
    private string _jetpackSetting;
    private bool _missingBody;
    private bool _missingHead;
    private bool _missingLArm;
    private bool _missingRArm;

    // Mod vars
    public static ModController s_instance;
    private PlayerCharacterController _characterController;
    private PlayerAnimController _animController;
    private GameObject _suitlessModel;
    private GameObject _suitlessBody;
    private GameObject _suitlessHead;
    private GameObject _suitlessLArm;
    private GameObject _suitlessRArm;
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

    public override object GetApi()
    {
        return new HatchlingOutfitAPI();
    }

    public override void Configure(IModConfig config)
    {
        base.Configure(config);
        _bodySetting = config.GetSettingsValue<string>("Body");
        _rArmSetting = config.GetSettingsValue<string>("Right Arm");
        _lArmSetting = config.GetSettingsValue<string>("Left Arm");
        _headSetting = config.GetSettingsValue<string>("Head");
        _jetpackSetting = config.GetSettingsValue<string>("Jetpack");
        _missingBody = config.GetSettingsValue<bool>("Missing Body");
        _missingHead = config.GetSettingsValue<bool>("Missing Head");
        _missingLArm = config.GetSettingsValue<bool>("Missing Left Arm");
        _missingRArm = config.GetSettingsValue<bool>("Missing Right Arm");
        UpdateOutfit();
    }

    private void Awake()
    {
        s_instance = this;
        Harmony.CreateAndPatchAll(typeof(ModController));
    }

    private void UpdateOutfit()
    {
        if (_characterController == null) return;

        bool isSuited = _characterController._isWearingSuit;

        // Jacket
        switch (_bodySetting)
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

        // Right Arm
        switch (_rArmSetting)
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

        // Right Arm
        switch (_lArmSetting)
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
        switch (_headSetting)
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
        switch (_jetpackSetting)
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
        if (_missingBody)
        {
            _suitlessBody.SetActive(false);
            _suitBody.SetActive(false);
        }
        if (_missingHead)
        {
            _suitlessHead.SetActive(false);
            _suitHead.SetActive(false);
        }
        if (_missingLArm)
        {
            _suitlessLArm.SetActive(false);
            _suitLArm.SetActive(false);
        }
        if (_missingRArm)
        {
            _suitlessRArm.SetActive(false);
            _suitRArm.SetActive(false);
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

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerAnimController), nameof(PlayerAnimController.Start))]
    private static void PlayerAnimControllerStart()
    {
        OWRigidbody playerBody = Locator.GetPlayerBody();
        GameObject playerModel = playerBody.transform.Find("Traveller_HEA_Player_v2").gameObject;

        // Get permanent vars and separately grab the suitless and suited models
        s_instance._characterController = Locator.GetPlayerController();
        s_instance._animController = FindObjectOfType<PlayerAnimController>();
        s_instance._suitlessModel = playerModel.transform.Find("player_mesh_noSuit:Traveller_HEA_Player").gameObject;
        s_instance._suitModel = playerModel.transform.Find("Traveller_Mesh_v01:Traveller_Geo").gameObject;
        s_instance._suitJetpackFX = playerBody.transform.Find("PlayerVFX").gameObject;

        // Get all individual parts for suitless
        s_instance._suitlessBody = s_instance._suitlessModel.transform.Find("player_mesh_noSuit:Player_Clothes").gameObject;
        s_instance._suitlessHead = s_instance._suitlessModel.transform.Find("player_mesh_noSuit:Player_Head").gameObject;
        s_instance._suitlessLArm = s_instance._suitlessModel.transform.Find("player_mesh_noSuit:Player_LeftArm").gameObject;
        s_instance._suitlessRArm = s_instance._suitlessModel.transform.Find("player_mesh_noSuit:Player_RightArm").gameObject;
        s_instance._suitlessHeadShader = s_instance._suitlessModel.transform.Find("player_mesh_noSuit:Player_Head_ShadowCaster").gameObject;
        s_instance._suitlessRArmShader = s_instance._suitlessModel.transform.Find("player_mesh_noSuit:Player_RightArm_ShadowCaster").gameObject;

        // Get all individual parts for suited
        s_instance._suitBody = s_instance._suitModel.transform.Find("Traveller_Mesh_v01:PlayerSuit_Body").gameObject;
        s_instance._suitHead = s_instance._suitModel.transform.Find("Traveller_Mesh_v01:PlayerSuit_Helmet").gameObject;
        s_instance._suitLArm = s_instance._suitModel.transform.Find("Traveller_Mesh_v01:PlayerSuit_LeftArm").gameObject;
        s_instance._suitRArm = s_instance._suitModel.transform.Find("Traveller_Mesh_v01:PlayerSuit_RightArm").gameObject;
        s_instance._suitHeadShader = s_instance._suitModel.transform.Find("Traveller_Mesh_v01:PlayerSuit_Helmet_ShadowCaster").gameObject;
        s_instance._suitRArmShader = s_instance._suitModel.transform.Find("Traveller_Mesh_v01:PlayerSuit_RightArm_ShadowCaster").gameObject;
        s_instance._suitJetpack = s_instance._suitModel.transform.Find("Traveller_Mesh_v01:Props_HEA_Jetpack").gameObject;

        // Now that all vars are set, make the actual ingame changes
        s_instance.UpdateOutfit();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerCharacterController), nameof(PlayerCharacterController.OnSuitUp))]
    [HarmonyPatch(typeof(PlayerCharacterController), nameof(PlayerCharacterController.OnRemoveSuit))]
    [HarmonyPatch(typeof(MapController), nameof(MapController.ExitMapView))]
    private static void SuitChanged()
    {
        s_instance.UpdateOutfit();
    }
}