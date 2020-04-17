using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using System.Reflection;
using UnityEngine;

namespace GiganticAmethyst
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [R2APISubmoduleDependency(new string[]
    {
        "ItemAPI",
        "ItemDropAPI",
        "ResourcesAPI",
        "AssetPlus",
    })]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    public class GiganticAmethyst : BaseUnityPlugin
    {
        private const string ModVer = "1.1.0";
        private const string ModName = "GiganticAmethyst";
        private const string ModGuid = "com.RicoValdezio.GiganticAmethyst";

        public static ConfigEntry<bool> RoR1Behavior;
        public static ConfigEntry<float> Cooldown;

        public static GiganticAmethyst instance;
        private void Awake()
        {

            if (GiganticAmethyst.instance == null)
            {
                GiganticAmethyst.instance = this;
            }

            GiganticAmethystConfig.Init();
            GiganticAmethystEquip.Init();
            GiganticAmethystHook.Init();
        }
    }
    internal class GiganticAmethystConfig
    {
        internal static void Init()
        {
            GiganticAmethyst.RoR1Behavior = GiganticAmethyst.instance.Config.Bind<bool>("MainSettings", "RoR1 Behavior", true, "Determines if equipment matches the RoR1 behavior and fully restocks the skills, or if it only refills a single stock.");
            GiganticAmethyst.Cooldown = GiganticAmethyst.instance.Config.Bind<float>("MainSettings", "Cooldown", 8, "Length of the cooldown in seconds.");
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
            R2API.AssetPlus.Languages.AddToken("AMETHYST_NAME_TOKEN", "Gigantic Amethyst");
            R2API.AssetPlus.Languages.AddToken("AMETHYST_PICKUP_TOKEN", "<style=cIsUtility>Reset</style> all your cooldowns.");
            R2API.AssetPlus.Languages.AddToken("AMETHYST_DESCRIPTION_TOKEN", "<style=cIsUtility>Reset</style> all your cooldowns on activation.");
            R2API.AssetPlus.Languages.AddToken("AMETHYST_LORE_TOKEN", "I highly suggest handling this thing with some form of protective gear; we're not sure if it has any effects on the human body.");
            EquipmentDef AmethystEquipmentDef = new EquipmentDef
            {
                name = "AMETHYST_NAME_TOKEN",
                cooldown = GiganticAmethyst.Cooldown.Value,
                pickupModelPath = PrefabPath,
                pickupIconPath = IconPath,
                nameToken = "AMETHYST_NAME_TOKEN",
                pickupToken = "AMETHYST_PICKUP_TOKEN",
                descriptionToken = "AMETHYST_DESCRIPTION_TOKEN",
                loreToken = "AMETHYST_LORE_TOKEN",
                canDrop = true,
                enigmaCompatible = true
            };

            ItemDisplayRule[] AmethystDisplayRules = null;

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
                    SkillLocator skillLocator = self.characterBody.skillLocator;
                    if (skillLocator)
                    {
                        if (GiganticAmethyst.RoR1Behavior.Value)
                        {
                            skillLocator.ResetSkills();
                        }
                        else
                        {
                            skillLocator.ApplyAmmoPack();
                        }
                    }
                    return true;
                }
                return orig(self, equipmentIndex);
            };
        }
    }
}