using System;
using UnityEngine;
using JoyLib.Code.Entities.Relationships;

namespace JoyLib.Code.Entities.Sexuality
{
    public class AbstractSexuality : ISexuality
    {
        public virtual string Name => throw new NotImplementedException("Someone forgot to override Name.");

        public virtual bool DecaysNeed => throw new NotImplementedException("Someone forgot to override DecaysNeed.");

        protected static EntitySexualityHandler s_SexualityHandler;

        public AbstractSexuality()
        {
            if(s_SexualityHandler is null)
            {
                s_SexualityHandler = GameObject.Find("GameManager").GetComponent<EntitySexualityHandler>();
            }
        }

        public virtual int MatingThreshold
        {
            get => throw new NotImplementedException("Someone forgot to override MatingThreshold.");
            set => throw new NotImplementedException("Someone forgot to override MatingThreshold.");
        }

        public virtual bool WillMateWith(Entity me, Entity them, IRelationship[] relationships)
        {
            throw new NotImplementedException("Someone forgot to override WillMateWith.");
        }
    }
}
