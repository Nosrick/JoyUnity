using JoyLib.Code.Entities;
using JoyLib.Code.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace JoyLib.Code.Cultures
{
    public class CultureType
    {
        private readonly List<string> m_RulerTypes;
        private readonly List<string> m_Crimes;
        private List<NameData> m_NameData;
        private readonly List<Sex> m_Sexes;
        private readonly Dictionary<Sexuality, int> m_SexualityPrevelence;
        private readonly int m_StatVariance;

        public CultureType(string nameRef, List<string> rulersRef, List<string> crimesRef, List<NameData> namesRef,
            string inhabitantsNameRef, List<Sex> sexesRef, Dictionary<Sexuality, int> sexualityRef, int statVariance, RelationshipType relationships)
        {
            CultureName = nameRef;
            m_RulerTypes = rulersRef;
            m_Crimes = crimesRef;
            m_NameData = namesRef;
            Inhabitants = inhabitantsNameRef;
            m_Sexes = sexesRef;
            m_SexualityPrevelence = sexualityRef;
            m_StatVariance = statVariance;
            RelationshipType = relationships;
        }

        public string GetRandomName(Sex sexRef)
        {
            List<NameData> validFirstNames = m_NameData.Where(x => x.sex == sexRef || x.sex == Sex.Neutral && (x.isSurname == false)).ToList();
            List<NameData> validLastNames = m_NameData.Where(x => x.isSurname).ToList();
            return validFirstNames[RNG.Roll(0, validFirstNames.Count - 1)].name + " " + validLastNames[RNG.Roll(0, validLastNames.Count - 1)].name;
        }

        public Sex ChooseSex()
        {
            return Sexes[RNG.Roll(0, Sexes.Count - 1)];
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

        public List<Sex> Sexes
        {
            get
            {
                return m_Sexes;
            }
        }

        public string Inhabitants
        {
            get;
            private set;
        }

        public string CultureName
        {
            get;
            private set;
        }

        public RelationshipType RelationshipType
        {
            get;
            protected set;
        }

        public List<string> RulerTypes
        {
            get
            {
                return m_RulerTypes;
            }
        }

        public List<string> Crimes
        {
            get
            {
                return m_Crimes;
            }
        }
    }

    public enum RelationshipType
    {
        Monoamorous,
        Polyamorous,
        Both
    }
}
