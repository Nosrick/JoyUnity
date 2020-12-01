using JoyLib.Code.States;

namespace Joy.Code.Managers
{
    public interface IStateManager
    {
        void ChangeState(IGameState newState);
        void LoadContent();
        void Start();
        void Update();
        void NextState();
    }
}