using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Needs;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class GUINeedsAlert : MonoBehaviour
    {
        [SerializeField] protected GameObject m_Container;
        [SerializeField] protected StringPairContainer m_PairPrefab;

        protected IEntity Player { get; set; }
        
        protected ILiveEntityHandler EntityHandler { get; set; }
        
        protected List<StringPairContainer> ChildList { get; set; }
        
        protected List<INeed> PlayerNeeds { get; set; }
        
        protected RectTransform ContainerRect { get; set; }

        protected int Counter { get; set; }
        protected const int MAXIMUM_FRAMES = 90;

        public void Awake()
        {
            if (this.ChildList is null)
            {
                this.ChildList = new List<StringPairContainer>();
            }
            this.GetBits();
        }

        protected void GetBits()
        {
            if (GlobalConstants.GameManager is null || !(this.Player is null))
            {
                return;
            }

            this.ContainerRect = this.m_Container.GetComponent<RectTransform>();
            this.EntityHandler = this.EntityHandler ?? GlobalConstants.GameManager.EntityHandler;
            this.Player = this.EntityHandler is null == false ? this.EntityHandler.GetPlayer() : this.Player;
            if (this.Player is null)
            {
                return;
            }
                
            this.PlayerNeeds = this.Player.Needs.Values.ToList();
            this.ChildList = new List<StringPairContainer>();
            foreach (StringPairContainer child in this.PlayerNeeds.Select(need => Instantiate(this.m_PairPrefab, this.m_Container.transform)))
            {
                child.gameObject.SetActive(false);
                this.ChildList.Add(child);
            }
        }

        protected void Update()
        {
            this.Counter += 1;
            this.Counter %= MAXIMUM_FRAMES;

            if(this.Counter == 0)
            {
                this.DoText();
            }
        }

        protected void DoText()
        {
            this.GetBits();
            if (this.Player is null)
            {
                return;
            }

            bool empty = true;
            for(int i = 0; i < this.PlayerNeeds.Count; i++)
            {
                INeed need = this.PlayerNeeds[i];
                if (need.ContributingHappiness == false)
                {
                    this.ChildList[i].gameObject.SetActive(true);
                    if (need.Value < need.HappinessThreshold / 2)
                    {
                        this.ChildList[i].Target = new Tuple<string, string>("",
                            "<color=red>" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(need.Name) + "</color>");
                    }
                    else
                    {
                        this.ChildList[i].Target = new Tuple<string, string>("",
                            "<color=yellow>" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(need.Name) + "</color>");
                    }

                    empty = false;
                }
                else
                {
                    this.ChildList[i].gameObject.SetActive(false);
                }
            }

            if (empty)
            {
                this.ChildList[0].gameObject.SetActive(true);
                this.ChildList[0].Target =
                    new Tuple<string, string>("", "<color=green>I feel fulfilled!</color>");
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(this.ContainerRect);
        }
    }
}
