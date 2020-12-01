using JoyLib.Code.Collections;
using JoyLib.Code.Cultures;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Graphics;
using JoyLib.Code.Rollers;
using System;
using System.Collections.Generic;
using JoyLib.Code.Unity.GUI;
using JoyLib.Code.Unity.GUI.Containers;
using UnityEngine;

namespace JoyLib.Code.States
{
    public class CharacterCreationState : GameState
    {
        protected CharacterCreationScreen CharacterCreationScreen { get; set; }

        public CharacterCreationState()
        {
        }

        public override void Start()
        {
            base.Start();
            SetUpUi();
        }

        public override void SetUpUi()
        {
            base.SetUpUi();
            GUIManager.OpenGUI(GlobalConstants.CHARACTER_CREATION);
            CharacterCreationScreen = GUIManager
                .GetGUI(GlobalConstants.CHARACTER_CREATION)
                .GetComponent<CharacterCreationScreen>();
        }

        public override void HandleInput()
        {
            base.HandleInput();
        }

        public override void Update()
        {
            base.Update();
        }

        public override GameState GetNextState()
        {
            return new WorldCreationState(new EntityPlayer(CharacterCreationScreen.CreatePlayer()));
        }
    }
}