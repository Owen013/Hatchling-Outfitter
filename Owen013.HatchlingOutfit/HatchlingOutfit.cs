using System.Collections;
using System.Collections.Generic;
using OWML.ModHelper;
using OWML.Common;
using UnityEngine;

namespace HatchlingOutfit
{
    public class HatchlingOutfit : ModBehaviour
    {
        // Config vars
        string suitBodySetting, suitArmsSetting, helmetSetting, jetpackSetting;

        // Mod vars
        public static HatchlingOutfit Instance;
        PlayerCharacterController characterController;
        PlayerAnimController animController;
        GameObject suitlessModel, suitlessBody, suitlessHead, suitlessRArm, suitlessLArm, suitlessHeadShader, suitlessRArmShader,
                   suitModel, suitBody, suitHead, suitRArm, suitLArm, suitHeadShader, suitRArmShader, suitJetpack, suitJetpackFX;

        public override void Configure(IModConfig config)
        {
            base.Configure(config);
            suitBodySetting = config.GetSettingsValue<string>("Suit Body");
            suitArmsSetting = config.GetSettingsValue<string>("Suit Arms");
            helmetSetting = config.GetSettingsValue<string>("Helmet");
            jetpackSetting = config.GetSettingsValue<string>("Jetpack");
            Setup();
        }

        private void Awake()
        {
            // Static reference to mod so it can be used in patches
            Instance = this;
        }

        void Start()
        {
            ModHelper.HarmonyHelper.AddPostfix<PlayerAnimController>(
                "Start", typeof(Patches), nameof(Patches.PlayerAnimControllerAwake));

            ModHelper.HarmonyHelper.AddPostfix<PlayerCharacterController>(
                "OnSuitUp", typeof(Patches), nameof(Patches.SuitChanged));

            ModHelper.HarmonyHelper.AddPostfix<PlayerCharacterController>(
                "OnRemoveSuit", typeof(Patches), nameof(Patches.SuitChanged));
        }

        public void Setup()
        {
            // Make sure that the scene is the SS or Eye
            OWScene scene = LoadManager.s_currentScene;
            if (scene == OWScene.SolarSystem || scene == OWScene.EyeOfTheUniverse)
            {
                // Temporary vars
                PlayerBody playerBody = FindObjectOfType<PlayerBody>();
                GameObject playerModel = playerBody.transform.Find("Traveller_HEA_Player_v2").gameObject;

                // Separately grab the suitless and suited models
                characterController = FindObjectOfType<PlayerCharacterController>();
                animController = FindObjectOfType<PlayerAnimController>();
                suitlessModel = playerModel.transform.Find("player_mesh_noSuit:Traveller_HEA_Player").gameObject;
                suitModel = playerModel.transform.Find("Traveller_Mesh_v01:Traveller_Geo").gameObject;
                suitJetpackFX = playerBody.transform.Find("PlayerVFX").gameObject;

                // Get all individual parts for suitless
                string suitless = "player_mesh_noSuit:Player_";
                suitlessBody = suitlessModel.transform.Find(suitless + "Clothes").gameObject;
                suitlessHead = suitlessModel.transform.Find(suitless + "Head").gameObject;
                suitlessRArm = suitlessModel.transform.Find(suitless + "RightArm").gameObject;
                suitlessLArm = suitlessModel.transform.Find(suitless + "LeftArm").gameObject;
                suitlessHeadShader = suitlessModel.transform.Find(suitless + "Head_ShadowCaster").gameObject;
                suitlessRArmShader = suitlessModel.transform.Find(suitless + "RightArm_ShadowCaster").gameObject;

                // Get all individual parts for suited
                string suit = "Traveller_Mesh_v01:";
                suitBody = suitModel.transform.Find(suit + "PlayerSuit_Body").gameObject;
                suitHead = suitModel.transform.Find(suit + "PlayerSuit_Helmet").gameObject;
                suitRArm = suitModel.transform.Find(suit + "PlayerSuit_RightArm").gameObject;
                suitLArm = suitModel.transform.Find(suit + "PlayerSuit_LeftArm").gameObject;
                suitHeadShader = suitModel.transform.Find(suit + "PlayerSuit_Helmet_ShadowCaster").gameObject;
                suitRArmShader = suitModel.transform.Find(suit + "PlayerSuit_RightArm_ShadowCaster").gameObject;
                suitJetpack = suitModel.transform.Find(suit + "Props_HEA_Jetpack").gameObject;

                // Now that all vars are set, make the actual ingame changes
                ChangeOutfit();
            }
        }

        public void ChangeOutfit()
        {
            bool isSuited = characterController._isWearingSuit;

            // Jacket
            switch (suitBodySetting)
            {
                case "Always Off":
                    suitBody.SetActive(false);
                    break;
                case "Default":
                    suitBody.SetActive(isSuited);
                    break;
                case "Always On":
                    suitBody.SetActive(true);
                    break;
                case "Opposite":
                    suitBody.SetActive(!isSuited);
                    break;
            }

            // Arms
            switch (suitArmsSetting)
            {
                case "Always Off":
                    suitRArm.SetActive(false);
                    suitLArm.SetActive(false);
                    break;
                case "Default":
                    suitRArm.SetActive(isSuited);
                    suitLArm.SetActive(isSuited);
                    break;
                case "Always On":
                    suitRArm.SetActive(true);
                    suitLArm.SetActive(true);
                    break;
                case "Opposite":
                    suitRArm.SetActive(!isSuited);
                    suitLArm.SetActive(!isSuited);
                    break;
            }

            // Helmet
            switch (helmetSetting)
            {
                case "Always Off":
                    suitHead.SetActive(false);
                    break;
                case "Default":
                    suitHead.SetActive(isSuited);
                    break;
                case "Always On":
                    suitHead.SetActive(true);
                    break;
                case "Opposite":
                    suitHead.SetActive(!isSuited);
                    break;
            }

            // Jetpack
            switch (jetpackSetting)
            {
                case "Always Off":
                    suitJetpack.SetActive(false);
                    suitJetpackFX.SetActive(false);
                    ChangeAnimGroup("Suitless");
                    break;
                case "Default":
                    suitJetpack.SetActive(isSuited);
                    suitJetpackFX.SetActive(isSuited);
                    if (!isSuited) ChangeAnimGroup("Suitless");
                    else ChangeAnimGroup("Suited");
                    break;
                case "Always On":
                    suitJetpack.SetActive(true);
                    suitJetpackFX.SetActive(true);
                    ChangeAnimGroup("Suited");
                    break;
                case "Opposite":
                    suitJetpack.SetActive(!isSuited);
                    suitJetpackFX.SetActive(!isSuited);
                    if (isSuited) ChangeAnimGroup("Suitless");
                    else ChangeAnimGroup("Suited");
                    break;
            }

            // Enable shaders for visible suit parts that have them
            suitHeadShader.SetActive(suitHead.activeSelf);
            suitRArmShader.SetActive(suitRArm.activeSelf);

            // Enable suitless body part if the cooresponding suited part is inactive
            suitlessBody.SetActive(!suitBody.activeSelf);
            suitlessHead.SetActive(!suitHead.activeSelf);
            suitlessRArm.SetActive(!suitRArm.activeSelf);
            suitlessLArm.SetActive(!suitLArm.activeSelf);
            suitlessHeadShader.SetActive(!suitHeadShader.activeSelf);
            suitlessRArmShader.SetActive(!suitRArmShader.activeSelf);

            // Set both suitless and suited models as visible and set individual parts' visibility
            suitlessModel.SetActive(true);
            suitModel.SetActive(true);
        }

        void ChangeAnimGroup(string animGroup)
        {
            switch (animGroup)
            {
                case "Suitless":
                    animController._animator.runtimeAnimatorController = animController._unsuitedAnimOverride;
                    animController._unsuitedGroup.SetActive(!PlayerState.InMapView());
                    animController._suitedGroup.SetActive(false);
                    break;
                case "Suited":
                    animController._animator.runtimeAnimatorController = animController._baseAnimController;
                    animController._unsuitedGroup.SetActive(false);
                    animController._suitedGroup.SetActive(!PlayerState.InMapView());
                    break;
            }
        }
    }

    public static class Patches
    {
        public static void PlayerAnimControllerAwake()
        {
            HatchlingOutfit.Instance.Setup();
        }

        public static void SuitChanged()
        {
            HatchlingOutfit.Instance.ChangeOutfit();
        }
    }
}