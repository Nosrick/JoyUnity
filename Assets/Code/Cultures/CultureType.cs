using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Sexes;
using JoyLib.Code.Entities.Sexuality;
using JoyLib.Code.Rollers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JoyLib.Code.Cultures
{
    public class CultureType
    {
        protected List<string> m_RulerTypes;
        protected List<string> m_Crimes;
        protected List<NameData> m_NameData;
        protected Dictionary<string, int> m_SexPrevelence;
        protected Dictionary<string, int> m_SexualityPrevelence;

        //The first number is the chance, the second is the actual number it can vary by
        protected Dictionary<string, Tuple<int, int>> m_StatVariance;
        protected List<string> m_RelationshipTypes;
        Dictionary<string, int> m_JobPrevelence;
        List<string> m_Inhabitants;

        public CultureType(string nameRef, List<string> rulersRef, List<string> crimesRef, List<NameData> namesRef, 
            Dictionary<string, int> jobRef, List<string> inhabitantsNameRef, Dictionary<string, int> sexualityPrevelenceRef, 
            Dictionary<string, int> sexPrevelence, Dictionary<string, Tuple<int, int>> statVariance, List<string> relationshipTypes)
        {
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
        }

        public string GetRandomName(IBioSex sexRef)
        {
            Dictionary<int, List<string>> allViablesNames = new Dictionary<int, List<string>>();

            List<NameData> thisSexNames = m_NameData.Where(nameData => nameData.sexes.Contains(sexRef.Name)).ToList();

            foreach(NameData name in thisSexNames)
            {
                foreach(int chain in name.chain)
                {
                    if(allViablesNames.ContainsKey(chain))
                    {
                        allViablesNames[chain].Add(name.name);
                    }
                    else
                    {
                        allViablesNames.Add(chain, new List<string>());
                        allViablesNames[chain].Add(name.name);
                    }
                }
            }

            string returnName = "";

            foreach(List<string> names in allViablesNames.Values)
            {
                returnName += names[RNG.instance.Roll(0, names.Count - 1)] + " ";
            }
            returnName = returnName.TrimEnd();

            return returnName;
        }

        public string GetNameForChain(int chain, string sex)
        {
            NameData[] names = m_NameData.Where(x => x.chain.Contains(chain) && x.sexes.Contains(sex.ToLower())).ToArray();

            int result = RNG.instance.Roll(0, names.Length - 1);
            return names[result].name;
        }

        public IBioSex ChooseSex()
        {
            int totalSex = 0;
            foreach(int value in m_SexPrevelence.Values)
            {
                totalSex += value;
            }

            int result = RNG.instance.Roll(0, totalSex - 1);
            int soFar = 0;
            foreach(KeyValuePair<string, int> pair in m_SexPrevelence)
            {
                soFar += pair.Value;
                if (result < soFar)
                {
                    return EntityBioSexHandler.instance.Get(pair.Key);
                }
            }
            return null;
        }

        public ISexuality ChooseSexuality()
        {
            int soFar = 0;
            int totalSex = 0;
            foreach(int value in m_SexualityPrevelence.Values)
            {
                totalSex += value;
            }
            int result = RNG.instance.Roll(0, totalSex - 1);

            foreach (KeyValuePair<string, int> pair in m_SexualityPrevelence)
            {
                soFar += pair.Value;
                if(result < soFar)
                {
                    return EntitySexualityHandler.Get(pair.Key);
                }
            }
            return null;
        }

        public JobType ChooseJob()
        {
            int soFar = 0;
            int totalJob = 0;
            foreach(int value in m_JobPrevelence.Values)
            {
                totalJob += value;
            }
            int result = RNG.instance.Roll(0, totalJob - 1);

            foreach(KeyValuePair<string, int> pair in m_JobPrevelence)
            {
                soFar += pair.Value;
                if(result < soFar)
                {
                    return JobHandler.instance.Get(pair.Key);
                }
            }
            return null;
        }

        public int GetStatVariance(string statistic)
        {
            if(m_StatVariance.ContainsKey(statistic))
            {
                if(RNG.instance.Roll(1, 100) < m_StatVariance[statistic].Item1)
                {
                    return RNG.instance.Roll(-m_StatVariance[statistic].Item2, m_StatVariance[statistic].Item2);
                }
            }
            return 0;
        }

        public string[] Inhabitants
        {
            get
            {
                return m_Inhabitants.ToArray();
            }
        }

        public string CultureName
        {
            get;
            private set;
        }

        public string[] RulerTypes
        {
            get
            {
                return m_RulerTypes.ToArray();
            }
        }

        public string[] Crimes
        {
            get
            {
                return m_Crimes.ToArray();
            }
        }

        public string[] RelationshipTypes
        {
            get
            {
                return m_RelationshipTypes.ToArray();
            }
        }

        public string[] Sexes
        {
            get
            {
                return m_SexPrevelence.Keys.ToArray();
            }
        }

        public string[] Sexualities
        {
            get
            {
                return m_SexualityPrevelence.Keys.ToArray();
            }
        }

        public NameData[] NameData
        {
            get
            {
                return m_NameData.ToArray();
            }
        }
    }
}
