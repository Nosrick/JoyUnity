using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Helpers;
using JoyLib.Code.Quests;

namespace JoyLib.Code.Combat
{
    public static class CombatEngine
    {
        public static void PerformCombat(Entity attackerRef, Entity defenderRef)
        {
            ItemInstance attackerMainWeapon = attackerRef.GetEquipment("Hand1");
            ItemInstance attackerOffWeapon = attackerRef.GetEquipment("Hand2");

            if (attackerMainWeapon != null)
            {
                int totalDamage = SwingWeapon(attackerRef, defenderRef, attackerMainWeapon);

                if (totalDamage > 0)
                {
                    defenderRef.DamageMe(totalDamage);
                }
            }

            if (attackerOffWeapon == null || attackerOffWeapon.GUID == attackerMainWeapon.GUID)
                return;

            int offHandDamage = SwingWeapon(attackerRef, defenderRef, attackerOffWeapon);

            if (offHandDamage > 0)
            {
                defenderRef.DamageMe(offHandDamage);
            }

            if(!defenderRef.Alive)
            {
                attackerRef.FulfillNeed(NeedIndex.Morality, defenderRef.Sentient ? -5 : -1, 0);
                attackerRef.AddExperience(defenderRef.Level);
                QuestTracker.PerformEntityDestruction(attackerRef, defenderRef);
            }
            else if(!attackerRef.Alive)
            {
                defenderRef.AddExperience(attackerRef.Level);
                QuestTracker.PerformEntityDestruction(defenderRef, attackerRef );
            }
        }

        public static int SwingWeapon(Entity attackerRef, JoyObject defenderRef, ItemInstance weaponRef, bool log = true)
        {
            if(weaponRef == null)
            {
                return 0;
            }

            int attackerToHit = RNG.Roll(1, attackerRef.Statistics[StatisticIndex.Agility]);
            int attackerSkillBonus = 0;
            if (weaponRef.ItemType.GoverningSkill != "None")
            {
                if (attackerRef.Skills[weaponRef.ItemType.GoverningSkill].value > 0)
                {
                    attackerSkillBonus = RNG.Roll(1, attackerRef.Skills[weaponRef.ItemType.GoverningSkill].value);
                }
            }
            else
            {
                attackerSkillBonus = RNG.Roll(1, attackerRef.Skills["Throwing"].value);
            }

            attackerToHit += attackerSkillBonus;

            if (defenderRef.GetType().Equals(typeof(Entity)))
            {
                Entity defender = (Entity)defenderRef;
                int defenderDodge = RNG.Roll(1, (int)defender.Statistics[StatisticIndex.Agility]);
                int defenderEvasion = 0;

                if (defender.Skills["Evasion"].value > 0)
                    defenderEvasion = RNG.Roll(1, defender.Skills["Evasion"].value);

                defenderDodge += defenderEvasion;

                if (attackerToHit < defenderDodge)
                {
                    if(log)
                        ActionLog.AddText(attackerRef.JoyName+ " misses " + defenderRef.JoyName, LogType.Information);
                    return 0;
                }
            }

            int totalDamage = attackerToHit + attackerSkillBonus + RNG.Roll(1, attackerRef.Statistics[StatisticIndex.Strength]);

            if (log)
                ActionLog.AddText(attackerRef.JoyName + " " + weaponRef.ItemType.ActionString + " " + defenderRef.JoyName + " for " + totalDamage, LogType.Information);

            return totalDamage;
        }
    }
}
