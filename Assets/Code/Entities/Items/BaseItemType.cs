using System;
using System.IO;

namespace JoyLib.Code.Entities.Items
{
    public class BaseItemType
    {
        protected string m_ClassName;

        public BaseItemType(string category, string description, string unidentifiedDescriptionRef, string unidentifiedNameRef, string identifiedNameRef, 
            string slotRef, float size, ItemMaterial material, string typeRef, string governingSkillRef, string actionStringRef, 
            string interactionFileRef, int valueRef, int spawnRef, int lightLevel = 0)
        {
            this.Category = category;
            this.SpawnWeighting = spawnRef;
            this.Value = valueRef;
            this.InteractionFileName = interactionFileRef;
            if (InteractionFileName != null && InteractionFileName != "")
            {
                InteractionFileContents = File.ReadAllText(Directory.GetCurrentDirectory() + InteractionFilePath);
            }

            this.Description = description;
            this.UnidentifiedDescription = unidentifiedDescriptionRef;
            this.UnidentifiedName = unidentifiedNameRef;
            this.Size = size;
            this.Material = material;
            this.Weight = this.Size * this.Material.weight;
            this.Slot = slotRef;
            this.GoverningSkill = governingSkillRef;
            this.ActionString = actionStringRef;
            this.LightLevel = lightLevel;
        }

        public int GetHitPoints()
        {
            return (int)(Math.Max(1, Size * Material.hardness));
        }

        public string Category
        {
            get;
            protected set;
        }

        public string InteractionFilePath
        {
            get
            {
                return GlobalConstants.SCRIPTS_FOLDER + "Items/" + InteractionFileName + ".lua";
            }
        }

        public string InteractionFileName
        {
            get;
            protected set;
        }

        public string InteractionFileContents
        {
            get;
            protected set;
        }

        public string Description
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

        public string Slot
        {
            get;
            protected set;
        }

        public int BaseProtection
        {
            get
            {
                return Material.bonus;
            }
        }

        public int BaseEfficiency
        {
            get
            {
                return Material.bonus;
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
                return "It is made of " + Material.name + ".";
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
    }
}
