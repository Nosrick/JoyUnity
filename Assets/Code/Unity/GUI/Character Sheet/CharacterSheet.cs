using DevionGames.UIWidgets;
using JoyLib.Code.Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI.Character_Sheet
{
    public class CharacterSheet : UIWidget
    {
        [SerializeField] protected Image PlayerSprite;
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
            
            Player = GlobalConstants.GameManager.Player;
            Statistics.SetValues(Player.Statistics.Values);
            Skills.SetValues(Player.Skills.Values);
            Abilities.SetValues(Player.Abilities);
            DerivedValues.SetValues(Player.DerivedValues.Values);
            PlayerSprite.sprite = Player.Sprite;
            PlayerName.text = Player.JoyName;
            PlayerJobAndSpecies.text = Player.CreatureType + " " + Player.CurrentJob.Name;
        }
    }
}