using System.Collections;
using JoyLib.Code.States;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JoyLib.Code.Unity.GUI.GameOver
{
    public class GameOverHandler : MonoBehaviour
    {
        public void ToMainMenu()
        {
            GlobalConstants.GameManager.SetNextState(new States.MainMenuState());
            this.StartCoroutine(this.LoadSceneAsync("MainMenu"));
        }

        public void NewCharacter()
        {
            GlobalConstants.GameManager.SetNextState(new CharacterCreationState());
            this.StartCoroutine(this.LoadSceneAsync("CharacterCreation"));
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        protected IEnumerator LoadSceneAsync(string name)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(name);
            operation.allowSceneActivation = false;
            while (operation.progress < 0.9f)
            {
                GlobalConstants.ActionLog.AddText("Loading... " + (operation.progress * 100) + "%");
                yield return new WaitForSeconds(0.2f);
            }
            operation.allowSceneActivation = true;
            GlobalConstants.ActionLog.AddText("Done loading!");
            yield return operation;
        }
    }
}