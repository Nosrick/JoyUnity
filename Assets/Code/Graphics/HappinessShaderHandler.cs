using System;
using System.Collections;
using System.Collections.Generic;
using JoyLib.Code.Events;
using JoyLib.Code.Helpers;
using JoyLib.Code.Settings;
using JoyLib.Code.Unity;
using UnityEngine;

namespace JoyLib.Code.Graphics
{
    public class HappinessShaderHandler : MonoBehaviour
    {
        protected SpriteRenderer[] SpriteRenderers { get; set; }
        protected IPosition GridPosition { get; set; }
        
        protected bool Enabled { get; set; }

        protected const string _HAPPINESS = "_Happiness";
        
        protected void Start()
        {
            this.SpriteRenderers = this.GetComponentsInChildren<SpriteRenderer>();
            if (this.TryGetComponent(out GridPosition position) == false)
            {
                this.GridPosition = this.GetComponent<MonoBehaviourHandler>();
            }
            else
            {
                this.GridPosition = position;
            }

            GlobalConstants.GameManager.SettingsManager.OnSettingChange -= this.UpdateSetting;
            GlobalConstants.GameManager.SettingsManager.OnSettingChange += this.UpdateSetting;
        }

        protected void UpdateSetting(SettingChangedEventArgs args)
        {
            if (args.Setting is JoyShaderSetting shaderSetting)
            {
                this.Enabled = shaderSetting.value;
            }
        }

        // Update is called once per frame
        protected void Update()
        {
            float happiness = GlobalConstants.GameManager?.Player is null || this.Enabled == false
                ? 1f
                : GlobalConstants.GameManager.Player.OverallHappiness;

            foreach (var renderer in this.SpriteRenderers)
            {
                renderer.material.SetFloat(_HAPPINESS, happiness);
            }
        }
    }
}

