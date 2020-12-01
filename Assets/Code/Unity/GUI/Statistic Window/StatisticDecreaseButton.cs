using UnityEngine;
using UnityEngine.EventSystems;

namespace JoyLib.Code.Unity.GUI.StatisticWindow
{
    public class StatisticDecreaseButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeReference] public StatisticItem m_StatisticItem;

        [SerializeReference] public StatisticWindow m_StatisticWindow;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (m_StatisticItem.Value > 1)
            {
                m_StatisticItem.Value -= 1;
                m_StatisticWindow.PointsRemaining += 1;
            }
        }
    }
}