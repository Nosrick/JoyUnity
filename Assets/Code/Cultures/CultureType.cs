using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Sexes;
using JoyLib.Code.Entities.Sexuality;
using JoyLib.Code.Rollers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Castle.Core.Internal;
using JoyLib.Code.Entities.Gender;
using JoyLib.Code.Entities.Romance;
using UnityEngine;

namespace JoyLib.Code.Cultures
{
    public class CultureType : ICulture
    {
        protected List<string> m_RulerTypes;
        protected List<string> m_Crimes;
        protected List<NameData> m_NameData;
        protected Dictionary<string, int> m_SexPrevelence;
        protected Dictionary<string, int> m_SexualityPrevelence;
        protected Dictionary<string, int> m_RomancePrevelence;
        protected Dictionary<string, int> m_GenderPrevelence;

        //The first number is the chance, the second is the actual number it can vary by
        protected Dictionary<string, Tuple<int, int>> m_StatVariance;
        protected List<string> m_RelationshipTypes;
        Dictionary<string, int> m_JobPrevelence;
        List<string> m_Inhabitants;

        public int LastGroup { get; protected set; }

        public string Tileset { get; protected set; }

        public string[] Inhabitants => m_Inhabitants.ToArray();

        public string CultureName
        {
            get;
            protected set;
        }

        public string[] RulerTypes => m_RulerTypes.ToArray();

        public string[] Crimes => m_Crimes.ToArray();

        public string[] RelationshipTypes => m_RelationshipTypes.ToArray();

        public string[] RomanceTypes => m_RomancePrevelence.Keys.ToArray();

        public string[] Sexes => m_SexPrevelence.Keys.ToArray();

        public string[] Sexualities => m_SexualityPrevelence.Keys.ToArray();

        public string[] Genders => m_GenderPrevelence.Keys.ToArray();

        public string[] Jobs => m_JobPrevelence.Keys.ToArray();
        
        public int NonConformingGenderChance { get; protected set; }

        public NameData[] NameData => m_NameData.ToArray();

        public RNG Roller { get; protected set; }

        public CultureType()
        {}

        public CultureType(
            string nameRef, 
            string tileset,
            List<string> rulersRef, 
            List<string> crimesRef, 
            List<NameData> namesRef,
            Dictionary<string, int> jobRef, 
            List<string> inhabitantsNameRef,
            Dictionary<string, int> sexualityPrevelenceRef,
            Dictionary<string, int> sexPrevelence, 
            Dictionary<string, Tuple<int, int>> statVariance,
            List<string> relationshipTypes,
            Dictionary<string, int> romancePrevelence,
            Dictionary<string, int> genderPrevelence,
            int nonConformingGenderChance,
            RNG roller = null)
        {
            Roller = roller is null ? new RNG() : roller;
            Tileset = tileset;
            CultureName = nameRef;
            m_RulerTypes = rulersRef;
            m_Crimes = crimesRef;
            m_NameData = namesRef;
            m_Inhabitants = inhabitantsNameRef;
            m_SexPrevelence = sexPrevelence;
            m_StatVariance = statVariance;
            m_JobPrevelence = jobRef;
            m_SexualityPrevelence = sexualityPrevelenceRef;
            m_StatVariance = statVariance;
            m_RelationshipTypes = relationshipTypes;
            m_RomancePrevelence = romancePrevelence;
            m_GenderPrevelence = genderPrevelence;
            NonConformingGenderChance = nonConformingGenderChance;
            
            ClearLastGroup();
        }

        public void ClearLastGroup()
        {
            LastGroup = Int32.MinValue;
        }

        public string GetRandomName(string genderRef)
        {
            string returnName = "";

            int maxChain = m_NameData.Where(data => data.genders.Contains(genderRef, GlobalConstants.STRING_COMPARER)
                || data.genders.Contains("all", GlobalConstants.STRING_COMPARER))
                .SelectMany(data => data.chain)
                .Distinct()
                .Max(data => data);

            for (int i = 0; i <= maxChain; i++)
            {
                returnName = String.Join(" ", returnName, GetNameForChain(i, genderRef, 25));
            }
            
            returnName = returnName.TrimEnd();
            
            ClearLastGroup();

            return returnName;
        }

        public string GetNameForChain(int chain, string gender, int group = Int32.MinValue)
        {
            NameData[] names;

            int chosenGroup = group == Int32.MinValue ? LastGroup : group;

            LastGroup = chosenGroup;
            
            if (chosenGroup == Int32.MinValue)
            {
                names = m_NameData.Where(x => x.chain.Contains(chain) 
                                              && (x.genders.Contains(gender, GlobalConstants.STRING_COMPARER) 
                                                  || x.genders.Any(s => s.Equals("all", StringComparison.OrdinalIgnoreCase)))
                                              && x.groups.IsNullOrEmpty()).ToArray();
            }
            else
            {
                names = m_NameData.Where(x => x.chain.Contains(chain)
                                              && (x.genders.Contains(gender, GlobalConstants.STRING_COMPARER)
                                                  || x.genders.Any(s => s.Equals("all", StringComparison.OrdinalIgnoreCase))
                                                  && x.groups.Contains(chosenGroup))).ToArray();
            }

            if (names.IsNullOrEmpty())
            {
                if (LastGroup != int.MinValue)
                {
                    ClearLastGroup();
                    return GetNameForChain(chain, gender, LastGroup);
                }
                else
                {
                    return "";
                }
            }
            int result = Roller.Roll(0, names.Length);
            return names[result].name;
        }

        public IBioSex ChooseSex(IBioSex[] sexes)
        {
            int totalSex = 0;
            foreach(int value in m_SexPrevelence.Values)
            {
                totalSex += value;
            }

            int result = Roller.Roll(0, totalSex);
            int soFar = 0;
            foreach(KeyValuePair<string, int> pair in m_SexPrevelence)
            {
                soFar += pair.Value;
                if (result < soFar)
                {
                    return sexes.First(sex => sex.Name.Equals(pair.Key));
                }
            }
            throw new InvalidOperationException("Could not assign sex from culture " + this.CultureName + ".");
        }

        public ISexuality ChooseSexuality(ISexuality[] sexualities)
        {
            int soFar = 0;
            int totalSexuality = 0;
            foreach(int value in m_SexualityPrevelence.Values)
            {
                totalSexuality += value;
            }
            int result = Roller.Roll(0, totalSexuality);

            foreach (KeyValuePair<string, int> pair in m_SexualityPrevelence)
            {
                soFar += pair.Value;
                if(result < soFar)
                {
                    return sexualities.First(sexuality => sexuality.Name.Equals(pair.Key, StringComparison.OrdinalIgnoreCase));
                }
            }
            throw new InvalidOperationException("Could not assign sexuality from culture " + this.CultureName + ".");
        }

        public IRomance ChooseRomance(IRomance[] romances)
        {
            int soFar = 0;
            int totalRomance = 0;
            foreach(int value in m_RomancePrevelence.Values)
            {
                totalRomance += value;
            }
            int result = Roller.Roll(0, totalRomance);

            foreach (KeyValuePair<string, int> pair in m_RomancePrevelence)
            {
                soFar += pair.Value;
                if(result < soFar)
                {
                    return romances.First(romance => romance.Name.Equals(pair.Key, StringComparison.OrdinalIgnoreCase));
                }
            }
            throw new InvalidOperationException("Could not assign romance from culture " + this.CultureName + ".");
        }

        public IGender ChooseGender(IBioSex sex, IGender[] genders)
        {
            int nonConforming = Roller.Roll(0, 100);
            if (nonConforming < NonConformingGenderChance)
            {
                int soFar = 0;
                int totalGender = 0;
                foreach(int value in m_GenderPrevelence.Values)
                {
                    totalGender += value;
                }
                int result = Roller.Roll(0, totalGender);

                foreach (KeyValuePair<string, int> pair in m_GenderPrevelence)
                {
                    soFar += pair.Value;
                    if(result < soFar)
                    {
                        return genders.First(gender => gender.Name.Equals(pair.Key, StringComparison.OrdinalIgnoreCase));
                    }
                }
            }
            else
            {
                return genders.First(gender => gender.Name.Equals(sex.Name, StringComparison.OrdinalIgnoreCase));
            }
            throw new InvalidOperationException("Could not assign gender from culture " + this.CultureName + ".");
        }

        public IJob ChooseJob(IJob[] jobs)
        {
            int soFar = 0;
            int totalJob = 0;
            foreach(int value in m_JobPrevelence.Values)
            {
                totalJob += value;
            }
            int result = Roller.Roll(0, totalJob);

            foreach(KeyValuePair<string, int> pair in m_JobPrevelence)
            {
                soFar += pair.Value;
                if(result < soFar)
                {
                    return jobs.First(job => job.Name.Equals(pair.Key, StringComparison.OrdinalIgnoreCase));
                }
            }
            throw new InvalidOperationException("Could not assign job from culture " + this.CultureName + ".");
        }

        public int GetStatVariance(string statistic)
        {
            if(m_StatVariance.ContainsKey(statistic))
            {
                if(Roller.Roll(0, 100) < m_StatVariance[statistic].Item1)
                {
                    return Roller.Roll(-m_StatVariance[statistic].Item2, m_StatVariance[statistic].Item2);
                }
            }
            return 0;
        }
    }
}
