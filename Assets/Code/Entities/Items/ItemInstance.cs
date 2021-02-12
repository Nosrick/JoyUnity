﻿using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using JoyLib.Code.Collections;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Events;
using JoyLib.Code.Graphics;
using JoyLib.Code.Rollers;
using JoyLib.Code.Scripting;
using JoyLib.Code.Unity;
using JoyLib.Code.World;
using Sirenix.OdinSerializer;
using UnityEngine;

namespace JoyLib.Code.Entities.Items
{
    [Serializable]
    public class ItemInstance : JoyObject, IItemInstance
    {
        [NonSerialized]
        protected const string DURABILITY = "durability";

        protected IEntity User { get; set; }
        
        [OdinSerialize]
        protected bool m_Identified;

        [OdinSerialize]
        protected List<Guid> m_Contents;
        
        [OdinSerialize]
        protected BaseItemType m_Type;

        [OdinSerialize]
        protected Guid m_OwnerGUID;
        
        [OdinSerialize]
        protected string m_OwnerString;
        
        protected int StateIndex { get; set; }

        [OdinSerialize]
        protected int m_Value;

        public override IWorldInstance MyWorld
        {
            get => this.m_World;
            set
            {
                this.m_World = value;
                foreach (IItemInstance content in this.Contents)
                {
                    content.MyWorld = value;
                }
            }
        }

        protected IWorldInstance m_World;

        public Guid OwnerGUID
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

        [OdinSerialize]
        public IEnumerable<IAbility> UniqueAbilities { get; protected set; }
        
        protected GameObject Prefab { get; set; }

        public IEnumerable<ISpriteState> Sprites => this.States;
        
        public ILiveItemHandler ItemHandler { get; set; }
        
        public ILiveEntityHandler EntityHandler { get; set; }

        public ItemInstance(
            Guid guid,
            BaseItemType type, 
            IDictionary<string, IDerivedValue> derivedValues,
            Vector2Int position, 
            bool identified, 
            IEnumerable<ISpriteState> sprites,
            IRollable roller = null,
            IEnumerable<IAbility> uniqueAbilities = null,
            IEnumerable<IJoyAction> actions = null,
            GameObject gameObject = null,
            List<IItemInstance> contents = null,
            bool active = false)
            : base(
                type.UnidentifiedName, 
                guid,
                derivedValues,
                position,
                actions,
                sprites,
                type.SpriteSheet,
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

            this.m_Contents = contents is null ? new List<Guid>() : contents.Select(instance => instance.Guid).ToList();

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

            this.ItemHandler = GlobalConstants.GameManager.ItemHandler;
            this.EntityHandler = GlobalConstants.GameManager.EntityHandler;

            if (this.Prefab is null == false)
            {
                this.Instantiate(true, gameObject, active);
            }
        }

        public void Deserialise()
        {
            this.EntityHandler = GlobalConstants.GameManager?.EntityHandler;
            this.ItemHandler = GlobalConstants.GameManager?.ItemHandler;
        }

        public void Instantiate(bool recursive = true, GameObject gameObject = null, bool active = false)
        {
            if (gameObject is null)
            {
                MonoBehaviourHandler monoBehaviourHandler = GlobalConstants.GameManager.ItemPool.Get()
                    .GetComponent<MonoBehaviourHandler>();
                monoBehaviourHandler.AttachJoyObject(this);
            }
            else
            {
                MonoBehaviourHandler monoBehaviourHandler = gameObject.GetComponent<MonoBehaviourHandler>();
                monoBehaviourHandler.AttachJoyObject(this);
            }
            this.MonoBehaviourHandler.SetSpriteLayer("Objects");
            this.MonoBehaviourHandler.Clear();
            this.MonoBehaviourHandler.AddSpriteState(this.States[this.StateIndex]);
            this.MonoBehaviourHandler.gameObject.SetActive(active);
            
            if (!recursive)
            {
                return;
            }
            
            foreach (IItemInstance item in this.Contents)
            {
                if (item is ItemInstance instance)
                {
                    instance.Instantiate(true, GlobalConstants.GameManager.ItemPool.Get());
                }
            }
        }

        public IItemInstance Copy(IItemInstance copy)
        {
            this.Initialise();

            ItemInstance newItem = new ItemInstance(
                copy.Guid,
                copy.ItemType,
                copy.DerivedValues,
                copy.WorldPosition,
                copy.Identified,
                copy.States,
                copy.Roller,
                copy.UniqueAbilities,
                copy.CachedActions.ToArray());

            this.ItemHandler.AddItem(newItem);
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

        protected void Initialise()
        {
            if (GlobalConstants.GameManager is null)
            {
                return;
            }
            this.Data = new NonUniqueDictionary<object, object>();
            ItemHandler = GlobalConstants.GameManager.ItemHandler;
            EntityHandler = GlobalConstants.GameManager.EntityHandler;
        }
        
        public void SetOwner(Guid newOwner, bool recursive = false)
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
            foreach (IItemInstance item in user.Contents)
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

        public Guid TakeMyItem(int index)
        {
            if(index > 0 && index < this.m_Contents.Count)
            {
                Guid item = this.m_Contents[index];
                this.m_Contents.RemoveAt(index);
                return item;
            }

            throw new InvalidOperationException("No item to take at selected index!");
        }

        public bool Contains(IItemInstance actor)
        {
            if (this.m_Contents.Contains(actor.Guid))
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
            if (actor.Guid == this.Guid 
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
                this.m_Contents.Add(actor.Guid);

                this.CalculateValue();
                this.ConstructDescription();

                actor.InWorld = false;
                
                this.ItemAdded?.Invoke(this, new ItemChangedEventArgs() { Item = actor });
                return true;
            }

            return false;
        }

        public bool AddContents(IEnumerable<IItemInstance> actors)
        {
            IEnumerable<IItemInstance> itemInstances = actors as IItemInstance[] ?? actors.ToArray();
            this.m_Contents.AddRange(itemInstances.Where(actor => 
                    this.m_Contents.Any(item => item == actor.Guid) == false)
                .Select(instance => instance.Guid));

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
            if (!this.m_Contents.Remove(actor.Guid))
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

        public override void Dispose()
        {
            if (this.MonoBehaviourHandler)
            {
                GlobalConstants.GameManager.ItemPool.Retire(this.MonoBehaviourHandler.gameObject);
            }
            base.Dispose();
            foreach (IItemInstance content in this.Contents)
            {
                content.Dispose();
            }
        }

        ~ItemInstance()
        {
            this.Dispose();
        }

        public bool Identified
        {
            get => this.m_Identified;
            protected set => this.m_Identified = value;
        }

        public bool Broken => this.GetValue(DerivedValueName.DURABILITY) <= 0;

        public int Efficiency => (int)(this.m_Type.Material.Bonus * (this.GetValue(DURABILITY) / (float)this.GetMaximum(DURABILITY)));

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

        [OdinSerialize]
        public bool InWorld
        {
            get;
            set;
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
            get => this.ItemHandler.GetItems(this.m_Contents);
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

                IDictionary<string, int> occurrences = new Dictionary<string, int>();
                foreach (IItemInstance item in items)
                {
                    if (occurrences.ContainsKey(item.JoyName))
                    {
                        occurrences[item.JoyName] += 1;
                    }
                    else
                    {
                        occurrences.Add(item.JoyName, 1);
                    }
                }

                IEnumerable<string> itemNames = occurrences.Select(pair => pair.Value > 1 ? pair.Key + "s" : pair.Key);
                contentString += string.Join(", ", itemNames);
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
