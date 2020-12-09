using System;
using System.Collections.Generic;

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
                AbilityTarget.Adjacent)
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
    }
}