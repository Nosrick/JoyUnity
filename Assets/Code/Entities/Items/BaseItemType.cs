using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities.Abilities;

namespace JoyLib.Code.Entities.Items
{
    public class BaseItemType
    {
        protected string m_ClassName;
        protected List<string> m_Tags;

        public BaseItemType(
            string[] tags, 
            string description, 
            string unidentifiedDescriptionRef, 
            string unidentifiedNameRef, 
            string identifiedNameRef, 
            string[] slotsRef, 
            float size, 
            ItemMaterial material, 
            string governingSkillRef, 
            string actionStringRef, 
            int valueRef, 
            int spawnRef, 
            string spriteSheet, 
            int lightLevel = 0,
            IAbility[] abilities = null)
        {
            m_Tags = tags.ToList();
            
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
            this.Effects = abilities;
        }

        public bool AddTag(string tag)
        {
            if(m_Tags.Contains(tag) == false)
            {
                m_Tags.Add(tag);
                return true;
            }
            return false;
        }

        public bool RemoveTag(string tag)
        {
            if(m_Tags.Contains(tag) == true)
            {
                m_Tags.Remove(tag);
                return true;
            }
            return false;
        }

        public bool HasTag(string tag)
        {
            return m_Tags.Contains(tag);
        }

        public bool HasSlot(string slot)
        {
            return Slots.Contains(slot);
        }

        public int GetHitPoints()
        {
            return (int)(Math.Max(1, Size * Material.Hardness));
        }

        public string Description
        {
            get;
            protected set;
        }

        public string IdentifiedName
        {
            get;
            protected set;
        }

        public string UnidentifiedDescription
        {
            get;
            protected set;
        }

        public string UnidentifiedName
        {
            get;
            protected set;
        }

        public IAbility[] Effects
        {
            get;
            protected set;
        }

        public float Weight
        {
            get;
            protected set;
        }

        public float Size
        {
            get;
            protected set;
        }

        public ItemMaterial Material
        {
            get;
            protected set;
        }

        public string[] Slots
        {
            get;
            protected set;
        }

        public int BaseProtection
        {
            get
            {
                return Material.Bonus;
            }
        }

        public int BaseEfficiency
        {
            get
            {
                return Material.Bonus;
            }
        }

        public string GoverningSkill
        {
            get;
            protected set;
        }

        public string MaterialDescription
        {
            get
            {
                return "It is made of " + Material.Name + ".";
            }
        }

        public string ActionString
        {
            get;
            protected set;
        }

        public int Value
        {
            get;
            protected set;
        }

        public string ValueString
        {
            get
            {
                return "It is worth " + Value + " gold pieces.";
            }
        }

        public int SpawnWeighting
        {
            get;
            protected set;
        }

        public int OwnerGUID
        {
            get;
            set;
        }

        public int LightLevel
        {
            get;
            protected set;
        }

        public string[] Tags
        {
            get
            {
                return m_Tags.ToArray();
            }
        }

        public string SpriteSheet
        {
            get;
            protected set;
        }
    }
}
