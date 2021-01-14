using System.Threading;
using JoyLib.Code.Unity.GUI;
using UnityEngine;
using UnityEngine.InputSystem;
using Cursor = JoyLib.Code.Unity.GUI.Cursor;

namespace JoyLib.Code.States
{
    public abstract class GameState : IGameState
    {
        protected GameState()
        {
            this.GUIManager = GlobalConstants.GameManager.GUIManager;
        }

        public abstract void LoadContent();

        public virtual void SetUpUi()
        {
            Thread.Sleep(500);

            this.GUIManager.Clear();
            GUIData[] guiData = GameObject.FindObjectsOfType<GUIData>();
            foreach (GUIData data in guiData)
            {
                this.GUIManager.AddGUI(data);
            }

            Cursor cursor = this.GUIManager.OpenGUI(GUINames.CURSOR)
                .GetComponent<Cursor>();
            cursor.SetCursorSprites(this.GUIManager.Cursor);
            cursor.SetCursorColours(this.GUIManager.CursorColours);
            this.GUIManager.CloseAllOtherGUIs(GUINames.CURSOR);
        }

        public abstract void Start();

        public abstract void Stop();

        //ALWAYS call base.Update() from derived classes
        public abstract void Update();

        public abstract void HandleInput(object data, InputActionChange action);

        public abstract GameState GetNextState();

        public bool Done
        {
            get;
            protected set;
        }

        public IGUIManager GUIManager { get; protected set; }
    }
}