using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Scripting;

namespace JoyLib.Code.Entities.Abilities
{
    public class AbilityHandler : IAbilityHandler
    {
        protected List<IAbility> Abilities { get; set; }

        public AbilityHandler()
        {
            this.Initialise();
        }

        private void Load()
        {
            this.Abilities = new List<IAbility>();
            this.Abilities.AddRange(ScriptingEngine.instance.FetchAndInitialiseChildren<IAbility>());
        }

        public bool Initialise()
        {
            if (this.Abilities is null)
            {
                this.Load();
            }

            return true;
        }

        public IAbility GetAbility(string nameRef)
        {
            this.Initialise();

            if (this.Abilities.Any(x => x.InternalName.Equals(nameRef, StringComparison.OrdinalIgnoreCase)
                                        || x.Name.Equals(nameRef, StringComparison.OrdinalIgnoreCase)))
            {
                return this.Abilities.First(x => x.InternalName.Equals(nameRef, StringComparison.OrdinalIgnoreCase)
                                                 || x.Name.Equals(nameRef, StringComparison.OrdinalIgnoreCase));
            }

            throw new InvalidOperationException("Could not find IAbility with name " + nameRef);
        }

        public IEnumerable<IAbility> GetAvailableAbilities(IEntity actor)
        {
            this.Initialise();

            IEnumerable<string> prereqs = this.Abilities.SelectMany(ability => ability.Prerequisites.Select(pair => pair.Key)).Distinct();

            IEnumerable<Tuple<string, int>> data = actor.GetData(prereqs);

            return this.Abilities.Where(ability => ability.MeetsPrerequisites(data));
        }

        public IEnumerable<IAbility> GetAvailableAbilities(IEntityTemplate template,
            IDictionary<string, IRollableValue<int>> stats,
            IDictionary<string, IEntitySkill> skills)
        {
            this.Initialise();

            List<Tuple<string, int>> data =
                stats.Select(stat => new Tuple<string, int>(stat.Key, stat.Value.Value)).ToList();

            data.AddRange(skills.Select(skill => new Tuple<string, int>(skill.Key, skill.Value.Value)));
            data.Add(new Tuple<string, int>(template.CreatureType, 1));

            return this.Abilities.Where(ability => ability.MeetsPrerequisites(data));
        }
    }
}