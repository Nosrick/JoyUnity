﻿using UnityEngine;
using UnityEngine.EventSystems;

namespace JoyLib.Code.Unity.GUI
{
    [DisallowMultipleComponent]
    public class GUIData : MonoBehaviour, IPointerDownHandler
    {
        protected IGUIManager m_GUIManager;

        public IGUIManager GUIManager
        {
            get => this.m_GUIManager;
            set
            {
                this.m_GUIManager = value;
                foreach (GUIData data in this.GetComponentsInChildren<GUIData>())
                {
                    data.m_GUIManager = value;
                }
            }
        }

        public virtual void Awake()
        {
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            this.GUIManager.BringToFront(this.name);
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

        public virtual void ButtonClose()
        {
            this.GUIManager.CloseGUI(this.name);
        }

        public bool m_RemovesControl;

        public bool m_ClosesOthers;

        public bool m_AlwaysOpen;

        public int m_Index;
    }
}