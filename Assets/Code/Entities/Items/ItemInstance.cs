using JoyLib.Code.Graphics;
using JoyLib.Code.Managers;
using JoyLib.Code.Scripting;
using JoyLib.Code.States;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JoyLib.Code.Entities.Items
{
    [Serializable]
    public class ItemInstance : JoyObject
    {
        protected bool m_Identified;
        protected long m_OwnerGUID;

        protected List<long> m_Contents;
        protected BaseItemType m_Type;

        protected string m_InteractionFile;

        public ItemInstance(BaseItemType type, Vector2Int position, bool identified, string interactionFile = null) :
            base(type.UnidentifiedName, type.GetHitPoints(), position, ObjectIcons.GetSprites(type.Category, type.UnidentifiedName), type.Category, false)
        {            
            this.m_Type = type;
            this.Move(position);

            this.m_HitPoints = type.GetHitPoints();
            this.m_HitPointsRemaining = this.m_HitPoints;
            this.GUID = GUIDManager.AssignGUID();
            this.Identified = identified;
            //chosenIcon = RNG.Roll(0, m_Icons.Length - 1);

            this.m_Contents = new List<long>();

            m_InteractionFile = interactionFile;
        }

        public ItemInstance(ItemInstance item) :
            base(item.ItemType.UnidentifiedName, item.ItemType.GetHitPoints(), item.WorldPosition, item.Icons, item.BaseType, item.IsAnimated, false, false)
        {
            this.m_Type = item.ItemType;
            this.Move(item.WorldPosition);

            this.m_HitPoints = item.ItemType.GetHitPoints();
            this.m_HitPointsRemaining = this.m_HitPoints;
            this.GUID = GUIDManager.AssignGUID();
            this.Identified = item.Identified;

            this.m_Contents = item.m_Contents;

            m_InteractionFile = item.m_InteractionFile;
        }

        /*
        public ItemInstance(BaseItemType type, int HPRemaining, Vector2Int position, bool identified) : 
            base(type.description, type.unidentifiedDescription, type.unidentifiedName, type.name, type.slot, type.size, 
            type.material, position, type.baseType, type.governingSkill, type.actionString, type.interactionFile, 
            type.value, type.spawnWeighting, type.LightLevel)
        {
            m_HPRemaining = HPRemaining;
            GUIDManager.ReleaseGUID(GUID);
            GUID = GUIDManager.AssignGUID();
            this.identified = identified;
            chosenIcon = RNG.Roll(0, m_Icons.Length - 1);

            m_Contents = new List<ItemInstance>();
        }

        [JsonConstructor]
        public ItemInstance(List<ItemInstance> contents, int HPRemaining, bool identified, string description, string unidentifiedDescription, 
            string unidentifiedName, string name, string slot, float size, Material material, Vector2Int position, string baseType, string governingSkill, 
            string actionString, string interactionFile, int value, int spawnWeighting, int lightLevel) :
            base(description, unidentifiedDescription, unidentifiedName, name, slot, size, material, position, 
                baseType, governingSkill, actionString, interactionFile, value, spawnWeighting, lightLevel)
        {
            m_HPRemaining = HPRemaining;
            GUIDManager.ReleaseGUID(GUID);
            GUID = GUIDManager.AssignGUID();
            this.identified = identified;
            chosenIcon = RNG.Roll(0, m_Icons.Length - 1);

            m_Contents = contents;
        }
        */

        public void Interact(Entity user)
        {
            //PythonEngine.ExecuteClassFunction(m_InteractionFile, m_ClassName, "Interact", new dynamic[] { user, this });

            //object[] arguments = { new MoonEntity(user), new MoonItem(this) };
            //ScriptingEngine.RunScript(ItemType.InteractionFileContents, ItemType.InteractionFileName, "Interact", arguments);

            object[] arguments = { user, this };
            ScriptingEngine.Execute(this.GetType().Name, "Interact", arguments);

            if(!Identified)
            {
                IdentifyMe();
                user.AddIdentifiedItem(DisplayName);
            }
            //Identify any identical items the user is carrying
            for(int i = 0; i < user.Backpack.Count; i++)
            {
                if(user.Backpack[i].DisplayName.Equals(DisplayName) && !user.Backpack[i].Identified)
                {
                    user.Backpack[i].IdentifyMe();
                }
            }
        }

        public void IdentifyMe()
        {
            Identified = true;
        }

        public void PutItem(long item)
        {
            m_Contents.Add(item);
        }

        public ItemInstance TakeMyItem(int index)
        {
            if(index > 0 && index < m_Contents.Count)
            {
                ItemInstance item = WorldState.ItemHandler.GetInstance(m_Contents[index]);
                m_Contents.RemoveAt(index);
                return item;
            }

            return null;
        }

        public long OwnerGUID
        {
            get
            {
                return m_OwnerGUID;
            }
            set
            {
                m_OwnerGUID = value;
            }
        }

        public bool Identified
        {
            get
            {
                return m_Identified;
            }
            protected set
            {
                m_Identified = value;
            }
        }

        public bool Broken
        {
            get
            {
                return this.HitPointsRemaining <= 0;
            }
        }

        public int Efficiency
        {
            get
            {
                return (int)(m_Type.Material.bonus * (float)(HitPointsRemaining / HitPoints));
            }
        }

        public string ConditionString
        {
            get
            {
                if (HitPointsRemaining / HitPoints == 1)
                {
                    return "It is in great condition.";
                }
                else if (HitPointsRemaining / HitPoints > 0.75)
                {
                    return "It is in good condition.";
                }
                else if (HitPointsRemaining / HitPoints > 0.5)
                {
                    return "It is in fair condition.";
                }
                else if (HitPointsRemaining / HitPoints > 0.25)
                {
                    return "It is in poor condition.";
                }
                else
                {
                    return "It is close to breaking.";
                }
            }
        }

        public string SlotString
        {
            get
            {
                if (this.ItemType.Slot == "None")
                {
                    return "This item can be thrown.";
                }
                else
                {
                    return "This is equipped to the " + this.ItemType.Slot + " slot.";
                }
            }
        }

        public string DisplayName
        {
            get
            {
                if (Identified)
                    return this.JoyName;
                else
                    return m_Type.UnidentifiedName;
            }
        }

        public string DisplayDescription
        {
            get
            {
                if (Identified)
                    return m_Type.Description;
                else
                    return m_Type.UnidentifiedDescription;
            }
        }

        public float Weight
        {
            get
            {
                float weight = m_Type.Weight;
                List<ItemInstance> contents = Contents;
                for(int i = 0; i < contents.Count; i++)
                {
                    weight += contents[i].ItemType.Weight;
                }

                return weight;
            }
        }

        public string WeightString
        {
            get
            {
                return "It weighs " + Weight + " grams.";
            }
        }

        public List<ItemInstance> Contents
        {
            get
            {
                List<ItemInstance> contents = new List<ItemInstance>();
                foreach(long GUID in m_Contents)
                {
                    contents.Add(WorldState.ItemHandler.GetInstance(GUID));
                }
                return contents;
            }
        }

        public string ContentString
        {
            get
            {
                string contentString = "It contains ";

                List<ItemInstance> items = Contents;

                if (items.Count == 0)
                {
                    contentString = "";
                    return contentString;
                }

                while (items.Count > 0)
                {
                    List<ItemInstance> types = items.Where(x => x.JoyName == items[0].JoyName).ToList();
                    string name = types[0].JoyName;
                    contentString += types.Count + " " + name + ", ";

                    List<ItemInstance> itemsToRemove = new List<ItemInstance>();
                    for (int i = 0; i < items.Count; i++)
                    {
                        if (items[i].JoyName == name)
                        {
                            itemsToRemove.Add(items[i]);
                        }
                    }

                    for (int i = 0; i < itemsToRemove.Count; i++)
                    {
                        items.Remove(itemsToRemove[i]);
                    }
                }
                contentString = contentString.Substring(0, contentString.Length - 2);
                return contentString;
            }
        }

        public BaseItemType ItemType
        {
            get
            {
                return m_Type;
            }
        }
    }
}
