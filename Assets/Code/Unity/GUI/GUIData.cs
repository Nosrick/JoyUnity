using UnityEngine;
using UnityEngine.EventSystems;

namespace JoyLib.Code.Unity.GUI
{
    public class GUIData : MonoBehaviour, IPointerDownHandler
    {
        public IGUIManager GUIManager { get; set; }
        
        public void Initialise(
            bool removesControl,
            bool closesOthers)
        {
            this.m_ClosesOthers = closesOthers;
            this.m_RemovesControl = removesControl;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            GUIManager.BringToFront(this.name);
        }

        public bool m_RemovesControl;

        public bool m_ClosesOthers;

        public int m_Index;
    }
}