using UnityEngine;
using UnityEngine.EventSystems;

namespace JoyLib.Code.Unity.GUI
{
    public class ChangeValueButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] protected ValueContainer ValueContainer;
        [SerializeField] protected ValueChange ValueChange;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (ValueChange == ValueChange.DECREASE && ValueContainer.AllowDecrease)
            {
                ValueContainer.DecreaseValue(ValueContainer.DecreaseDelta);
            }
            else if (ValueChange == ValueChange.INCREASE && ValueContainer.AllowIncrease)
            {
                ValueContainer.IncreaseValue(ValueContainer.IncreaseDelta);
            }
        }
    }

    public enum ValueChange
    {
        INCREASE,
        DECREASE
    }
}