using System.Threading;
using TMPro;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class FontSizeManager : MonoBehaviour
    {
        protected TextMeshProUGUI[] TextsToManage { get; set; }
        
        public void OnEnable()
        {
            Thread.Sleep(500);
            
            if (this.TextsToManage is null)
            {
                this.ResizeFonts();
            }
        }

        public void ResizeFonts()
        {
            this.TextsToManage = this.GetComponentsInChildren<TextMeshProUGUI>();

            float minSize = float.MaxValue;
            foreach (TextMeshProUGUI text in this.TextsToManage)
            {
                //text.enableAutoSizing = false;
                if (text.fontSize < minSize)
                {
                    minSize = text.fontSize;
                }
            }

            foreach (TextMeshProUGUI text in this.TextsToManage)
            {
                text.fontSize = minSize;
                text.enableAutoSizing = false;
            }
        }
    }
}