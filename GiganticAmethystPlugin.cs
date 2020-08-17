using BepInEx;
using R2API.Utils;

namespace GiganticAmethyst
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [R2APISubmoduleDependency(new string[] { "ItemAPI", "ItemDropAPI", "ResourcesAPI", "LanguageAPI", "AssetPlus" })]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class GiganticAmethyst : BaseUnityPlugin
    {
        private const string ModVer = "1.2.0";
        private const string ModName = "GiganticAmethyst";
        private const string ModGuid = "com.RicoValdezio.GiganticAmethyst";

        public static GiganticAmethyst instance;
        private void Awake()
        {
            if (instance == null) instance = this;
            GiganticAmethystConfig.Init();
            GiganticAmethystEquip.Init();
            GiganticAmethystHook.Init();
        }
    }
}