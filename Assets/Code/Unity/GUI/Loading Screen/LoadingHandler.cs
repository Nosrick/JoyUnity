using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JoyLib.Code.Unity.GUI.Loading_Screen
{
    public class LoadingHandler : MonoBehaviour
    {
        [SerializeField] protected GameManager m_GameManager;
        [SerializeField] protected TextMeshProUGUI m_LoadingMessage;
        
        protected bool LoadingMenu { get; set; }
        public void FixedUpdate()
        {
            if (this.m_GameManager is null == false)
            {
                this.m_LoadingMessage.text = this.m_GameManager.LoadingMessage;

                if (this.m_GameManager.BegunInitialisation == false)
                {
                    this.StartCoroutine(this.m_GameManager.Initialise());
                }

                if (this.LoadingMenu == false && this.m_GameManager.Initialised)
                {
                    this.LoadingMenu = true;
                    this.m_GameManager.SetNextState(new States.MainMenuState());
                    this.StartCoroutine(this.LoadSceneAsync());
                }
            }
        }

        protected IEnumerator LoadSceneAsync()
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync("MainMenu");
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