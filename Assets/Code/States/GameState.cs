using Joy.Code.Managers;
using JoyLib.Code.Unity.GUI;
using UnityEditor;
using UnityEngine;

namespace JoyLib.Code.States
{
    public abstract class GameState : IGameState
    {
        protected GameState()
        {
            GUIManager = GlobalConstants.GameManager.GUIManager;
        }

        public virtual void LoadContent()
        {
        }

        public virtual void SetUpUi()
        {
            GUIData[] guiData = GameObject.FindObjectsOfType<GUIData>();
            foreach (GUIData data in guiData)
            {
                GUIManager.AddGUI(data);
            }
        }

        public virtual void Start()
        {
        }

        public virtual void Stop()
        {
        }

        //ALWAYS call base.Update() from derived classes
        public virtual void Update()
        {
        }

        public virtual void HandleInput()
        {
        }

        public abstract GameState GetNextState();

        public static bool InFocus
        {
            get;
            set;
        }

        public bool Done
        {
            get;
            protected set;
        }

        public IGUIManager GUIManager { get; protected set; }
    }
}