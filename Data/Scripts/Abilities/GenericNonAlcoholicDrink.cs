﻿using System;
using System.Collections.Generic;
using JoyLib.Code.Entities.Items;

namespace JoyLib.Code.Entities.Abilities
{
    public class GenericNonAlcoholicDrink : AbstractAbility
    {
        public GenericNonAlcoholicDrink() : base(
            "drink",
            "genericnonalcoholicdrink",
            "Quench your thirst with a cool refreshment.",
            false,
            0,
            0,
            0,
            false,
            new string[] { "fulfillneedaction" },
            new Tuple<string, int>[0], 
            GetPrerequisites(), 
            AbilityTarget.Self)
        { }

        protected static Dictionary<string, int> GetPrerequisites()
        {
            Dictionary<string, int> prereqs = new Dictionary<string, int>();
            prereqs.Add("drink", 1);
            return prereqs;
        }

        public override bool OnUse(IEntity user, IJoyObject target)
        {
            if(target is IItemInstance item)
            {
                this.m_CachedActions["fulfillneedaction"].Execute(
                    new IJoyObject[] { user },
                    new string[] { "thirst", "need", "fulfill" },
                    new object[] { "thirst", item.ItemType.Value, 10 }
                );
                user.RemoveContents(item);
                return true;
            }
            return false;
        }
    }
}