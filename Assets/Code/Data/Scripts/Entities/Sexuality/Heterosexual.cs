using System;
using JoyLib.Code.Entities.Relationships;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JoyLib.Code.Entities.Sexuality
{
    public class Heterosexual : AbstractSexuality
    {
        public override string Name
        {
            get
            {
                return "heterosexual";
            }
        }

        public override bool DecaysNeed
        {
            get
            {
                return true;
            }
        }

        public override int MatingThreshold
        {
            get
            {
                return 0;
            }
        }

        public Heterosexual()
        {
            AddTag("hetero");
        }

        public override bool WillMateWith(IEntity me, IEntity them, IRelationship[] relationships)
        {
            int highestValue = relationships.Max(relationship => relationship.GetRelationshipValue(me.GUID, them.GUID));
            if(highestValue < MatingThreshold
                || me.Gender.Name.Equals(them.Gender.Name))
            {
                return false;
            }
            return me.Sentient == them.Sentient;
        }

        public override bool Compatible(IEntity me, IEntity them)
        {
            return me.Sentient == them.Sentient
                   && me.Gender.Name.Equals(them.Gender.Name, StringComparison.OrdinalIgnoreCase) == false;
        }
    }
}
