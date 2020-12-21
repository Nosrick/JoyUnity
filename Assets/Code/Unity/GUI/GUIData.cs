using UnityEngine;
using UnityEngine.EventSystems;

namespace JoyLib.Code.Unity.GUI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class GUIData : MonoBehaviour, IPointerDownHandler
    {
        public IGUIManager GUIManager { get; set; }

        public void Awake()
        {
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            GUIManager.BringToFront(this.name);
        }

        public void Show()
        {
            this.gameObject.SetActive(true);
        }

        public void Close()
        {
            this.gameObject.SetActive(false);
        }

        public bool m_RemovesControl;

        public bool m_ClosesOthers;

        public int m_Index;
    }
}