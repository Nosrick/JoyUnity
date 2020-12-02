using JoyLib.Code.Unity.GUI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JoyLib.Code.Unity.GUI
{
    public class ChangeValueButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] protected ValueContainer ValueContainer;
        [SerializeField] protected ValueChange ValueChange;
        [SerializeField] protected int Delta = 1;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (ValueChange == ValueChange.DECREASE)
            {
                ValueContainer.DecreaseValue(Delta);
            }
            else if (ValueChange == ValueChange.INCREASE)
            {
                ValueContainer.IncreaseValue(Delta);
            }
        }
    }

    public enum ValueChange
    {
        INCREASE,
        DECREASE
    }
}