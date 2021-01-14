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
            if (this.ValueChange == ValueChange.DECREASE && this.ValueContainer.AllowDecrease)
            {
                this.ValueContainer.DecreaseValue(this.ValueContainer.DecreaseDelta);
            }
            else if (this.ValueChange == ValueChange.INCREASE && this.ValueContainer.AllowIncrease)
            {
                this.ValueContainer.IncreaseValue(this.ValueContainer.IncreaseDelta);
            }
        }
    }

    public enum ValueChange
    {
        INCREASE,
        DECREASE
    }
}