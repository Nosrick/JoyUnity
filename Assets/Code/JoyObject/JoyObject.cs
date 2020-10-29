using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Managers;
using JoyLib.Code.Rollers;
using JoyLib.Code.Scripting;
using System;
using System.Collections.Generic;
using JoyLib.Code.Collections;
using System.Linq;
using JoyLib.Code.World;
using UnityEngine;

namespace JoyLib.Code
{
    [Serializable]
    public class JoyObject : IComparable, IJoyObject
    {
        public BasicValueContainer<IDerivedValue> DerivedValues { get; protected set; }

        public string Tileset { get; protected set; }
        
        public Vector2Int WorldPosition { get; protected set; }

        public bool IsAnimated { get; protected set; }

        public int ChosenIcon { get; protected set; }
        
        public int LastIcon { get; protected set; }

        public int FramesSinceLastChange { get; protected set; }

        public List<string> Tags { get; protected set; }

        public bool IsWall { get; protected set; }

        public bool IsDestructible { get; protected set; }
        
        public WorldInstance MyWorld { get; set; }

        [NonSerialized]
        protected List<IJoyAction> m_CachedActions;

        [NonSerialized]
        protected const int FRAMES_PER_SECOND = 30;

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
            BasicValueContainer<IDerivedValue> derivedValues, 
            Vector2Int position, 
            string tileSet, 
            string[] actions,
            Sprite[] sprites, 
            params string[] tags)
        {
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
            BasicValueContainer<IDerivedValue> derivedValues, 
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

        private void Initialise(
            string name, 
            BasicValueContainer<IDerivedValue> derivedValues, 
            Vector2Int position, 
            string tileSet, 
            IJoyAction[] actions,
            Sprite[] sprites, 
            params string[] tags)
        {
            this.JoyName = name;
            this.GUID = GUIDManager.Instance.AssignGUID();

            this.DerivedValues = derivedValues;

            this.Tileset = tileSet;
            this.Tags = tags.ToList();

            this.WorldPosition = position;
            this.Move(this.WorldPosition);

            this.Icons = sprites;

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
                this.ChosenIcon = RNG.instance.Roll(0, sprites.Length - 1);
            }
            else
            {
                this.ChosenIcon = 0;
            }

            this.LastIcon = 0;
            this.FramesSinceLastChange = 0;

            this.m_CachedActions = new List<IJoyAction>(actions);
        }

        ~JoyObject()
        {
            GUIDManager.Instance.ReleaseGUID(this.GUID);
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
                return DerivedValues[name].ModifyValue(value);
            }

            throw new InvalidOperationException("Derived value of " + name + " not found on JoyObject " + this.ToString());
        }

        //Used for deserialisation
        public void SetIcons(Sprite[] sprites)
        {
            Icons = sprites;
        }

        // Update is called once per frame
        public virtual void Update ()
        {
            FramesSinceLastChange += 1;

            if(IsAnimated == false)
            {
                return;
            }

            if (FramesSinceLastChange != FRAMES_PER_SECOND)
            {
                return;
            }
            
            ChosenIcon += 1;
            ChosenIcon %= Icons.Length;

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
        
        public Sprite Icon => Icons[ChosenIcon];

        public Sprite[] Icons { get; protected set; }

        public long GUID { get; protected set; }

        public string JoyName { get; protected set; }

        public int HitPointsRemaining => GetValue("hitpoints");

        public int HitPoints => GetMaximum("hitpoints");

        public bool Conscious => HitPointsRemaining > 0;

        public bool Alive => HitPointsRemaining > (HitPoints * (-1));
    }    
}
