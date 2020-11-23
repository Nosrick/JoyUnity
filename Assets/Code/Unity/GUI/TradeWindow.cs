using System.Globalization;
using Castle.DynamicProxy;
using DevionGames.InventorySystem;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Relationships;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class TradeWindow : MonoBehaviour
    {
        public Entity Left { get; protected set; }
        public Entity Right { get; protected set; }

        [SerializeField]
        protected TextMeshProUGUI LeftValue;
        [SerializeField]
        protected TextMeshProUGUI RightValue;

        [SerializeField] protected TextMeshProUGUI RightName;

        [SerializeField] protected MutableItemContainer LeftInventory;
        [SerializeField] protected MutableItemContainer LeftOffering;

        [SerializeField] protected MutableItemContainer RightInventory;
        [SerializeField] protected MutableItemContainer RightOffering;

        protected EntityRelationshipHandler RelationshipHandler { get; set; }
        
        protected RectTransform RectTransform { get; set; }

        public void Awake()
        {
            if (RelationshipHandler is null)
            {
                RelationshipHandler = GlobalConstants.GameManager.GetComponent<EntityRelationshipHandler>();
            }

            RectTransform = this.GetComponent<RectTransform>();

            LeftInventory.OnAddItem += Tally;
            LeftOffering.OnAddItem += Tally;
            LeftOffering.OnRemoveItem += Tally;
            RightInventory.OnAddItem += Tally;
            RightOffering.OnAddItem += Tally;
            RightOffering.OnRemoveItem += Tally;
        }

        public void SetActors(Entity left, Entity right)
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
                LeftInventory.StackOrAdd(item.Item);
            }

            foreach (ItemInstance item in Right.Backpack)
            {
                RightInventory.StackOrAdd(item.Item);
            }

            RightName.text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Right.Gender.Personal) + " " + Right.Gender.IsOrAre + " offering";
            LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);
        }

        public bool Trade()
        {
            int leftValue = 0;
            int rightValue = 0;
            
            foreach (Item item in LeftOffering.Collection)
            {
                if (!(item is JoyItem joyItem))
                {
                    continue;
                }

                leftValue += joyItem.ItemInstance.Value;
            }

            foreach (Item item in RightOffering.Collection)
            {
                if (!(item is JoyItem joyItem))
                {
                    continue;
                }

                rightValue += joyItem.ItemInstance.Value;
            }

            int relationshipValue = RelationshipHandler.GetHighestRelationshipValue(Right, Left);

            if (leftValue + relationshipValue >= rightValue)
            {
                int difference = leftValue - rightValue;

                IRelationship[] relationships = RelationshipHandler.Get(new IJoyObject[] { Left, Right });
                foreach (IRelationship relationship in relationships)
                {
                    relationship.ModifyValueOfParticipant(Left.GUID, Right.GUID, difference);
                }

                for(int i = LeftOffering.Collection.Count - 1; i >= 0; i--)
                {
                    if (!(LeftOffering.Collection[i] is JoyItem joyItem))
                    {
                        continue;
                    }

                    LeftOffering.RemoveItem(joyItem);
                    Left.RemoveItemFromBackpack(joyItem.ItemInstance);
                    Right.AddContents(joyItem.ItemInstance);
                }

                for(int i = RightOffering.Collection.Count - 1; i >= 0; i--)
                {
                    if (!(RightOffering.Collection[i] is JoyItem joyItem))
                    {
                        continue;
                    }

                    RightOffering.RemoveItem(joyItem);
                    Right.RemoveItemFromBackpack(joyItem.ItemInstance);
                    Left.AddContents(joyItem.ItemInstance);
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
                if (!(item is JoyItem joyItem))
                {
                    continue;
                }

                leftValue += joyItem.ItemInstance.Value;
            }

            foreach (Item item in RightOffering.Collection)
            {
                if (!(item is JoyItem joyItem))
                {
                    continue;
                }

                rightValue += joyItem.ItemInstance.Value;
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