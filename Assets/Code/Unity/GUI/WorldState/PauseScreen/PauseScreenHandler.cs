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
            this.Save();
            GlobalConstants.GameManager.GUIManager.CloseGUI(GUINames.PAUSE);
        }

        public void SaveAndQuit()
        {
            this.Save();
            GlobalConstants.GameManager.SetNextState(new States.MainMenuState());
            this.StartCoroutine(this.LoadSceneAsync("MainMenu"));
        }

        protected void Save()
        {
            GlobalConstants.GameManager.WorldSerialiser.Serialise(
                GlobalConstants.GameManager.Player.MyWorld.GetOverworld());
        }

        public void QuitNoSave()
        {
            GlobalConstants.GameManager.SetNextState(new States.MainMenuState());
            this.StartCoroutine(this.LoadSceneAsync("MainMenu"));
        }
    }
}