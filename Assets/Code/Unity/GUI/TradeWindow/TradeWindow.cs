using System.Collections.Generic;
using System.Globalization;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Events;
using JoyLib.Code.Scripting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class TradeWindow : MonoBehaviour
    {
        public IEntity Left { get; protected set; }
        public IEntity Right { get; protected set; }

        [SerializeField] protected TextMeshProUGUI LeftValue;
        [SerializeField] protected TextMeshProUGUI RightValue;

        [SerializeField] protected TextMeshProUGUI RightName;

        [SerializeField] protected ItemContainer LeftInventory;
        [SerializeField] protected ItemContainer LeftOffering;

        [SerializeField] protected ItemContainer RightInventory;
        [SerializeField] protected ItemContainer RightOffering;

        public static IEntityRelationshipHandler RelationshipHandler { get; set; }

        protected RectTransform RectTransform { get; set; }

        public void Awake()
        {
            this.RectTransform = this.GetComponent<RectTransform>();

            this.LeftOffering.OnAddItem -= this.Tally;
            this.LeftOffering.OnRemoveItem -= this.Tally;

            this.RightOffering.OnAddItem -= this.Tally;
            this.RightOffering.OnRemoveItem -= this.Tally;

            this.LeftOffering.OnAddItem += this.Tally;
            this.LeftOffering.OnRemoveItem += this.Tally;
            this.RightOffering.OnAddItem += this.Tally;
            this.RightOffering.OnRemoveItem += this.Tally;
        }

        public void OnDisable()
        {
            foreach (IItemInstance item in this.LeftOffering.Contents)
            {
                this.LeftInventory.StackOrAdd(item);
            }

            this.LeftOffering.RemoveAllItems();

            foreach (IItemInstance item in this.RightOffering.Contents)
            {
                this.RightOffering.StackOrAdd(item);
            }

            this.RightOffering.RemoveAllItems();
            
            GlobalConstants.GameManager.GUIManager.CloseGUI(GUINames.TOOLTIP);
        }

        public void SetActors(IEntity left, IEntity right)
        {
            this.Left = left;
            this.Right = right;

            this.LeftInventory.Owner = this.Left;
            this.LeftOffering.Owner = new VirtualStorage();

            this.RightInventory.Owner = this.Right;
            this.RightOffering.Owner = new VirtualStorage();

            this.RightName.text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(this.Right.Gender.PersonalSubject) +
                                  " " + this.Right.Gender.IsOrAre + " offering";
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(this.RectTransform);
        }

        public bool Trade()
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

            int? relationshipValue = RelationshipHandler?.GetHighestRelationshipValue(this.Right, this.Left);

            if (relationshipValue is null)
            {
                return false;
            }

            if (!(leftValue + relationshipValue >= rightValue))
            {
                return false;
            }

            int difference = leftValue - rightValue;

            IEnumerable<IRelationship> relationships =
                RelationshipHandler?.Get(new IJoyObject[] {this.Left, this.Right});
            foreach (IRelationship relationship in relationships)
            {
                relationship.ModifyValueOfParticipant(this.Left.GUID, this.Right.GUID, difference);
            }

            ScriptingEngine.Instance.FetchAction("tradeaction").Execute(
                new IJoyObject[] {this.Left, this.Right},
                new[] {"trade", "give", "item"},
                this.LeftOffering.Contents,
                    this.RightOffering.Contents);

            this.LeftOffering.RemoveAllItems();
            this.RightOffering.RemoveAllItems();

            this.SetActors(this.Left, this.Right);
            this.Tally();

            return true;
        }

        protected void Tally()
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
            this.RightValue.text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(this.Right.Gender.Possessive) +
                                   " value: " + rightValue;
        }

        protected void Tally(IItemContainer container, ItemChangedEventArgs args)
        {
            this.Tally();
        }
    }
}