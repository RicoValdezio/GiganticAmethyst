using RoR2;

namespace GiganticAmethyst
{
    class GiganticAmethystHook
    {
        internal static void Init()
        {
            On.RoR2.EquipmentSlot.PerformEquipmentAction += (orig, self, equipmentIndex) =>
            {
                if (equipmentIndex == GiganticAmethystEquip.index)
                {
                    SkillLocator skillLocator = self.characterBody.skillLocator;
                    if (skillLocator)
                    {
                        if (GiganticAmethystConfig.fullReset) skillLocator.ResetSkills();
                        else skillLocator.ApplyAmmoPack();
                    }
                    return true;
                }
                return orig(self, equipmentIndex);
            };
        }
    }
}
