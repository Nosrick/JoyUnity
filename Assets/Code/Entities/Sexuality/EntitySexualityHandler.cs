using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Scripting;

namespace JoyLib.Code.Entities.Sexuality
{
    public class EntitySexualityHandler : IEntitySexualityHandler
    {
        protected Dictionary<string, ISexuality> m_Sexualities;

        public EntitySexualityHandler()
        {
            Initialise();
        }

        protected void Initialise()
        {
            Load();
        }

        protected bool Load()
        {
            if (m_Sexualities != null)
            {
                return true;
            }

            m_Sexualities = new Dictionary<string, ISexuality>();

            IEnumerable<ISexuality> sexualities = ScriptingEngine.instance.FetchAndInitialiseChildren<ISexuality>();

            foreach (ISexuality sexuality in sexualities)
            {
                m_Sexualities.Add(sexuality.Name, sexuality);
            }

            return true;
        }

        public ISexuality Get(string sexuality)
        {
            if (m_Sexualities is null)
            {
                throw new InvalidOperationException("Sexuality search was null.");
            }

            if (m_Sexualities.ContainsKey(sexuality))
            {
                return m_Sexualities[sexuality];
            }
            throw new InvalidOperationException("Sexuality of type " + sexuality + " not found.");
        }

        public ISexuality[] Sexualities
        {
            get
            {
                if (m_Sexualities is null)
                {
                    Initialise();
                }

                return m_Sexualities.Values.ToArray();
            }
        }
    }
}
