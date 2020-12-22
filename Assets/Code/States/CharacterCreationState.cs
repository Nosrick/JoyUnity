using System.Linq;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Unity.GUI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

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
            SetUpUi();
        }

        public override void Stop()
        {
        }

        public override void LoadContent()
        {
        }

        public override void SetUpUi()
        {
            base.SetUpUi();
            CharacterCreationScreen = GUIManager
                .GetGUI(GUINames.CHARACTER_CREATION_PART_1)
                .GetComponent<CharacterCreationScreen>();
            CharacterCreationScreen.Initialise();
            GUIManager.OpenGUI(GUINames.CHARACTER_CREATION_PART_1);
        }

        public override void HandleInput(InputValue inputValue)
        {
        }

        public override void Update()
        {
        }

        public override GameState GetNextState()
        {
            IEntity player = this.CharacterCreationScreen.CreatePlayer();
            player.AddExperience(500);
            foreach (string jobName in player.Cultures.SelectMany(culture => culture.Jobs))
            {
                IJob job = GlobalConstants.GameManager.JobHandler.Get(jobName);
                job.AddExperience(300);
                player.AddJob(job);
            }
            
            return new WorldCreationState(player);
        }
    }
}