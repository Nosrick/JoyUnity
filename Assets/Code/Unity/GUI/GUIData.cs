using UnityEngine;
using UnityEngine.EventSystems;

namespace JoyLib.Code.Unity.GUI
{
    [DisallowMultipleComponent]
    public class GUIData : MonoBehaviour, IPointerDownHandler
    {
        public IGUIManager GUIManager { get; set; }

        public virtual void Awake()
        {
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            GUIManager.BringToFront(this.name);
        }

        public virtual void Show()
        {
            this.gameObject.SetActive(true);
        }

        public virtual void Close()
        {
            this.gameObject.SetActive(false);
        }

        public bool m_RemovesControl;

        public bool m_ClosesOthers;

        public bool m_AlwaysOpen;

        public int m_Index;
    }
}