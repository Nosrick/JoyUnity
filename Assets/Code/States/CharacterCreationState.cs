using System.Linq;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Unity.GUI;

namespace JoyLib.Code.States
{
    public class CharacterCreationState : GameState
    {
        protected CharacterCreationScreen CharacterCreationScreen { get; set; }

        public CharacterCreationState()
        {
        }

        public override void Start()
        {
            base.Start();
            SetUpUi();
        }

        public override void SetUpUi()
        {
            base.SetUpUi();
            CharacterCreationScreen = GUIManager
                .GetGUI(GlobalConstants.CHARACTER_CREATION_PART_1)
                .GetComponent<CharacterCreationScreen>();
            CharacterCreationScreen.Initialise();
            GUIManager.OpenGUI(GlobalConstants.CHARACTER_CREATION_PART_1);
        }

        public override void HandleInput()
        {
            base.HandleInput();
        }

        public override void Update()
        {
            base.Update();
        }

        public override GameState GetNextState()
        {
            IEntity player = CharacterCreationScreen.CreatePlayer();
            player.AddExperience(500);
            foreach (string jobName in player.Cultures.SelectMany(culture => culture.Jobs))
            {
                IJob job = GlobalConstants.GameManager.JobHandler.Get(jobName);
                job.AddExperience(300);
                player.AddJob(job);
            }
            
            return new WorldCreationState(new EntityPlayer(player));
        }
    }
}