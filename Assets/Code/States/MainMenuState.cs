using JoyLib.Code.IO;
using JoyLib.Code.World;
using System;

namespace JoyLib.Code.States
{
    class MainMenuState : GameState
    {
        private GameState m_NextState;

        public MainMenuState() :
            base()
        {
            SetUpUi();
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void SetUpUi()
        {
            base.SetUpUi();
        }

        public override void Draw()
        {
        }

        public override void OnGui()
        {
            base.OnGui();
        }

        public override void Update()
        {
            base.Update();
        }

        private void NewGame(object sender, EventArgs eventArgs)
        {
            Done = true;
            m_NextState = new CharacterCreationState();
        }

        private void ContinueGame(object sender, EventArgs eventArgs)
        {
            WorldInstance overworld = WorldSerialiser.Deserialise("Everse");
            Done = true;

            WorldInstance playerWorld = overworld.GetWorldOfEntity(overworld.Player.GUID);
            m_NextState = new WorldState(overworld, playerWorld, Gameplay.GameplayFlags.Moving);
        }

        public override GameState GetNextState()
        {
            return m_NextState;
        }
    }
}