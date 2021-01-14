using JoyLib.Code.Cultures;
using JoyLib.Code.IO;
using JoyLib.Code.Unity.GUI;
using JoyLib.Code.World;
using UnityEngine.InputSystem;

namespace JoyLib.Code.States
{
    class MainMenuState : GameState
    {
        protected GameState m_NextState;

        protected WorldSerialiser m_WorldSerialiser = new WorldSerialiser();

        public MainMenuState() :
            base()
        {
        }

        public override void LoadContent()
        {
        }

        public override void SetUpUi()
        {
            base.SetUpUi();
            ManagedBackground mainMenu = this.GUIManager.OpenGUI("Main Menu").GetComponent<ManagedBackground>();
            Cursor cursor = this.GUIManager.OpenGUI(GUINames.CURSOR).GetComponent<Cursor>();

            ICulture[] cultures = GlobalConstants.GameManager.CultureHandler.Cultures;
            int result = GlobalConstants.GameManager.Roller.Roll(0, cultures.Length);
            ICulture randomCulture = cultures[result];
            mainMenu.SetBackground(this.GUIManager.Background);
            mainMenu.SetColours(randomCulture.BackgroundColours);
            //cursor.SetCursorSprites(this.GUIManager.Cursor);
            //cursor.SetCursorSize(64, 64);
            cursor.SetCursorColours(randomCulture.CursorColours);
        }

        public override void Start()
        {
        }

        public override void Stop()
        {
        }

        public override void Update()
        {
        }

        public override void HandleInput(object data, InputActionChange action)
        {
        }

        private void NewGame()
        {
            this.Done = true;
            this.m_NextState = new CharacterCreationState();
        }

        private void ContinueGame()
        {
            IWorldInstance overworld = this.m_WorldSerialiser.Deserialise("Everse");
            this.Done = true;

            IWorldInstance playerWorld = overworld.Player.MyWorld;
            this.m_NextState = new WorldState(overworld, playerWorld);
        }

        public override GameState GetNextState()
        {
            return this.m_NextState;
        }
    }
}