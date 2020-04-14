using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using System.Reflection;
using UnityEngine;

namespace GiganticAmethyst
{
    [BepInDependency("com.bepis.r2api")]
    [R2APISubmoduleDependency(nameof(ItemAPI), nameof(ItemDropAPI), nameof(ResourcesAPI))]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    public class WickedRing : BaseUnityPlugin
    {
        private const string ModVer = "1.0.0";
        private const string ModName = "GiganticAmethyst";
        private const string ModGuid = "com.RicoValdezio.GiganticAmethyst";

        private void Awake()
        {
            GiganticAmethystEquip.Init();
            GiganticAmethystHook.Init();
        }
    }

    internal class GiganticAmethystEquip
    {
        internal static GameObject AmethystPrefab;
        internal static EquipmentIndex AmethystEquipmentIndex;
        internal static AssetBundleResourcesProvider AmethystProvider;
        internal static AssetBundle AmethystBundle;

        private const string ModPrefix = "@GiganticAmethyst:";
        private const string PrefabPath = ModPrefix + "Assets/Amethyst.prefab";
        private const string IconPath = ModPrefix + "Assets/Amethyst_Icon.png";

        internal static void Init()
        {
            using (System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("GiganticAmethyst.amethyst"))
            {
                AmethystBundle = AssetBundle.LoadFromStream(stream);
                AmethystProvider = new AssetBundleResourcesProvider(ModPrefix.Trim(':'), AmethystBundle);
                ResourcesAPI.AddProvider(AmethystProvider);
                AmethystPrefab = AmethystBundle.LoadAsset<GameObject>("Assets/Amethyst.prefab");
            };

            AmethystAsEquip();
        }

        private static void AmethystAsEquip()
        {
            EquipmentDef AmethystEquipmentDef = new EquipmentDef
            {
                name = "GiganticAmethystEquipment",
                cooldown = 30f,
                pickupModelPath = PrefabPath,
                pickupIconPath = IconPath,
                nameToken = "Gigantic Amethyst",
                pickupToken = "<style=cIsUtility>Reset</style> all your cooldowns.",
                descriptionToken = "<style=cIsUtility>Reset</style> all your cooldowns.",
                loreToken = "Used for focus lasers, so they say.",
                canDrop = true,
                enigmaCompatible = true
            };
            
            ItemDisplayRule[] AmethystDisplayRules = new ItemDisplayRule[1];
            AmethystDisplayRules[0].followerPrefab = AmethystPrefab;
            AmethystDisplayRules[0].childName = "Chest";
            AmethystDisplayRules[0].localScale = new Vector3(10f, 10f, 10f);
            AmethystDisplayRules[0].localAngles = new Vector3(0f, 0f, 0f);
            AmethystDisplayRules[0].localPos = new Vector3(0f, 0f, 0f);

            CustomEquipment AmethystEquipment = new CustomEquipment(AmethystEquipmentDef, AmethystDisplayRules);

            AmethystEquipmentIndex = ItemAPI.Add(AmethystEquipment);
        }
    }

    internal class GiganticAmethystHook
    {
        internal static void Init()
        {
            On.RoR2.EquipmentSlot.PerformEquipmentAction += (orig, self, equipmentIndex) =>
            {
                if (equipmentIndex == GiganticAmethystEquip.AmethystEquipmentIndex)
                {
                    SkillLocator Skills = self.characterBody.skillLocator;
                    if (Skills.primary)
                    {
                        Skills.primary.Reset();
                    }
                    if (Skills.secondary)
                    {
                        Skills.secondary.Reset();
                    }
                    if (Skills.utility)
                    {
                        Skills.utility.Reset();
                    }
                    if (Skills.special)
                    {
                        Skills.special.Reset();
                    }
                    return true;
                }

                return orig(self, equipmentIndex);
            };
        }
    }
}
