using System;
using System.Collections.Generic;
using JoyLib.Code.Entities.Sexes.Processors;
using Sirenix.OdinSerializer;

namespace JoyLib.Code.Entities.Sexes
{
    [Serializable]
    public class BaseBioSex : IBioSex
    {
        [OdinSerialize]
        public bool CanBirth { get; protected set; }
        [OdinSerialize]
        public bool CanFertilise { get; protected set; }
        [OdinSerialize]
        public string Name { get; protected set; }

        [OdinSerialize]
        protected IBioSexProcessor Processor { get; set; }
        
        public IEnumerable<string> Tags
        {
            get => this.m_Tags;
            protected set => this.m_Tags = new HashSet<string>(value);
        }

        [OdinSerialize]
        protected HashSet<string> m_Tags;

        public BaseBioSex()
        {
            this.Name = "DEFAULT";
            this.CanBirth = false;
            this.CanFertilise = false;
            this.Processor = new NeutralProcessor();
            this.m_Tags = new HashSet<string>();
        }
        
        public BaseBioSex(
            string name,
            bool canBirth,
            bool canFertilise,
            IBioSexProcessor processor,
            IEnumerable<string> tags = null)
        {
            this.Name = name;
            this.CanBirth = canBirth;
            this.CanFertilise = canFertilise;
            this.Processor = processor;
            this.Tags = tags ?? new HashSet<string>
            {
                "neutral"
            };
        }
        
        public IEntity CreateChild(IEnumerable<IEntity> parents)
        {
            throw new System.NotImplementedException();
        }
    }
}