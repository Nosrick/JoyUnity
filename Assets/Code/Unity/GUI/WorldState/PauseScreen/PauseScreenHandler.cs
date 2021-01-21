namespace JoyLib.Code.Unity.GUI.PauseScreen
{
    public class PauseScreenHandler : ScreenHandler
    {
        public void GoToSettings()
        {
            GlobalConstants.GameManager.GUIManager.OpenGUI(GUINames.SETTINGS);
        }

        public void SaveAndContinue()
        {
            GlobalConstants.GameManager.GUIManager.CloseGUI(GUINames.PAUSE);
        }

        public void SaveAndQuit()
        {
            GlobalConstants.GameManager.SetNextState(new States.MainMenuState());
            this.StartCoroutine(this.LoadSceneAsync("MainMenu"));
        }

        public void QuitNoSave()
        {
            GlobalConstants.GameManager.SetNextState(new States.MainMenuState());
            this.StartCoroutine(this.LoadSceneAsync("MainMenu"));
        }
    }
}