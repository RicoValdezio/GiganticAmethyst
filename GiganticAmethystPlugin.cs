using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using System.Reflection;
using UnityEngine;

namespace GiganticAmethyst
{
    [BepInDependency("com.bepis.r2api",  BepInDependency.DependencyFlags.HardDependency)]
    //Sushi/Engima - Use an array to request submodules
    [R2APISubmoduleDependency(new string[]
    {
        "ItemAPI",
        "ItemDropAPI",
        "ResourcesAPI",
    })]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    //Sushi/Engima - Was this a wickedring mod?
    //Sushi/Engima - You can hook the itemcatalog, check if the itemdef is the wickedring, then make your changes if you're still interested in making one. 
    //Sushi/Engima - public class WickedRing : BaseUnityPlugin
    public class GiganticAmethyst : BaseUnityPlugin
    {
        private const string ModVer = "1.1.0";
        private const string ModName = "GiganticAmethyst";
        private const string ModGuid = "com.RicoValdezio.GiganticAmethyst";

        //Sushi/Engima - Make it static so it's accessible
        public static ConfigEntry<bool> RoR1Behavior;
        public static ConfigEntry<float> Cooldown;
        private void Awake()
        {
            //Sushi/Engima - Set our values before doing anything else
            InitConfig();

            GiganticAmethystEquip.Init();

            //Sushi/Engima - Subscribe instead of making a whole new class. 
            On.RoR2.EquipmentSlot.PerformEquipmentAction += EquipmentSlot_PerformEquipmentAction;
        }
        private bool EquipmentSlot_PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentIndex equipmentIndex)
        {
            if (equipmentIndex == GiganticAmethystEquip.AmethystEquipmentIndex)
            {
                SkillLocator skillLocator = self.characterBody.skillLocator;
                if (skillLocator)
                {
                    //Sushi/Engima - There's one aspect of mod development that you should know of
                    //Sushi/Engima - Praying to god that the mod doesn't need additional networking
                    if (RoR1Behavior.Value) skillLocator.ResetSkills();
                    //Sushi/Engima - It's an on and off behavior, if it isn't on it's off and vice versa.
                    else skillLocator.ApplyAmmoPack();
                    //Sushi/Engima - Watch it completely error though!
                }
                return true;
            }
            //Sushi/Engima - Gotta do this.
            return orig(self, equipmentIndex);
        }
        private void InitConfig()
        {
            //Sushi/Enigma - y'know, this reminds me of a github pull request
            //https://github.com/space-wizards/space-station-14/pull/801
            RoR1Behavior = Config.Bind<bool>(
            "RoR1Behavior",
            "Uses the RoR1 behavior. Turn this off to use an alternate behavior.",
            true
            );
            //Sushi/Enigma - Made a cooldown option because let's be completely honest here, no one wants to dive into the code every time they want to change a fucking int.
            Cooldown = Config.Bind<float>(
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

        //Sushi/Engima - I haven't done assetbundle stuff at all.
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
                //Sushi/Engima - Doubled the cooldown
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
}
