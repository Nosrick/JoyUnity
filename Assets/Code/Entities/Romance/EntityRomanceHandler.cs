using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Scripting;

namespace JoyLib.Code.Entities.Romance
{
    public class EntityRomanceHandler : IEntityRomanceHandler
    {
        protected Dictionary<string, IRomance> RomanceTypes { get; set; }
        
        public IRomance[] Romances => this.RomanceTypes.Values.ToArray();

        public EntityRomanceHandler()
        {
            this.Initialise();
        }

        protected void Initialise()
        {
            this.Load();
        }

        protected bool Load()
        {
            if (this.RomanceTypes != null)
            {
                return true;
            }

            this.RomanceTypes = new Dictionary<string, IRomance>();
            
            IEnumerable<IRomance> romanceTypes = ScriptingEngine.instance.FetchAndInitialiseChildren<IRomance>();
            foreach (IRomance romance in romanceTypes)
            {
                this.RomanceTypes.Add(romance.Name, romance);
            }

            return true;
        }
        
        public IRomance Get(string romance)
        {
            if (this.RomanceTypes is null)
            {
                throw new InvalidOperationException("Sexuality search was null.");
            }

            if (this.RomanceTypes.Any(r => r.Key.Equals(romance, StringComparison.OrdinalIgnoreCase)))
            {
                return this.RomanceTypes.First(r => r.Key.Equals(romance, StringComparison.OrdinalIgnoreCase)).Value;
            }
            throw new InvalidOperationException("Sexuality of type " + romance + " not found.");
        }
    }
}