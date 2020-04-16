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
    //Sushi/Engima - Use an array to request submodules
    [R2APISubmoduleDependency(new string[]
    {
        "ItemAPI",
        "ItemDropAPI",
        "ResourcesAPI",
    })]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    public class GiganticAmethyst : BaseUnityPlugin
    {
        //Sushi/Enigma - Up the version
        private const string ModVer = "1.2.0";
        private const string ModName = "GiganticAmethyst";
        private const string ModGuid = "com.RicoValdezio.GiganticAmethyst";

        //Sushi/Engima - Make it static so it's accessible
        public static ConfigEntry<bool> RoR1Behavior;
        public static ConfigEntry<float> Cooldown;

        //Sushi/Enigma - Make an instance of the plugin so we can do stuff like GiganticAmethyst.instance.Config.Bind outside of its class.
        public static GiganticAmethyst instance;
        private void Awake()
        {
            //Sushi/Engima - Set an instance
            if (GiganticAmethyst.instance == null) GiganticAmethyst.instance = this;

            //Set config values before anything else.
            GiganticAmethystConfig.Init();
            GiganticAmethystEquip.Init();
            GiganticAmethystHook.Init();
        }
    }
    internal class GiganticAmethystConfig
    {
        internal static void Init()
        {
            //Sushi/Enigma - y'know, this reminds me of a github pull request for SS14
            //https://github.com/space-wizards/space-station-14/pull/801
            GiganticAmethyst.RoR1Behavior = GiganticAmethyst.instance.Config.Bind<bool>(
            "RoR1Behavior",
            "Uses the RoR1 behavior. Turn this off to use an alternate behavior.",
            true
            );
            //Sushi/Enigma - Made a cooldown option because let's be completely honest here, no one wants to dive into the code every time they want to change a fucking float.
            GiganticAmethyst.Cooldown = GiganticAmethyst.instance.Config.Bind<float>(
            "Cooldown",
            "The cooldown of the equipment. Is a float.",
            8
            );
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
            //Sushi/Engima - Do our token stuff here.
            R2API.AssetPlus.Languages.AddToken("AMETHYST_NAME", "Gigantic Amethyst");
            R2API.AssetPlus.Languages.AddToken("AMETHYST_PICKUP", "<style=cIsUtility>Reset</style> all your cooldowns.");
            R2API.AssetPlus.Languages.AddToken("AMETHYST_DESCRIPTION", "<style=cIsUtility>Reset</style> all your cooldowns on activation.");
            //Sushi/Engima - I changed the lore.
            R2API.AssetPlus.Languages.AddToken("AMETHYST_LORE", "I highly suggest handling this thing with some form of protective gear; we're not sure if it has any effects on the human body.");
            EquipmentDef AmethystEquipmentDef = new EquipmentDef
            {
                name = "AMETHYST_NAME",
                //Sushi/Engima - Set the cooldown to config value
                cooldown = GiganticAmethyst.Cooldown.Value,
                pickupModelPath = PrefabPath,
                pickupIconPath = IconPath,
                //Sushi/Engima - Changed these fields so they correctly used tokens
                nameToken = "AMETHYST_NAME",
                pickupToken = "AMETHYST_PICKUP",
                descriptionToken = "AMETHYST_DESCRIPTION",
                loreToken = "AMETHYST_LORE",
                canDrop = true,
                enigmaCompatible = true
            };

            //Sushi/Engima - idk what this is lol
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
                    SkillLocator skillLocator = self.characterBody.skillLocator;
                    if (skillLocator)
                    {
                        if (GiganticAmethyst.RoR1Behavior.Value) skillLocator.ResetSkills();
                        //Sushi/Engima - Using else because it's an on and off behavior, if it isn't on it's off and vice versa.
                        else skillLocator.ApplyAmmoPack();
                        //Sushi/Engima - Watch it completely error!
                    }
                    return true;
                }
                return orig(self, equipmentIndex);
            };
        }
    }
}