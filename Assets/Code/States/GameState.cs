using Joy.Code.Managers;

namespace JoyLib.Code.States
{
    public abstract class GameState
    {
        protected GameState()
        {
        }

        public virtual void LoadContent()
        {
        }

        protected virtual void SetUpUi()
        {
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

        /// <summary>
        /// Should be called last, so as to draw the current GUI
        /// </summary>
        public virtual void Draw()
        {
        }

        public virtual void OnGui()
        {
        }

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
    }
}