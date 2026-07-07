using HarmonyLib;
using UnityEngine;

namespace VSPlus.Patches
{
    [HarmonyPatch(typeof(VirtualStumpTeleporter), "OnTriggerEnter")]
    class TeleportCountdownChange
    {
        static void Prefix(VirtualStumpTeleporter __instance)
        {
            Traverse.Create(__instance)
                .Field("stayInTriggerDuration")
                .SetValue(Plugin.TeleportCountdown.Value);
        }
    }
}