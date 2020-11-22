using System.Globalization;
using DevionGames.InventorySystem;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Relationships;
using TMPro;
using UnityEngine;

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

        [SerializeField] protected MutableItemContainer LeftInventory;
        [SerializeField] protected MutableItemContainer LeftOffering;

        [SerializeField] protected MutableItemContainer RightInventory;
        [SerializeField] protected MutableItemContainer RightOffering;

        protected EntityRelationshipHandler RelationshipHandler { get; set; }

        public void Awake()
        {
            if (RelationshipHandler is null)
            {
                RelationshipHandler = GlobalConstants.GameManager.GetComponent<EntityRelationshipHandler>();
            }

            LeftInventory.OnAddItem += Tally;
            LeftOffering.OnAddItem += Tally;
            RightInventory.OnAddItem += Tally;
            RightOffering.OnAddItem += Tally;
        }

        public void SetActors(Entity left, Entity right)
        {
            Left = left;
            Right = right;

            LeftInventory.Collection.Clear();
            LeftOffering.Collection.Clear();
            RightInventory.Collection.Clear();
            RightOffering.Collection.Clear();

            foreach (ItemInstance item in Left.Backpack)
            {
                LeftInventory.StackOrAdd(item.Item);
            }

            foreach (ItemInstance item in Right.Backpack)
            {
                RightInventory.StackOrAdd(item.Item);
            }
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

            int relatioshipValue = RelationshipHandler.GetHighestRelationshipValue(Left, Right);

            if (leftValue + relatioshipValue >= rightValue)
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
            RightValue.text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Right.Gender.Personal) + " value: " + rightValue;
        }
    }
}