using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Collections;
using JoyLib.Code.Graphics;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity
{
    [RequireComponent(typeof(RectTransform))]
    public class ManagedSprite : MonoBehaviour, IAnimated
    {
        [SerializeField] protected Image m_SpritePrefab;
        
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
        
        protected List<Image> SpriteParts { get; set; }

        protected static float TimeBetweenFrames = 1f / GlobalConstants.FRAMES_PER_SECOND;

        public virtual void Awake()
        {
            this.SpriteParts = new List<Image>();
            this.m_States = new NonUniqueDictionary<string, ISpriteState>();
            this.MyRect = this.GetComponent<RectTransform>();
        }

        public virtual void AddSpriteState(ISpriteState state)
        {
            this.m_States.Add(state.Name, state);
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

        public void Clear()
        {
            this.m_States = new NonUniqueDictionary<string, ISpriteState>();
            foreach (Image part in this.SpriteParts)
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
            if (!(this.TimeSinceLastChange >= TimeBetweenFrames))
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
            foreach (Image spritePart in this.SpriteParts)
            {
                spritePart.gameObject.SetActive(false);
            }
            if (this.SpriteParts.Count < this.CurrentSpriteState.SpriteParts.Count)
            {
                for (int i = this.SpriteParts.Count; i < this.CurrentSpriteState.SpriteParts.Count; i++)
                {
                    this.SpriteParts.Add(Instantiate(this.m_SpritePrefab, this.transform));
                }
            }

            List<Sprite> sprites = this.CurrentSpriteState.SpriteParts.ToList();
            List<Color> spriteColours = this.CurrentSpriteState.SpriteColours.ToList();
            for (int i = 0; i < sprites.Count; i++)
            {
                RectTransform partRect = this.SpriteParts[i].GetComponent<RectTransform>();
                partRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this.MyRect.rect.width);
                partRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this.MyRect.rect.height);
                this.SpriteParts[i].gameObject.SetActive(true);
                this.SpriteParts[i].sprite = sprites[i];
                this.SpriteParts[i].color = spriteColours[i];
            }
        }
    }
}