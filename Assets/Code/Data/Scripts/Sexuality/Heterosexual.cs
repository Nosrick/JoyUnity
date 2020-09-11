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

        public override bool WillMateWith(Entity me, Entity them, IRelationship[] relationships)
        {
            List<long> participants = new List<long>() { me.GUID, them.GUID };
            List<string> tags = new List<string>() { "sexual" };
            
            foreach (IRelationship relationship in relationships)
            {
                if(relationship.GetRelationshipValue(me.GUID, them.GUID) > MatingThreshold)
                {
                    Debug.Log("Mating Threshold passed.");
                }
                else
                {
                    Debug.Log("Mating Threshold failed!");
                    return false;
                }

                if(me.Sex.CanBirth != them.Sex.CanBirth)
                {
                    Debug.Log("Heterosexual relationship established.");
                }
                else
                {
                    Debug.Log("Non-heterosexual relationship!");
                    return false;
                }

                if(me.Sentient == them.Sentient)
                {
                    Debug.Log("Sentience equality.");
                }
                else
                {
                    Debug.Log("Sentience inequality!");
                    return false;
                }

                return true;

                /*
                if (relationship.GetRelationshipValue(me.GUID, them.GUID) > MatingThreshold &&
                    me.Sex.CanBirth != them.Sex.CanBirth && me.Sentient == them.Sentient)
                {
                    return true;
                }
                */
            }
            return false;
        }
    }
}
