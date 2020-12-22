using JoyLib.Code.States;
using UnityEngine.InputSystem;

namespace Joy.Code.Managers
{
    public class StateManager : IStateManager
    {
        private IGameState m_ActiveState;

        public StateManager()
        {
            m_ActiveState = new MainMenuState();
        }

        public void ChangeState(IGameState newState)
        {
            m_ActiveState.Stop();
            m_ActiveState = newState;
            m_ActiveState.Start();
            m_ActiveState.LoadContent();
        }

        public void LoadContent()
        {
            m_ActiveState.LoadContent();
        }

        public void Start()
        {
            m_ActiveState = new MainMenuState();
            m_ActiveState.Start();
        }

        public void Update()
        {
            m_ActiveState.Update();

            if(m_ActiveState.Done)
            {
                ChangeState(m_ActiveState.GetNextState());
            }
        }

        public void OnMove(InputValue inputValue)
        {
            this.m_ActiveState.HandleInput(inputValue);
        }

        public void NextState()
        {
            IGameState newState = m_ActiveState.GetNextState();
            ChangeState(newState);
        }
    }
}