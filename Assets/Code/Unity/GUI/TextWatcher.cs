using TMPro;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public delegate void TextChanged(object sender, TextChangedEventArgs args);
    
    [RequireComponent(typeof(TMP_InputField))]
    public class TextWatcher : MonoBehaviour
    {
        public event TextChanged OnTextChange;
        
        protected TMP_InputField Text { get; set; }
        protected string LastText { get; set; }

        public void Awake()
        {
            this.Text = this.GetComponent<TMP_InputField>();
            this.LastText = this.Text.text;
        }

        public void FixedUpdate()
        {
            if (this.Text.text == this.LastText)
            {
                return;
            }
            this.OnTextChange?.Invoke(this, new TextChangedEventArgs
            {
                LastValue = this.LastText,
                NewValue = this.Text.text
            });
            this.LastText = this.Text.text;
        }
    }

    public class TextChangedEventArgs
    {
        public string LastValue { get; set; }
        public string NewValue { get; set; }
    }
}