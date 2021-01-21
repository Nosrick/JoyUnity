using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JoyLib.Code.Unity
{
    public class ScreenHandler : MonoBehaviour
    {
        protected IEnumerator LoadSceneAsync(string sceneName)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
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