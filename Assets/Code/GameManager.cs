using Joy.Code.Managers;
using JoyLib.Code;
using JoyLib.Code.Collections;
using JoyLib.Code.Cultures;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Sexes;
using JoyLib.Code.Entities.Sexuality;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Entities.AI.Drivers;
using JoyLib.Code.Graphics;
using JoyLib.Code.Rollers;
using JoyLib.Code.States;
using System;
using System.Collections.Generic;
using DevionGames.InventorySystem;
using JoyLib.Code.Conversation;
using JoyLib.Code.Conversation.Subengines.Rumours;
using JoyLib.Code.Conversation.Subengines.Rumours.Parameters;
using JoyLib.Code.Entities.Gender;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Entities.Romance;
using JoyLib.Code.Helpers;
using JoyLib.Code.Physics;
using JoyLib.Code.Quests;
using JoyLib.Code.Unity.GUI;
using JoyLib.Code.World;
using UnityEngine;

public class GameManager : MonoBehaviour, IGameManager
{
    protected StateManager m_StateManager;

    // Use this for initialization
	protected void Awake ()
    {
        if (GlobalConstants.GameManager is null)
        {
            GlobalConstants.GameManager = this;
        }

        MyGameObject = this.gameObject;
        
        Roller = new RNG();

        PhysicsManager = new PhysicsManager();

        RelationshipHandler = new EntityRelationshipHandler();
        
        GUIManager = new GUIManager();
        
        AbilityHandler = new AbilityHandler();
        
        MaterialHandler = new MaterialHandler();
        ObjectIconHandler = new ObjectIconHandler(Roller);

        NeedHandler = new NeedHandler();
        CultureHandler = new CultureHandler();
        BioSexHandler = new EntityBioSexHandler();
        SexualityHandler = new EntitySexualityHandler();
        RomanceHandler = new EntityRomanceHandler();
        JobHandler = new JobHandler(Roller);
        GenderHandler = new GenderHandler();
        SkillHandler = new EntitySkillHandler(NeedHandler);
        EntityTemplateHandler = new EntityTemplateHandler(SkillHandler);
        
        WorldInfoHandler = new WorldInfoHandler(ObjectIconHandler);
        
        ParameterProcessorHandler = new ParameterProcessorHandler();
        
        EntityHandler = new LiveEntityHandler();
        ItemHandler = new LiveItemHandler(ObjectIconHandler, MaterialHandler, AbilityHandler, Roller);
        
        EntityFactory = new EntityFactory(
            NeedHandler,
            ObjectIconHandler,
            CultureHandler,
            SexualityHandler,
            BioSexHandler,
            GenderHandler,
            RomanceHandler,
            JobHandler, 
            PhysicsManager,
            Roller);
        
        ItemFactory = new ItemFactory(
            ItemHandler,
            ObjectIconHandler,
            Roller);

        QuestProvider = new QuestProvider(
            RelationshipHandler,
            ItemHandler,
            ItemFactory,
            Roller);
        QuestTracker = new QuestTracker();
        
        ConversationEngine = new ConversationEngine(RelationshipHandler);

        m_StateManager = new StateManager();
        
        m_StateManager.ChangeState(new CharacterCreationState());
    }
	
	// Update is called once per frame
	protected void Update ()
    {
        if(!(m_StateManager is null))
        {
            m_StateManager.Update();
        }
    }

    public void NextState()
    {
        m_StateManager.NextState();
    }
    
    public IQuestTracker QuestTracker { get; protected set; }
    public IQuestProvider QuestProvider { get; protected set; }
    public IEntityRelationshipHandler RelationshipHandler { get; protected set; }
    public IObjectIconHandler ObjectIconHandler { get; protected set; }
    public IMaterialHandler MaterialHandler { get; protected set; }
    public ICultureHandler CultureHandler { get; protected set; }
    public IEntityTemplateHandler EntityTemplateHandler { get; protected set; }
    public IEntityBioSexHandler BioSexHandler { get; protected set; }
    public IEntitySexualityHandler SexualityHandler { get; protected set; }
    public IJobHandler JobHandler { get; protected set; }
    public IEntityRomanceHandler RomanceHandler { get; protected set; }
    public IGenderHandler GenderHandler { get; protected set; }
    public IGUIManager GUIManager { get; protected set; }
    public IParameterProcessorHandler ParameterProcessorHandler { get; protected set; }
    public ILiveEntityHandler EntityHandler { get; protected set; }
    public ILiveItemHandler ItemHandler { get; protected set; }
    public INeedHandler NeedHandler { get; protected set; }
    public IEntitySkillHandler SkillHandler { get; protected set; }
    public IWorldInfoHandler WorldInfoHandler { get; protected set; }
    public IPhysicsManager PhysicsManager { get; protected set; }
    public IConversationEngine ConversationEngine { get; protected set; }
    public IAbilityHandler AbilityHandler { get; protected set; }
    public RNG Roller { get; protected set; }

    public IEntityFactory EntityFactory { get; protected set; }
    public IItemFactory ItemFactory { get; protected set; }
    public GameObject MyGameObject { get; protected set; }

    public IEntity Player { get; set; }
}
