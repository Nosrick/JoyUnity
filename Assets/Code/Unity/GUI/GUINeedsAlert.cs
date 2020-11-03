using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Needs;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class GUINeedsAlert : MonoBehaviour
    {
        protected Text m_Text;

        protected Entity m_Player;

        protected int m_Counter = 0;
        protected const int MAXIMUM_FRAMES = 90;

        public void SetPlayer(Entity player)
        {
            m_Player = player;
        }

        public void Start()
        {
            m_Text = GameObject.Find("NeedsText").GetComponent<Text>();
        }

        public void Update()
        {
            m_Counter += 1;
            m_Counter %= MAXIMUM_FRAMES;

            if(m_Counter == 0)
            {
                DoText();
            }
        }

        protected void DoText()
        {
            m_Text.text = "";
            if(m_Player == null)
            {
                return;
            }

            foreach(INeed need in m_Player.Needs.Collection)
            {
                if(!need.ContributingHappiness)
                {
                    m_Text.text += "<color=yellow>" + need.Name + "</color>\r\n";
                }
            }
        }
    }
}
