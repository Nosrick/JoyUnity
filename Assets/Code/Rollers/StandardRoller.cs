using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace JoyLib.Code.Rollers
{
    public class StandardRoller : IRollable
    {
        public int Roll(IRollableValue[] statistics, IGrowingValue[] skills, IAbility[] modifiers, params string[] tags)
        {
            int successes = 0;

            List<IAbility> applicableAbilities = new List<IAbility>();
            foreach (IAbility ability in modifiers)
            {
                int matches = 0;
                foreach (string tag in tags)
                {
                    if (ability.HasTag(tag))
                    {
                        matches += 1;
                    }
                }
                if (matches == tags.Length)
                {
                    applicableAbilities.Add(ability);
                }
            }

            int newThreshold = GlobalConstants.DEFAULT_SUCCESS_THRESHOLD;
            foreach(IRollableValue statistic in statistics)
            {
                if(statistic.SuccessThreshold != GlobalConstants.DEFAULT_SUCCESS_THRESHOLD)
                {
                    newThreshold += AlgorithmsElf.Difference(newThreshold, 
                        AlgorithmsElf.Difference(GlobalConstants.DEFAULT_SUCCESS_THRESHOLD, statistic.SuccessThreshold));
                }
            }

            foreach(IGrowingValue skill in skills)
            {
                if(skill.SuccessThreshold != GlobalConstants.DEFAULT_SUCCESS_THRESHOLD)
                {
                    newThreshold += AlgorithmsElf.Difference(newThreshold,
                        AlgorithmsElf.Difference(GlobalConstants.DEFAULT_SUCCESS_THRESHOLD, skill.SuccessThreshold));
                }
            }

            int newNumber = statistics.Sum(stat => stat.Value) + skills.Sum(skill => skill.Value);

            foreach (IAbility ability in applicableAbilities)
            {
                newThreshold = ability.OnCheckRollModifyThreshold(newThreshold);
                newNumber = ability.OnCheckRollModifyDice(newNumber);
            }

            for (int i = 0; i < newNumber; i++)
            {
                int result = RNG.instance.Roll(1, 10);

                if (result == 1)
                {
                    successes -= 1;
                }
                else if (result == 10)
                {
                    successes += 2;
                }
                else if (result >= newThreshold)
                {
                    successes += 1;
                }
            }

            foreach (IAbility ability in applicableAbilities)
            {
                successes = ability.OnCheckSuccess(successes);
            }

            return successes;
        }
    }
}
