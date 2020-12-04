using DevionGames;
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
            Widget.RegisterListener("OnClose", RemoveActiveGUI);
            Widget.RegisterListener("OnOpen", ShowThis);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            GUIManager.BringToFront(this.name);
        }

        public void Show()
        {
            GUIManager.OpenGUI(this.name);
            Widget.Show();
        }

        public void ShowThis(CallbackEventData data)
        {
            GUIManager.OpenGUI(this.name);
        }

        public void RemoveActiveGUI(CallbackEventData data)
        {
            GUIManager.RemoveActiveGUI(this.name);
        }

        public void Close()
        {
            GUIManager.RemoveActiveGUI(this.name);
            Widget.Close();
        }

        public bool m_RemovesControl;

        public bool m_ClosesOthers;

        public int m_Index;
    }
}