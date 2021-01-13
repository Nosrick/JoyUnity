using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using JoyLib.Code.Entities.Gender;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Romance;
using JoyLib.Code.Entities.Sexes;
using JoyLib.Code.Entities.Sexuality;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Rollers;
using UnityEngine;

namespace JoyLib.Code.Cultures
{
    public class CultureType : ICulture
    {
        protected List<string> m_RulerTypes;
        protected List<string> m_Crimes;
        protected List<NameData> m_NameData;
        protected Dictionary<string, int> m_SexPrevalence;
        protected Dictionary<string, int> m_SexualityPrevalence;
        protected Dictionary<string, int> m_RomancePrevalence;
        protected Dictionary<string, int> m_GenderPrevalence;

        //The first number is the chance, the second is the actual number it can vary by
        protected Dictionary<string, Tuple<int, int>> m_StatVariance;
        protected List<string> m_RelationshipTypes;
        Dictionary<string, int> m_JobPrevalence;
        List<string> m_Inhabitants;

        public int LastGroup { get; protected set; }

        public string Tileset { get; protected set; }

        public string[] Inhabitants => this.m_Inhabitants.ToArray();

        public string CultureName { get; protected set; }

        public string[] RulerTypes => this.m_RulerTypes.ToArray();

        public string[] Crimes => this.m_Crimes.ToArray();

        public string[] RelationshipTypes => this.m_RelationshipTypes.ToArray();

        public string[] RomanceTypes => this.m_RomancePrevalence.Keys.ToArray();

        public string[] Sexes => this.m_SexPrevalence.Keys.ToArray();

        public string[] Sexualities => this.m_SexualityPrevalence.Keys.ToArray();

        public string[] Genders => this.m_GenderPrevalence.Keys.ToArray();

        public string[] Jobs => this.m_JobPrevalence.Keys.ToArray();

        public int NonConformingGenderChance { get; protected set; }

        public NameData[] NameData => this.m_NameData.ToArray();

        public RNG Roller { get; protected set; }

        public IDictionary<string, Color> CursorColours { get; protected set; }
        public IDictionary<string, Color> BackgroundColours { get; protected set; }

        protected const int NO_GROUP = int.MinValue;

        public CultureType()
        { }

        public CultureType(
            string nameRef,
            string tileset,
            List<string> rulersRef,
            List<string> crimesRef,
            List<NameData> namesRef,
            Dictionary<string, int> jobRef,
            List<string> inhabitantsNameRef,
            Dictionary<string, int> sexualityPrevalenceRef,
            Dictionary<string, int> sexPrevalence,
            Dictionary<string, Tuple<int, int>> statVariance,
            List<string> relationshipTypes,
            Dictionary<string, int> romancePrevalence,
            Dictionary<string, int> genderPrevalence,
            int nonConformingGenderChance,
            IDictionary<string, Color> cursorColours,
            IDictionary<string, Color> backgroundColours,
            RNG roller = null)
        {
            this.Roller = roller ?? new RNG();
            this.Tileset = tileset;
            this.CultureName = nameRef;
            this.m_RulerTypes = rulersRef;
            this.m_Crimes = crimesRef;
            this.m_NameData = namesRef;
            this.m_Inhabitants = inhabitantsNameRef;
            this.m_SexPrevalence = sexPrevalence;
            this.m_StatVariance = statVariance;
            this.m_JobPrevalence = jobRef;
            this.m_SexualityPrevalence = sexualityPrevalenceRef;
            this.m_StatVariance = statVariance;
            this.m_RelationshipTypes = relationshipTypes;
            this.m_RomancePrevalence = romancePrevalence;
            this.m_GenderPrevalence = genderPrevalence;
            this.NonConformingGenderChance = nonConformingGenderChance;
            this.CursorColours = cursorColours;
            this.BackgroundColours = backgroundColours;

            this.ClearLastGroup();
        }

        public void ClearLastGroup()
        {
            this.LastGroup = Int32.MinValue;
        }

        public string GetRandomName(string genderRef)
        {
            string returnName = "";

            int maxChain = this.m_NameData.Where(data =>
                    data.genders.Contains(genderRef, GlobalConstants.STRING_COMPARER)
                    || data.genders.Contains("all", GlobalConstants.STRING_COMPARER))
                .SelectMany(data => data.chain)
                .Distinct()
                .Max(data => data);
            int groupCount = this.m_NameData.SelectMany(data => data.groups)
                .Count();

            int groupChance = (100 / (groupCount + 1)) * groupCount;
            
            int chosenGroup = NO_GROUP;
            
            if (this.Roller.Roll(0, 100) < groupChance)
            {
                int[] availableGroups = this.m_NameData.Where(data =>
                        data.genders.Contains(genderRef, GlobalConstants.STRING_COMPARER)
                        || data.genders.Contains("all", GlobalConstants.STRING_COMPARER))
                    .SelectMany(data => data.groups)
                    .Distinct()
                    .ToArray();

                if (availableGroups.Length > 0)
                {
                    chosenGroup = availableGroups[this.Roller.Roll(0, availableGroups.Length)];
                }
            }

            for (int i = 0; i <= maxChain; i++)
            {
                returnName = string.Join(" ", returnName, this.GetNameForChain(i, genderRef, chosenGroup));
            }

            returnName = returnName.Trim();

            this.ClearLastGroup();

            return returnName;
        }

        public string GetNameForChain(int chain, string gender, int group = NO_GROUP)
        {
            NameData[] names;

            if (group == NO_GROUP)
            {
                names = this.m_NameData.Where(x => x.chain.Contains(chain)
                                                   && (x.genders.Contains(gender, GlobalConstants.STRING_COMPARER)
                                                       || x.genders.Any(s =>
                                                           s.Equals("all", StringComparison.OrdinalIgnoreCase)))
                                                   && x.groups.IsNullOrEmpty()).ToArray();
            }
            else
            {
                names = this.m_NameData.Where(x => x.chain.Contains(chain)
                                                   && (x.genders.Contains(gender, GlobalConstants.STRING_COMPARER)
                                                       || x.genders.Any(s =>
                                                           s.Equals("all", StringComparison.OrdinalIgnoreCase)))
                                                       && x.groups.Contains(group)).ToArray();
            }

            if (names.IsNullOrEmpty())
            {
                if (group == NO_GROUP)
                {
                    return "";
                }

                this.ClearLastGroup();
                return this.GetNameForChain(chain, gender, this.LastGroup);
            }

            this.LastGroup = group;
            int result = this.Roller.Roll(0, names.Length);
            return names[result].name;
        }

        public IBioSex ChooseSex(IEnumerable<IBioSex> sexes)
        {
            int totalSex = 0;
            foreach (int value in this.m_SexPrevalence.Values)
            {
                totalSex += value;
            }

            int result = this.Roller.Roll(0, totalSex);
            int soFar = 0;
            foreach (KeyValuePair<string, int> pair in this.m_SexPrevalence)
            {
                soFar += pair.Value;
                if (result < soFar)
                {
                    return sexes.First(sex => sex.Name.Equals(pair.Key));
                }
            }

            throw new InvalidOperationException("Could not assign sex from culture " + this.CultureName + ".");
        }

        public ISexuality ChooseSexuality(IEnumerable<ISexuality> sexualities)
        {
            int soFar = 0;
            int totalSexuality = 0;
            foreach (int value in this.m_SexualityPrevalence.Values)
            {
                totalSexuality += value;
            }

            int result = this.Roller.Roll(0, totalSexuality);

            foreach (KeyValuePair<string, int> pair in this.m_SexualityPrevalence)
            {
                soFar += pair.Value;
                if (result < soFar)
                {
                    return sexualities.First(sexuality =>
                        sexuality.Name.Equals(pair.Key, StringComparison.OrdinalIgnoreCase));
                }
            }

            throw new InvalidOperationException("Could not assign sexuality from culture " + this.CultureName + ".");
        }

        public IRomance ChooseRomance(IEnumerable<IRomance> romances)
        {
            int soFar = 0;
            int totalRomance = 0;
            foreach (int value in this.m_RomancePrevalence.Values)
            {
                totalRomance += value;
            }

            int result = this.Roller.Roll(0, totalRomance);

            foreach (KeyValuePair<string, int> pair in this.m_RomancePrevalence)
            {
                soFar += pair.Value;
                if (result < soFar)
                {
                    return romances.First(romance => romance.Name.Equals(pair.Key, StringComparison.OrdinalIgnoreCase));
                }
            }

            throw new InvalidOperationException("Could not assign romance from culture " + this.CultureName + ".");
        }

        public IGender ChooseGender(IBioSex sex, IEnumerable<IGender> genders)
        {
            int nonConforming = this.Roller.Roll(0, 100);
            if (nonConforming < this.NonConformingGenderChance)
            {
                int soFar = 0;
                int totalGender = 0;
                foreach (int value in this.m_GenderPrevalence.Values)
                {
                    totalGender += value;
                }

                int result = this.Roller.Roll(0, totalGender);

                foreach (KeyValuePair<string, int> pair in this.m_GenderPrevalence)
                {
                    soFar += pair.Value;
                    if (result < soFar)
                    {
                        return genders.First(gender =>
                            gender.Name.Equals(pair.Key, StringComparison.OrdinalIgnoreCase));
                    }
                }
            }
            else
            {
                return genders.First(gender => gender.Name.Equals(sex.Name, StringComparison.OrdinalIgnoreCase));
            }

            throw new InvalidOperationException("Could not assign gender from culture " + this.CultureName + ".");
        }

        public IJob ChooseJob(IEnumerable<IJob> jobs)
        {
            int soFar = 0;
            int totalJob = 0;
            foreach (int value in this.m_JobPrevalence.Values)
            {
                totalJob += value;
            }

            int result = this.Roller.Roll(0, totalJob);

            foreach (KeyValuePair<string, int> pair in this.m_JobPrevalence)
            {
                soFar += pair.Value;
                if (result < soFar)
                {
                    return jobs.First(job => job.Name.Equals(pair.Key, StringComparison.OrdinalIgnoreCase));
                }
            }

            throw new InvalidOperationException("Could not assign job from culture " + this.CultureName + ".");
        }

        public IDictionary<string, IRollableValue<int>> GetStats(IDictionary<string, IRollableValue<int>> baseStats)
        {
            Dictionary<string, IRollableValue<int>> stats = new Dictionary<string, IRollableValue<int>>(baseStats);
            foreach (string stat in baseStats.Keys)
            {
                stats[stat].ModifyValue(this.GetStatVariance(stat));
            }

            return stats;
        }

        public int GetStatVariance(string statistic)
        {
            if (this.m_StatVariance.ContainsKey(statistic))
            {
                if (this.Roller.Roll(0, 100) < this.m_StatVariance[statistic].Item1)
                {
                    return this.Roller.Roll(-this.m_StatVariance[statistic].Item2,
                        this.m_StatVariance[statistic].Item2 + 1);
                }
            }

            return 0;
        }
    }
}