using System.Collections.Generic;
using JoyLib.Code.Collections;
using JoyLib.Code.Graphics;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity
{
    public class ManagedUISprite : ManagedSprite
    {
        protected List<Image> ImageParts { get; set; }

        public override void Awake()
        {
            base.Awake();
            this.ImageParts = new List<Image>();
        }

        public override void Clear()
        {
            this.m_States = new NonUniqueDictionary<string, ISpriteState>();
            foreach (Image part in this.ImageParts)
            {
                part.gameObject.SetActive(false);
            }
        }
        
        protected override void UpdateSprites()
        {
            foreach (Image spritePart in this.ImageParts)
            {
                spritePart.gameObject.SetActive(false);
            }
            if (this.ImageParts.Count < this.CurrentSpriteState.SpriteData.m_Parts.Count)
            {
                for (int i = this.ImageParts.Count; i < this.CurrentSpriteState.SpriteData.m_Parts.Count; i++)
                {
                    this.ImageParts.Add(GameObject.Instantiate(this.m_Prefab, this.transform).GetComponent<Image>());
                }
            }

            var data = this.CurrentSpriteState.GetSpriteForFrame(this.FrameIndex);
            for (int i = 0; i < data.Count; i++)
            {
                this.ImageParts[i].name = this.CurrentSpriteState.SpriteData.m_Parts[i].m_Name;
                this.ImageParts[i].GetComponent<Canvas>().sortingOrder =
                    this.CurrentSpriteState.SpriteData.m_Parts[i].m_SortingOrder;
                this.ImageParts[i].gameObject.SetActive(true);
                this.ImageParts[i].sprite = data[i].Item2;
                this.ImageParts[i].color = data[i].Item1;
            }
        }
    }
}