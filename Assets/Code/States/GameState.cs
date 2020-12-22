using System.Threading;
using JoyLib.Code.Unity.GUI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JoyLib.Code.States
{
    public abstract class GameState : IGameState
    {
        protected GameState()
        {
            GUIManager = GlobalConstants.GameManager.GUIManager;
        }

        public abstract void LoadContent();

        public virtual void SetUpUi()
        {
            Thread.Sleep(500);
            
            GUIData[] guiData = GameObject.FindObjectsOfType<GUIData>();
            foreach (GUIData data in guiData)
            {
                GUIManager.AddGUI(data);
            }
        }

        public abstract void Start();

        public abstract void Stop();

        //ALWAYS call base.Update() from derived classes
        public abstract void Update();

        public abstract void HandleInput(InputValue inputValue);

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