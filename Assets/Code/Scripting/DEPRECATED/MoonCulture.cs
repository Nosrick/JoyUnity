/*
DEPRECATED CODE

using JoyLib.Code.Cultures;
using MoonSharp.Interpreter;
using System.Collections.Generic;

namespace JoyLib.Code.Scripting
{
    public class MoonCulture
    {
        protected CultureType m_AssociatedCulture;

        [MoonSharpHidden]
        public MoonCulture(CultureType culture)
        {
            m_AssociatedCulture = culture;
        }

        public List<string> RulerTypes()
        {
            return new List<string>(m_AssociatedCulture.RulerTypes);
        }

        public string RelationshipType()
        {
            return m_AssociatedCulture.RelationshipType.ToString();
        }

        public string Name()
        {
            return m_AssociatedCulture.CultureName;
        }

        public List<string> Crimes()
        {
            return new List<string>(m_AssociatedCulture.Crimes);
        }
    }
}

*/