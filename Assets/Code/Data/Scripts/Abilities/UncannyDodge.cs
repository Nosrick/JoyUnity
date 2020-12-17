using System;
using System.Collections.Generic;
using JoyLib.Code.Entities.Statistics;

namespace JoyLib.Code.Entities.Abilities
{
    public class UncannyDodge : AbstractAbility
    {
        public UncannyDodge()
            :base(
                "keen reflexes",
                "keenreflexes",
                "Reduces any incoming damage by your Cunning value.",
                false,
                0,
                0,
                0,
                false,
                new string[0],
                new Tuple<string, int>[0],
                GetPrerequisites(),
                AbilityTarget.Self,
                new[] {"defend", "success", "cunning"})
        {}
        
        protected static Dictionary<string, int> GetPrerequisites()
        {
            Dictionary<string, int> prereqs = new Dictionary<string, int>();
            prereqs.Add("agility", 6);
            prereqs.Add("cunning", 6);
            return prereqs;
        }

        public override int OnTakeHit(IEntity attacker, IEntity defender, int damage)
        {
            damage -= (defender.Statistics[EntityStatistic.AGILITY].Value +
                       defender.Statistics[EntityStatistic.CUNNING].Value);

            return damage;
        }
    }
}