using UnityEngine;
using UnityEngine.EventSystems;

namespace JoyLib.Code.Unity.GUI
{
    public class GUIData : MonoBehaviour, IPointerDownHandler
    {
        public static IGUIManager GUIManager { get; set; }
        
        public void Initialise(
            bool removesControl,
            bool closesOthers)
        {
            this.ClosesOthers = closesOthers;
            this.RemovesControl = removesControl;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            GUIManager.BringToFront(this.name);
        }

        public bool RemovesControl
        {
            get;
            protected set;
        }

        public bool ClosesOthers
        {
            get;
            protected set;
        }

        public int Index { get; set; }
    }
}