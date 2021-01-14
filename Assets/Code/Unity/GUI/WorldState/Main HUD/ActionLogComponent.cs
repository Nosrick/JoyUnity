using System.Collections.Generic;
using JoyLib.Code.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    [RequireComponent(typeof(VerticalLayoutGroup))]
    public class ActionLogComponent : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI m_Component;
        
        protected List<TextMeshProUGUI> LogText { get; set; }

        public void Awake()
        {
            this.LogText = new List<TextMeshProUGUI>();
            for (int i = 0; i < ActionLog.LINES_TO_KEEP; i++)
            {
                this.LogText.Add(Instantiate(this.m_Component, this.transform));
                this.LogText[i].gameObject.SetActive(false);
            }
        }

        public void FixedUpdate()
        {
            if (GlobalConstants.ActionLog is null)
            {
                return;
            }

            var log = GlobalConstants.ActionLog.History;
            
            for (int i = 0; i < log.Count; i++)
            {
                this.LogText[i].text = log[i];
                this.LogText[i].gameObject.SetActive(true);
            }
        }
    }
}