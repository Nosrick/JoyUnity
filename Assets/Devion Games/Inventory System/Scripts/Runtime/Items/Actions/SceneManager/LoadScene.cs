using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DevionGames.InventorySystem.ItemActions
{
    [ComponentMenu("SceneManager/LoadScene")]
    public class LoadScene : ItemAction
    {
        [SerializeField]
        private string m_Scene=string.Empty;

        public override ActionStatus OnUpdate()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            if (currentScene.name != this.m_Scene)
            {
                if (InventoryManager.SavingLoading.autoSave) { InventoryManager.Save(); };
                SceneManager.LoadScene(this.m_Scene);
            }
            return ActionStatus.Success;
        }
    }
}