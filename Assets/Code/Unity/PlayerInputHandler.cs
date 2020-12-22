using UnityEngine;
using UnityEngine.InputSystem;

namespace JoyLib.Code.Unity
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public InputAction.CallbackContext LastValue { get; protected set; } 
        
        public void OnMove(InputAction.CallbackContext value)
        {
            this.LastValue = value;
        }

        public void OnLeftClick(InputAction.CallbackContext value)
        {
            this.LastValue = value;
        }

        public void OnRightClick(InputAction.CallbackContext value)
        {
            this.LastValue = value;
        }
    }
}