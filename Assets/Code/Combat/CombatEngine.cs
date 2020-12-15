using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities;
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
            List<EntityStatistic> attackerStatistics = attacker.Statistics
                .Where(pair => attackerTags.Any(tag => tag.Equals(pair.Key, StringComparison.OrdinalIgnoreCase)))
                .Select(pair => pair.Value).ToList();

            List<EntitySkill> attackerSkills = attacker.Skills
                .Where(pair => attackerTags.Any(tag => tag.Equals(pair.Key, StringComparison.OrdinalIgnoreCase)))
                .Select(pair => pair.Value).ToList();
            
            List<EntityStatistic> defenderStatistics = defender.Statistics
                .Where(pair => defenderTags.Any(tag => tag.Equals(pair.Key, StringComparison.OrdinalIgnoreCase)))
                .Select(pair => pair.Value).ToList();

            List<EntitySkill> defenderskills = defender.Skills
                .Where(pair => defenderTags.Any(tag => tag.Equals(pair.Key, StringComparison.OrdinalIgnoreCase)))
                .Select(pair => pair.Value).ToList();

            int attackerSuccesses = 0;
            foreach(EntityStatistic stat in attackerStatistics)
            {
                attackerSuccesses += this.Roller.RollSuccesses(
                    stat.Value,
                    stat.SuccessThreshold);
            }
            foreach (EntitySkill skill in attackerSkills)
            {
                attackerSuccesses += this.Roller.RollSuccesses(
                    skill.Value,
                    skill.SuccessThreshold);
            }

            int defenderSuccesses = 0;
            foreach(EntityStatistic stat in defenderStatistics)
            {
                defenderSuccesses += this.Roller.RollSuccesses(
                    stat.Value,
                    stat.SuccessThreshold);
            }
            foreach (EntitySkill skill in defenderskills)
            {
                defenderSuccesses += this.Roller.RollSuccesses(
                    skill.Value,
                    skill.SuccessThreshold);
            }

            return attackerSuccesses - defenderSuccesses;
        }
    }
}