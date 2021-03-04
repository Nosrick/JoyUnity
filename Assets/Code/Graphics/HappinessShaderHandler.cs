using System;
using System.Collections;
using System.Collections.Generic;
using JoyLib.Code.Helpers;
using JoyLib.Code.Unity;
using UnityEngine;

namespace JoyLib.Code.Graphics
{
    public class HappinessShaderHandler : MonoBehaviour
    {
        protected SpriteRenderer[] SpriteRenderers { get; set; }
        protected IPosition GridPosition { get; set; }

        protected const string _HAPPINESS = "_Happiness";
        protected const string _VISION_COLOUR = "_VisionColour";
        
        public void Start()
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
            
        }

        // Update is called once per frame
        void Update()
        {
            float happiness = GlobalConstants.GameManager?.Player is null
                ? 1f
                : GlobalConstants.GameManager.Player.OverallHappiness;
            Color visionColour = GlobalConstants.GameManager?.Player is null
                ? Color.clear
                : LightLevelHelper.GetColour(
                    GlobalConstants.GameManager.Player.MyWorld.LightCalculator.Light.GetLight(
                        this.GridPosition.WorldPosition),
                    GlobalConstants.GameManager.Player.VisionProvider);
            foreach (var renderer in this.SpriteRenderers)
            {
                
                var material = renderer.material;
                material.SetFloat(_HAPPINESS, happiness);
                //material.SetColor(_VISION_COLOUR, visionColour);
            }
        }
    }
}

