using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Collections;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Managers;
using JoyLib.Code.Rollers;
using JoyLib.Code.Scripting;
using JoyLib.Code.Unity;
using JoyLib.Code.World;
using UnityEngine;

namespace JoyLib.Code
{
    [Serializable]
    public class JoyObject : IComparable, IJoyObject
    {
        public IDictionary<string, IDerivedValue<int>> DerivedValues { get; protected set; }

        public string TileSet { get; protected set; }
        
        public Vector2Int WorldPosition { get; protected set; }

        public bool IsAnimated { get; protected set; }

        public int ChosenSprite { get; protected set; }
        
        public int LastIndex { get; protected set; }

        public int FramesSinceLastChange { get; protected set; }

        public List<string> Tags { get; protected set; }

        public bool IsWall { get; protected set; }

        public bool IsDestructible { get; protected set; }
        
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

        public RNG Roller { get; protected set; }

        public JoyObject()
        {}

        /// <summary>
        /// Creation of a JoyObject (MonoBehaviour) using a List of Sprites
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
            IDictionary<string, IDerivedValue<int>> derivedValues, 
            Vector2Int position, 
            string tileSet, 
            string[] actions,
            Sprite[] sprites, 
            RNG roller = null,
            params string[] tags)
        {
            Roller = roller is null ? new RNG() : roller; 
            List<IJoyAction> tempActions = new List<IJoyAction>(); 
            foreach(string action in actions)
            {
                tempActions.Add(ScriptingEngine.instance.FetchAction(action));
            }
            
            Initialise(
                name,
                derivedValues,
                position,
                tileSet,
                tempActions.ToArray(),
                sprites,
                tags);
        }

        public JoyObject(
            string name, 
            IDictionary<string, IDerivedValue<int>> derivedValues, 
            Vector2Int position, 
            string tileSet, 
            IJoyAction[] actions,
            Sprite[] sprites, 
            params string[] tags)
        {
            Initialise(
                name,
                derivedValues,
                position,
                tileSet,
                actions,
                sprites,
                tags);
        }

        protected void Initialise(
            string name, 
            IDictionary<string, IDerivedValue<int>> derivedValues, 
            Vector2Int position, 
            string tileSet, 
            IJoyAction[] actions,
            Sprite[] sprites, 
            params string[] tags)
        {
            this.Data = new NonUniqueDictionary<object, object>();
            
            this.JoyName = name;
            this.GUID = GUIDManager.Instance.AssignGUID();

            this.DerivedValues = derivedValues;

            this.TileSet = tileSet;
            this.Tags = tags.ToList();

            this.WorldPosition = position;
            this.Move(this.WorldPosition);

            this.Sprites = sprites;

            if (tags.Any(tag => tag.Equals("animated", StringComparison.OrdinalIgnoreCase)))
            {
                this.IsAnimated = true;
            }

            if (tags.Any(tag => tag.Equals("invulnerable", StringComparison.OrdinalIgnoreCase)))
            {
                this.IsDestructible = false;
            }

            if (tags.Any(tag => tag.Equals("wall", StringComparison.OrdinalIgnoreCase)))
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

            this.CachedActions = new List<IJoyAction>(actions);
        }

        ~JoyObject()
        {
            GUIDManager.Instance.ReleaseGUID(this.GUID);
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
            if (this.DerivedValues.Keys.Any(key => key.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                return this.DerivedValues.First(pair => pair.Key.Equals(name, StringComparison.OrdinalIgnoreCase)).Value.Value;
            }
            
            throw new InvalidOperationException("Derived value of " + name + " not found on JoyObject " + this.ToString());
        }

        public int GetMaximum(string name)
        {
            if (this.DerivedValues.Keys.Any(key => key.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                return this.DerivedValues.First(pair => pair.Key.Equals(name, StringComparison.OrdinalIgnoreCase)).Value.Maximum;
            }
            
            throw new InvalidOperationException("Derived value of " + name + " not found on JoyObject " + this.ToString());
        }

        public virtual int ModifyMaximum(string name, int value)
        {
            if (this.DerivedValues.ContainsKey(name))
            {
                this.DerivedValues[name].Maximum += value;
                return this.DerivedValues[name].Maximum;
            }
            
            throw new InvalidOperationException("Derived value of " + name + " not found on JoyObject " + this.ToString());
        }

        public virtual int ModifyValue(string name, int value)
        {
            if (this.DerivedValues.ContainsKey(name))
            {
                this.DerivedValues[name].Value += value;
                return this.DerivedValues[name].Value;
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
    }    
}
