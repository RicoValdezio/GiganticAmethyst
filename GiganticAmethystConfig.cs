namespace GiganticAmethyst
{
    class GiganticAmethystConfig
    {
        internal static bool fullReset;
        internal static float cooldown;
        internal static void Init()
        {
            fullReset = GiganticAmethyst.instance.Config.Bind("MainSettings", "Full Skill Reset", true, "Determines if usage refills all stocks (true), or only a single stock (false).").Value;
            cooldown = GiganticAmethyst.instance.Config.Bind<float>("MainSettings", "Cooldown", 8, "Length of the cooldown in seconds.").Value;
        }
    }
}
