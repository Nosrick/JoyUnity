using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    public class Trigger2D : Trigger
    {
        public override bool CanUse(GameObject player)
        {
            //Return false if the trigger is already used
            if (InUse || (Trigger.currentUsedTrigger != null && Trigger.currentUsedTrigger.InUse))
            {
                InventoryManager.Notifications.inUse.Show();
                return false;
            }

            //Return false if the player is not in range
            if (!InRange)
            {
                InventoryManager.Notifications.toFarAway.Show();
                return false;
            }

          /*  Vector3 targetPosition = InventoryManager.current.GetBounds(gameObject).center;//transform.position;
            Vector3 playerPosition = InventoryManager.current.PlayerInfo.transform.position;
            Bounds bounds = InventoryManager.current.PlayerInfo.bounds;
            playerPosition.y += bounds.center.y + bounds.extents.y;
            Vector3 direction = targetPosition - playerPosition;

            InventoryManager.current.PlayerInfo.collider2D.enabled = false;
            RaycastHit hit;

            bool raycast = Physics.Raycast(playerPosition, direction, out hit);
            InventoryManager.current.PlayerInfo.collider2D.enabled = true;
            if (raycast && hit.transform != transform && gameObject != InventoryManager.current.PlayerInfo.gameObject)
            {
                if (InventoryManager.DefaultSettings.debugMessages)
                    Debug.Log("Can't use Trigger: " + transform.name + ". Raycast failed hit: " + hit.transform.name);
                return false;
            }*/
            //Trigger can be used
            return true;
        }

        protected override void CreateTriggerCollider()
        {
            Vector2 position = Vector2.zero;

            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                position = collider.bounds.center;
                position.y = (collider.bounds.center - collider.bounds.extents).y;
                position = transform.InverseTransformPoint(position);
            }

            CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
            circleCollider.isTrigger = true;
            circleCollider.offset = position;
            Vector3 scale = transform.lossyScale;
            circleCollider.radius = useDistance / Mathf.Max(scale.x, scale.y);
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            //Check if the collider other is player 
            if (other.tag == InventoryManager.current.PlayerInfo.gameObject.tag)
            {
                //Set that player is in range
                InRange = true;
            }
        }

        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            //Check if the collider other is player
            if (other.tag == InventoryManager.current.PlayerInfo.gameObject.tag)
            {
                //Set that player is out of range
                InRange = false;
            }
        }
    }
}