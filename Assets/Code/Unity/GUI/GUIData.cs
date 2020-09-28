using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class GUIData : MonoBehaviour
    {
        public void Initialise(
            bool removesControl,
            bool closesOthers)
        {
            this.ClosesOthers = closesOthers;
            this.RemovesControl = removesControl;
        }
        
        public bool RemovesControl
        {
            get;
            protected set;
        }

        public bool ClosesOthers
        {
            get;
            protected set;
        }
    }
}