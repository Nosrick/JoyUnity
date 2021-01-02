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
            this.Initialise();
        }

        protected void Initialise()
        {
            this.Load();
        }

        protected bool Load()
        {
            if (this.m_Sexualities != null)
            {
                return true;
            }

            this.m_Sexualities = new Dictionary<string, ISexuality>();

            IEnumerable<ISexuality> sexualities = ScriptingEngine.instance.FetchAndInitialiseChildren<ISexuality>();

            foreach (ISexuality sexuality in sexualities)
            {
                this.m_Sexualities.Add(sexuality.Name, sexuality);
            }

            return true;
        }

        public ISexuality Get(string sexuality)
        {
            if (this.m_Sexualities is null)
            {
                throw new InvalidOperationException("Sexuality search was null.");
            }

            if (this.m_Sexualities.ContainsKey(sexuality))
            {
                return this.m_Sexualities[sexuality];
            }
            throw new InvalidOperationException("Sexuality of type " + sexuality + " not found.");
        }

        public ISexuality[] Sexualities
        {
            get
            {
                if (this.m_Sexualities is null)
                {
                    this.Initialise();
                }

                return this.m_Sexualities.Values.ToArray();
            }
        }
    }
}
