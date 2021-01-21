using JoyLib.Code.Helpers;
using JoyLib.Code.States;
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
            GlobalConstants.ActionLog.AddText("Not quite implemented yet!", LogLevel.Warning);
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