using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    public class PlayerInfo 
    {
        private GameObject m_GameObject;
        public GameObject gameObject {
            get {
				if (this.m_GameObject == null)
				{
					GameObject[] players = GameObject.FindGameObjectsWithTag(InventoryManager.DefaultSettings.playerTag);
					for (int i = 0; i < players.Length; i++)
					{
						GameObject player = players[i];

						this.m_GameObject = player;
					}
				}
				return this.m_GameObject;
			}
        }

        private Transform m_Transform;
        public Transform transform
        {
            get
            {
                if (gameObject != null)
                {
                    return this.gameObject.transform;
                }
                return null;
            }
        }

        private Collider m_Collider;
        public Collider collider
        {
            get
            {
                if (this.m_Collider == null)
                {
                    this.m_Collider = this.gameObject.GetComponent<Collider>();
                }
                return this.m_Collider;
            }
        }

        private Collider2D m_Collider2D;
        public Collider2D collider2D
        {
            get
            {
                if (this.m_Collider2D == null)
                {
                    this.m_Collider2D = this.gameObject.GetComponent<Collider2D>();
                }
                return this.m_Collider2D;
            }
        }

        private Animator m_Animator;
        public Animator animator
        {
            get
            {
                if (this.m_Animator == null)
                {
                    this.m_Animator = this.gameObject.GetComponent<Animator>();
                }
                return this.m_Animator;
            }
        }

        public Bounds bounds
        {
            get
            {

                Bounds bounds = new Bounds();
                Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
                if (renderers.Length > 0)
                {
                    foreach (Renderer renderer in renderers)
                    {
                        if (renderer.enabled)
                        {
                            bounds = renderer.bounds;
                            break;
                        }
                    }
                    foreach (Renderer renderer in renderers)
                    {
                        if (renderer.enabled)
                        {
                            bounds.Encapsulate(renderer.bounds);
                        }
                    }
                }
                return bounds;
            }
        }

      
    }
}