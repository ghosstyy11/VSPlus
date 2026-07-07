using System;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using Utilla;
using Utilla.Attributes;
using GorillaNetworking;
using GorillaTagScripts.VirtualStumpCustomMaps;
using HarmonyLib;
using PlayFab.Internal;
using System.IO;

namespace VSPlus
{
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        bool inRoom;
        bool gameLoaded;

        bool autoTeleported;

        // Config values
        public static ConfigEntry<float> TeleportCountdown;
        public static ConfigEntry<bool> AutoEnterVStump;

        void Start()
        {
            // Config loading
            TeleportCountdown = Config.Bind(
                "Settings",
                "TeleportCountdown",
                1f,
                "How long the countdown to enter Virtual Stump is."
            );
            AutoEnterVStump = Config.Bind(
                "Settings",
                "AutoEnterVStump",
                false,
                "Should VStump be entered on game start? (WARNING: you will only teleport once UGC is enabled, which requires waiting for the game to fully load.)"
            );

            Logger.LogInfo("VS+ Loaded!");
        }

        void OnEnable()
        {
            HarmonyPatches.ApplyHarmonyPatches();
            GorillaTagger.OnPlayerSpawned(OnGameInitialized);
        }

        void OnDisable()
        {
            HarmonyPatches.RemoveHarmonyPatches();
        }

        void OnGameInitialized()
        {
            gameLoaded = true;
        }

        void Update()
        {
            // try teleporting to vstump if haven't already
            if (!autoTeleported && AutoEnterVStump.Value)
            {
                if (!CustomMapManager.WaitingForRoomJoin && !CustomMapManager.WaitingForDisconnect)
                {
                    VirtualStumpTeleporter teleporter = FindFirstObjectByType<VirtualStumpTeleporter>();
                    if (teleporter != null)
                    {
                        bool accessDenied = Traverse.Create(teleporter).Field("accessDenied").GetValue<bool>();
                        if (!accessDenied && gameLoaded)
                        {
                            autoTeleported = true;
                            Logger.LogInfo("Auto teleporting to VStump!");
                            teleporter.TeleportPlayer();
                        }
                    }
                }
            }
        }


        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            inRoom = true;
        }

        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            inRoom = false;
        }
    }
}
