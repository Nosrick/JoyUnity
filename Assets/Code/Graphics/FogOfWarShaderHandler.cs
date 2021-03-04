using System;
using JoyLib.Code.Entities;
using JoyLib.Code.Helpers;
using JoyLib.Code.Unity;
using JoyLib.Code.World;
using UnityEngine;

namespace JoyLib.Code.Graphics
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(GridPosition))]
    public class FogOfWarShaderHandler : MonoBehaviour
    {
        protected SpriteRenderer SpriteRenderer { get; set; }
        protected Material Material { get; set; }
        protected GridPosition GridPosition { get; set; }

        public void Start()
        {
            this.SpriteRenderer = this.GetComponent<SpriteRenderer>();
            this.Material = this.SpriteRenderer.material;
            this.GridPosition = this.GetComponent<GridPosition>();
        }

        public void Update()
        {
            IEntity player = GlobalConstants.GameManager?.Player;

            if (player is null)
            {
                return;
            }

            IWorldInstance world = player.MyWorld;
            if (GlobalConstants.GameManager.Cheats.CheatBank.TryGetValue("fullbright", out bool cheat) && cheat)
            {
                this.Material.SetColor("_LightColour", Color.clear);
            }
            else
            {
                bool canSee = player.VisionProvider.CanSee(player, world, this.GridPosition.WorldPosition);
                if (canSee)
                {
                    int lightLevel = world.LightCalculator.Light.GetLight(this.GridPosition.WorldPosition);
                    this.Material.SetColor(
                        "_LightColour",
                        LightLevelHelper.GetColour(
                                lightLevel,
                                player.VisionProvider));
                }
                else
                {
                    this.Material.SetColor("_LightColour", player.VisionProvider.DarkColour);
                }
            }
        }
    }
}