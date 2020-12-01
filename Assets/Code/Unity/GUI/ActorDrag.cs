using System;
using Lean.Gui;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class ActorDrag : LeanDrag
    {
        [SerializeField] protected bool m_DynamicReconstrain;
        [SerializeField] protected bool m_ReverseHorizontal;
        [SerializeField] protected bool m_ReverseVertical;
        [SerializeField] protected LayoutGroup m_Container;
        protected RectTransform m_ContainerRect;
        [SerializeField] protected RectTransform m_ContainerChild;
        [SerializeField] protected LerpType m_Lerp;
        [SerializeField] protected VerticalConstraint m_VerticalConstraint;
        [SerializeField] protected HorizontalConstraint m_HorizontalConstraint;

        protected RectTransform m_TargetParent;

        [SerializeField] protected Vector2 TargetConstraintMin;
        [SerializeField] protected Vector2 TargetConstraintMax;

        protected Vector3 m_ContainerOriginalPosition;
        protected Vector3 m_TargetOriginalPosition;

        public void Awake()
        {
            m_ContainerRect = m_Container.GetComponent<RectTransform>();
            m_TargetParent = Target.transform.parent.GetComponent<RectTransform>();
            m_ContainerOriginalPosition = m_Container.transform.position;
            m_TargetOriginalPosition = Target.position;
        }

        protected void CalculateConstraints()
        {
            if (m_DynamicReconstrain)
            {
                float verticalConstraint = 0;
                float horizontalConstraint = 0;
                if (m_Container is VerticalLayoutGroup vertical)
                {
                    verticalConstraint = vertical.transform.childCount * 
                                         (m_ContainerChild.rect.height + vertical.spacing + vertical.padding.top + vertical.padding.bottom);
                }
                else if (m_Container is HorizontalLayoutGroup horizontal)
                {
                    horizontalConstraint = m_Container.transform.childCount *
                                           (m_ContainerChild.rect.width + horizontal.spacing + horizontal.padding.left + horizontal.padding.right);
                }

                float xPositionMin, xPositionMax, yPositionMin, yPositionMax;
                switch (m_HorizontalConstraint)
                {
                    case HorizontalConstraint.LEFT:
                        xPositionMin = horizontalConstraint > m_ContainerRect.rect.width 
                            ? m_ContainerOriginalPosition.x - horizontalConstraint 
                            : m_ContainerOriginalPosition.x;
                        xPositionMax = m_ContainerOriginalPosition.x;
                        break;
                    
                    case HorizontalConstraint.RIGHT:
                        xPositionMin = m_ContainerOriginalPosition.x;
                        xPositionMax = horizontalConstraint > m_ContainerRect.rect.width 
                            ? m_ContainerOriginalPosition.x + horizontalConstraint
                            : m_ContainerOriginalPosition.x;
                        break;
                    
                    default:
                        xPositionMin = xPositionMax = m_ContainerOriginalPosition.x;
                        break;
                }

                switch (m_VerticalConstraint)
                {
                    case VerticalConstraint.UP:
                        yPositionMin = verticalConstraint > m_ContainerRect.rect.height 
                            ? m_ContainerOriginalPosition.y - verticalConstraint
                            : m_ContainerOriginalPosition.y;
                        yPositionMax = m_ContainerOriginalPosition.y;
                        break;
                    
                    case VerticalConstraint.DOWN:
                        yPositionMin = m_ContainerOriginalPosition.y;
                        yPositionMax = verticalConstraint > m_ContainerRect.rect.height 
                            ? m_ContainerOriginalPosition.y + verticalConstraint
                            : m_ContainerOriginalPosition.y;
                        break;
                    
                    default:
                        yPositionMin = yPositionMax = m_ContainerOriginalPosition.y;
                        break;
                }
                
                TargetConstraintMin = new Vector2(xPositionMin, yPositionMin);
                TargetConstraintMax = new Vector2(xPositionMax, yPositionMax);
            }
            else
            {
                 throw new NotImplementedException("Haven't done static constraints yet!");
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            CalculateConstraints();

            if (TargetConstraintMin == TargetConstraintMax)
            {
                return;
            }
            
            // Only drag if OnBeginDrag was successful
            if (dragging == true)
            {
                // Only allow dragging if certain conditions are met
                if (MayDrag(eventData) == true)
                {
                    Vector2 oldVector = default(Vector2);
                    RectTransform target = TargetTransform;

                    // Get the previous pointer position relative to this rect transform
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(target, eventData.position - eventData.delta, eventData.pressEventCamera, out oldVector) == true)
                    {
                        Vector2 newVector = default(Vector2);

                        // Get the current pointer position relative to this rect transform
                        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(target, eventData.position, eventData.pressEventCamera, out newVector) == true)
                        {
                            Vector2 anchoredPosition = target.anchoredPosition;

                            int reverseX = m_ReverseHorizontal ? -1 : 1;
                            int reverseY = m_ReverseVertical ? -1 : 1;

                            Vector2 newDragActorOffset = Vector2.zero;

                            if (m_Lerp != LerpType.NONE)
                            {
                                switch (m_Lerp)
                                {
                                    case LerpType.VERTICAL:
                                        float lerp = (target.position.y - m_TargetOriginalPosition.y) /
                                                     (TargetConstraintMax.y - TargetConstraintMin.y);

                                        lerp = Math.Abs(lerp);
                                        Debug.Log("LERP BEFORE NORMALISATION: " + lerp);
                                        
                                        lerp = m_ReverseVertical ? 1.0f - lerp : lerp;
                                        
                                        Debug.Log("LERP: " + lerp);
                                        
                                        newDragActorOffset = m_ReverseHorizontal 
                                            ? Vector2.Lerp(TargetConstraintMin, TargetConstraintMax, lerp)
                                            : Vector2.Lerp(TargetConstraintMax, TargetConstraintMin, lerp);
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
                            Debug.Log("OLD POSITION: " + m_ContainerRect.position);
                            Debug.Log("NEW POSITION: " + newDragActorOffset);
                            m_ContainerRect.position = newDragActorOffset;
                        }
                    }
                }
            }
        }
    }

    public enum LerpType
    {
        NONE,
        HORIZONTAL,
        VERTICAL,
        BOTH
    }

    public enum VerticalConstraint
    {
        NONE,
        UP,
        DOWN
    }

    public enum HorizontalConstraint
    {
        NONE,
        LEFT,
        RIGHT
    }
}