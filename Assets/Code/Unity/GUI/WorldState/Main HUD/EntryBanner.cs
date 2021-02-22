using System.Collections;
using Lean.Gui;
using TMPro;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    [RequireComponent(typeof(LeanWindow))]
    [RequireComponent(typeof(CanvasGroup))]
    public class EntryBanner : MonoBehaviour
    {
        protected LeanWindow MyWindow { get; set; }
        protected CanvasGroup MyCanvasGroup { get; set; }

        [SerializeField] protected TextMeshProUGUI m_AreaName;

        public void Awake()
        {
            this.MyWindow = this.GetComponent<LeanWindow>();
            this.MyCanvasGroup = this.GetComponent<CanvasGroup>();
        }

        public void Activate(string areaName)
        {
            this.m_AreaName.text = areaName;
            this.MyWindow.On = true;
            this.StartCoroutine(this.FadeOut());
        }

        protected IEnumerator FadeOut()
        {
            if (this.MyWindow.On)
            {
                yield return new WaitForSeconds(2.0f);
            }
            
            this.MyWindow.On = false;

            yield return new WaitForSeconds(1.0f);
            GlobalConstants.GameManager.GUIManager.CloseGUI(this.name);
            
            yield return null;
        }
    }
}