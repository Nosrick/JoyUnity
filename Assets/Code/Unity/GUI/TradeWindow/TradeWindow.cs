using System.Collections.Generic;
using System.Globalization;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class TradeWindow : GUIData
    {
        public IEntity Left { get; protected set; }
        public IEntity Right { get; protected set; }

        [SerializeField]
        protected TextMeshProUGUI LeftValue;
        [SerializeField]
        protected TextMeshProUGUI RightValue;

        [SerializeField] protected TextMeshProUGUI RightName;

        [SerializeField] protected ItemContainer LeftInventory;
        [SerializeField] protected ItemContainer LeftOffering;

        [SerializeField] protected ItemContainer RightInventory;
        [SerializeField] protected ItemContainer RightOffering;

        public static IEntityRelationshipHandler RelationshipHandler { get; set; }
        
        protected RectTransform RectTransform { get; set; }

        public override void Awake()
        {
            base.Awake();
            this.RectTransform = this.GetComponent<RectTransform>();

            this.LeftOffering.OnAddItem -= Tally;
            this.LeftOffering.OnRemoveItem -= Tally;

            this.RightOffering.OnAddItem -= this.Tally;
            this.RightOffering.OnRemoveItem -= this.Tally;
            
            this.LeftOffering.OnAddItem += Tally;
            this.LeftOffering.OnRemoveItem += Tally;
            this.RightOffering.OnAddItem += Tally;
            this.RightOffering.OnRemoveItem += Tally;
        }

        public void SetActors(IEntity left, IEntity right)
        {
            this.Left = left;
            this.Right = right;

            this.LeftInventory.Owner = this.Left;
            this.LeftOffering.Owner = this.Left;

            this.RightInventory.Owner = this.Right;
            this.RightOffering.Owner = this.Right;

            this.RightName.text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(this.Right.Gender.PersonalSubject) + " " + this.Right.Gender.IsOrAre + " offering";
            LayoutRebuilder.ForceRebuildLayoutImmediate(this.RectTransform);
        }

        public bool Trade()
        {
            int leftValue = 0;
            int rightValue = 0;
            
            foreach (IItemInstance item in LeftOffering.Contents)
            {
                leftValue += item.Value;
            }

            foreach (IItemInstance item in RightOffering.Contents)
            {
                rightValue += item.Value;
            }

            int? relationshipValue = RelationshipHandler?.GetHighestRelationshipValue(Right, Left);

            if (relationshipValue is null)
            {
                return false;
            }

            if (leftValue + relationshipValue >= rightValue)
            {
                int difference = leftValue - rightValue;

                IEnumerable<IRelationship> relationships = RelationshipHandler?.Get(new IJoyObject[] { Left, Right });
                foreach (IRelationship relationship in relationships)
                {
                    relationship.ModifyValueOfParticipant(Left.GUID, Right.GUID, difference);
                }

                foreach(IItemInstance item in this.LeftOffering.Contents)
                {
                    LeftOffering.RemoveItem(item);
                    Left.RemoveContents(item);
                    Right.AddContents(item);
                }

                foreach(IItemInstance item in this.RightOffering.Contents)
                {
                    RightOffering.RemoveItem(item);
                    Right.RemoveContents(item);
                    Left.AddContents(item);
                }
                
                SetActors(Left, Right);

                return true;
            }

            return false;
        }

        protected void Tally(IItemContainer container, ItemChangedEventArgs args)
        {
            int leftValue = 0;
            int rightValue = 0;
            
            foreach (IItemInstance item in this.LeftOffering.Contents)
            {
                leftValue += item.Value;
            }

            foreach (IItemInstance item in this.RightOffering.Contents)
            {
                rightValue += item.Value;
            }

            this.LeftValue.text = "Your value: " + leftValue;
            this.RightValue.text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Right.Gender.Possessive) + " value: " + rightValue;
        }
    }
}