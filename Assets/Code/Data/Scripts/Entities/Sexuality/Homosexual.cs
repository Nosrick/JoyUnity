using System;
using JoyLib.Code.Entities.Relationships;
using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.Entities.Sexuality
{
    public class Homosexual : AbstractSexuality
    {
        public override string Name
        {
            get
            {
                return "homosexual";
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

        public Homosexual()
        {
            AddTag("homo");
        }

        public override bool WillMateWith(Entity me, Entity them, IRelationship[] relationships)
        {
            foreach (IRelationship relationship in relationships)
            {
                if(relationship.GetRelationshipValue(me.GUID, them.GUID) < MatingThreshold)
                {
                    return false;
                }

                if(me.Gender.Name.Equals(them.Gender.Name, StringComparison.OrdinalIgnoreCase) == false)
                {
                    return false;
                }

                return me.Sentient == them.Sentient;
            }
            return false;
        }
    }
}
