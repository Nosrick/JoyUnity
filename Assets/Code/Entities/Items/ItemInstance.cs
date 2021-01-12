using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Events;
using JoyLib.Code.Graphics;
using JoyLib.Code.Rollers;
using JoyLib.Code.Scripting;
using JoyLib.Code.Unity;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JoyLib.Code.Entities.Items
{
    [Serializable]
    public class ItemInstance : JoyObject, IItemInstance
    {
        protected const string DURABILITY = "durability";

        protected IEntity User { get; set; }
        
        protected bool m_Identified;

        protected List<long> m_Contents;
        protected BaseItemType m_Type;

        protected long m_OwnerGUID;
        protected string m_OwnerString;
        
        protected int StateIndex { get; set; }

        protected int m_Value;

        public long OwnerGUID
        {
            get
            {
                return this.m_OwnerGUID;
            }
            protected set
            {
                this.m_OwnerGUID = value;
                this.m_OwnerString = EntityHandler?.Get(this.m_OwnerGUID).JoyName;
            }
        }

        public string OwnerString
        {
            get => this.m_OwnerString;
        }

        public IEnumerable<IAbility> UniqueAbilities { get; protected set; }
        
        protected GameObject Prefab { get; set; }

        public IEnumerable<ISpriteState> Sprites => this.States;

        public static ILiveItemHandler ItemHandler { get; set; }
        public static ILiveEntityHandler EntityHandler { get; set; }

        public ItemInstance(
            BaseItemType type, 
            IDictionary<string, IDerivedValue> derivedValues,
            Vector2Int position, 
            bool identified, 
            IEnumerable<ISpriteState> sprites,
            IRollable roller = null,
            IEnumerable<IAbility> uniqueAbilities = null,
            IEnumerable<IJoyAction> actions = null,
            GameObject gameObject = null,
            bool active = false)
            : base(
                type.UnidentifiedName,
                derivedValues,
                position,
                actions,
                sprites,
                roller,
                type.Tags)
        {
            if (this.Prefab is null)
            {
                if (gameObject is null == false)
                {
                    this.Prefab = gameObject;
                }
                this.Prefab = Resources.Load<GameObject>("Prefabs/ItemInstance");
            }

            this.Initialise();
                
            this.m_Type = type;
            
            this.Identified = identified;

            this.m_Contents = new List<long>();

            this.UniqueAbilities = uniqueAbilities is null == false ? new List<IAbility>(uniqueAbilities) : new List<IAbility>();

            this.CalculateValue();
            this.ConstructDescription();

            if (this.States.Count > 1)
            {
                this.StateIndex = this.Roller.Roll(0, this.States.Count);
            }
            else
            {
                this.StateIndex = 0;
            }

            if (this.Prefab is null == false)
            {
                this.Instantiate(gameObject, active);
            }
        }

        public void Instantiate(GameObject gameObject = null, bool active = false)
        {
            if (gameObject is null)
            {
                GameObject newOne = Object.Instantiate(this.Prefab);
                newOne.GetComponent<MonoBehaviourHandler>().AttachJoyObject(this);
            }
            else
            {
                MonoBehaviourHandler monoBehaviourHandler = gameObject.GetComponent<MonoBehaviourHandler>();
                monoBehaviourHandler.AttachJoyObject(this);
            }
            this.MonoBehaviourHandler.Clear();
            this.MonoBehaviourHandler.AddSpriteState(this.States[this.StateIndex]);
            this.MonoBehaviourHandler.gameObject.SetActive(active);
        }

        public IItemInstance Copy(IItemInstance copy)
        {
            this.Initialise();

            ItemInstance newItem = new ItemInstance(
                copy.ItemType,
                copy.DerivedValues,
                copy.WorldPosition,
                copy.Identified,
                copy.States,
                copy.Roller,
                copy.UniqueAbilities,
                copy.CachedActions.ToArray());

            ItemHandler.AddItem(newItem);
            return newItem;
        }

        protected void ConstructDescription()
        {
            List<Tuple<string, string>> data = new List<Tuple<string, string>>
            {
                new Tuple<string, string>(
                    "",
                    this.ConditionString),
                new Tuple<string, string>(
                    "",
                    this.WeightString),
                new Tuple<string, string>(
                    "",
                    this.ItemType.MaterialDescription),
                new Tuple<string, string>(
                    "",
                    this.SlotString)
            };
            /*data.Add(new Tuple<string, string>(
                "",
                this.Identified
                ? this.ItemType.Description
                : this.ItemType.UnidentifiedDescription));*/
            if (this.ContentString.IsNullOrEmpty() == false)
            {
                data.Add(new Tuple<string, string>(
                    "",
                    this.ContentString));
            }
            
            if (this.OwnerString.IsNullOrEmpty() == false)
            {
                data.Add(new Tuple<string, string>(
                    "",
                    "Owned by " + this.OwnerString));
            }
            else
            {
                data.Add(new Tuple<string, string>(
                    "",
                    "This is not owned"));
            }
            
            data.Add(new Tuple<string, string>(
                    "",
                    "Worth " + this.Value));

            this.Tooltip = data;
        }
        
        public void SetUser(IEntity user)
        {
            this.User = user;
        }

        public void Use()
        {
            if (this.AllAbilities.Any() == false || this.User is null)
            {
                return;
            }

            foreach (IAbility ability in this.AllAbilities)
            {
                ability.OnUse(this.User, this);
            }

            foreach (IAbility ability in this.User.Abilities)
            {
                ability.OnUse(this.User, this);
            }
            
            this.CalculateValue();
            this.ConstructDescription();
        }

        public override string ToString()
        {
            return "{ " + this.JoyName + " : " + this.GUID + "}";
        }

        protected void Initialise()
        {
            if (GlobalConstants.GameManager is null)
            {
                return;
            }
            ItemHandler = GlobalConstants.GameManager.ItemHandler;
            EntityHandler = GlobalConstants.GameManager.EntityHandler;
        }
        
        public void SetOwner(long newOwner, bool recursive = false)
        {
            this.OwnerGUID = newOwner;

            if (recursive)
            {
                foreach (IItemInstance item in this.Contents)
                {
                    item.SetOwner(newOwner, true);
                }
            }

            this.ConstructDescription();
        }

        public void Interact(IEntity user)
        {
            this.SetUser(user);

            this.Use();

            if(!this.Identified)
            {
                this.IdentifyMe();
                user.AddIdentifiedItem(this.DisplayName);
            }
            //Identify any identical items the user is carrying
            foreach (IItemInstance item in user.Backpack)
            {
                if(item.DisplayName.Equals(this.DisplayName) && !item.Identified)
                {
                    item.IdentifyMe();
                }
            }
        }

        public void IdentifyMe()
        {
            this.Identified = true;
            this.JoyName = this.IdentifiedName;
            
            this.ConstructDescription();
        }

        public IItemInstance TakeMyItem(int index)
        {
            if(index > 0 && index < this.m_Contents.Count)
            {
                IItemInstance item = ItemHandler?.GetItem(this.m_Contents[index]);
                this.m_Contents.RemoveAt(index);
                return item;
            }

            return null;
        }

        public bool Contains(IItemInstance actor)
        {
            if (this.Contents.Contains(actor))
            {
                return true;
            }

            bool result = false;
            IEnumerable<IItemInstance> items = this.Contents.Where(instance => instance.HasTag("container"));
            foreach (IItemInstance c in items)
            {
                result |= c.Contains(actor);
                if (result)
                {
                    return true;
                }
            }

            return false;
        }

        public bool CanAddContents(IItemInstance actor)
        {
            if (actor.GUID == this.GUID 
            || this.Contains(actor) 
            || actor.Contains(this))
            {
                return false;
            }

            return true;
        }

        public bool AddContents(IItemInstance actor)
        {
            if(this.CanAddContents(actor))
            {
                this.m_Contents.Add(actor.GUID);

                this.CalculateValue();
                this.ConstructDescription();
                
                this.ItemAdded?.Invoke(this, new ItemChangedEventArgs() { Item = actor });
                return true;
            }

            return false;
        }

        public bool AddContents(IEnumerable<IItemInstance> actors)
        {
            IEnumerable<IItemInstance> itemInstances = actors as IItemInstance[] ?? actors.ToArray();
            this.m_Contents.AddRange(itemInstances.Where(actor => 
                    this.m_Contents.Any(itemGUID => itemGUID == actor.GUID) == false)
                .Select(actor => actor.GUID));

            this.CalculateValue();
            this.ConstructDescription();
            foreach (IItemInstance actor in itemInstances)
            {
                this.ItemAdded?.Invoke(this, new ItemChangedEventArgs { Item = actor });
            }

            return true;
        }

        public bool RemoveContents(IItemInstance actor)
        {
            if (!this.m_Contents.Remove(actor.GUID))
            {
                return false;
            }

            this.CalculateValue();
            this.ConstructDescription();
            this.ItemRemoved?.Invoke(this, new ItemChangedEventArgs { Item = actor });

            return true;
        }

        public virtual bool RemoveContents(IEnumerable<IItemInstance> actors)
        {
            return actors.Aggregate(true, (current, actor) => current & this.RemoveContents(actor));
        }

        public void Clear()
        {
            List<IItemInstance> copy = new List<IItemInstance>(this.Contents);
            foreach (IItemInstance item in copy)
            {
                this.RemoveContents(item);
            }

            this.CalculateValue();
            this.ConstructDescription();
        }

        protected void CalculateValue()
        {
            this.m_Value = (int)(this.m_Type.Value * this.m_Type.Material.ValueMod);
            foreach (IItemInstance item in this.Contents)
            {
                this.m_Value += item.Value;
            }
        }

        public bool Identified
        {
            get => this.m_Identified;
            protected set => this.m_Identified = value;
        }

        public bool Broken => this.HitPointsRemaining <= 0;

        public int Efficiency => (int)(this.m_Type.Material.Bonus * (this.HitPointsRemaining / (float)this.HitPoints));

        public string ConditionString
        {
            get
            {
                float durability = this.GetValue(DURABILITY);
                float maximum = this.GetMaximum(DURABILITY);
                if (durability == 0)
                {
                    return "Broken";
                }
                if (durability / maximum < 0.25)
                {
                    return "In poor condition";
                }

                if (durability / maximum < 0.5)
                {
                    return "In fair condition";
                }
                if (durability / maximum < 0.75)
                {
                    return "In good condition";
                }

                return "In great condition";
            }
        }

        public string SlotString
        {
            get
            {
                if (this.ItemType.Slots.Contains("None") || this.ItemType.Slots.IsNullOrEmpty())
                {
                    return "This item can be thrown";
                }
                return "This is equipped to " + string.Join(", ", this.ItemType.Slots);
            }
        }

        public string DisplayName => this.Identified ? this.JoyName : this.m_Type.UnidentifiedName;

        public string IdentifiedName => this.JoyName;

        public string DisplayDescription => this.Identified ? this.m_Type.Description : this.m_Type.UnidentifiedDescription;

        public float Weight
        {
            get
            {
                return this.m_Type.Weight + this.Contents.Sum(item => item.Weight);
            }
        }

        public string WeightString 
        {
            get
            {
                const string weight = "Weighs ";
                if (this.Weight < 1000)
                {
                    return weight + this.Weight + " grams";
                }
                return weight + (this.Weight / 1000f) + " kilograms";
            }
        }

        public IEnumerable<IItemInstance> Contents
        {
            get
            {
                return this.m_Contents.Select(itemGUID => ItemHandler?.GetItem(itemGUID)).ToList();
            }
        }

        public string ContentString
        {
            get
            {
                string contentString = "It contains ";

                List<IItemInstance> items = this.Contents.ToList();
                if (items.Any() == false)
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

        public event ItemRemovedEventHandler ItemRemoved;
        public event ItemAddedEventHandler ItemAdded;

        public IEnumerable<IAbility> AllAbilities
        {
            get
            {
                List<IAbility> abilities = new List<IAbility>();
                abilities.AddRange(this.UniqueAbilities);
                abilities.AddRange(this.ItemType.Abilities);
                return abilities;
            }
        }

        public BaseItemType ItemType => this.m_Type;

        public int Value => this.m_Value;
    }
}
