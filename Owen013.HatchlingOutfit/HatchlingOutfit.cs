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
        string bodySetting, armsSetting, headSetting, jetpackSetting, suitOneArm;
        bool missingBody, missingHead, missingLArm, missingRArm;

        // Mod vars
        public static HatchlingOutfit Instance;
        PlayerCharacterController characterController;
        PlayerAnimController animController;
        GameObject suitlessModel, suitlessBody, suitlessHead, suitlessLArm, suitlessRArm, suitlessHeadShader, suitlessRArmShader,
                   suitModel, suitBody, suitHead, suitLArm, suitRArm, suitHeadShader, suitRArmShader, suitJetpack, suitJetpackFX;

        public override void Configure(IModConfig config)
        {
            base.Configure(config);
            bodySetting = config.GetSettingsValue<string>("Body");
            armsSetting = config.GetSettingsValue<string>("Arms");
            headSetting = config.GetSettingsValue<string>("Head");
            jetpackSetting = config.GetSettingsValue<string>("Jetpack");
            suitOneArm = config.GetSettingsValue<string>("Only Suit One Arm");
            missingBody = config.GetSettingsValue<bool>("Missing Body");
            missingHead = config.GetSettingsValue<bool>("Missing Head");
            missingLArm = config.GetSettingsValue<bool>("Missing Left Arm");
            missingRArm = config.GetSettingsValue<bool>("Missing Right Arm");
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
                suitlessLArm = suitlessModel.transform.Find(suitless + "LeftArm").gameObject;
                suitlessRArm = suitlessModel.transform.Find(suitless + "RightArm").gameObject;
                suitlessHeadShader = suitlessModel.transform.Find(suitless + "Head_ShadowCaster").gameObject;
                suitlessRArmShader = suitlessModel.transform.Find(suitless + "RightArm_ShadowCaster").gameObject;

                // Get all individual parts for suited
                string suit = "Traveller_Mesh_v01:";
                suitBody = suitModel.transform.Find(suit + "PlayerSuit_Body").gameObject;
                suitHead = suitModel.transform.Find(suit + "PlayerSuit_Helmet").gameObject;
                suitLArm = suitModel.transform.Find(suit + "PlayerSuit_LeftArm").gameObject;
                suitRArm = suitModel.transform.Find(suit + "PlayerSuit_RightArm").gameObject;
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
            switch (bodySetting)
            {
                case "Always Suitless":
                    suitBody.SetActive(false);
                    break;
                case "Default":
                    suitBody.SetActive(isSuited);
                    break;
                case "Always Suited":
                    suitBody.SetActive(true);
                    break;
                case "Opposite":
                    suitBody.SetActive(!isSuited);
                    break;
            }

            // Arms
            switch (armsSetting)
            {
                case "Always Suitless":
                    suitLArm.SetActive(false);
                    suitRArm.SetActive(false);
                    break;
                case "Default":
                    suitLArm.SetActive(isSuited);
                    suitRArm.SetActive(isSuited);
                    break;
                case "Always Suited":
                    suitLArm.SetActive(true);
                    suitRArm.SetActive(true);
                    break;
                case "Opposite":
                    suitLArm.SetActive(!isSuited);
                    suitRArm.SetActive(!isSuited);
                    break;
            }

            // Helmet
            switch (headSetting)
            {
                case "Always Suitless":
                    suitHead.SetActive(false);
                    break;
                case "Default":
                    suitHead.SetActive(isSuited);
                    break;
                case "Always Suited":
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

            // Set both suitless and suited whole models as visible
            suitlessModel.SetActive(true);
            suitModel.SetActive(true);

            // Enable suitless body part if the cooresponding suited part is inactive
            suitlessBody.SetActive(!suitBody.activeSelf);
            suitlessHead.SetActive(!suitHead.activeSelf);
            suitlessLArm.SetActive(!suitLArm.activeSelf);
            suitlessRArm.SetActive(!suitRArm.activeSelf);

            // If player chose to suit only one arm, unsuit the other
            switch (suitOneArm)
            {
                case "Off":
                    break;

                case "Left":
                    suitlessRArm.SetActive(true);
                    suitRArm.SetActive(false);
                    break;

                case "Right":
                    suitlessLArm.SetActive(true);
                    suitLArm.SetActive(false);
                    break;
            }

            // Remove chosen body parts
            if (missingBody)
            {
                suitlessBody.SetActive(false);
                suitBody.SetActive(false);
            }
            if (missingHead)
            {
                suitlessHead.SetActive(false);
                suitHead.SetActive(false);
            }
            if (missingLArm)
            {
                suitlessLArm.SetActive(false);
                suitLArm.SetActive(false);
            }
            if (missingRArm)
            {
                suitlessRArm.SetActive(false);
                suitRArm.SetActive(false);
            }

            // Enable shaders for visible parts that have them
            suitlessHeadShader.SetActive(suitHeadShader.activeSelf);
            suitlessRArmShader.SetActive(suitRArmShader.activeSelf);
            suitHeadShader.SetActive(suitHead.activeSelf);
            suitRArmShader.SetActive(suitRArm.activeSelf);
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