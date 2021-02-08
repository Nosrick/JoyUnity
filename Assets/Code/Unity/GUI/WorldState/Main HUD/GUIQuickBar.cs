using JoyLib.Code.Entities;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class GUIQuickBar : MonoBehaviour
    {
        [SerializeField] protected DerivedValuesInformation m_DerivedValuesContainer;
        
        protected IEntity Player { get; set; }
        
        protected IGUIManager GUIManager { get; set; }
        
        protected bool Initialised { get; set; }

        public void Update()
        {
            this.GetBits();
        }

        protected void GetBits()
        {
            if (this.Initialised && this.m_DerivedValuesContainer.Initialised)
            {
                return;
            }
            
            if (GlobalConstants.GameManager is null)
            {
                return;
            }

            if (this.m_DerivedValuesContainer.Initialised == false)
            {
                return;
            }

            this.Player = GlobalConstants.GameManager.Player;
            this.GUIManager = GlobalConstants.GameManager.GUIManager;

            RectTransform myRect = this.GetComponent<RectTransform>();
            Vector2 anchorDif = myRect.anchorMax - myRect.anchorMin;

            myRect.anchorMin = new Vector2(myRect.anchorMin.x, this.m_DerivedValuesContainer.RectTransform.anchorMax.y);
            myRect.anchorMax = new Vector2(myRect.anchorMax.x , myRect.anchorMin.y + anchorDif.y);
            //myRect.anchoredPosition = Vector2.zero;
            
            this.Initialised = true;
        }
        
        public void OpenInventory()
        {
            this.GUIManager.ToggleGUI(GUINames.INVENTORY);
        }

        public void OpenEquipment()
        {
            this.GUIManager.ToggleGUI(GUINames.EQUIPMENT);
        }

        public void OpenQuestJournal()
        {
            this.GUIManager.ToggleGUI(GUINames.QUEST_JOURNAL);
        }

        public void OpenCharacterSheet()
        {
            this.GUIManager.ToggleGUI(GUINames.CHARACTER_SHEET);
        }

        public void OpenJobManagement()
        {
            this.GUIManager.ToggleGUI(GUINames.JOB_MANAGEMENT);
        }
    }
}