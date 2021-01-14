using System.Collections;
using JoyLib.Code.States;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JoyLib.Code.Unity.GUI.MainMenuState
{
    public class MainMenuHandler : MonoBehaviour
    {
        [SerializeField] protected GameManager m_GameManager;

        public void NewGame()
        {
            this.m_GameManager.SetNextState(new CharacterCreationState());
            this.StartCoroutine(this.LoadSceneAsync());
        }

        protected IEnumerator LoadSceneAsync()
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync("CharacterCreation");
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

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}