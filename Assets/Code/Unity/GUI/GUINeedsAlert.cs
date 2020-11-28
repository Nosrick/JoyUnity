using System;
using System.Globalization;
using System.Text;
using DevionGames;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Needs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class GUINeedsAlert : MonoBehaviour
    {
        protected TextMeshProUGUI Text { get; set; }

        protected Entity Player { get; set; }
        
        protected ILiveEntityHandler EntityHandler { get; set; }

        protected int Counter { get; set; }
        protected const int MAXIMUM_FRAMES = 90;

        public void Awake()
        {
            if (EntityHandler is null)
            {
                EntityHandler = GlobalConstants.GameManager.EntityHandler;
                Text = this.gameObject.FindChild("NeedsText", true).GetComponent<TextMeshProUGUI>();
            }
        }

        public void SetPlayer(Entity player)
        {
            Player = player;
        }

        public void Update()
        {
            Counter += 1;
            Counter %= MAXIMUM_FRAMES;

            if(Counter == 0)
            {
                DoText();
            }
        }

        protected void DoText()
        {
            Text.text = "";
            if(Player is null)
            {
                Player = EntityHandler.GetPlayer();

                if (Player is null)
                {
                    return;
                }
            }

            StringBuilder builder = new StringBuilder();
            foreach(INeed need in Player.Needs.Collection)
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

            Text.text = builder.ToString();
        }
    }
}
