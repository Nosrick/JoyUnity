using JoyLib.Code.States;

namespace Joy.Code.Managers
{
    public class StateManager
    {
        private GameState m_ActiveState;

        public StateManager()
        {
            m_ActiveState = new MainMenuState();
        }

        public void ChangeState(GameState newState)
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
            m_ActiveState.HandleInput();

            if(m_ActiveState.Done)
            {
                ChangeState(m_ActiveState.GetNextState());
            }
        }

        public void BeginDraw()
        {
        }

        public void Draw()
        {
            m_ActiveState.Draw();
        }

        public void OnGui()
        {
            m_ActiveState.OnGui();
        }
    }
}