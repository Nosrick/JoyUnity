using JoyLib.Code.World;
using UnityEngine.InputSystem;

namespace JoyLib.Code.States
{
    public class WorldDestructionState : GameState
    {
        protected IWorldInstance m_OverWorld;
        protected IWorldInstance m_NextWorld;

        public WorldDestructionState(IWorldInstance overworld, IWorldInstance nextWorld)
        {
            m_OverWorld = overworld;
            m_NextWorld = nextWorld;
        }

        public override void LoadContent()
        {
        }

        public override void Start()
        {
            DestroyWorld();
        }

        public override void Stop()
        {
        }

        public override void Update()
        {
        }

        public override void HandleInput(InputValue inputValue)
        {
        }

        protected void DestroyWorld()
        {
            IGameManager gameManager = GlobalConstants.GameManager;
            gameManager.EntityPool.RetireAll();
            gameManager.ItemPool.RetireAll();
            gameManager.FogPool.RetireAll();
            gameManager.FloorPool.RetireAll();
            gameManager.WallPool.RetireAll();

            Done = true;
        }

        public override GameState GetNextState()
        {
            return new WorldInitialisationState(m_OverWorld, m_NextWorld);
        }
    }
}
