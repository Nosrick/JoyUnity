using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Collections;
using JoyLib.Code.Graphics;
using UnityEngine;

namespace JoyLib.Code.Unity
{
    [RequireComponent(typeof(RectTransform))]
    public class ManagedSprite : MonoBehaviour, IAnimated
    {
        [SerializeField] protected GameObject m_Prefab;
        
        protected RectTransform MyRect { get; set; }

        public ISpriteState CurrentSpriteState
        {
            get
            {
                if (this.m_States.ContainsKey(this.ChosenSpriteState))
                {
                    if (this.m_States[this.ChosenSpriteState].Count > this.FrameIndex)
                    {
                        return this.m_States[this.ChosenSpriteState][this.FrameIndex];
                    }
                }

                return null;
            }
        }
        public int FrameIndex { get; protected set; }
        public string ChosenSpriteState { get; protected set; }
        public string TileSet { get; protected set; }
        public float TimeSinceLastChange { get; protected set; }
        public bool IsAnimated { get; set; }
        
        public List<ISpriteState> States => this.m_States.Values;

        protected NonUniqueDictionary<string, ISpriteState> m_States;
        
        protected List<SpriteRenderer> SpriteParts { get; set; }

        protected const float TIME_BETWEEN_FRAMES = 1f / GlobalConstants.FRAMES_PER_SECOND;

        public virtual void Awake()
        {
            this.SpriteParts = new List<SpriteRenderer>();
            this.m_States = new NonUniqueDictionary<string, ISpriteState>();
            this.MyRect = this.GetComponent<RectTransform>();
        }

        public virtual void AddSpriteState(ISpriteState state, bool changeToNew = false)
        {
            this.m_States.Add(state.Name, state);
            if (changeToNew)
            {
                this.ChangeState(state);
            }
        }

        public virtual bool RemoveStatesByName(string name)
        {
            return this.m_States.RemoveByKey(name) > 0;
        }

        public virtual ISpriteState GetState(string name)
        {
            return this.m_States.First(pair => pair.Item1.Equals(name, StringComparison.OrdinalIgnoreCase)).Item2;
        }

        public virtual IEnumerable<ISpriteState> GetStatesByName(string name)
        {
            return this.m_States.Where(pair => pair.Item1.Equals(name, StringComparison.OrdinalIgnoreCase))
                .Select(pair => pair.Item2);
        }

        public virtual void ChangeState(string name)
        {
            if (this.m_States.ContainsKey(name))
            {
                this.ChosenSpriteState = name;
                this.UpdateSprites();
            }
        }

        public virtual void ChangeState(ISpriteState state)
        {
            if (this.m_States.ContainsKey(state.Name))
            {
                this.ChosenSpriteState = state.Name;
                this.UpdateSprites();
            }
        }

        public virtual void Clear()
        {
            this.m_States = new NonUniqueDictionary<string, ISpriteState>();
            foreach (SpriteRenderer part in this.SpriteParts)
            {
                part.gameObject.SetActive(false);
            }
        }

        public virtual void Update()
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
            this.FrameIndex %= this.m_States[this.ChosenSpriteState].Count;
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
                this.SpriteParts[i].name = this.CurrentSpriteState.SpriteData.m_Parts[i].m_Name;
                this.SpriteParts[i].gameObject.SetActive(true);
                this.SpriteParts[i].sprite = data[i].Item2;
                this.SpriteParts[i].color = data[i].Item1;
            }
        }
    }
}