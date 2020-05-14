using R2API;
using RoR2;
using System.Reflection;
using UnityEngine;

namespace GiganticAmethyst
{
    class GiganticAmethystEquip
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
            LanguageAPI.Add("AMETHYST_NAME_TOKEN", "Gigantic Amethyst");
            LanguageAPI.Add("AMETHYST_PICKUP_TOKEN", "<style=cIsUtility>Reset</style> all your cooldowns.");
            LanguageAPI.Add("AMETHYST_DESCRIPTION_TOKEN", "<style=cIsUtility>Reset</style> all your cooldowns on activation.");
            LanguageAPI.Add("AMETHYST_LORE_TOKEN", "I highly suggest handling this thing with some form of protective gear; we're not sure if it has any effects on the human body.");
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

            ItemDisplayRule[] AmethystDisplayRules = new ItemDisplayRule[1];


            CustomEquipment AmethystEquipment = new CustomEquipment(AmethystEquipmentDef, AmethystDisplayRules);

            AmethystEquipmentIndex = ItemAPI.Add(AmethystEquipment);
        }
    }
}
