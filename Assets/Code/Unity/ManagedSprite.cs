using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using JoyLib.Code.Graphics;
using UnityEngine;

namespace JoyLib.Code.Unity
{
    [RequireComponent(typeof(RectTransform))]
    public class ManagedSprite : MonoBehaviour, IAnimated
    {
        [SerializeField] protected GameObject m_Prefab;
        
        protected string SortingLayer { get; set; }
        
        protected RectTransform MyRect { get; set; }

        public ISpriteState CurrentSpriteState
        {
            get
            {
                if (this.IsDirty)
                {
                    if (this.m_States.ContainsKey(this.ChosenSprite)
                        && this.m_States[this.ChosenSprite].SpriteData.m_State.Equals(this.ChosenState, StringComparison.OrdinalIgnoreCase))
                    {
                        this.CachedState = this.m_States[this.ChosenSprite];
                        this.LastSprite = this.ChosenSprite;
                        this.LastState = this.ChosenState;
                        this.IsDirty = false;
                    }
                }

                return this.CachedState;
            }
        }
        
        protected bool IsDirty { get; set; }
        
        protected ISpriteState CachedState { get; set; } 
        public int FrameIndex { get; protected set; }

        public string ChosenSprite
        {
            get => this.m_ChosenSprite;
            protected set
            {
                this.m_ChosenSprite = value;
                this.IsDirty = true;
            }
        }
        protected string m_ChosenSprite;

        public string ChosenState
        {
            get => this.m_ChosenState;
            protected set
            {
                this.m_ChosenState = value;
                this.IsDirty = true;
            }
        }
        protected string m_ChosenState;
        
        protected string LastState { get; set; }
        protected string LastSprite { get; set; }
        
        public string TileSet { get; protected set; }
        public float TimeSinceLastChange { get; protected set; }
        public bool IsAnimated { get; set; }
        
        public IEnumerable<ISpriteState> States => this.m_States.Values;

        protected IDictionary<string, ISpriteState> m_States;
        
        protected List<SpriteRenderer> SpriteParts { get; set; }

        protected const float TIME_BETWEEN_FRAMES = 1f / GlobalConstants.FRAMES_PER_SECOND;

        public virtual void Awake()
        {
            if (this.m_States is null == false)
            {
                return;
            }
            
            this.SpriteParts = new List<SpriteRenderer>();
            this.m_States = new Dictionary<string, ISpriteState>();
            this.MyRect = this.GetComponent<RectTransform>();
        }

        public virtual void AddSpriteState(ISpriteState state, bool changeToNew = true)
        {
            this.m_States.Add(state.Name, state);
            this.IsDirty = true;
            if (changeToNew)
            {
                this.ChangeState(state);
            }
        }

        public virtual void SetSpriteLayer(string layerName)
        {
            this.SortingLayer = layerName;
            foreach (SpriteRenderer spriteRenderer in this.SpriteParts)
            {
                spriteRenderer.sortingLayerName = layerName;
            }
        }

        public virtual bool RemoveStatesByName(string name)
        {
            return this.m_States.Remove(name);
        }

        public virtual ISpriteState GetState(string name)
        {
            return this.m_States.First(pair => pair.Key.Equals(name, StringComparison.OrdinalIgnoreCase)).Value;
        }

        public virtual IEnumerable<ISpriteState> GetStatesByName(string name)
        {
            return this.m_States.Where(pair => pair.Key.Equals(name, StringComparison.OrdinalIgnoreCase))
                .Select(pair => pair.Value);
        }

        public virtual void ChangeState(string name)
        {
            if (this.m_States.ContainsKey(name))
            {
                this.ChosenSprite = name;
                this.UpdateSprites();
            }
        }

        public virtual void ChangeState(ISpriteState state)
        {
            if (this.m_States.ContainsKey(state.Name))
            {
                this.ChosenSprite = state.Name;
                this.ChosenState = state.SpriteData.m_State;
                this.UpdateSprites();
            }
        }

        public virtual void Clear()
        {
            this.m_States = new Dictionary<string, ISpriteState>();
            foreach (SpriteRenderer part in this.SpriteParts)
            {
                part.gameObject.SetActive(false);
            }
        }

        public virtual void FixedUpdate()
        {
            if (!this.IsAnimated)
            {
                return;
            }
            
            this.TimeSinceLastChange += Time.unscaledDeltaTime;
            if (!(this.TimeSinceLastChange >= TIME_BETWEEN_FRAMES))
            {
                return;
            }

            int lastIndex = this.FrameIndex;
            this.TimeSinceLastChange = 0f;
            this.FrameIndex += 1;
            this.FrameIndex %= this.CurrentSpriteState.SpriteData.m_Parts.Max(part => part.m_Frames);
            if (lastIndex != this.FrameIndex)
            {
                this.UpdateSprites();
            }
        }

        protected virtual void UpdateSprites()
        {
            foreach (SpriteRenderer spritePart in this.SpriteParts)
            {
                spritePart.gameObject.SetActive(false);
            }
            if (this.SpriteParts.Count < this.CurrentSpriteState.SpriteData.m_Parts.Count)
            {
                for (int i = this.SpriteParts.Count; i < this.CurrentSpriteState.SpriteData.m_Parts.Count; i++)
                {
                    this.SpriteParts.Add(GameObject.Instantiate(this.m_Prefab, this.transform).GetComponent<SpriteRenderer>());
                }
            }

            var data = this.CurrentSpriteState.GetSpriteForFrame(this.FrameIndex);
            for (int i = 0; i < data.Count; i++)
            {
                //RectTransform partRect = this.SpriteParts[i].GetComponent<RectTransform>();
                //partRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this.MyRect.sizeDelta.x);
                //partRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this.MyRect.sizeDelta.y);
                if (this.SortingLayer.IsNullOrEmpty() == false)
                {
                    this.SpriteParts[i].sortingLayerName = this.SortingLayer;
                }
                this.SpriteParts[i].name = this.CurrentSpriteState.SpriteData.m_Parts[i].m_Name;
                this.SpriteParts[i].gameObject.SetActive(true);
                this.SpriteParts[i].sprite = data[i].Item2;
                this.SpriteParts[i].color = data[i].Item1;
                this.SpriteParts[i].sortingOrder = this.CurrentSpriteState.SpriteData.m_Parts[i].m_SortingOrder;
                this.SpriteParts[i].sortingLayerName = this.SortingLayer;
            }
        }
    }
}