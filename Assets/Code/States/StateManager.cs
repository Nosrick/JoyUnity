using JoyLib.Code.States;
using UnityEngine.InputSystem;

namespace Joy.Code.Managers
{
    public class StateManager : IStateManager
    {
        protected IGameState m_ActiveState;

        public StateManager()
        {
            InputSystem.onActionChange -= this.OnMove;
            InputSystem.onActionChange += this.OnMove;
        }

        public void ChangeState(IGameState newState)
        {
            this.m_ActiveState?.Stop();
            this.m_ActiveState = newState;
            this.m_ActiveState.Start();
            this.m_ActiveState.LoadContent();
            this.m_ActiveState.SetUpUi();
        }

        public void LoadContent()
        {
            this.m_ActiveState.LoadContent();
        }

        public void Start()
        {
            this.m_ActiveState = new MainMenuState();
            this.m_ActiveState.Start();
        }

        public void Update()
        {
            if (this.m_ActiveState is null)
            {
                return;
            }
            
            this.m_ActiveState.Update();

            if(this.m_ActiveState.Done)
            {
                this.ChangeState(this.m_ActiveState.GetNextState());
            }
        }

        public void OnMove(object data, InputActionChange change)
        {
            this.m_ActiveState?.HandleInput(data, change);
        }

        public void NextState()
        {
            IGameState newState = this.m_ActiveState.GetNextState();
            this.ChangeState(newState);
        }
    }
}