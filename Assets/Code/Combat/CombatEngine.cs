using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Helpers;
using JoyLib.Code.Quests;

namespace JoyLib.Code.Combat
{
    public static class CombatEngine
    {
        /*
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
        */

        //PUT IN EVENT PROCESSING TRIGGERS AND STUFF
        public static int SwingWeapon(Entity attackerRef, JoyObject defenderRef, bool log = true)
        {
            ItemInstance mainHand = attackerRef.GetEquipment("Hand1");
            ItemInstance offHand = attackerRef.GetEquipment("Hand2");

            if(mainHand == null)
            {
                return 0;
            }

            int attackerToHit = RNG.RollSuccesses(attackerRef.Statistics[StatisticIndex.Agility].Value, attackerRef.Statistics[StatisticIndex.Agility].SuccessThreshold);
            int attackerSkillBonus = 0;
            if (mainHand.ItemType.GoverningSkill != "None")
            {
                if (attackerRef.Skills[mainHand.ItemType.GoverningSkill].value > 0)
                {
                    attackerSkillBonus = RNG.RollSuccesses(attackerRef.Skills[mainHand.ItemType.GoverningSkill].value, attackerRef.Skills[mainHand.ItemType.GoverningSkill].SuccessThreshold);
                }
            }
            else
            {
                attackerSkillBonus = RNG.RollSuccesses(attackerRef.Skills["Throwing"].value, attackerRef.Skills["Throwing"].SuccessThreshold);
            }

            attackerToHit += attackerSkillBonus;

            if (defenderRef.GetType().Equals(typeof(Entity)))
            {
                Entity defender = (Entity)defenderRef;
                int defenderDodge = RNG.RollSuccesses(attackerRef.Statistics[StatisticIndex.Agility].Value, attackerRef.Statistics[StatisticIndex.Agility].SuccessThreshold);
                int defenderEvasion = 0;

                if (defender.Skills["Evasion"].value > 0)
                {
                    defenderEvasion = RNG.RollSuccesses(defender.Skills["Evasion"].value, defender.Skills["Evasion"].SuccessThreshold);
                }

                defenderDodge += defenderEvasion;

                if (attackerToHit < defenderDodge)
                {
                    if (log)
                    {
                        ActionLog.AddText(attackerRef.JoyName + " misses " + defenderRef.JoyName, LogType.Information);
                    }

                    return 0;
                }
            }

            int totalDamage = attackerToHit + attackerSkillBonus + RNG.RollSuccesses(attackerRef.Statistics[StatisticIndex.Strength].Value, attackerRef.Statistics[StatisticIndex.Strength].SuccessThreshold);

            if (log)
            {
                ActionLog.AddText(attackerRef.JoyName + " " + mainHand.ItemType.ActionString + " " + defenderRef.JoyName + " for " + totalDamage, LogType.Information);
            }

            return totalDamage;
        }
    }
}
