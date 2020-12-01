using UnityEngine;
using UnityEngine.EventSystems;

namespace JoyLib.Code.Unity.GUI
{
    public class StatisticIncreaseButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeReference] public StatisticItem m_StatisticItem;

        [SerializeReference] public StatisticWindow m_StatisticWindow;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (m_StatisticWindow.PointsRemaining > 0 && m_StatisticItem.Value < 10)
            {
                m_StatisticItem.Value += 1;
                m_StatisticWindow.PointsRemaining -= 1;
            }
        }
    }
}