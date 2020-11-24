using System;
using Lean.Gui;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JoyLib.Code.Unity.GUI
{
    public class ActorDrag : LeanDrag
    {
        [SerializeField] protected bool m_ReverseHorizontal;
        [SerializeField] protected bool m_ReverseVertical;
        [SerializeField] protected RectTransform m_DragActor;
        [SerializeField] protected LerpType m_Lerp;
        [SerializeField] protected LeanConstrainAnchoredPosition m_TargetConstraint;
        [SerializeField] protected LeanConstrainAnchoredPosition m_DragActorConstraint;

        public override void OnDrag(PointerEventData eventData)
        {
            RectTransform targetRect = m_TargetConstraint.GetComponent<RectTransform>();
            
            Vector2 targetConstraintMin = new Vector2(
                m_TargetConstraint.Horizontal ? m_TargetConstraint.HorizontalMin : targetRect.anchoredPosition.x, 
                m_TargetConstraint.Vertical ? m_TargetConstraint.VerticalMin : targetRect.anchoredPosition.y);
            
            Vector2 targetConstraintMax = new Vector2(
                m_TargetConstraint.Horizontal ? m_TargetConstraint.HorizontalMax : targetRect.anchoredPosition.x, 
                m_TargetConstraint.Vertical ? m_TargetConstraint.VerticalMax : targetRect.anchoredPosition.y);

            Vector2 dragActorConstraintMin =
                new Vector2(
                    m_DragActorConstraint.Horizontal ? m_DragActorConstraint.HorizontalMin : m_DragActor.anchoredPosition.x, 
                    m_DragActorConstraint.Vertical ? m_DragActorConstraint.VerticalMin : m_DragActor.anchoredPosition.y);
            
            Vector2 dragActorConstraintMax = new Vector2(
                m_DragActorConstraint.Horizontal ? m_DragActorConstraint.HorizontalMax : m_DragActor.anchoredPosition.x,
                m_DragActorConstraint.Vertical ? m_DragActorConstraint.VerticalMax : m_DragActor.anchoredPosition.y);

            // Only drag if OnBeginDrag was successful
            if (dragging == true)
            {
                // Only allow dragging if certain conditions are met
                if (MayDrag(eventData) == true)
                {
                    var oldVector = default(Vector2);
                    var target    = TargetTransform;

                    // Get the previous pointer position relative to this rect transform
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(target, eventData.position - eventData.delta, eventData.pressEventCamera, out oldVector) == true)
                    {
                        var newVector = default(Vector2);

                        // Get the current pointer position relative to this rect transform
                        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(target, eventData.position, eventData.pressEventCamera, out newVector) == true)
                        {
                            var anchoredPosition = target.anchoredPosition;

                            int reverseX = m_ReverseHorizontal ? -1 : 1;
                            int reverseY = m_ReverseVertical ? -1 : 1;

                            Vector2 newDragActorOffset = Vector2.zero;

                            if (m_Lerp != LerpType.NONE)
                            {
                                switch (m_Lerp)
                                {
                                    case LerpType.VERTICAL:
                                        float lerp = (target.localPosition.y - targetConstraintMin.y) /
                                                     (targetConstraintMax.y - targetConstraintMin.y);

                                        lerp = m_ReverseVertical ? 1.0f - lerp : lerp;
                                        
                                        newDragActorOffset = Vector2.Lerp(dragActorConstraintMin, dragActorConstraintMax,
                                            lerp);
                                        break;
                                    
                                    case LerpType.HORIZONTAL:
                                        throw new NotImplementedException("This hasn't been implemented yet!");
                                        //newDragActorOffset = Vector2.Lerp(m_ConstrainMin, m_ConstrainMax, newVector.x / m_ConstrainMax.x);
                                        break;
                                    
                                    case LerpType.BOTH:
                                        throw new NotImplementedException("This hasn't been implemented yet!");
                                        break;
                                    
                                    default:
                                        throw new InvalidOperationException("Invalid LerpType on ConfigurableDrag component");
                                }
                            }
                            else
                            {
                                newDragActorOffset = (newVector - oldVector);
                            }
                            
                            currentPosition += (Vector2)(target.localRotation * (newVector - oldVector));

                            if (horizontal == true)
                            {
                                anchoredPosition.x = currentPosition.x;
                            }

                            if (vertical == true)
                            {
                                anchoredPosition.y = currentPosition.y;
                            }

                            // Offset the anchored position by the difference
                            target.anchoredPosition = anchoredPosition;
                            m_DragActor.anchoredPosition = newDragActorOffset;
                        }
                    }
                }
            }
        }
    }

    public enum LerpType
    {
        HORIZONTAL,
        VERTICAL,
        BOTH,
        NONE
    }
}