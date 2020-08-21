using R2API;
using RoR2;
using System;
using System.Reflection;
using UnityEngine;

namespace GiganticAmethyst
{
    class GiganticAmethystEquip
    {
        internal static EquipmentIndex index;

        internal static void Init()
        {
            AddProvider();
            AddTokens();
            AddEquipment();
        }

        private static void AddProvider()
        {
            using (System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("GiganticAmethyst.amethyst"))
            {
                AssetBundle bundle = AssetBundle.LoadFromStream(stream);
                AssetBundleResourcesProvider provider = new AssetBundleResourcesProvider("@Amethyst", bundle);
                ResourcesAPI.AddProvider(provider);
            };
        }

        private static void AddTokens()
        {
            LanguageAPI.Add("AMETHYST_NAME_TOKEN", "Gigantic Amethyst");
            LanguageAPI.Add("AMETHYST_PICK_TOKEN", "<style=cIsUtility>Reset</style> all ability cooldowns.");
            LanguageAPI.Add("AMETHYST_DESC_TOKEN", "<style=cIsUtility>Reset</style> all ability cooldowns on activation.");
            string longLore = "Order: One Gigantic Amethyst" + Environment.NewLine +
                              "Tracking Number: 09******" + Environment.NewLine +
                              "Estimated Delivery: June 26, 2396" + Environment.NewLine +
                              "Shipping Method: Fragile" + Environment.NewLine +
                              "Shipping Address: 69 Main St, Atherton QLD 4883, Australia" + Environment.NewLine +
                              "Shipping Notes:" + Environment.NewLine + Environment.NewLine +
                              "This is the last known fragment of the infamous Empress of Uruguay that your old man used to own. There really must be something up with your family and crystals for you to want this back after the collapse." + Environment.NewLine + Environment.NewLine +
                              "Anyways, handle this thing with care. I have no idea how far it's travelled to get in my hands, and I guarantee that it won't be a short trip home to you.";
            LanguageAPI.Add("AMETHYST_LORE_TOKEN", longLore);
        }

        private static void AddEquipment()
        {
            EquipmentDef def = new EquipmentDef
            {
                name = "AMETHYST_NAME_TOKEN",
                cooldown = GiganticAmethystConfig.cooldown,
                pickupModelPath = "@Amethyst:Assets/Amethyst.prefab",
                pickupIconPath = "@Amethyst:Assets/Amethyst_Icon.png",
                nameToken = "AMETHYST_NAME_TOKEN",
                pickupToken = "AMETHYST_PICK_TOKEN",
                descriptionToken = "AMETHYST_DESC_TOKEN",
                loreToken = "AMETHYST_LORE_TOKEN",
                canDrop = true,
                enigmaCompatible = true
            };

            GameObject followerPrefab = Resources.Load<GameObject>("@Amethyst:Assets/Amethyst.prefab");

            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = followerPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 0.25f, 0f),
                    localAngles = new Vector3(-55f, 0f, 0f),
                    localScale = new Vector3(0.2f, 0.2f, 0.2f)
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = followerPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 0.2f, 0f),
                    localAngles = new Vector3(-55f, 0f, 0f),
                    localScale = new Vector3(0.15f, 0.15f, 0.15f)
                }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = followerPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, -0.5f, 2.5f),
                    localAngles = new Vector3(-65f, 0f, 0f),
                    localScale = new Vector3(1f, 1f, 1f)
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = followerPrefab,
                    childName = "Chest", //Why doesn't Engi have a Head?
                    localPos = new Vector3(0f, 0.65f, 0f),
                    localAngles = new Vector3(-45f, 0f, 0f),
                    localScale = new Vector3(0.2f, 0.2f, 0.2f)
                }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = followerPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 0.07f, -0.05f),
                    localAngles = new Vector3(-60f, 0f, 0f),
                    localScale = new Vector3(0.12f, 0.12f, 0.12f)
                }
            });

            CustomEquipment equip = new CustomEquipment(def, rules);
            index = ItemAPI.Add(equip);
        }
    }
}
