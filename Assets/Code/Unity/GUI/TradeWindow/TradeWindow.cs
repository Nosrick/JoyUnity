using System.Collections.Generic;
using System.Globalization;
using DevionGames.InventorySystem;
using DevionGames.UIWidgets;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Relationships;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class TradeWindow : UIWidget
    {
        public IEntity Left { get; protected set; }
        public IEntity Right { get; protected set; }

        [SerializeField]
        protected TextMeshProUGUI LeftValue;
        [SerializeField]
        protected TextMeshProUGUI RightValue;

        [SerializeField] protected TextMeshProUGUI RightName;

        [SerializeField] protected MutableItemContainer LeftInventory;
        [SerializeField] protected MutableItemContainer LeftOffering;

        [SerializeField] protected MutableItemContainer RightInventory;
        [SerializeField] protected MutableItemContainer RightOffering;

        public static IEntityRelationshipHandler RelationshipHandler { get; set; }
        
        protected RectTransform RectTransform { get; set; }

        public new void Awake()
        {
            base.Awake();
            RectTransform = this.GetComponent<RectTransform>();

            LeftInventory.OnAddItem += Tally;
            LeftOffering.OnAddItem += Tally;
            LeftOffering.OnRemoveItem += Tally;
            RightInventory.OnAddItem += Tally;
            RightOffering.OnAddItem += Tally;
            RightOffering.OnRemoveItem += Tally;
        }

        public void SetActors(IEntity left, IEntity right)
        {
            Left = left;
            Right = right;

            for (int i = LeftInventory.Collection.Count - 1; i >= 0; i--)
            {
                Item item = LeftInventory.Collection[i];
                LeftInventory.RemoveItem(item);
            }

            for (int i = RightInventory.Collection.Count - 1; i >= 0; i--)
            {
                Item item = RightInventory.Collection[i];
                RightInventory.RemoveItem(item);
            }

            foreach (ItemInstance item in Left.Backpack)
            {
                LeftInventory.StackOrAdd(item);
            }

            foreach (ItemInstance item in Right.Backpack)
            {
                RightInventory.StackOrAdd(item);
            }

            this.LeftInventory.Owner = Left;
            this.LeftOffering.Owner = Left;

            this.RightInventory.Owner = Right;
            this.RightOffering.Owner = Right;

            RightName.text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Right.Gender.PersonalSubject) + " " + Right.Gender.IsOrAre + " offering";
            LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);
        }

        public bool Trade()
        {
            int leftValue = 0;
            int rightValue = 0;
            
            foreach (Item item in LeftOffering.Collection)
            {
                if (!(item is IItemInstance joyItem))
                {
                    continue;
                }

                leftValue += joyItem.Value;
            }

            foreach (Item item in RightOffering.Collection)
            {
                if (!(item is IItemInstance joyItem))
                {
                    continue;
                }

                rightValue += joyItem.Value;
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

                for(int i = LeftOffering.Collection.Count - 1; i >= 0; i--)
                {
                    if (!(LeftOffering.Collection[i] is ItemInstance joyItem))
                    {
                        continue;
                    }

                    LeftOffering.RemoveItem(joyItem);
                    Left.RemoveContents(joyItem);
                    Right.AddContents(joyItem);
                }

                for(int i = RightOffering.Collection.Count - 1; i >= 0; i--)
                {
                    if (!(RightOffering.Collection[i] is ItemInstance joyItem))
                    {
                        continue;
                    }

                    RightOffering.RemoveItem(joyItem);
                    Right.RemoveContents(joyItem);
                    Left.AddContents(joyItem);
                }
                
                SetActors(Left, Right);

                return true;
            }

            return false;
        }

        protected void Tally(Item itemRef, Slot slot)
        {
            int leftValue = 0;
            int rightValue = 0;
            
            foreach (Item item in LeftOffering.Collection)
            {
                if (!(item is ItemInstance joyItem))
                {
                    continue;
                }

                leftValue += joyItem.Value;
            }

            foreach (Item item in RightOffering.Collection)
            {
                if (!(item is ItemInstance joyItem))
                {
                    continue;
                }

                rightValue += joyItem.Value;
            }

            LeftValue.text = "Your value: " + leftValue;
            RightValue.text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Right.Gender.Possessive) + " value: " + rightValue;
        }

        protected void Tally(Item itemRef, int value, Slot slot)
        {
            Tally(itemRef, slot);
        }
    }
}