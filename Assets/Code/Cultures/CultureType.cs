using JoyLib.Code.Entities;
using JoyLib.Code.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace JoyLib.Code.Cultures
{
    public class CultureType
    {
        private List<string> m_RulerTypes;
        private List<string> m_Crimes;
        private List<NameData> m_NameData;
        private List<Sex> m_sexs;
        private Dictionary<Sexuality, int> m_SexualityPrevelence;
        private int m_StatVariance;

        public CultureType(string nameRef, List<string> rulersRef, List<string> crimesRef, List<NameData> namesRef,
            string inhabitantsNameRef, List<Sex> sexsRef, Dictionary<Sexuality, int> sexualityRef, int statVariance)
        {
            cultureName = nameRef;
            m_RulerTypes = rulersRef;
            m_Crimes = crimesRef;
            m_NameData = namesRef;
            inhabitants = inhabitantsNameRef;
            m_sexs = sexsRef;
            m_SexualityPrevelence = sexualityRef;
            m_StatVariance = statVariance;
        }

        public string GetRandomName(Sex sexRef)
        {
            List<NameData> validFirstNames = m_NameData.Where(x => x.sex == sexRef || x.sex == Sex.Neutral && (x.isSurname == false)).ToList();
            List<NameData> validLastNames = m_NameData.Where(x => x.isSurname).ToList();
            return validFirstNames[RNG.Roll(0, validFirstNames.Count - 1)].name + " " + validLastNames[RNG.Roll(0, validLastNames.Count - 1)].name;
        }

        public Sex Choosesex()
        {
            return sexs[RNG.Roll(0, sexs.Count - 1)];
        }

        public Sexuality ChooseSexuality()
        {
            int result = RNG.Roll(0, 99);
            int soFar = 0;
            foreach(KeyValuePair<Sexuality, int> pair in m_SexualityPrevelence)
            {
                soFar += pair.Value;
                if(result < soFar)
                {
                    return pair.Key;
                }
            }
            return Sexuality.Asexual;
        }

        public int StatVariance()
        {
            return RNG.Roll(-m_StatVariance, m_StatVariance);
        }

        public List<Sex> sexs
        {
            get
            {
                return m_sexs;
            }
        }

        public string inhabitants
        {
            get;
            private set;
        }

        public string cultureName
        {
            get;
            private set;
        }
    }
}
