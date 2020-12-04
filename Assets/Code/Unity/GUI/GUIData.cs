using System;
using DevionGames.UIWidgets;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JoyLib.Code.Unity.GUI
{
    [RequireComponent(typeof(UIWidget))]
    public class GUIData : MonoBehaviour
    {
        public IGUIManager GUIManager { get; set; }
        
        protected UIWidget Widget { get; set; }

        public void Awake()
        {
            Widget = this.GetComponent<UIWidget>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            GUIManager.BringToFront(this.name);
        }

        public void Show()
        {
            Widget.Show();
        }

        public void Close()
        {
            Widget.Close();
        }

        public bool m_RemovesControl;

        public bool m_ClosesOthers;

        public int m_Index;
    }
}