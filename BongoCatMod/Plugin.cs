using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace BongoCatMod
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }
        public static ConfigEntry<bool> AutoBuyEnabled;
        public static ConfigEntry<int> ClickMultiplier;
        private static Harmony _harmony;

        public new static BepInEx.Logging.ManualLogSource Logger { get; private set; }

        private void Awake()
        {
            Instance = this;
            Logger = base.Logger;

            // Config
            AutoBuyEnabled = Config.Bind("General", "AutoBuyEnabled", true, "Enable automatic chest buying");
            ClickMultiplier = Config.Bind("General", "ClickMultiplier", 1000, "Multiplier for click counts (default: 1000)");

            // Add config change listeners
            AutoBuyEnabled.SettingChanged += (sender, args) =>
            {
                Logger.LogInfo($"Auto Buy setting changed to: {AutoBuyEnabled.Value}");
            };

            ClickMultiplier.SettingChanged += (sender, args) =>
            {
                Logger.LogInfo($"Click Multiplier setting changed to: {ClickMultiplier.Value}x");
            };

            // Apply Harmony patches
            _harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            _harmony.PatchAll(typeof(ShopPatch));
            _harmony.PatchAll(typeof(GlobalKeyHookPatch));

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            Logger.LogInfo($"Auto Buy: {AutoBuyEnabled.Value}");
            Logger.LogInfo($"Click Multiplier: {ClickMultiplier.Value}x");
        }
    }

    // Patch for automatic chest buying
    [HarmonyPatch]
    public class ShopPatch
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method("BongoCat.Shop:TimerUpdate");
        }

        static IEnumerator Postfix(IEnumerator original, object __instance)
        {
            while (original.MoveNext())
            {
                yield return original.Current;

                // Check if auto-buy is enabled
                if (Plugin.AutoBuyEnabled.Value)
                {
                    var stockRefreshTimeLeft = Traverse.Create(__instance).Field("StockRefreshTimeLeft").GetValue<int>();

                    if (stockRefreshTimeLeft <= 0)
                    {
                        var shopItem = Traverse.Create(__instance).Field("_shopItem").GetValue();
                        var canBuy = Traverse.Create(shopItem).Method("CanBuy").GetValue<bool>();

                        if (canBuy)
                        {
                            var isEmoteShop = Traverse.Create(__instance).Field("_isEmoteShop").GetValue<bool>();

                            // Wait if it's an emote shop
                            if (isEmoteShop)
                            {
                                yield return new WaitForSecondsRealtime(3f);
                            }

                            // Buy the item
                            Traverse.Create(shopItem).Method("Buy").GetValue();
                        }
                    }
                }
            }
        }
    }

    // Patch for click multiplier
    // This patches: this._keysDown += GlobalKeyHook.IsDown.Count((bool x) => x);
    // To: this._keysDown += GlobalKeyHook.IsDown.Count((bool x) => x) * ClickMultiplier;
    [HarmonyPatch]
    public class GlobalKeyHookPatch
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method("BongoCat.GlobalKeyHook:Process");
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            var multiplierMethod = AccessTools.Method(typeof(GlobalKeyHookPatch), nameof(GetMultiplier));

            bool patched = false;

            // Find the pattern: ldfld _keysDown, <count operation>, add, stfld _keysDown
            // We want to insert the multiplier call between the count and the add
            for (int i = 0; i < codes.Count - 1; i++)
            {
                // Look for the Add opcode followed by stfld (storing to _keysDown field)
                if (codes[i].opcode == OpCodes.Add && i + 1 < codes.Count && codes[i + 1].opcode == OpCodes.Stfld)
                {
                    // Check if the field being stored to is _keysDown
                    var fieldInfo = codes[i + 1].operand as FieldInfo;
                    if (fieldInfo != null && fieldInfo.Name == "_keysDown")
                    {
                        // Insert multiplier call right before the Add instruction
                        codes.Insert(i, new CodeInstruction(OpCodes.Call, multiplierMethod));
                        Plugin.Logger.LogInfo($"Click multiplier patch applied successfully at instruction {i}");
                        patched = true;
                        break;
                    }
                }
            }

            if (!patched)
            {
                Plugin.Logger.LogWarning("Failed to find injection point for click multiplier patch!");
            }

            return codes.AsEnumerable();
        }

        static int GetMultiplier(int value)
        {
            return value * Plugin.ClickMultiplier.Value;
        }
    }
}
