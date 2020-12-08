using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Internal;
using DevionGames.InventorySystem;
using JoyLib.Code.Collections;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Managers;
using JoyLib.Code.Rollers;
using JoyLib.Code.Scripting;
using JoyLib.Code.Unity;
using JoyLib.Code.World;
using UnityEngine;

namespace JoyLib.Code.Entities.Items
{
    [Serializable]
    public class ItemInstance : EquipmentItem, IItemInstance
    {
        public BasicValueContainer<IDerivedValue> DerivedValues { get; protected set; }

        public string TileSet { get; protected set; }
        
        public Vector2Int WorldPosition { get; protected set; }

        public bool IsAnimated { get; protected set; }

        public int ChosenSprite { get; protected set; }
        
        public int LastIndex { get; protected set; }

        public int FramesSinceLastChange { get; protected set; }

        public List<string> Tags { get; protected set; }

        public bool IsWall { get; protected set; }

        public bool IsDestructible { get; protected set; }

        public RNG Roller { get; protected set; }
        public IWorldInstance MyWorld { get; set; }
        
        public Sprite Sprite => Sprites[ChosenSprite];

        public Sprite[] Sprites { get; set; }

        public long GUID { get; protected set; }

        public string JoyName { get; protected set; }

        public int HitPointsRemaining => GetValue("hitpoints");

        public int HitPoints => GetMaximum("hitpoints");

        public bool Conscious => HitPointsRemaining > 0;

        public bool Alive => HitPointsRemaining > (HitPoints * (-1));
        
        protected NonUniqueDictionary<object, object> Data { get; set; }

        public List<IJoyAction> CachedActions { get; protected set; }
        
        public MonoBehaviourHandler MonoBehaviourHandler { get; protected set; }
        
        protected IEntity User { get; set; }
        
        protected bool m_Identified;

        protected List<long> m_Contents;
        protected BaseItemType m_Type;

        protected long m_OwnerGUID;
        protected string m_OwnerString;

        protected int m_Value;

        public long OwnerGUID
        {
            get
            {
                return m_OwnerGUID;
            }
            protected set
            {
                m_OwnerGUID = value;
                m_OwnerString = EntityHandler?.Get(m_OwnerGUID).JoyName;
            }
        }

        public string OwnerString
        {
            get => m_OwnerString;
        }

        public IEnumerable<IAbility> UniqueAbilities { get; protected set; }

        public static ILiveItemHandler ItemHandler { get; set; }
        public static ILiveEntityHandler EntityHandler { get; set; }

        public void Initialise(
            BaseItemType type, 
            BasicValueContainer<IDerivedValue> derivedValues,
            Vector2Int position, 
            bool identified, 
            Sprite[] sprites,
            RNG roller = null,
            IEnumerable<IAbility> uniqueAbilities = null,
            IEnumerable<IJoyAction> actions = null)
        {
            if (this.Prefab is null)
            {
                this.Prefab = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/ItemInstance"));
            }
            
            this.Roller = roller is null ? new RNG() : roller;
            this.Data = new NonUniqueDictionary<object, object>();

            this.JoyName = identified ? type.IdentifiedName : type.UnidentifiedName;
            this.Name = this.JoyName;
            this.GUID = GUIDManager.Instance.AssignGUID();

            this.DerivedValues = derivedValues;

            this.TileSet = type.SpriteSheet;
            this.Tags = type.Tags.ToList();

            this.m_Contents = new List<long>();

            this.WorldPosition = position;
            this.Move(this.WorldPosition);

            this.Sprites = sprites;

            if (this.Tags.Any(tag => tag.Equals("animated", StringComparison.OrdinalIgnoreCase)))
            {
                this.IsAnimated = true;
            }

            if (this.Tags.Any(tag => tag.Equals("invulnerable", StringComparison.OrdinalIgnoreCase)))
            {
                this.IsDestructible = false;
            }

            if (this.Tags.Any(tag => tag.Equals("wall", StringComparison.OrdinalIgnoreCase)))
            {
                this.IsWall = true;
            }

            //If it's not animated, select a random icon to represent it
            if (!this.IsAnimated && sprites != null)
            {
                this.ChosenSprite = Roller.Roll(0, sprites.Length);
            }
            else
            {
                this.ChosenSprite = 0;
            }

            this.LastIndex = 0;
            this.FramesSinceLastChange = 0;

            this.CachedActions = actions is null ? new List<IJoyAction>() : new List<IJoyAction>(actions);

            Initialise();
                
            this.m_Type = type;
            
            this.Identified = identified;
            //chosenIcon = RNG.instance.Roll(0, m_Icons.Length - 1);

            UniqueAbilities = uniqueAbilities is null == false ? new List<IAbility>(uniqueAbilities) : new List<IAbility>();
            
            this.Region = new List<EquipmentRegion>();
            foreach (string slot in type.Slots)
            {
                this.Region.Add(InventoryManager.Database.equipments.First(region => region.Name.Equals(slot)));
            }

            m_Icon = Sprite;
            CalculateValue();
            ConstructDescription();

            this.Prefab.GetComponent<MonoBehaviourHandler>().AttachJoyObject(this);
        }

        public IItemInstance Copy(IItemInstance copy)
        {
            Initialise();

            ItemInstance newItem = ScriptableObject.CreateInstance<ItemInstance>();
            
            newItem.Initialise(
                copy.ItemType,
                copy.DerivedValues,
                copy.WorldPosition,
                copy.Identified,
                copy.Sprites,
                copy.Roller,
                copy.UniqueAbilities,
                copy.CachedActions.ToArray());

            return newItem;
        }

        protected void ConstructDescription()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(this.Identified ? this.ItemType.Description : this.ItemType.UnidentifiedDescription);
            builder.AppendLine(this.WeightString);
            builder.AppendLine(this.ItemType.MaterialDescription);
            if (this.ContentString.IsNullOrEmpty() == false)
            {
                builder.AppendLine(this.ContentString);
            }
            builder.AppendLine(this.ConditionString);
            
            if (this.OwnerString.IsNullOrEmpty() == false)
            {
                builder.AppendLine("Owned by " + this.OwnerString + ".");
            }
            else
            {
                builder.AppendLine("This item is not owned.");
            }

            builder.AppendLine("It is worth " + this.Value + ".");
            
            m_Description = builder.ToString();
        }
        
        public void SetUser(IEntity user)
        {
            User = user;
        }

        public override void Use()
        {
            if ((this.AllAbilities.Count() == 0) || User is null)
            {
                return;
            }

            foreach (IAbility ability in this.AllAbilities)
            {
                ability.OnUse(User, this);
            }

            foreach (IAbility ability in User.Abilities)
            {
                ability.OnUse(User, this);
            }
            
            CalculateValue();
            ConstructDescription();
        }

        public IJoyAction FetchAction(string name)
        {
            return CachedActions.First(action => action.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public bool AddTag(string tag)
        {
            if (Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase)) != false)
            {
                return false;
            }
            
            Tags.Add(tag);
            return true;
        }

        public bool RemoveTag(string tag)
        {
            if (!Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
            
            Tags.Remove(tag);
            return true;
        }

        public bool HasTag(string tag)
        {
            return Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase));
        }

        public void Move(Vector2Int newPosition)
        {
            this.WorldPosition = newPosition;
            foreach (IJoyObject joyObject in Contents)
            {
                joyObject.Move(newPosition);
            }
        }

        public int DamageValue(string name, int value)
        {
            return this.ModifyValue(name, -value);
        }

        public int RestoreValue(string name, int value)
        {
            return this.ModifyValue(name, value);
        }

        public int GetValue(string name)
        {
            if (DerivedValues.Has(name))
            {
                return DerivedValues.Get(name);
            }
            
            throw new InvalidOperationException("Derived value of " + name + " not found on JoyObject " + this.ToString());
        }

        public int GetMaximum(string name)
        {
            if (DerivedValues.Has(name))
            {
                return DerivedValues.GetRawValue(name).Maximum;
            }
            
            throw new InvalidOperationException("Derived value of " + name + " not found on JoyObject " + this.ToString());
        }

        public int ModifyValue(string name, int value)
        {
            if (DerivedValues.Has(name))
            {
                int result = DerivedValues[name].ModifyValue(value);
                ConstructDescription();
                return result;
            }

            throw new InvalidOperationException("Derived value of " + name + " not found on JoyObject " + this.ToString());
        }

        //Used for deserialisation
        public void SetIcons(Sprite[] sprites)
        {
            Sprites = sprites;
        }

        // Update is called once per frame
        public virtual void Update ()
        {
            FramesSinceLastChange += 1;

            if(IsAnimated == false)
            {
                return;
            }

            if (FramesSinceLastChange != GlobalConstants.FRAMES_PER_SECOND)
            {
                return;
            }
            
            ChosenSprite += 1;
            ChosenSprite %= Sprites.Length;

            FramesSinceLastChange = 0;

            m_Icon = Sprite;
        }

        public int CompareTo(object obj)
        {
            switch (obj)
            {
                case null:
                    return 1;
                
                case JoyObject joyObject:
                    return this.GUID.CompareTo(joyObject.GUID);
                
                default:
                    throw new ArgumentException("Object is not a JoyObject");
            }
        }

        public override string ToString()
        {
            return "{ " + this.JoyName + " : " + this.GUID + "}";
        }

        public bool AddData(object key, object value)
        {
            Data.Add(key, value);
            return true;
        }

        public bool RemoveData(object key)
        {
            return Data.RemoveByKey(key) > 0;
        }

        public bool HasDataKey(object search)
        {
            return Data.ContainsKey(search);
        }

        public bool HasDataValue(object search)
        {
            return Data.ContainsValue(search);
        }

        public object[] GetDataValues(object key)
        {
            return Data.Where(tuple => tuple.Item1.Equals(key))
                .Select(tuple => tuple.Item2)
                .ToArray();
        }

        public object[] GetDataKeysForValue(object value)
        {
            return Data.Where(tuple => tuple.Item2.Equals(value))
                .Select(tuple => tuple.Item1)
                .ToArray();
        }

        public void AttachMonoBehaviourHandler(MonoBehaviourHandler mbh)
        {
            MonoBehaviourHandler = mbh;
        }

        protected void Initialise()
        {
            if(GlobalConstants.GameManager is null == false)
            {
                ItemHandler = GlobalConstants.GameManager.ItemHandler;
                EntityHandler = GlobalConstants.GameManager.EntityHandler;
            }
        }
        
        public void SetOwner(long newOwner, bool recursive = false)
        {
            OwnerGUID = newOwner;

            if (recursive)
            {
                foreach (ItemInstance item in this.Contents)
                {
                    item.SetOwner(newOwner, recursive);
                }
            }
            
            ConstructDescription();
        }

        public void Interact(IEntity user)
        {
            SetUser(user);

            Use();

            if(!Identified)
            {
                IdentifyMe();
                user.AddIdentifiedItem(DisplayName);
            }
            //Identify any identical items the user is carrying
            foreach (ItemInstance item in user.Backpack)
            {
                if(item.DisplayName.Equals(DisplayName) && !item.Identified)
                {
                    item.IdentifyMe();
                }
            }
        }

        public void IdentifyMe()
        {
            Identified = true;
        }

        public IItemInstance TakeMyItem(int index)
        {
            if(index > 0 && index < m_Contents.Count)
            {
                IItemInstance item = ItemHandler?.GetItem(m_Contents[index]);
                m_Contents.RemoveAt(index);
                return item;
            }

            return null;
        }

        public bool AddContents(IItemInstance actor)
        {
            m_Contents.Add(actor.GUID);

            CalculateValue();
            ConstructDescription();

            return true;
        }

        public bool AddContents(IEnumerable<IItemInstance> actors)
        {
            m_Contents.AddRange(actors.Select(instance => instance.GUID));
            
            CalculateValue();
            ConstructDescription();

            return true;
        }

        public bool RemoveContents(IItemInstance actor)
        {
            bool result = m_Contents.Remove(actor.GUID);
            if (!result)
            {
                return result;
            }
            CalculateValue();
            ConstructDescription();

            return result;
        }

        public void Clear()
        {
            m_Contents.Clear();
            CalculateValue();
            ConstructDescription();
        }

        protected void CalculateValue()
        {
            m_Value = (int)(m_Type.Value * m_Type.Material.ValueMod);
            foreach (ItemInstance item in this.Contents)
            {
                m_Value += item.Value;
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

        public new string DisplayName
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
                List<IItemInstance> contents = Contents;
                for(int i = 0; i < contents.Count; i++)
                {
                    weight += contents[i].Weight;
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

        public List<IItemInstance> Contents
        {
            get
            {
                List<IItemInstance> contents = new List<IItemInstance>();
                foreach(long GUID in m_Contents)
                {
                    contents.Add(ItemHandler?.GetItem(GUID));
                }
                return contents;
            }
        }

        public string ContentString
        {
            get
            {
                string contentString = "It contains ";

                List<IItemInstance> items = Contents;

                if (items.Count == 0)
                {
                    contentString = "";
                    return contentString;
                }

                while (items.Count > 0)
                {
                    List<IItemInstance> types = items.Where(x => x.JoyName == items[0].JoyName).ToList();
                    string name = types[0].JoyName;
                    contentString += types.Count + " " + name + (types.Count > 1 ? "s, " : ", ");

                    List<IItemInstance> itemsToRemove = new List<IItemInstance>();
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
                contentString = contentString.Substring(0, contentString.Length - 2) + ".";
                return contentString;
            }
        }

        public IEnumerable<IAbility> AllAbilities
        {
            get
            {
                List<IAbility> abilities = new List<IAbility>();
                abilities.AddRange(UniqueAbilities);
                abilities.AddRange(ItemType.Abilities);
                return abilities;
            }
        }

        public BaseItemType ItemType => m_Type;

        public int Value => m_Value;
    }
}
