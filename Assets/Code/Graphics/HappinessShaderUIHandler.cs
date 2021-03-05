using JoyLib.Code.Entities;
using JoyLib.Code.Events;
using JoyLib.Code.Settings;
using JoyLib.Code.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Graphics
{
    public class HappinessShaderUIHandler : MonoBehaviour
    {
        protected Image[] Images { get; set; }
        protected IPosition GridPosition { get; set; }

        protected bool Enabled { get; set; }

        protected bool UpdatedSinceLastSettingChange { get; set; }

        protected const string _HAPPINESS = "_Happiness";

        protected bool PlayerDetected { get; set; }

        protected void Start()
        {
            this.Images = this.GetComponentsInChildren<Image>();
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
            
            JoyShaderSetting setting = GlobalConstants.GameManager.SettingsManager.GetSetting("Joy Shader") as JoyShaderSetting;
            this.Enabled = setting?.value ?? false;

            this.SetHappiness(1f);
        }

        protected void UpdateSetting(SettingChangedEventArgs args)
        {
            if (args.Setting is JoyShaderSetting shaderSetting)
            {
                this.Enabled = shaderSetting.value;
                this.UpdatedSinceLastSettingChange = false;
            }
        }

        // Update is called once per frame
        protected void Update()
        {
            IEntity player = GlobalConstants.GameManager?.Player;

            if (player is null)
            {
                return;
            }

            if (this.PlayerDetected == false)
            {
                this.PlayerDetected = true;
                player.HappinessIsDirty = true;
            }
            
            if (player.HappinessIsDirty == false
                || this.Enabled == false)
            {
                return;
            }

            this.SetHappiness(player.OverallHappiness);
        }

        protected void SetHappiness(float happinessRef)
        {
            float happiness = this.Enabled == false
                ? 1f
                : happinessRef;

            foreach (var renderer in this.Images)
            {
                renderer.material.SetFloat(_HAPPINESS, happiness);
            }

            this.UpdatedSinceLastSettingChange = true;
        }
    }
}