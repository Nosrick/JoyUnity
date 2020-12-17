using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities.Statistics;

namespace JoyLib.Code.Entities.Abilities
{
    public class Backdraft : AbstractAbility
    {
        public Backdraft()
            : base(
                "backdraft",
                "backdraft",
                "Deal heavy damage to a target, but take a portion of the damage dealt.",
                false,
                0,
                0,
                0,
                false,
                new string[0], 
                GetCosts(),
                GetPrerequisites(), 
                AbilityTarget.Adjacent,
                new []{ "attack", "success" })
        {}

        protected static Tuple<string, int>[] GetCosts()
        {
            List<Tuple<string, int>> costs = new List<Tuple<string, int>>();
            costs.Add(new Tuple<string, int>("mana", 5));
            return costs.ToArray();
        }

        protected static Dictionary<string, int> GetPrerequisites()
        {
            Dictionary<string, int> prereqs = new Dictionary<string, int>();
            prereqs.Add("warrior", 1);
            return prereqs;
        }

        public override bool OnAttack(IEntity attacker, IEntity target, IEnumerable<string> attackerTags,
            IEnumerable<string> defenderTags)
        {
            int hp = attacker.DerivedValues[ConcreteDerivedIntValue.HITPOINTS].Value;
            hp -= (hp / 5);
            attacker.ModifyValue(ConcreteDerivedIntValue.HITPOINTS, hp);
            return true;
        }

        public override int OnCheckSuccess(int successes, IEnumerable<IBasicValue<int>> values,
            IEnumerable<string> attackerTags, IEnumerable<string> defenderTags)
        {
            if (attackerTags.Any(tag => tag.Equals("physical", StringComparison.OrdinalIgnoreCase))
            && attackerTags.Any(tag => tag.Equals("attack", StringComparison.OrdinalIgnoreCase)))
            {
                return successes *= 2;
            }
            else
            {
                return successes;
            }
        }
    }
}