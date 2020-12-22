using Code.Collections;
using JoyLib.Code.Combat;
using JoyLib.Code.Conversation;
using JoyLib.Code.Conversation.Subengines.Rumours;
using JoyLib.Code.Cultures;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Abilities;
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
using JoyLib.Code.Physics;
using JoyLib.Code.Quests;
using JoyLib.Code.Rollers;
using JoyLib.Code.Unity;
using JoyLib.Code.Unity.GUI;
using JoyLib.Code.World;
using UnityEngine;

namespace JoyLib.Code
{
    public interface IGameManager
    {
        ActionLog ActionLog { get; }
        ICombatEngine CombatEngine { get; }
        IQuestTracker QuestTracker { get; }
        IQuestProvider QuestProvider { get; }
        IEntityRelationshipHandler RelationshipHandler { get; }
        IEntityStatisticHandler StatisticHandler { get; }
        IMaterialHandler MaterialHandler { get; }
        IObjectIconHandler ObjectIconHandler { get; }
        ICultureHandler CultureHandler { get; }
        IEntityTemplateHandler EntityTemplateHandler { get; }
        IEntityBioSexHandler BioSexHandler { get; }
        IEntitySexualityHandler SexualityHandler { get; }
        IGenderHandler GenderHandler { get; }
        IJobHandler JobHandler { get; }
        IEntityRomanceHandler RomanceHandler { get; }
        IGUIManager GUIManager { get; }
        IParameterProcessorHandler ParameterProcessorHandler { get; }
        ILiveEntityHandler EntityHandler { get; }
        ILiveItemHandler ItemHandler { get; }
        INeedHandler NeedHandler { get; }
        IEntitySkillHandler SkillHandler { get; }
        IWorldInfoHandler WorldInfoHandler { get; }
        IPhysicsManager PhysicsManager { get; }
        IConversationEngine ConversationEngine { get; }
        IAbilityHandler AbilityHandler { get; }
        IDerivedValueHandler DerivedValueHandler { get; }
        
        NaturalWeaponHelper NaturalWeaponHelper { get; }
        
        RNG Roller { get; }
        
        IEntityFactory EntityFactory { get; }
        IItemFactory ItemFactory { get; }
        
        GameObject MyGameObject { get; }
        
        IEntity Player { get; set; }
        PlayerInputHandler PlayerInputHandler { get; }
        GameObjectPool FloorPool { get; }
        GameObjectPool WallPool { get; }
        GameObjectPool EntityPool { get; }
        GameObjectPool ItemPool { get; }
        GameObjectPool FogPool { get; }
    }
}