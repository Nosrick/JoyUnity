using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities.Abilities;
using Sirenix.OdinSerializer;

namespace JoyLib.Code.Entities.Items
{
    [Serializable]
    public class BaseItemType
    {
        protected string m_ClassName;
        protected List<string> m_Tags;
        
        public BaseItemType()
        {}

        public BaseItemType(
            string[] tags, 
            string description, 
            string unidentifiedDescriptionRef, 
            string unidentifiedNameRef, 
            string identifiedNameRef, 
            string[] slotsRef, 
            float size, 
            IItemMaterial material, 
            string governingSkillRef, 
            string actionStringRef, 
            int valueRef, 
            int spawnRef, 
            string spriteSheet, 
            int lightLevel = 0,
            IAbility[] abilities = null)
        {
            this.m_Tags = tags.ToList();
            
            this.SpawnWeighting = spawnRef;
            this.Value = valueRef;

            this.IdentifiedName = identifiedNameRef;
            this.Description = description;
            this.UnidentifiedDescription = unidentifiedDescriptionRef;
            this.UnidentifiedName = unidentifiedNameRef;
            this.Size = size;
            this.Material = material;
            this.Weight = this.Size * this.Material.Density;
            this.Slots = slotsRef;
            this.GoverningSkill = governingSkillRef;
            this.ActionString = actionStringRef;
            this.LightLevel = lightLevel;
            this.SpriteSheet = spriteSheet;
            this.Abilities = abilities;
        }

        public bool AddTag(string tag)
        {
            if(this.m_Tags.Contains(tag) == false)
            {
                this.m_Tags.Add(tag);
                return true;
            }
            return false;
        }

        public bool RemoveTag(string tag)
        {
            if(this.m_Tags.Contains(tag) == true)
            {
                this.m_Tags.Remove(tag);
                return true;
            }
            return false;
        }

        public bool HasTag(string tag)
        {
            return this.m_Tags.Contains(tag);
        }

        public bool HasSlot(string slot)
        {
            return this.Slots.Contains(slot);
        }

        public int GetHitPoints()
        {
            return (int)(Math.Max(1, this.Size * this.Material.Hardness));
        }

        [OdinSerialize]
        public string Description
        {
            get;
            protected set;
        }

        [OdinSerialize]
        public string IdentifiedName
        {
            get;
            protected set;
        }

        [OdinSerialize]
        public string UnidentifiedDescription
        {
            get;
            protected set;
        }

        [OdinSerialize]
        public string UnidentifiedName
        {
            get;
            protected set;
        }

        [OdinSerialize]
        public IAbility[] Abilities
        {
            get;
            protected set;
        }

        [OdinSerialize]
        public float Weight
        {
            get;
            protected set;
        }

        [OdinSerialize]
        public float Size
        {
            get;
            protected set;
        }

        [OdinSerialize]
        public IItemMaterial Material
        {
            get;
            protected set;
        }

        [OdinSerialize]
        public string[] Slots
        {
            get;
            protected set;
        }

        public int BaseProtection
        {
            get
            {
                return this.Material.Bonus;
            }
        }

        public int BaseEfficiency
        {
            get
            {
                return this.Material.Bonus;
            }
        }

        [OdinSerialize]
        public string GoverningSkill
        {
            get;
            protected set;
        }

        public string MaterialDescription
        {
            get
            {
                return "It is made of " + this.Material.Name;
            }
        }

        [OdinSerialize]
        public string ActionString
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

        public string ValueString
        {
            get
            {
                return "It is worth " + this.Value + " gold pieces.";
            }
        }

        [OdinSerialize]
        public int SpawnWeighting
        {
            get;
            protected set;
        }

        [OdinSerialize]
        public int OwnerGUID
        {
            get;
            set;
        }

        [OdinSerialize]
        public int LightLevel
        {
            get;
            protected set;
        }

        public string[] Tags
        {
            get
            {
                return this.m_Tags.ToArray();
            }
        }

        [OdinSerialize]
        public string SpriteSheet
        {
            get;
            protected set;
        }
    }
}
