namespace GiganticAmethyst
{
    class GiganticAmethystConfig
    {
        internal static void Init()
        {
            GiganticAmethyst.RoR1Behavior = GiganticAmethyst.instance.Config.Bind<bool>("MainSettings", "RoR1 Behavior", true, "Determines if equipment matches the RoR1 behavior and fully restocks the skills, or if it only refills a single stock.");
            GiganticAmethyst.Cooldown = GiganticAmethyst.instance.Config.Bind<float>("MainSettings", "Cooldown", 8, "Length of the cooldown in seconds.");
        }
    }
}
