using System;
using JoyLib.Code.Entities.Relationships;
using System.Collections.Generic;
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
            foreach (IRelationship relationship in relationships)
            {
                if(relationship.GetRelationshipValue(me.GUID, them.GUID) < MatingThreshold)
                {
                    return false;
                }

                if(me.Gender.Name.Equals(them.Gender.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                return me.Sentient == them.Sentient;
            }
            return false;
        }
    }
}
