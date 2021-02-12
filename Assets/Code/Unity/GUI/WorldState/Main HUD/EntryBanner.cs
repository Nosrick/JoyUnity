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
            GlobalConstants.ActionLog.AddText("FadeIn");
            this.StartCoroutine(this.FadeIn());
            this.MyWindow.On = true;
            GlobalConstants.ActionLog.AddText("FadeOut");
            this.StartCoroutine(this.FadeOut());
        }

        protected IEnumerator FadeIn()
        {
            while (this.MyCanvasGroup.alpha < 0.95f)
            {
                GlobalConstants.ActionLog.AddText("Alpha: " + this.MyCanvasGroup.alpha);
                yield return new WaitForSeconds(0.1f);
            }

            yield return null;
        }

        protected IEnumerator FadeOut()
        {
            while (this.MyCanvasGroup.alpha < 0.95f)
            {
                GlobalConstants.ActionLog.AddText("Alpha: " + this.MyCanvasGroup.alpha);
                yield return new WaitForSeconds(0.1f);
            }

            if (this.MyWindow.On)
            {
                yield return new WaitForSeconds(2.0f);
            }
            
            GlobalConstants.ActionLog.AddText("Closing Entry Banner");
            this.MyWindow.On = false;
            
            while (this.MyCanvasGroup.alpha > 0)
            {
                GlobalConstants.ActionLog.AddText("Alpha: " + this.MyCanvasGroup.alpha);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}