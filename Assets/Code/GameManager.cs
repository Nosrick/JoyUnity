using System.Collections;
using Code.Collections;
using Joy.Code.Managers;
using JoyLib.Code.Combat;
using JoyLib.Code.Conversation;
using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Conversation.Subengines.Rumours;
using JoyLib.Code.Conversation.Subengines.Rumours.Parameters;
using JoyLib.Code.Cultures;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.AI.LOS.Providers;
using JoyLib.Code.Entities.Gender;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Entities.Romance;
using JoyLib.Code.Entities.Sexes;
using JoyLib.Code.Entities.Sexuality;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using JoyLib.Code.Helpers;
using JoyLib.Code.IO;
using JoyLib.Code.Physics;
using JoyLib.Code.Quests;
using JoyLib.Code.Rollers;
using JoyLib.Code.Settings;
using JoyLib.Code.States;
using JoyLib.Code.Unity.GUI;
using JoyLib.Code.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JoyLib.Code
{
    public class GameManager : MonoBehaviour, IGameManager
    {
        protected StateManager m_StateManager;

        protected IGameState NextState { get; set; }

        // Use this for initialization
        protected void Awake()
        {
            if (GlobalConstants.GameManager is null)
            {
                GlobalConstants.GameManager = this;
            }

            SceneManager.sceneLoaded += (arg0, mode) => this.OnSceneLoad();

            DontDestroyOnLoad(this);
            DontDestroyOnLoad(GameObject.Find("WorldHolder"));

            this.LoadingMessage = "Just waking up";
        }

        public IEnumerator Initialise()
        {
            this.BegunInitialisation = true;
            this.LoadingMessage = "Initialising action log";
            this.ActionLog = new ActionLog();

            GlobalConstants.ActionLog = this.ActionLog;

            this.SettingsManager = new SettingsManager();

            this.LoadingMessage = "Initialising object pools";
            GameObject objectHolder = GameObject.Find("WorldObjects");
            GameObject entityHolder = GameObject.Find("WorldEntities");
            GameObject fogHolder = GameObject.Find("WorldFog");
            GameObject wallHolder = GameObject.Find("WorldWalls");
            GameObject floorHolder = GameObject.Find("WorldFloors");

            GameObject prefab = Resources.Load<GameObject>("Prefabs/MonoBehaviourHandler");
            GameObject itemPrefab = Resources.Load<GameObject>("Prefabs/ItemInstance");
            GameObject positionableSprite = Resources.Load<GameObject>("Prefabs/PositionableSprite");
            GameObject fog = Resources.Load<GameObject>("Prefabs/Fog of War");
            this.FloorPool = new GameObjectPool(positionableSprite, floorHolder);
            this.WallPool = new GameObjectPool(prefab, wallHolder);
            this.EntityPool = new GameObjectPool(prefab, entityHolder);
            this.ItemPool = new GameObjectPool(itemPrefab, objectHolder);
            this.FogPool = new GameObjectPool(fog, fogHolder);

            this.MyGameObject = this.gameObject;

            this.Roller = new RNG();

            this.LoadingMessage = "Revving up engines";
            this.CombatEngine = new CombatEngine();

            this.PhysicsManager = new PhysicsManager();

            this.RelationshipHandler = new EntityRelationshipHandler();

            this.AbilityHandler = new AbilityHandler();

            this.MaterialHandler = new MaterialHandler();
            this.ObjectIconHandler = new ObjectIconHandler(this.Roller);

            this.GUIManager = new GUIManager();

            this.VisionProviderHandler = new VisionProviderHandler();

            this.LoadingMessage = "Initialising entity gubbinz";
            this.StatisticHandler = new EntityStatisticHandler();
            this.NeedHandler = new NeedHandler();
            this.CultureHandler = new CultureHandler();
            this.BioSexHandler = new EntityBioSexHandler();
            this.SexualityHandler = new EntitySexualityHandler();
            this.RomanceHandler = new EntityRomanceHandler();
            this.JobHandler = new JobHandler(this.AbilityHandler, this.Roller);
            this.GenderHandler = new GenderHandler();
            this.SkillHandler = new EntitySkillHandler(this.NeedHandler);
            this.EntityTemplateHandler = new EntityTemplateHandler(
                this.SkillHandler,
                this.VisionProviderHandler,
                this.AbilityHandler);

            this.DerivedValueHandler = new DerivedValueHandler(this.StatisticHandler, this.SkillHandler);

            this.WorldInfoHandler = new WorldInfoHandler(this.ObjectIconHandler);

            this.ParameterProcessorHandler = new ParameterProcessorHandler();

            this.EntityHandler = new LiveEntityHandler();
            this.ItemHandler = new LiveItemHandler(this.ObjectIconHandler, this.MaterialHandler, this.AbilityHandler,
                this.Roller);

            this.EntityFactory = new EntityFactory(this.NeedHandler, this.ObjectIconHandler, this.CultureHandler,
                this.SexualityHandler, this.BioSexHandler, this.GenderHandler, this.RomanceHandler, this.JobHandler,
                this.PhysicsManager, this.SkillHandler,
                this.DerivedValueHandler, this.Roller);

            this.ItemFactory = new ItemFactory(this.ItemHandler, this.ObjectIconHandler,
                this.DerivedValueHandler,
                this.ItemPool, this.Roller);

            this.QuestProvider =
                new QuestProvider(this.RelationshipHandler, this.ItemHandler, this.ItemFactory, this.Roller);
            this.QuestTracker = new QuestTracker(this.ItemHandler);

            this.ConversationEngine = new ConversationEngine(this.RelationshipHandler);

            this.NaturalWeaponHelper = new NaturalWeaponHelper(this.MaterialHandler, this.ItemFactory);

            this.m_StateManager = new StateManager();

            this.WorldSerialiser = new WorldSerialiser();

            this.LoadingMessage = "Setting static bad things";
            TradeWindow.RelationshipHandler = this.RelationshipHandler;

            TopicData.ConversationEngine = this.ConversationEngine;
            TopicData.RelationshipHandler = this.RelationshipHandler;

            Entity.QuestTracker = this.QuestTracker;
            Entity.RelationshipHandler = this.RelationshipHandler;
            Entity.SkillHandler = this.SkillHandler;
            Entity.NaturalWeaponHelper = this.NaturalWeaponHelper;
            Entity.DerivedValueHandler = this.DerivedValueHandler;

            ItemInstance.EntityHandler = this.EntityHandler;
            ItemInstance.ItemHandler = this.ItemHandler;

            this.Initialised = true;
            this.LoadingMessage = "Done!";

            yield return null;
        }

        // Update is called once per frame
        protected void Update()
        {
            this.m_StateManager?.Update();
            this.ActionLog.Update();
        }

        public void SetNextState(IGameState nextState = null)
        {
            this.NextState = nextState;
        }

        protected void OnSceneLoad()
        {
            if (this.NextState is null)
            {
                return;
            }

            GlobalConstants.ActionLog.AddText(SceneManager.GetActiveScene().name);
            this.m_StateManager.ChangeState(this.NextState);
        }
        
        public bool BegunInitialisation { get; protected set; }

        public bool Initialised { get; protected set; }

        public int LoadingPercentage { get; protected set; }

        public string LoadingMessage { get; protected set; }

        public WorldSerialiser WorldSerialiser { get; protected set; }
        public ActionLog ActionLog { get; protected set; }
        public ICombatEngine CombatEngine { get; protected set; }
        public IQuestTracker QuestTracker { get; protected set; }
        public IQuestProvider QuestProvider { get; protected set; }
        public IEntityRelationshipHandler RelationshipHandler { get; protected set; }
        public IObjectIconHandler ObjectIconHandler { get; protected set; }
        public IMaterialHandler MaterialHandler { get; protected set; }
        public ICultureHandler CultureHandler { get; protected set; }
        public IEntityStatisticHandler StatisticHandler { get; protected set; }
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
        public IDerivedValueHandler DerivedValueHandler { get; protected set; }
        public IVisionProviderHandler VisionProviderHandler { get; protected set; }

        public NaturalWeaponHelper NaturalWeaponHelper { get; protected set; }
        public RNG Roller { get; protected set; }
        public SettingsManager SettingsManager { get; protected set; }
        public IEntityFactory EntityFactory { get; protected set; }
        public IItemFactory ItemFactory { get; protected set; }
        public GameObject MyGameObject { get; protected set; }

        public IEntity Player => this.EntityHandler.GetPlayer();

        public GameObjectPool FloorPool { get; protected set; }
        public GameObjectPool WallPool { get; protected set; }
        public GameObjectPool EntityPool { get; protected set; }
        public GameObjectPool ItemPool { get; protected set; }
        public GameObjectPool FogPool { get; protected set; }
    }
}