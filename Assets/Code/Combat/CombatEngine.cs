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
        protected IRollable Roller { get; set; }

        public CombatEngine(IRollable roller = null)
        {
            this.Roller = roller is null ? new RNG() : roller;
        }

        public int MakeAttack(
            IEntity attacker,
            IEntity defender,
            string[] attackerTags,
            string[] defenderTags)
        {
            List<IRollableValue<int>> attackerStuff = attacker.Statistics
                .Where(pair => attackerTags.Any(tag => tag.Equals(pair.Key, StringComparison.OrdinalIgnoreCase)))
                .Select(pair => pair.Value).ToList();

            attackerStuff.AddRange(attacker.Skills
                .Where(pair => attackerTags.Any(tag => tag.Equals(pair.Key, StringComparison.OrdinalIgnoreCase)))
                .Select(pair => pair.Value));

            List<IRollableValue<int>> defenderStuff = defender.Statistics
                .Where(pair => defenderTags.Any(tag => tag.Equals(pair.Key, StringComparison.OrdinalIgnoreCase)))
                .Select(pair => pair.Value).ToList();

            defenderStuff.AddRange(defender.Skills
                .Where(pair => defenderTags.Any(tag => tag.Equals(pair.Key, StringComparison.OrdinalIgnoreCase)))
                .Select(pair => pair.Value));

            List<IItemInstance> attackerWeapons = attacker.Equipment.Select(tuple => tuple.Item2).Where(instance =>
                instance.Tags.Any(tag => tag.Equals("weapon", StringComparison.OrdinalIgnoreCase))).ToList();

            List<IItemInstance> defenderArmour = defender.Equipment.Select(tuple => tuple.Item2).Where(instance =>
                    instance.Tags.Any(tag => tag.Equals("armour", StringComparison.OrdinalIgnoreCase)))
                .ToList();

            List<IAbility> attackerAbilities = attacker.Abilities.Where(ability =>
                ability.Tags.Intersect(attackerTags).Count() > 0).ToList();

            List<IAbility> defenderAbilities = defender.Abilities.Where(ability =>
                ability.Tags.Intersect(attackerTags).Count() > 0).ToList();

            attackerAbilities.ForEach(ability => ability.OnAttack(
                attacker, 
                defender, 
                attackerTags, 
                defenderTags));
            
            int attackerSuccesses = 0;
            int totalDice = 0;
            int successThreshold = GlobalConstants.DEFAULT_SUCCESS_THRESHOLD;
            foreach (IRollableValue<int> stat in attackerStuff)
            {
                totalDice += stat.Value;
                successThreshold = Math.Min(successThreshold, stat.SuccessThreshold);
            }
            
            attackerAbilities.ForEach(ability => totalDice = ability.OnCheckRollModifyDice(
                totalDice, 
                attackerStuff, 
                attackerTags, 
                defenderTags));
            attackerAbilities.ForEach(ability =>
                successThreshold = ability.OnCheckRollModifyThreshold(
                    successThreshold, 
                    attackerStuff, 
                    attackerTags, 
                    defenderTags));
                
            attackerSuccesses = this.Roller.RollSuccesses(
                totalDice,
                successThreshold);
            
            attackerAbilities.ForEach(ability =>
                attackerSuccesses = ability.OnCheckSuccess(
                    attackerSuccesses, 
                    attackerStuff, 
                    attackerTags, 
                    defenderTags));

            int defenderSuccesses = 0;
            totalDice = 0;
            successThreshold = GlobalConstants.DEFAULT_SUCCESS_THRESHOLD;
            foreach (IRollableValue<int> stat in defenderStuff)
            {
                totalDice += stat.Value;
                successThreshold = Math.Min(successThreshold, stat.SuccessThreshold);
            }

            defenderAbilities.ForEach(ability => totalDice = ability.OnCheckRollModifyDice(
                totalDice, 
                defenderStuff, 
                attackerTags, 
                defenderTags));
            defenderAbilities.ForEach(ability =>
                successThreshold = ability.OnCheckRollModifyThreshold(
                    successThreshold, 
                    defenderStuff, 
                    attackerTags, 
                    defenderTags));

            defenderSuccesses = this.Roller.RollSuccesses(
                totalDice,
                successThreshold);

            defenderAbilities.ForEach(ability =>
                defenderSuccesses = ability.OnCheckSuccess(
                    defenderSuccesses, 
                    defenderStuff, 
                    attackerTags, 
                    defenderTags));

            defenderAbilities.ForEach(ability => defenderSuccesses = ability.OnTakeHit(
                attacker, 
                defender, 
                attackerSuccesses, 
                attackerTags, 
                defenderTags));

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