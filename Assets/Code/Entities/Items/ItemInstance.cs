using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using JoyLib.Code.Managers;
using JoyLib.Code.States;
using System;
using System.Collections.Generic;
using System.Linq;
using DevionGames.InventorySystem;
using UnityEngine;

namespace JoyLib.Code.Entities.Items
{
    [Serializable]
    public class ItemInstance : JoyObject, IItemContainer
    {
        protected bool m_Identified;
        protected long m_OwnerGUID;

        protected List<long> m_Contents;
        protected BaseItemType m_Type;

        protected IAbility m_Ability;

        protected static LiveItemHandler s_ItemHandler;

        public ItemInstance(BaseItemType type, Vector2Int position, bool identified, Sprite[] sprites, Item itemSO, IAbility abilityRef = null) :
            base(type.UnidentifiedName, 
                EntityDerivedValue.GetDefaultForItem(
                    type.Material.Bonus,
                    type.Weight),
                position, 
                type.SpriteSheet, 
                new string[] {},
                sprites, 
                type.Tags)
        {        
            FindItemHandler();
                
            this.m_Type = type;
            
            this.Identified = identified;
            //chosenIcon = RNG.instance.Roll(0, m_Icons.Length - 1);

            this.m_Contents = new List<long>();

            m_Ability = abilityRef;
            this.Item = itemSO;
        }

        public ItemInstance(ItemInstance copy) :
            base(copy.m_Type.UnidentifiedName,
            EntityDerivedValue.GetDefaultForItem(
                copy.m_Type.Material.Bonus,
                copy.m_Type.Weight),
                copy.WorldPosition,
                copy.m_Type.SpriteSheet,
                copy.m_CachedActions.ToArray(),
                copy.m_Icons,
                copy.m_Type.Tags)
        {
            FindItemHandler();

            this.m_Type = copy.m_Type;
            this.Identified = copy.Identified;
            this.m_Contents = copy.m_Contents;
            this.m_Ability = copy.m_Ability;
            this.Item = copy.Item;
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
            chosenIcon = RNG.instance.Roll(0, m_Icons.Length - 1);

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
            chosenIcon = RNG.instance.Roll(0, m_Icons.Length - 1);

            m_Contents = contents;
        }
        */

        protected void FindItemHandler()
        {
            if(s_ItemHandler is null)
            {
                s_ItemHandler = GameObject.Find("GameManager")
                                    .GetComponent<LiveItemHandler>();
            }
        }

        public void Interact(Entity user)
        {
            //PythonEngine.ExecuteClassFunction(m_InteractionFile, m_ClassName, "Interact", new dynamic[] { user, this });

            //object[] arguments = { new MoonEntity(user), new MoonItem(this) };
            //ScriptingEngine.RunScript(ItemType.InteractionFileContents, ItemType.InteractionFileName, "Interact", arguments);

            m_Ability.OnInteract(user, this);

            if(!Identified)
            {
                IdentifyMe();
                user.AddIdentifiedItem(DisplayName);
            }
            //Identify any identical items the user is carrying
            for(int i = 0; i < user.Backpack.Length; i++)
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
                ItemInstance item = s_ItemHandler.GetInstance(m_Contents[index]);
                m_Contents.RemoveAt(index);
                return item;
            }

            return null;
        }

        public List<ItemInstance> GetContents()
        {
            List<ItemInstance> contents = new List<ItemInstance>(m_Contents.Count);

            foreach(long id in m_Contents)
            {
                contents.Add(s_ItemHandler.GetInstance(id));
            }

            return contents;
        }

        public bool AddContents(JoyObject actor)
        {
            m_Contents.Add(actor.GUID);

            return true;
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
                return (int)(m_Type.Material.Bonus * (float)(HitPointsRemaining / HitPoints));
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
                if (this.ItemType.Slots.Contains("None"))
                {
                    return "This item can be thrown.";
                }
                else
                {
                    ;
                    return "This is equipped to " + string.Join(", ", this.ItemType.Slots);
                }
            }
        }

        public string DisplayName
        {
            get
            {
                if (Identified)
                {
                    return this.JoyName;
                }
                else
                {
                    return m_Type.UnidentifiedName;
                }
            }
        }

        public string IdentifiedName
        {
            get
            {
                return this.JoyName;
            }
        }

        public string DisplayDescription
        {
            get
            {
                if (Identified)
                {
                    return m_Type.Description;
                }
                else
                {
                    return m_Type.UnidentifiedDescription;
                }
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
                    contents.Add(s_ItemHandler.GetInstance(GUID));
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

        public int Value
        {
            get
            {
                return (int)(m_Type.Value * m_Type.Material.ValueMod);
            }
        }

        public Item Item
        {
            get;
            protected set;
        }
    }
}
