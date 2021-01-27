﻿using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Collections;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Events;
using JoyLib.Code.Graphics;
using JoyLib.Code.Managers;
using JoyLib.Code.Rollers;
using JoyLib.Code.Scripting;
using JoyLib.Code.Unity;
using JoyLib.Code.World;
using Sirenix.OdinSerializer;
using UnityEngine;

namespace JoyLib.Code
{
    [Serializable]
    public class JoyObject : IComparable, IJoyObject
    {
        public event ValueChangedEventHandler OnDerivedValueChange;
        public event ValueChangedEventHandler OnMaximumChange;
        
        protected List<string> m_Tags;
        
        [OdinSerialize]
        public IDictionary<string, IDerivedValue> DerivedValues { get; protected set; }
        
        public Vector2Int WorldPosition { get; protected set; }

        public IEnumerable<string> Tags
        {
            get => this.m_Tags;
            protected set => this.m_Tags = new List<string>(value);
        }

        [OdinSerialize]
        public bool IsWall { get; protected set; }

        [OdinSerialize]
        public bool IsDestructible { get; protected set; }
        
        public IWorldInstance MyWorld { get; set; }

        [OdinSerialize]
        public long GUID { get; protected set; }

        [OdinSerialize]
        public string JoyName { get; protected set; }

        public virtual int HitPointsRemaining => this.GetValue("hitpoints");

        public virtual int HitPoints => this.GetMaximum("hitpoints");

        public bool Alive => this.HitPointsRemaining > (this.HitPoints * (-1));
        
        [OdinSerialize]
        protected NonUniqueDictionary<object, object> Data { get; set; }

        [OdinSerialize]
        public List<ISpriteState> States { get; protected set; }

        [OdinSerialize]
        public List<IJoyAction> CachedActions { get; protected set; }
        
        public MonoBehaviourHandler MonoBehaviourHandler { get; protected set; }

        public IRollable Roller { get; protected set; }

        public virtual IEnumerable<Tuple<string, string>> Tooltip
        {
            get
            {
                return this.m_Tooltip;
            }
            set
            {
                this.m_Tooltip = value;
            }
        }
        
        [OdinSerialize]
        protected IEnumerable<Tuple<string, string>> m_Tooltip;

        public JoyObject()
        {
            this.GUID = GUIDManager.Instance.AssignGUID();
        }

        /// <summary>
        /// Creation of a JoyObject
        /// </summary>
        /// <param name="name"></param>
        /// <param name="hitPoints"></param>
        /// <param name="position"></param>
        /// <param name="sprites"></param>
        /// <param name="baseType"></param>
        /// <param name="isAnimated"></param>
        /// <param name="isWall"></param>
        public JoyObject(
            string name, 
            IDictionary<string, IDerivedValue> derivedValues, 
            Vector2Int position, 
            IEnumerable<string> actions,
            IEnumerable<ISpriteState> sprites, 
            RNG roller = null,
            params string[] tags)
        {
            this.Roller = roller is null ? new RNG() : roller; 
            List<IJoyAction> tempActions = new List<IJoyAction>(); 
            foreach(string action in actions)
            {
                tempActions.Add(ScriptingEngine.Instance.FetchAction(action));
            }

            this.Initialise(
                name,
                derivedValues,
                position,
                tempActions.ToArray(),
                sprites,
                tags);
        }

        public JoyObject(
            string name, 
            IDictionary<string, IDerivedValue> derivedValues, 
            Vector2Int position,
            IEnumerable<IJoyAction> actions,
            IEnumerable<ISpriteState> sprites,
            IRollable roller = null,
            params string[] tags)
        {
            this.Roller = roller is null ? new RNG() : roller; 
            this.Initialise(
                name,
                derivedValues,
                position,
                actions,
                sprites,
                tags);
        }

        protected void Initialise(
            string name, 
            IDictionary<string, IDerivedValue> derivedValues, 
            Vector2Int position, 
            IEnumerable<IJoyAction> actions,
            IEnumerable<ISpriteState> sprites, 
            params string[] tags)
        {
            this.Data = new NonUniqueDictionary<object, object>();
            
            this.JoyName = name;
            this.GUID = GUIDManager.Instance.AssignGUID();

            this.DerivedValues = derivedValues;

            this.Tags = tags.ToList();

            this.States = sprites.ToList();

            this.WorldPosition = position;
            this.Move(this.WorldPosition);

            if (tags.Any(tag => tag.Equals("invulnerable", StringComparison.OrdinalIgnoreCase)))
            {
                this.IsDestructible = false;
            }

            if (tags.Any(tag => tag.Equals("wall", StringComparison.OrdinalIgnoreCase)))
            {
                this.IsWall = true;
            }

            this.CachedActions = new List<IJoyAction>(actions);

            //this.Tooltip = new List<Tuple<string, string>>();
        }

        public IJoyAction FetchAction(string name)
        {
            return this.CachedActions.First(action => action.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        protected virtual void OnMaximumChanged(object sender, ValueChangedEventArgs args)
        {
            this.OnMaximumChange?.Invoke(sender, args);
        }

        protected virtual void OnDerivedValueChanged(object sender, ValueChangedEventArgs args)
        {
            this.OnDerivedValueChange?.Invoke(sender, args);
        }

        public bool AddTag(string tag)
        {
            if (this.HasTag(tag))
            {
                return false;
            }
            
            this.m_Tags.Add(tag);
            return true;
        }

        public bool RemoveTag(string tag)
        {
            if (!this.HasTag(tag))
            {
                return false;
            }
            
            this.m_Tags.Remove(tag);
            return true;
        }

        public bool HasTag(string tag)
        {
            return this.m_Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase));
        }

        public void Move(Vector2Int newPosition)
        {
            this.WorldPosition = newPosition;
        }

        public int DamageValue(string name, int value)
        {
            return this.ModifyValue(name, -value);
        }

        public int RestoreValue(string name, int value)
        {
            return this.ModifyValue(name, value);
        }

        public virtual int GetValue(string name)
        {
            if (this.DerivedValues.Keys.Any(key => key.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                return this.DerivedValues.First(pair => pair.Key.Equals(name, StringComparison.OrdinalIgnoreCase)).Value.Value;
            }
            
            throw new InvalidOperationException("Derived value of " + name + " not found on JoyObject " + this.ToString());
        }

        public virtual int GetMaximum(string name)
        {
            if (this.DerivedValues.Keys.Any(key => key.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                return this.DerivedValues.First(pair => pair.Key.Equals(name, StringComparison.OrdinalIgnoreCase)).Value.Maximum;
            }
            
            throw new InvalidOperationException("Derived value of " + name + " not found on JoyObject " + this.ToString());
        }

        public int SetBase(string name, int value)
        {
            if (!this.DerivedValues.ContainsKey(name))
            {
                throw new InvalidOperationException("Derived value of " + name + " not found on JoyObject " + this);
            }

            int old = this.DerivedValues[name].Base;
            this.DerivedValues[name].SetBase(value);
            this.OnMaximumChange?.Invoke(this, new ValueChangedEventArgs
            {
                Delta = value - old,
                Name = name,
                NewValue = this.DerivedValues[name].Base
            });
            return this.DerivedValues[name].Base;
        }
        
        public int SetEnhancement(string name, int value)
        {
            if (!this.DerivedValues.ContainsKey(name))
            {
                throw new InvalidOperationException("Derived value of " + name + " not found on JoyObject " + this);
            }

            int old = this.DerivedValues[name].Enhancement;
            this.DerivedValues[name].SetEnhancement(value);
            this.OnMaximumChange?.Invoke(this, new ValueChangedEventArgs
            {
                Delta = value - old,
                Name = name,
                NewValue = this.DerivedValues[name].Enhancement
            });
            return this.DerivedValues[name].Enhancement;
        }

        public virtual int ModifyValue(string name, int value)
        {
            if (!this.DerivedValues.ContainsKey(name))
            {
                throw new InvalidOperationException("Derived value of " + name + " not found on JoyObject " + this);
            }
            this.DerivedValues[name].ModifyValue(value);
            this.OnDerivedValueChange?.Invoke(this, new ValueChangedEventArgs
            {
                Delta = value,
                Name = name,
                NewValue = this.DerivedValues[name].Value
            });
            return this.DerivedValues[name].Value;
        }

        public virtual int SetValue(string name, int value)
        {
            if (!this.DerivedValues.ContainsKey(name))
            {
                throw new InvalidOperationException("Derived value of " + name + " not found on JoyObject " + this);
            }

            int old = this.DerivedValues[name].Value;
            this.DerivedValues[name].SetValue(value);
            this.OnDerivedValueChange?.Invoke(this, new ValueChangedEventArgs
            {
                Delta = value - old,
                Name = name,
                NewValue = this.DerivedValues[name].Value
            });
            return this.DerivedValues[name].Value;
        }

        // Update is called once per frame
        public virtual void Update ()
        {
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
            return "{ " + this.JoyName + " : " + this.GUID + " }";
        }

        public bool AddData(object key, object value)
        {
            this.Data.Add(key, value);
            return true;
        }

        public bool RemoveData(object key)
        {
            return this.Data.RemoveByKey(key) > 0;
        }

        public bool HasDataKey(object search)
        {
            return this.Data.ContainsKey(search);
        }

        public bool HasDataValue(object search)
        {
            return this.Data.ContainsValue(search);
        }

        public object[] GetDataValues(object key)
        {
            return this.Data.Where(tuple => tuple.Item1.Equals(key))
                .Select(tuple => tuple.Item2)
                .ToArray();
        }

        public object[] GetDataKeysForValue(object value)
        {
            return this.Data.Where(tuple => tuple.Item2.Equals(value))
                .Select(tuple => tuple.Item1)
                .ToArray();
        }

        public void AttachMonoBehaviourHandler(MonoBehaviourHandler mbh)
        {
            this.MonoBehaviourHandler = mbh;
        }

        public void Dispose()
        {
            GUIDManager.Instance.ReleaseGUID(this.GUID);
            GC.SuppressFinalize(this);
        }

        ~JoyObject()
        {
            this.Dispose();
        }
    }    
}
