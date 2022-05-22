using System.Collections;
using System.Collections.Generic;
using OWML.ModHelper;
using OWML.Common;
using UnityEngine;

namespace HatchlingOutfit
{
    public class HatchlingOutfit : ModBehaviour
    {
        bool enableMod, jacketOn, armsOn, wearingHelmet;

        public static HatchlingOutfit Instance;
        PlayerCharacterController characterController;
        GameObject suitlessModel, suitlessBody, suitlessHead, suitlessRArm, suitlessLArm, suitlessHeadShader, suitlessRArmShader,
                   suitModel, suitBody, suitHead, suitRArm, suitLArm, suitHeadShader, suitRArmShader, suitJetpack;
        List<GameObject> suitlessParts, suitParts, allParts;

        public override void Configure(IModConfig config)
        {
            base.Configure(config);
            enableMod = config.GetSettingsValue<bool>("Enable Outfitter");
            jacketOn = config.GetSettingsValue<bool>("Suit Jacket On");
            armsOn = config.GetSettingsValue<bool>("Suit Arms On");
            wearingHelmet = config.GetSettingsValue<bool>("Wearing Helmet");
            Setup();
        }

        private void Awake()
        {
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
            int count = 0;
            OWScene scene = LoadManager.s_currentScene;
            if (scene == OWScene.SolarSystem || scene == OWScene.EyeOfTheUniverse)
            {
                characterController = FindObjectOfType<PlayerCharacterController>();
                PlayerBody playerBody = FindObjectOfType<PlayerBody>();
                GameObject playerModel = playerBody.transform.Find("Traveller_HEA_Player_v2").gameObject;
                suitlessModel = playerModel.transform.Find("player_mesh_noSuit:Traveller_HEA_Player").gameObject;
                suitModel = playerModel.transform.Find("Traveller_Mesh_v01:Traveller_Geo").gameObject;
                string suitless = "player_mesh_noSuit:Player_";
                suitlessBody = suitlessModel.transform.Find(suitless + "Clothes").gameObject;
                suitlessHead = suitlessModel.transform.Find(suitless + "Head").gameObject;
                suitlessRArm = suitlessModel.transform.Find(suitless + "RightArm").gameObject;
                suitlessLArm = suitlessModel.transform.Find(suitless + "LeftArm").gameObject;
                suitlessHeadShader = suitlessModel.transform.Find(suitless + "Head_ShadowCaster").gameObject;
                suitlessRArmShader = suitlessModel.transform.Find(suitless + "RightArm_ShadowCaster").gameObject;
                suitlessParts = new List<GameObject>();
                suitlessParts.Add(suitlessBody);
                suitlessParts.Add(suitlessHead);
                suitlessParts.Add(suitlessRArm);
                suitlessParts.Add(suitlessLArm);
                suitlessParts.Add(suitlessHeadShader);
                suitlessParts.Add(suitlessRArmShader);
                string suit = "Traveller_Mesh_v01:";
                suitBody = suitModel.transform.Find(suit + "PlayerSuit_Body").gameObject;
                suitHead = suitModel.transform.Find(suit + "PlayerSuit_Helmet").gameObject;
                suitRArm = suitModel.transform.Find(suit + "PlayerSuit_RightArm").gameObject;
                suitLArm = suitModel.transform.Find(suit + "PlayerSuit_LeftArm").gameObject;
                suitHeadShader = suitModel.transform.Find(suit + "PlayerSuit_Helmet_ShadowCaster").gameObject;
                suitRArmShader = suitModel.transform.Find(suit + "PlayerSuit_RightArm_ShadowCaster").gameObject;
                suitJetpack = suitModel.transform.Find(suit + "Props_HEA_Jetpack").gameObject;
                suitParts = new List<GameObject>();
                suitParts.Add(suitBody);
                suitParts.Add(suitHead);
                suitParts.Add(suitRArm);
                suitParts.Add(suitLArm);
                suitParts.Add(suitHeadShader);
                suitParts.Add(suitRArmShader);
                allParts = new List<GameObject>();
                allParts.AddRange(suitlessParts);
                allParts.AddRange(suitParts);

                ChangeOutfit();
            }
        }

        public void ChangeOutfit()
        {
            switch (enableMod)
            {
                case false:
                    if (!characterController._isWearingSuit)
                    {
                        foreach (GameObject part in allParts) part.SetActive(true);
                        suitlessModel.SetActive(true);
                        suitModel.SetActive(false);
                    }
                    else
                    {
                        foreach (GameObject part in allParts) part.SetActive(true);
                        suitlessModel.SetActive(false);
                        suitModel.SetActive(true);
                    }

                    break;

                case true:
                    suitlessModel.SetActive(true);
                    suitModel.SetActive(true);

                    if (jacketOn) suitBody.SetActive(true);
                    else suitBody.SetActive(false);

                    if (armsOn)
                    {
                        suitRArm.SetActive(true);
                        suitLArm.SetActive(true);
                    }
                    else
                    {
                        suitRArm.SetActive(false);
                        suitLArm.SetActive(false);
                    }

                    if (wearingHelmet) suitHead.SetActive(true);
                    else suitHead.SetActive(false);

                    if (characterController._isWearingSuit) suitJetpack.SetActive(true);
                    else suitJetpack.SetActive(false);

                    suitHeadShader.SetActive(suitHead.activeSelf);
                    suitRArmShader.SetActive(suitRArm.activeSelf);

                    suitlessBody.SetActive(!suitBody.activeSelf);
                    suitlessHead.SetActive(!suitHead.activeSelf);
                    suitlessRArm.SetActive(!suitRArm.activeSelf);
                    suitlessLArm.SetActive(!suitLArm.activeSelf);
                    suitlessHeadShader.SetActive(!suitHeadShader.activeSelf);
                    suitlessRArmShader.SetActive(!suitRArmShader.activeSelf);

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