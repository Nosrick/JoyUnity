using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Collections;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Scripting;

namespace JoyLib.Code.Entities.Abilities
{
    public class AbilityHandler : IAbilityHandler
    {
        protected List<IAbility> Abilities { get; set; }

        public AbilityHandler()
        {
            Initialise();
        }

        private void Load()
        {
            Abilities = new List<IAbility>();
            Abilities.AddRange(ScriptingEngine.instance.FetchAndInitialiseChildren<IAbility>());
        }

        public bool Initialise()
        {
            if (Abilities is null)
            {
                Load();
            }
                
            return true;
        }

        public IAbility GetAbility(string nameRef)
        {
            Initialise();
            
            if(Abilities.Any(x => x.InternalName.Equals(nameRef, StringComparison.OrdinalIgnoreCase)))
            {
                return Abilities.First(x => x.InternalName.Equals(nameRef, StringComparison.OrdinalIgnoreCase));
            }
            throw new InvalidOperationException("Could not find IAbility with name " + nameRef);
        }

        public IEnumerable<IAbility> GetAvailableAbilities(IEntity actor)
        {
            Initialise();

            IEnumerable<string> prereqs =
                Abilities.SelectMany(ability => ability.Prerequisites.Select(pair => pair.Key)).Distinct();

            IEnumerable<Tuple<string, int>> data = actor.GetData(prereqs);

            return Abilities.Where(ability => ability.MeetsPrerequisites(data));
        }

        public IEnumerable<IAbility> GetAvailableAbilities(
            IEntityTemplate template, 
            BasicValueContainer<IRollableValue> stats, 
            BasicValueContainer<IGrowingValue> skills)
        {
            Initialise();

            List<Tuple<string, int>> data =
                stats.Select(stat => new Tuple<string, int>(stat.Key, stat.Value.Value)).ToList();

            data.AddRange(skills.Select(skill => new Tuple<string, int>(skill.Key, skill.Value.Value)));
            data.Add(new Tuple<string, int>(template.CreatureType, 1));

            return Abilities.Where(ability => ability.MeetsPrerequisites(data));
        }
    }
}
