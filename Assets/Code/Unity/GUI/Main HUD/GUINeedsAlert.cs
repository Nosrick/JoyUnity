using System.Globalization;
using System.Text;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Needs;
using TMPro;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class GUINeedsAlert : GUIData
    {
        [SerializeField] protected TextMeshProUGUI m_Text;

        protected IEntity Player { get; set; }
        
        protected ILiveEntityHandler EntityHandler { get; set; }

        protected int Counter { get; set; }
        protected const int MAXIMUM_FRAMES = 90;

        public override void Awake()
        {
            base.Awake();
            this.GetBits();
        }

        protected void GetBits()
        {
            if (GlobalConstants.GameManager is null == false)
            {
                this.EntityHandler = this.EntityHandler ?? GlobalConstants.GameManager.EntityHandler;
                this.Player = this.EntityHandler is null == false ? this.EntityHandler.GetPlayer() : this.Player;
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
            this.m_Text.text = "";
            this.GetBits();
            if (this.Player is null)
            {
                return;
            }

            StringBuilder builder = new StringBuilder();
            foreach(INeed need in this.Player.Needs.Values)
            {
                if(!need.ContributingHappiness)
                {
                    if (need.Value < need.HappinessThreshold / 2)
                    {
                        builder.AppendLine("<color=red>" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(need.Name) + "</color>");
                    }
                    else
                    {
                        builder.AppendLine("<color=yellow>" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(need.Name) + "</color>");
                    }
                }
            }

            this.m_Text.text = builder.ToString();
        }
    }
}
