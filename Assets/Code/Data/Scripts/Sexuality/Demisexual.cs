﻿using JoyLib.Code.Entities.Relationships;
using System.Collections.Generic;

namespace JoyLib.Code.Entities.Sexuality
{
    public class Demisexual : AbstractSexuality
    {
        public override string Name
        {
            get
            {
                return "demisexual";
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
            get;
            set;
        }

        public override bool WillMateWith(Entity me, Entity them, IRelationship[] relationships)
        {
            List<long> participants = new List<long>() { me.GUID, them.GUID };
            List<string> tags = new List<string>() { "sexual", "romantic" };
            
            foreach (IRelationship relationship in relationships)
            {
                if (relationship.GetRelationshipValue(me.GUID, them.GUID) > MatingThreshold && me.Sentient == them.Sentient)
                {
                    return true;
                }
            }
            return false;
        }
    }
}