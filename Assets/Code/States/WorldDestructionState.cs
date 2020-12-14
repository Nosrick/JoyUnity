using JoyLib.Code.World;

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

        public override void Start()
        {
            base.Start();
            DestroyWorld();
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
