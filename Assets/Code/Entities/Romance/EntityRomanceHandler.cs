using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Cultures;
using JoyLib.Code.Scripting;
using UnityEngine;

namespace JoyLib.Code.Entities.Romance
{
    public class EntityRomanceHandler : IEntityRomanceHandler
    {
        protected Dictionary<string, IRomance> RomanceTypes { get; set; }
        
        public IRomance[] Romances => RomanceTypes.Values.ToArray();

        public EntityRomanceHandler()
        {
            Initialise();
        }

        protected void Initialise()
        {
            Load();
        }

        protected bool Load()
        {
            if (RomanceTypes != null)
            {
                return true;
            }

            RomanceTypes = new Dictionary<string, IRomance>();
            
            IRomance[] romanceTypes = ScriptingEngine.instance.FetchAndInitialiseChildren<IRomance>();
            foreach (IRomance romance in romanceTypes)
            {
                RomanceTypes.Add(romance.Name, romance);
            }

            return true;
        }
        
        public IRomance Get(string romance)
        {
            if (RomanceTypes is null)
            {
                throw new InvalidOperationException("Sexuality search was null.");
            }

            if (RomanceTypes.Any(r => r.Key.Equals(romance, StringComparison.OrdinalIgnoreCase)))
            {
                return RomanceTypes.First(r => r.Key.Equals(romance, StringComparison.OrdinalIgnoreCase)).Value;
            }
            throw new InvalidOperationException("Sexuality of type " + romance + " not found.");
        }
    }
}