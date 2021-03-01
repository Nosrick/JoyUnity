using System;
using System.Collections.Generic;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Statistics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace JoyLib.Code.Unity.Cheats
{
    public class CheatInterface : MonoBehaviour
    {
        [SerializeField] protected int m_ActionsRequired = 5;
        
        protected int ActionCount { get; set; }
        protected bool Active { get; set; }
        
        public IDictionary<string, bool> CheatBank { get; protected set; }

        public void Awake()
        {
            InputSystem.onActionChange += this.HandleInput;
            this.ActionCount = 0;
            this.Active = false;
            this.CheatBank = new Dictionary<string, bool>();
            if (GlobalConstants.GameManager is null == false)
            {
                GlobalConstants.GameManager.Cheats = this;
            }
        }

        protected void HandleInput(object data, InputActionChange change)
        {
            if (GlobalConstants.GameManager is null == false
                && GlobalConstants.GameManager.Cheats is null)
            {
                GlobalConstants.GameManager.Cheats = this;
            }
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
            else if(action.name.Equals("click", StringComparison.OrdinalIgnoreCase) == false 
            && action.activeControl is ButtonControl)
            {
                this.ResetCount();
            }

            if (this.ActionCount >= this.m_ActionsRequired)
            {
                this.Active = !this.Active;
                this.ActionCount = 0;
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
                if (GlobalConstants.GameManager.Player is null)
                {
                    return;
                }
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

                if (GUILayout.Button("Knock Player Unconscious"))
                {
                    GlobalConstants.GameManager.Player.SetValue(
                        DerivedValueName.HITPOINTS,
                        0);
                }

                if (GUILayout.Button("Kill Player"))
                {
                    GlobalConstants.GameManager.Player.SetValue(
                        DerivedValueName.HITPOINTS,
                        -GlobalConstants.GameManager.Player.GetMaximum(DerivedValueName.HITPOINTS));
                }

                if (GUILayout.Button("Fully Heal Player"))
                {
                    GlobalConstants.GameManager.Player.SetValue(
                        DerivedValueName.HITPOINTS,
                        GlobalConstants.GameManager.Player.GetMaximum(DerivedValueName.HITPOINTS));
                }

                if (GUILayout.Button("Give 100 JX"))
                {
                    GlobalConstants.GameManager.Player.CurrentJob.AddExperience(100);
                }

                if (GUILayout.Button("Fullbright Toggle"))
                {
                    if (this.CheatBank.ContainsKey("fullbright"))
                    {
                        this.CheatBank["fullbright"] = !this.CheatBank["fullbright"];
                    }
                    else
                    {
                        this.CheatBank.Add("fullbright", true);
                    }
                }
            }
        }
    }
}