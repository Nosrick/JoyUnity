using JoyLib.Code.Graphics;
using JoyLib.Code.Helpers;
using JoyLib.Code.Managers;
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
        protected int m_OwnerGUID;

        protected List<ItemInstance> m_Contents;
        protected BaseItemType m_Type;

        public static ItemInstance Create(BaseItemType type, Vector2Int position, bool identified)
        {
            ItemInstance newInstance = Instantiate(Resources.Load<ItemInstance>("Prefabs/ItemInstance"));

            Texture2D[] textures = ObjectIcons.GetIcons(type.Category, type.UnidentifiedName);
            List<Sprite> sprites = new List<Sprite>(textures.Length);
            for (int i = 0; i < textures.Length; i++)
            {
                sprites.Add(Sprite.Create(textures[i], new Rect(0, 0, 16, 16), Vector2.zero, 16));
            }

            newInstance.Initialise(type.UnidentifiedName, type.GetHitPoints(), position, sprites, type.Category, false);

            newInstance.m_Type = type;
            newInstance.Move(position);

            newInstance.m_HitPoints = type.GetHitPoints();
            newInstance.m_HitPointsRemaining = newInstance.m_HitPoints;
            newInstance.GUID = GUIDManager.AssignGUID();
            newInstance.Identified = identified;
            //chosenIcon = RNG.Roll(0, m_Icons.Length - 1);

            newInstance.m_Contents = new List<ItemInstance>();

            return newInstance;
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

        public void Interact(Entity user)
        {
            if (m_InteractionFile == null)
                return;

            PythonEngine.ExecuteClassFunction(m_InteractionFile, m_ClassName, "Interact", new dynamic[] { user, this });
            if(!identified)
            {
                IdentifyMe();
                user.AddIdentifiedItem(name);
            }
            for(int i = 0; i < user.backpack.Count; i++)
            {
                if(user.backpack[i].name.Equals(name) && !user.backpack[i].identified)
                {
                    user.backpack[i].IdentifyMe();
                }
            }
        }
        */

        public void IdentifyMe()
        {
            Identified = true;
        }

        public void PutItem(ItemInstance item)
        {
            m_Contents.Add(item);
        }

        public ItemInstance TakeItem(int index)
        {
            if(index > 0 && index < m_Contents.Count)
            {
                ItemInstance item = m_Contents[index];
                m_Contents.RemoveAt(index);
                return item;
            }

            return null;
        }

        public int OwnerGUID
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

                for(int i = 0; i < m_Contents.Count; i++)
                {
                    weight += m_Contents[i].ItemType.Weight;
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
                return m_Contents.ToList();
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
                    List<ItemInstance> types = items.Where(x => x.name == items[0].name).ToList();
                    string name = types[0].name;
                    contentString += types.Count + " " + name + ", ";

                    List<ItemInstance> itemsToRemove = new List<ItemInstance>();
                    for (int i = 0; i < items.Count; i++)
                    {
                        if (items[i].name == name)
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
