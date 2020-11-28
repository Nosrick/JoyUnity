﻿using System;
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
        
        protected ICultureHandler CultureHandler { get; set; }
        
        public IRomance[] Romances => RomanceTypes.Values.ToArray();

        public EntityRomanceHandler(ICultureHandler cultureHandler)
        {
            this.CultureHandler = cultureHandler;
            Initialise();
        }

        protected void Initialise()
        {
            Load(CultureHandler.Cultures);
        }

        protected bool Load(ICulture[] cultures)
        {
            if (RomanceTypes != null)
            {
                return true;
            }

            RomanceTypes = new Dictionary<string, IRomance>();

            foreach (CultureType culture in cultures)
            {
                foreach (string romance in culture.RomanceTypes)
                {
                    if (RomanceTypes.ContainsKey(romance) == false)
                    {
                        IRomance romanceType = (IRomance)ScriptingEngine.instance.FetchAndInitialise(romance);
                        if (!(romanceType is null))
                        {
                            RomanceTypes.Add(romance, romanceType);
                        }
                    }
                }
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