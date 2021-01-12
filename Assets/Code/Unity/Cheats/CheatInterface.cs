using System;
using JoyLib.Code.Entities.Needs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JoyLib.Code.Unity.Cheats
{
    public class CheatInterface : MonoBehaviour
    {
        [SerializeField] protected int m_ActionsRequired = 5;
        
        protected int ActionCount { get; set; }
        protected bool Active { get; set; }

        public void Awake()
        {
            InputSystem.onActionChange += this.HandleInput;
            this.ActionCount = 0;
            this.Active = false;
        }

        protected void HandleInput(object data, InputActionChange change)
        {
            if (change != InputActionChange.ActionPerformed)
            {
                return;
            }

            if (!(data is InputAction action))
            {
                return;
            }

            if (action.name.Equals("unstack", StringComparison.OrdinalIgnoreCase))
            {
                this.ActionCount += 1;
            }
            else if (action.name.Equals("close all windows", StringComparison.OrdinalIgnoreCase))
            {
                this.ResetCount();
            }
            else
            {
                this.ActionCount = 0;
            }

            if (this.ActionCount >= this.m_ActionsRequired)
            {
                this.Active = true;
            }
        }

        protected void ResetCount()
        {
            this.ActionCount = 0;
            this.Active = false;
        }

        protected void OnGUI()
        {
            if (this.Active)
            {
                if (GUILayout.Button("Fill Needs"))
                {
                    foreach (INeed need in GlobalConstants.GameManager.Player.Needs.Values)
                    {
                        need.Fulfill(need.HappinessThreshold);
                    }
                }

                if (GUILayout.Button("Empty Needs"))
                {
                    foreach (INeed need in GlobalConstants.GameManager.Player.Needs.Values)
                    {
                        need.Decay(need.Value);
                    }
                }
            }
        }
    }
}