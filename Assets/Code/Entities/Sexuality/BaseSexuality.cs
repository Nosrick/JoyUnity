﻿using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities.Relationships;
using Sirenix.OdinSerializer;

namespace JoyLib.Code.Entities.Sexuality
{
    [Serializable]
    public class BaseSexuality : ISexuality
    {
        [OdinSerialize]
        protected HashSet<string> m_Tags;
        
        [OdinSerialize]
        public string Name { get; protected set; }

        [OdinSerialize]
        public bool DecaysNeed { get; protected set; }

        [OdinSerialize]
        public int MatingThreshold { get; set; }
        
        [OdinSerialize]
        protected ISexualityPreferenceProcessor Processor { get; set; }

        public IEnumerable<string> Tags
        {
            get => this.m_Tags;
            protected set => this.m_Tags = new HashSet<string>(value);
        }

        public BaseSexuality()
        {
            this.Name = "DEFAULT";
            this.DecaysNeed = false;
            this.MatingThreshold = 0;
            this.Processor = new AsexualProcessor();
            this.m_Tags = new HashSet<string>
            {
                "sexual"
            };
        }

        public BaseSexuality(
            string name,
            bool decaysNeed,
            int matingThreshold,
            ISexualityPreferenceProcessor processor,
            IEnumerable<string> tags = null)
        {
            this.Name = name;
            this.DecaysNeed = decaysNeed;
            this.MatingThreshold = matingThreshold;
            this.Processor = processor;
            this.Tags = tags ?? new HashSet<string>();

            this.m_Tags.Add("sexual");
        }
        
        public bool HasTag(string tag)
        {
            return this.m_Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase));
        }

        public bool AddTag(string tag)
        {
            if (this.HasTag(tag))
            {
                return false;
            }
            this.m_Tags.Add(tag);
            return true;
        }

        public bool RemoveTag(string tag)
        {
            if (!this.HasTag(tag))
            {
                return false;
            }
            this.m_Tags.Remove(tag);
            return true;

        }

        public bool WillMateWith(IEntity me, IEntity them, IEnumerable<IRelationship> relationships)
        {
            return this.Processor.WillMateWith(me, them, relationships);
        }

        public bool Compatible(IEntity me, IEntity them)
        {
            return this.Processor.Compatible(me, them);
        }
    }
}