using System;
using JoyLib.Code.Collections;
using JoyLib.Code.Entities.Needs;
using Sirenix.OdinSerializer;

namespace JoyLib.Code.Entities.Statistics
{
    [Serializable]
    public class EntitySkill : IEntitySkill
    {
        public EntitySkill()
        {
        }
        
        public EntitySkill(
            string name, 
            int value, 
            int successThreshold)
        {
            this.Name = name;
            this.Value = value;
            this.SuccessThreshold = successThreshold;
        }

        public int ModifyValue(int value)
        {
            this.Value = Math.Max(0, this.Value + value);
            return this.Value;
        }

        public int SetValue(int value)
        {
            this.Value = Math.Max(0, value);
            return this.Value;
        }

        [OdinSerialize]
        public string Name
        {
            get;
            protected set;
        }

        [OdinSerialize]
        public int Value
        {
            get;
            protected set;
        }

        [OdinSerialize]
        public int SuccessThreshold
        {
            get;
            protected set;
        }
    }
}