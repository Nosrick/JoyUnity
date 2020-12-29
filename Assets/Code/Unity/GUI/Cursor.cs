using UnityEngine;
using UnityEngine.InputSystem;

namespace JoyLib.Code.Unity.GUI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Cursor : GUIData
    {
        protected CanvasGroup CanvasGroup { get; set; }

        public void OnEnable()
        {
            this.CanvasGroup = this.GetComponent<CanvasGroup>();
        }

        public void Update()
        {
            if (this.enabled)
            {
                this.transform.position = Mouse.current.position.ReadValue();
            }
        }

        public override void Close()
        {
            base.Close();
            this.CanvasGroup.alpha = 0.0f;
        }

        public override void Show()
        {
            base.Show();
            this.CanvasGroup.alpha = 1.0f;
        }
    }
}