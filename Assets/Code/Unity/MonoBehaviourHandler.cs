using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using JoyLib.Code.Helpers;
using JoyLib.Code.Unity.GUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using ContextMenu = JoyLib.Code.Unity.GUI.ContextMenu;

namespace JoyLib.Code.Unity
{
    public class MonoBehaviourHandler : ManagedSprite, IPointerEnterHandler, IPointerExitHandler
    {
        public IJoyObject JoyObject { get; protected set; }
        protected ManagedSprite SpeechBubble { get; set; }
        protected bool PointerOver { get; set; }

        protected static IGUIManager GUIManager { get; set; }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (this.JoyObject == null)
            {
                return;
            }

            this.JoyObject.Update();
            this.transform.position = new Vector3(this.JoyObject.WorldPosition.x, this.JoyObject.WorldPosition.y);
        }

        public void Start()
        {
            InputSystem.onActionChange -= this.HandleInput;
            InputSystem.onActionChange += this.HandleInput;
        }

        public override void SetSpriteLayer(string layerName)
        {
            base.SetSpriteLayer(layerName);
            if (this.SpeechBubble is null == false)
            {
                this.SpeechBubble.SetSpriteLayer(layerName);
            }
        }

        public virtual void AttachJoyObject(IJoyObject joyObject)
        {
            if (GUIManager is null)
            {
                GUIManager = GlobalConstants.GameManager.GUIManager;
            }

            this.JoyObject = joyObject;
            this.JoyObject.AttachMonoBehaviourHandler(this);
            Transform transform = this.transform.Find("Speech Bubble");
            if (transform is null == false)
            {
                this.SpeechBubble = transform.GetComponent<ManagedSprite>();
            }

            this.name = this.JoyObject.JoyName + ":" + this.JoyObject.GUID;
            this.transform.position = new Vector3(this.JoyObject.WorldPosition.x, this.JoyObject.WorldPosition.y, 0.0f);
            this.SetSpeechBubble(false);
        }

        public void SetSpeechBubble(bool on, ISpriteState need = null)
        {
            if (this.SpeechBubble is null)
            {
                return;
            }

            this.SpeechBubble.gameObject.SetActive(on);
            if (on)
            {
                this.SpeechBubble.Clear();
                this.SpeechBubble.AddSpriteState(need, true);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            this.PointerOver = true;

            if (GUIManager.RemovesControl())
            {
                return;
            }
            
            if (GUIManager.IsActive(GUINames.CONTEXT_MENU) == false
                && GlobalConstants.GameManager.Player.VisionProvider.CanSee(
                    GlobalConstants.GameManager.Player,
                    this.JoyObject.MyWorld,
                    this.JoyObject.WorldPosition))
            {
                GUIManager.OpenGUI(GUINames.TOOLTIP).GetComponent<Tooltip>().Show(
                    this.JoyObject.JoyName,
                    null,
                    this.CurrentSpriteState,
                    this.JoyObject.Tooltip);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.PointerOver = false;
            GUIManager.CloseGUI(GUINames.TOOLTIP);
        }

        protected virtual void OpenContextMenu()
        {
            ContextMenu contextMenu = GUIManager.GetGUI(GUINames.CONTEXT_MENU).GetComponent<ContextMenu>();
            contextMenu.Clear();

            if (this.JoyObject.Equals(GlobalConstants.GameManager.Player) == false
                && GlobalConstants.GameManager.Player.VisionProvider.CanSee(
                    GlobalConstants.GameManager.Player,
                    this.JoyObject.MyWorld,
                    this.JoyObject.WorldPosition))
            {
                bool adjacent = AdjacencyHelper.IsAdjacent(
                    this.JoyObject.WorldPosition,
                    GlobalConstants.GameManager.Player.WorldPosition);
                if (this.JoyObject is IEntity)
                {
                    if (adjacent)
                    {
                        contextMenu.AddMenuItem("Talk", this.TalkToPlayer);
                        contextMenu.AddMenuItem("Attack", this.Attack);
                    }
                    else
                    {
                        contextMenu.AddMenuItem("Call Over", this.CallOver);
                    }
                }
            }
            else
            {
                contextMenu.AddMenuItem("Open Inventory", this.OpenInventory);
            }

            if (contextMenu.GetComponentsInChildren<MenuItem>().Length > 0)
            {
                GUIManager.OpenGUI(GUINames.CONTEXT_MENU);
                contextMenu.Show();
            }
        }

        protected virtual void HandleInput(object data, InputActionChange change)
        {
            if (this.PointerOver == false)
            {
                return;
            }

            if (change != InputActionChange.ActionPerformed)
            {
                return;
            }

            if (!(data is InputAction action))
            {
                return;
            }

            if (action.name.Equals("open context menu", StringComparison.OrdinalIgnoreCase))
            {
                this.OpenContextMenu();
            }
        }

        protected void OpenInventory()
        {
            GUIManager.OpenGUI(GUINames.INVENTORY);
        }

        protected void TalkToPlayer()
        {
            GUIManager.CloseGUI(GUINames.CONTEXT_MENU);
            GUIManager.OpenGUI(GUINames.CONVERSATION);
            GlobalConstants.GameManager.ConversationEngine.SetActors(
                GlobalConstants.GameManager.Player,
                this.JoyObject as IEntity);

            GlobalConstants.GameManager.ConversationEngine.Converse();
        }

        protected void CallOver()
        {
            GUIManager.CloseGUI(GUINames.CONTEXT_MENU);
            this.JoyObject.FetchAction("seekaction")
                .Execute(
                    new IJoyObject[] {this.JoyObject, GlobalConstants.GameManager.Player},
                    new[] {"call over"},
                    new object[] {"friendship"});
        }

        protected void Attack()
        {
            IEntity player = this.JoyObject.MyWorld.Player;
            IEntity defender = this.JoyObject as IEntity;
            int playerAttack = this.PlayerAttack(player);

            IEnumerable<IRelationship> relationships =
                GlobalConstants.GameManager.RelationshipHandler.Get(new[] {player, defender});
            int bestRelationship = int.MinValue;
            foreach (IRelationship relationship in relationships)
            {
                relationship.ModifyValueOfOtherParticipants(player.GUID, -50);
                int value = relationship.GetRelationshipValue(defender.GUID, player.GUID);
                if (value > bestRelationship)
                {
                    bestRelationship = value;
                }
            }

            defender.ModifyValue(DerivedValueName.HITPOINTS, -playerAttack);
            if (defender.Alive == false)
            {
                player.MyWorld.RemoveEntity(defender.WorldPosition);
                GlobalConstants.ActionLog.AddText(player.JoyName + " has killed " + defender.JoyName + "!");
            }
            else if (defender.Conscious == false)
            {
                GlobalConstants.ActionLog.AddText(player.JoyName + " has knocked " + defender.JoyName +
                                                  " unconscious!");
            }
            else if (defender.Conscious)
            {
                if (bestRelationship < -50)
                {
                    int defenderAttack = this.MyAttack(player);
                    if (defenderAttack > 0)
                    {
                        player.ModifyValue(DerivedValueName.HITPOINTS, -defenderAttack);
                    }
                }
            }

            this.JoyObject.MyWorld.Tick();
        }

        protected int PlayerAttack(IEntity player)
        {
            IEntity defender = this.JoyObject as IEntity;

            List<string> attackerTags = new List<string>
            {
                "physical",
                "attack"
            };
            attackerTags.AddRange(player.Equipment.Contents
                .Where(equipment => equipment.HasTag("weapon") && equipment.HasTag("physical"))
                .SelectMany(equipment => equipment.Tags)
                .Distinct());
            if (attackerTags.Count == 2)
            {
                attackerTags.AddRange(new[]
                {
                    "martial arts",
                    "agility",
                    "strength"
                });
            }

            attackerTags = attackerTags.Distinct().ToList();

            List<string> defenderTags = new List<string>
            {
                "evasion",
                "agility",
                "defend",
                "physical"
            };
            defenderTags.AddRange(defender.Equipment.Contents
                .Where(equipment => equipment.HasTag("armour") && equipment.HasTag("physical"))
                .SelectMany(equipment => equipment.Tags)
                .Distinct());
            defenderTags = defenderTags.Distinct().ToList();
            return GlobalConstants.GameManager.CombatEngine.MakeAttack(
                player,
                defender,
                attackerTags,
                defenderTags);
        }

        protected int MyAttack(IEntity player)
        {
            IEntity myself = this.JoyObject as IEntity;

            List<string> attackerTags = new List<string>
            {
                "physical",
                "attack"
            };
            attackerTags.AddRange(myself.Equipment.Contents
                .Where(equipment => equipment.HasTag("weapon") && equipment.HasTag("physical"))
                .SelectMany(equipment => equipment.Tags)
                .Distinct());

            if (attackerTags.Count == 2)
            {
                attackerTags.AddRange(new[]
                {
                    "agility",
                    "strength",
                    "martial arts"
                });
            }

            List<string> defenderTags = new List<string>
            {
                "evasion",
                "agility",
                "physical",
                "defend"
            };
            defenderTags.AddRange(player.Equipment.Contents
                .Where(equipment => equipment.HasTag("armour") && equipment.HasTag("physical"))
                .SelectMany(equipment => equipment.Tags)
                .Distinct());
            defenderTags = defenderTags.Distinct().ToList();
            return GlobalConstants.GameManager.CombatEngine.MakeAttack(
                myself,
                player,
                attackerTags,
                defenderTags);
        }
    }
}