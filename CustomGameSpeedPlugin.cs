using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Eremite.View.HUD;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Eremite.Services;
using BepInEx.Configuration;

namespace CustomGameSpeed
{
    [BepInPlugin(MyGUID, PluginName, VersionString)]
    public class CustomGameSpeedPlugin : BaseUnityPlugin
    {
        private const string MyGUID = "com.madmer.CustomGameSpeed";
        private const string PluginName = "CustomGameSpeed";
        private const string VersionString = "1.0.0";

        private static readonly Harmony Harmony = new Harmony(MyGUID);

        public static ConfigEntry<float> Speed3;
        public static ConfigEntry<float> Speed4;
        public static ConfigEntry<float> Speed5;
        public static ManualLogSource Log;

        private void Awake()
        {
            Log = Logger;

            Speed3 = Config.Bind("General", "Speed 3", 3f, "Custom time scale for slot 3");
            Speed4 = Config.Bind("General", "Speed 4", 5f, "Custom time scale for slot 4");
            Speed5 = Config.Bind("General", "Speed 5", 10f, "Custom time scale for slot 5");

            Harmony.PatchAll();
            Logger.LogInfo($"[CustomGameSpeed] {PluginName} {VersionString} loaded.");
        }
    }

    [HarmonyPatch(typeof(TimeScalePanel), "Start")]
    public static class TimeScalePanelPatch
    {
        static void Postfix(TimeScalePanel __instance)
        {
            __instance.StartCoroutine(WaitAndPatchSlots(__instance));
        }

        static IEnumerator WaitAndPatchSlots(TimeScalePanel panel)
        {
            TimeScaleSlot[] slots = null;
            float timeout = 5f;

            while (timeout > 0f)
            {
                slots = panel.GetComponentsInChildren<TimeScaleSlot>(true);
                if (slots != null && slots.Length >= 5)
                    break;

                timeout -= Time.deltaTime;
                yield return null;
            }

            if (slots == null || slots.Length < 5)
            {
                CustomGameSpeedPlugin.Log.LogWarning("[CustomGameSpeed] TimeScaleSlots not found after waiting.");
                yield break;
            }

            AddSpeedOverride(slots[2], CustomGameSpeedPlugin.Speed3.Value);
            UpdateSlotText(slots[2], $"x{CustomGameSpeedPlugin.Speed3.Value}");
            AddSpeedOverride(slots[3], CustomGameSpeedPlugin.Speed4.Value);
            UpdateSlotText(slots[3], $"x{CustomGameSpeedPlugin.Speed4.Value}");
            AddSpeedOverride(slots[4], CustomGameSpeedPlugin.Speed5.Value);
            UpdateSlotText(slots[4], $"x{CustomGameSpeedPlugin.Speed5.Value}");

            CustomGameSpeedPlugin.Log.LogInfo("[CustomGameSpeed] Custom speed overrides applied to slots 3–5 (delayed)");
        }

        static void AddSpeedOverride(TimeScaleSlot slot, float newSpeed)
        {
            var button = slot.GetComponent<Button>();
            if (button == null)
            {
                CustomGameSpeedPlugin.Log.LogWarning("[CustomGameSpeed] No Button found on TimeScaleSlot.");
                return;
            }

            CustomGameSpeedPlugin.Log.LogInfo($"[CustomGameSpeed] Hooked onClick for {slot.name}, target speed: {newSpeed}");

            button.onClick.AddListener(() =>
            {
                Time.timeScale = newSpeed;
                CustomGameSpeedPlugin.Log.LogInfo($"[CustomGameSpeed] Applied custom time scale: {newSpeed}");
            });
        }

        static void UpdateSlotText(TimeScaleSlot slot, string text)
        {
            var desc = slot.transform.Find("Desc");
            if (desc == null)
            {
                CustomGameSpeedPlugin.Log.LogWarning("[CustomGameSpeed] Desc child not found in slot.");
                return;
            }

            var textComponent = desc.GetComponent<TMPro.TextMeshProUGUI>();
            if (textComponent == null)
            {
                textComponent = desc.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            }

            if (textComponent != null)
            {
                textComponent.text = text;
                CustomGameSpeedPlugin.Log.LogInfo($"[CustomGameSpeed] Updated slot text to '{text}'");
            }
            else
            {
                CustomGameSpeedPlugin.Log.LogWarning("[CustomGameSpeed] TextMeshProUGUI not found in Desc.");
            }
        }

        [HarmonyPatch(typeof(TimeScaleService), "Change")]
        public static class PatchTimeScaleChange
        {
            static void Postfix(float scale)
            {
                float customScale = scale;

                if (Mathf.Approximately(scale, 1.5f)) customScale = CustomGameSpeedPlugin.Speed3.Value;
                else if (Mathf.Approximately(scale, 2f)) customScale = CustomGameSpeedPlugin.Speed4.Value;
                else if (Mathf.Approximately(scale, 3f)) customScale = CustomGameSpeedPlugin.Speed5.Value;

                if (!Mathf.Approximately(Time.timeScale, customScale))
                {
                    Time.timeScale = customScale;
                    CustomGameSpeedPlugin.Log.LogInfo($"[CustomGameSpeed] Forced time scale override: {customScale} (was {scale})");
                }
            }
        }
    }
}
