using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Rollers;

namespace JoyLib.Code.Combat
{
    public class CombatEngine : ICombatEngine
    {
        protected RNG Roller { get; set; }

        public CombatEngine(RNG roller = null)
        {
            this.Roller = roller is null ? new RNG() : roller;
        }

        public int MakeAttack(
            IEntity attacker,
            IEntity defender,
            string[] attackerTags,
            string[] defenderTags)
        {
            List<IRollableValue<int>> attackerStatistics = attacker.Statistics
                .Where(pair => attackerTags.Any(tag => tag.Equals(pair.Key, StringComparison.OrdinalIgnoreCase)))
                .Select(pair => pair.Value).ToList();

            List<IEntitySkill> attackerSkills = attacker.Skills
                .Where(pair => attackerTags.Any(tag => tag.Equals(pair.Key, StringComparison.OrdinalIgnoreCase)))
                .Select(pair => pair.Value).ToList();

            List<IRollableValue<int>> defenderStatistics = defender.Statistics
                .Where(pair => defenderTags.Any(tag => tag.Equals(pair.Key, StringComparison.OrdinalIgnoreCase)))
                .Select(pair => pair.Value).ToList();

            List<IEntitySkill> defenderSkills = defender.Skills
                .Where(pair => defenderTags.Any(tag => tag.Equals(pair.Key, StringComparison.OrdinalIgnoreCase)))
                .Select(pair => pair.Value).ToList();

            List<IItemInstance> attackerWeapons = attacker.Equipment.Select(tuple => tuple.Item2).Where(instance =>
                instance.Tags.Any(tag => tag.Equals("weapon", StringComparison.OrdinalIgnoreCase))).ToList();

            List<IItemInstance> defenderArmour = defender.Equipment.Select(tuple => tuple.Item2).Where(instance =>
                    instance.Tags.Any(tag => tag.Equals("armour", StringComparison.OrdinalIgnoreCase)))
                .ToList();

            List<IAbility> attackerAbilities = attacker.Abilities.Where(ability =>
                ability.Tags.Any(tag => tag.Equals("attack", StringComparison.OrdinalIgnoreCase)
                                        || tag.Equals("threshold", StringComparison.OrdinalIgnoreCase)
                                        || tag.Equals("success", StringComparison.OrdinalIgnoreCase))).ToList();

            List<IAbility> defenderAbilities = defender.Abilities.Where(ability =>
                ability.Tags.Any(tag => tag.Equals("defend", StringComparison.OrdinalIgnoreCase)
                                        || tag.Equals("threshold", StringComparison.OrdinalIgnoreCase)
                                        || tag.Equals("success", StringComparison.OrdinalIgnoreCase))).ToList();

            attackerAbilities.ForEach(ability => ability.OnAttack(attacker, defender));
            
            int attackerSuccesses = 0;
            foreach (IRollableValue<int> stat in attackerStatistics)
            {
                int statValue = stat.Value;
                attackerAbilities.ForEach(ability => statValue = ability.OnCheckRollModifyDice(statValue, attackerStatistics));
                
                int successThreshold = stat.SuccessThreshold;
                attackerAbilities.ForEach(ability =>
                    successThreshold = ability.OnCheckRollModifyThreshold(successThreshold, attackerStatistics));
                
                attackerSuccesses += this.Roller.RollSuccesses(
                    statValue,
                    successThreshold);
                attackerAbilities.ForEach(ability =>
                    attackerSuccesses = ability.OnCheckSuccess(attackerSuccesses, attackerStatistics));
            }

            foreach (IEntitySkill skill in attackerSkills)
            {
                int statValue = skill.Value;
                attackerAbilities.ForEach(ability => statValue = ability.OnCheckRollModifyDice(statValue, attackerSkills));
                
                int successThreshold = skill.SuccessThreshold;
                attackerAbilities.ForEach(ability =>
                    successThreshold = ability.OnCheckRollModifyThreshold(successThreshold, attackerSkills));
                
                attackerSuccesses += this.Roller.RollSuccesses(
                    statValue,
                    successThreshold);
                attackerAbilities.ForEach(ability =>
                    attackerSuccesses = ability.OnCheckSuccess(attackerSuccesses, attackerSkills));
            }

            int defenderSuccesses = 0;
            foreach (IRollableValue<int> stat in defenderStatistics)
            {
                int statValue = stat.Value;
                defenderAbilities.ForEach(ability => statValue = ability.OnCheckRollModifyDice(statValue, defenderStatistics));
                
                int successThreshold = stat.SuccessThreshold;
                defenderAbilities.ForEach(ability =>
                    successThreshold = ability.OnCheckRollModifyThreshold(successThreshold, defenderStatistics));
                
                defenderSuccesses += this.Roller.RollSuccesses(
                    statValue,
                    successThreshold);
                defenderAbilities.ForEach(ability =>
                    defenderSuccesses = ability.OnCheckSuccess(attackerSuccesses, defenderStatistics));
            }

            foreach (IEntitySkill skill in defenderSkills)
            {
                int statValue = skill.Value;
                defenderAbilities.ForEach(ability => statValue = ability.OnCheckRollModifyDice(statValue, defenderSkills));
                
                int successThreshold = skill.SuccessThreshold;
                defenderAbilities.ForEach(ability =>
                    successThreshold = ability.OnCheckRollModifyThreshold(successThreshold, defenderSkills));
                
                defenderSuccesses += this.Roller.RollSuccesses(
                    statValue,
                    successThreshold);
                defenderAbilities.ForEach(ability =>
                    defenderSuccesses = ability.OnCheckSuccess(attackerSuccesses, defenderSkills));
            }
            
            defenderAbilities.ForEach(ability => attackerSuccesses = ability.OnTakeHit(attacker, defender, attackerSuccesses));

            int result = attackerSuccesses - defenderSuccesses;
            if (result > 0)
            {
                result += attackerWeapons.Select(instance => instance.Efficiency).Sum();
                result -= defenderArmour.Select(instance => instance.Efficiency).Sum();
                result = Math.Max(0, result);
            }

            return result;
        }
    }
}