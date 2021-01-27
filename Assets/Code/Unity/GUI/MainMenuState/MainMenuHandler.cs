using JoyLib.Code.IO;
using JoyLib.Code.States;
using JoyLib.Code.World;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI.MainMenuState
{
    public class MainMenuHandler : ScreenHandler
    {
        public void NewGame()
        {
            GlobalConstants.GameManager.SetNextState(new CharacterCreationState());
            this.StartCoroutine(this.LoadSceneAsync("CharacterCreation"));
        }

        public void LoadGame()
        {
            WorldSerialiser serialiser = new WorldSerialiser();
            IWorldInstance world = serialiser.Deserialise("Everse");
            GlobalConstants.GameManager.SetNextState(new WorldInitialisationState(world, world.GetPlayerWorld(world)));
            this.StartCoroutine(this.LoadSceneAsync("MainGame"));
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public void GoToSettings()
        {
            GlobalConstants.GameManager.GUIManager.OpenGUI(GUINames.SETTINGS);
        }
    }
}