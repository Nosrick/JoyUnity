using System;
using System.Collections;
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
    public class MonoBehaviourHandler : 
        ManagedSprite, 
        IPointerEnterHandler, 
        IPointerExitHandler,
        IDisposable,
        IPosition
    {
        public IJoyObject JoyObject { get; protected set; }
        protected ManagedSprite SpeechBubble { get; set; }
        protected ManagedSprite SpeechBubbleBackground { get; set; }
        protected ParticleSystem ParticleSystem { get; set; }
        protected bool PointerOver { get; set; }
        protected IGUIManager GUIManager { get; set; }
        protected Sprite AttackParticle { get; set; }

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

        public void OnEnable()
        {
            InputSystem.onActionChange -= this.HandleInput;
            InputSystem.onActionChange += this.HandleInput;
            this.GUIManager = GlobalConstants.GameManager?.GUIManager;
            this.AttackParticle = GlobalConstants.GameManager?.ObjectIconHandler.GetFrame(
                "Particles",
                "AttackParticle")
                .m_Parts.First()
                .m_FrameSprites.First();
        }

        public override void SetSpriteLayer(string layerName)
        {
            base.SetSpriteLayer(layerName);
            if (this.SpeechBubble is null == false)
            {
                this.SpeechBubbleBackground.SetSpriteLayer(layerName);
                this.SpeechBubble.SetSpriteLayer(layerName);
                Renderer particleRenderer = this.ParticleSystem.GetComponent<Renderer>();
                particleRenderer.sortingLayerName = layerName;
                particleRenderer.sortingOrder = 0;
            }
        }

        public virtual void AttachJoyObject(IJoyObject joyObject)
        {
            if (this.GUIManager is null)
            {
                this.GUIManager = GlobalConstants.GameManager.GUIManager;
            }

            this.JoyObject = joyObject;
            this.JoyObject.AttachMonoBehaviourHandler(this);
            if (this.JoyObject.MyWorld is null == false)
            {
                this.JoyObject.MyWorld.OnTick -= this.Tick;
                this.JoyObject.MyWorld.OnTick += this.Tick;
            }

            Transform transform = this.transform.Find("Speech Bubble");
            if (transform is null == false)
            {
                var sprites = transform.GetComponents<ManagedSprite>();
                this.SpeechBubble = sprites.First(sprite => sprite.ElementName.Equals("NeedForeground"));
                this.SpeechBubbleBackground =
                    sprites.First(sprite => sprite.ElementName.Equals("NeedBackground"));
                this.SpeechBubbleBackground.Clear();
                this.SpeechBubbleBackground.AddSpriteState(
                    new SpriteState("NeedBackground",
                    GlobalConstants.GameManager.ObjectIconHandler.GetFrame("Needs", "NeedBackground"),
                    AnimationType.Forward,
                    false,
                    false));
            }

            transform = this.transform.Find("Particle System");
            if (transform is null == false)
            {
                this.ParticleSystem = transform.GetComponent<ParticleSystem>();
            }

            this.name = this.JoyObject.JoyName + ":" + this.JoyObject.Guid;
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
            if (on && need is null == false)
            {
                this.SpeechBubble.Clear();
                this.SpeechBubble.AddSpriteState(need);
                Sprite needSprite = need.SpriteData.m_Parts.First(
                        part => part.m_Data.Any(data => data.Equals("need", StringComparison.OrdinalIgnoreCase)))
                    .m_FrameSprites[0];
                this.SetParticleSystem(needSprite);
                
                this.ParticleSystem.Play();
            }
        }

        public virtual void SetParticleSystem(Sprite sprite)
        {
            if (this.ParticleSystem.textureSheetAnimation.spriteCount == 0)
            {
                this.ParticleSystem.textureSheetAnimation.AddSprite(sprite);
            }
            else
            {
                this.ParticleSystem.textureSheetAnimation.SetSprite(0, sprite);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            this.PointerOver = true;

            if (this.GUIManager.RemovesControl())
            {
                return;
            }
            
            if (this.GUIManager.IsActive(GUINames.CONTEXT_MENU) == false
                && GlobalConstants.GameManager.Player.VisionProvider.CanSee(
                    GlobalConstants.GameManager.Player,
                    this.JoyObject.MyWorld,
                    this.JoyObject.WorldPosition))
            {
                this.GUIManager.OpenGUI(GUINames.TOOLTIP).GetComponent<Tooltip>().Show(
                    this.JoyObject.JoyName,
                    null,
                    this.CurrentSpriteState,
                    this.JoyObject.Tooltip);
            }
        }

        protected void Tick(object sender, EventArgs args)
        {
            this.JoyObject?.Tick();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.PointerOver = false;
            this.GUIManager.CloseGUI(GUINames.TOOLTIP);
        }

        protected virtual void OpenContextMenu()
        {
            ContextMenu contextMenu = this.GUIManager.Get(GUINames.CONTEXT_MENU).GetComponent<ContextMenu>();
            contextMenu.Clear();

            IEntity player = GlobalConstants.GameManager.Player;
            if (this.JoyObject.Equals(player) == false
                && player.VisionProvider.CanSee(
                    player,
                    this.JoyObject.MyWorld,
                    this.JoyObject.WorldPosition))
            {
                bool adjacent = AdjacencyHelper.IsAdjacent(player.WorldPosition, this.JoyObject.WorldPosition);
                bool inRange = player.Equipment.Contents.Any(instance =>
                    AdjacencyHelper.IsInRange(
                        player.WorldPosition,
                        this.JoyObject.WorldPosition,
                        instance.ItemType.Range));
                if (this.JoyObject is IEntity)
                {
                    if (adjacent)
                    {
                        contextMenu.AddMenuItem("Talk", this.TalkToPlayer);
                    }
                    else
                    {
                        contextMenu.AddMenuItem("Call Over", this.CallOver);
                    }

                    if (inRange)
                    {
                        contextMenu.AddMenuItem("Attack", this.AttackFromContextMenu);
                    }
                }
            }
            else
            {
                contextMenu.AddMenuItem("Open Inventory", this.OpenInventory);
            }

            if (contextMenu.GetComponentsInChildren<MenuItem>().Length > 0)
            {
                this.GUIManager.CloseGUI(GUINames.TOOLTIP);
                this.GUIManager.OpenGUI(GUINames.CONTEXT_MENU);
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
            this.GUIManager.OpenGUI(GUINames.INVENTORY);
        }

        protected void TalkToPlayer()
        {
            this.GUIManager.CloseGUI(GUINames.CONTEXT_MENU);
            this.GUIManager.OpenGUI(GUINames.CONVERSATION);
            GlobalConstants.GameManager.ConversationEngine.SetActors(
                GlobalConstants.GameManager.Player,
                this.JoyObject as IEntity);

            GlobalConstants.GameManager.ConversationEngine.Converse();
        }

        protected void CallOver()
        {
            this.GUIManager.CloseGUI(GUINames.CONTEXT_MENU);
            this.JoyObject.FetchAction("seekaction")
                .Execute(
                    new[] {this.JoyObject, GlobalConstants.GameManager.Player},
                    new[] {"call over"},
                    new Dictionary<string, object>
                    {
                        {"need", "friendship"}
                    });
        }

        protected void AttackFromContextMenu()
        {
            if (!(this.JoyObject is IEntity defender))
            {
                return;
            }
            
            IEntity player = this.JoyObject.MyWorld.Player;
            int playerAttack = this.Attack(player, defender);

            IEnumerable<IRelationship> relationships =
                GlobalConstants.GameManager.RelationshipHandler.Get(new[] {player, defender});
            int bestRelationship = int.MinValue;
            foreach (IRelationship relationship in relationships)
            {
                relationship.ModifyValueOfOtherParticipants(player.Guid, -50);
                int value = relationship.GetRelationshipValue(defender.Guid, player.Guid);
                if (value > bestRelationship)
                {
                    bestRelationship = value;
                }
            }

            defender.ModifyValue(DerivedValueName.HITPOINTS, -playerAttack);
            if (defender.Alive == false)
            {
                player.MyWorld.RemoveEntity(defender.WorldPosition);
                GlobalConstants.ActionLog.AddText(player.JoyName + " has killed " + defender.JoyName + "!", LogLevel.Gameplay);
            }
            else if (defender.Conscious == false)
            {
                GlobalConstants.ActionLog.AddText(player.JoyName + " has knocked " + defender.JoyName +
                                                  " unconscious!", LogLevel.Gameplay);
            }
            else if (defender.Conscious)
            {
                if (bestRelationship < -50 
                    && (defender.Equipment.Contents.Any(instance => 
                        AdjacencyHelper.IsInRange(
                            defender.WorldPosition, 
                            player.WorldPosition, 
                            instance.ItemType.Range))
                    || AdjacencyHelper.IsAdjacent(defender.WorldPosition, player.WorldPosition)))
                {
                    int defenderAttack = this.Attack(defender, player);
                    if (defenderAttack > 0)
                    {
                        player.ModifyValue(DerivedValueName.HITPOINTS, -defenderAttack);
                    }
                }
            }

            this.JoyObject.MyWorld.Tick();
        }

        protected int Attack(IEntity aggressor, IEntity defender, string type = "physical")
        {
            bool adjacent = AdjacencyHelper.IsAdjacent(aggressor.WorldPosition, defender.WorldPosition);
            List<string> attackerTags = new List<string>
            {
                type,
                "attack"
            };

            attackerTags.Add(adjacent ? "adjacent" : "ranged");

            attackerTags.AddRange(aggressor.Equipment.Contents
                .Where(equipment => equipment.HasTag("weapon") && equipment.HasTag(type))
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
                type
            };

            defenderTags.Add(adjacent ? "adjacent" : "ranged");

            defenderTags.AddRange(defender.Equipment.Contents
                .Where(equipment => equipment.HasTag("armour") && equipment.HasTag(type))
                .SelectMany(equipment => equipment.Tags)
                .Distinct());
            defenderTags = defenderTags.Distinct().ToList();

            defender.MonoBehaviourHandler.SetParticleSystem(this.AttackParticle);
            defender.MonoBehaviourHandler.ParticleSystem.Play();
            defender.MonoBehaviourHandler.FlashMySprite(Color.red, Color.white);
            
            return GlobalConstants.GameManager.CombatEngine.MakeAttack(
                aggressor,
                defender,
                attackerTags,
                defenderTags);
        }

        public void FlashMySprite(Color onColour, Color offColour)
        {
            this.StartCoroutine(this.FlashDelay(true, 0.15f, 2, onColour, offColour));
        }

        protected IEnumerator FlashDelay(bool on, float delay, int cycles, Color onColour, Color offColour)
        {
            foreach (var spritePart in this.SpriteParts)
            {
                spritePart.material.color = on ? onColour : offColour;
            }

            yield return new WaitForSeconds(delay);

            if (cycles > 0)
            {
                yield return this.FlashDelay(!on, delay, on == false ? cycles - 1 : cycles, onColour, offColour);
            }
            else if(on)
            {
                yield return this.FlashDelay(false, delay, 0, onColour, offColour);
            }

            yield return null;
        }

        public void Dispose()
        {
            GarbageMan.Dispose(this.m_States);
            this.m_States = null;
        }

        public Vector2Int WorldPosition => this.JoyObject.WorldPosition;
        
        public void Move(Vector2Int position)
        {
            this.transform.position = new Vector3(position.x, position.y, 0);
            this.JoyObject.Move(position);
        }
    }
}