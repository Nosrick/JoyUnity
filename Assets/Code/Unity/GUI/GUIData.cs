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
            GUIData[] children = this.gameObject.GetComponentsInChildren<GUIData>(true);
            foreach (GUIData child in children)
            {
                if (child.Equals(this) == false)
                {
                    child.Show();
                }
            }
        }

        public virtual void Close()
        {
            this.gameObject.SetActive(false);
            GUIData[] children = this.gameObject.GetComponentsInChildren<GUIData>(true);
            foreach (GUIData child in children)
            {
                if (child.Equals(this) == false)
                {
                    child.Close();
                }
            }
        }

        public bool m_RemovesControl;

        public bool m_ClosesOthers;

        public bool m_AlwaysOpen;

        public int m_Index;
    }
}