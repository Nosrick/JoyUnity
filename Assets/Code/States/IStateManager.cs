using JoyLib.Code.States;

namespace Joy.Code.Managers
{
    public interface IStateManager
    {
        void ChangeState(GameState newState);
        void LoadContent();
        void Start();
        void Update();
    }
}