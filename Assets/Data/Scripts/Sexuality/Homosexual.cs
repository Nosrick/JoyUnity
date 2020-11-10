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

        public override bool WillMateWith(Entity me, Entity them, IRelationship[] relationships)
        {
            List<long> participants = new List<long>() { me.GUID, them.GUID };
            List<string> tags = new List<string>() { "sexual" };

            foreach (IRelationship relationship in relationships)
            {
                if(relationship.GetRelationshipValue(me.GUID, them.GUID) < MatingThreshold)
                {
                    return false;
                }

                if(me.Sex.CanBirth != them.Sex.CanBirth)
                {
                    return false;
                }

                return me.Sentient == them.Sentient;
            }
            return false;
        }
    }
}
