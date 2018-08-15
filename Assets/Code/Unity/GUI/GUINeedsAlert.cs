using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Needs;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class GUINeedsAlert : MonoBehaviour
    {
        private Text m_Text;

        private Entity m_Player;

        private int m_Counter = 0;
        private const int MAXIMUM_FRAMES = 90;

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

        private void DoText()
        {
            m_Text.text = "";
            if(m_Player == null)
            {
                return;
            }

            foreach(EntityNeed need in m_Player.Needs.Values)
            {
                if(!need.contributingHappiness)
                {
                    m_Text.text += "<color=yellow>" + need.name + "</color>\r\n";
                }
            }
        }
    }
}
