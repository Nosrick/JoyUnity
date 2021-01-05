using JoyLib.Code.Entities;
using TMPro;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI.Character_Sheet
{
    public class CharacterSheet : GUIData
    {
        [SerializeField] protected ManagedSprite PlayerSprite;
        [SerializeField] protected TextMeshProUGUI PlayerName;
        [SerializeField] protected TextMeshProUGUI PlayerJobAndSpecies;
        [SerializeField] protected StaticDerivedValueDisplay DerivedValues;
        [SerializeField] protected StaticValueDisplay Statistics;
        [SerializeField] protected StaticValueDisplay Skills;
        [SerializeField] protected StaticAbilityDisplay Abilities;
        
        protected IEntity Player;

        public void OnEnable()
        {
            if (GlobalConstants.GameManager is null)
            {
                return;
            }

            this.Player = GlobalConstants.GameManager.Player;
            this.Statistics.SetValues(this.Player.Statistics.Values);
            this.Skills.SetValues(this.Player.Skills.Values);
            this.Abilities.SetValues(this.Player.Abilities);
            this.DerivedValues.SetValues(this.Player.DerivedValues);
            this.PlayerSprite.Clear();
            this.PlayerSprite.AddSpriteState(this.Player.MonoBehaviourHandler.CurrentSpriteState);
            this.PlayerName.text = this.Player.JoyName;
            this.PlayerJobAndSpecies.text = this.Player.CreatureType + " " + this.Player.CurrentJob.Name;
        }

        public override void Close()
        {
            this.GUIManager.CloseGUI(GUINames.TOOLTIP);
            this.GUIManager.CloseGUI(GUINames.CONTEXT_MENU);
            base.Close();
        }
    }
}