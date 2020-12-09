using System;
using System.Collections.Generic;
using JoyLib.Code.Collections;
using JoyLib.Code.Cultures;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.AI;
using JoyLib.Code.Entities.AI.Drivers;
using JoyLib.Code.Entities.AI.LOS.Providers;
using JoyLib.Code.Entities.Gender;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Romance;
using JoyLib.Code.Entities.Sexes;
using JoyLib.Code.Entities.Sexuality;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Quests;
using UnityEngine;

namespace JoyLib.Code.Entities
{
    public interface IEntity : IJoyObject, IItemContainer
    {
        BasicValueContainer<IRollableValue> Statistics { get; }
        BasicValueContainer<IGrowingValue> Skills { get; }
        BasicValueContainer<INeed> Needs { get; }
        List<IAbility> Abilities { get; }
        NonUniqueDictionary<string, IItemInstance> Equipment { get; }
        List<IItemInstance> Backpack { get; }
        IItemInstance NaturalWeapons { get; }
        IBioSex Sex { get; }
        ISexuality Sexuality { get; }
        IRomance Romance { get; }
        
        IGender Gender { get; }
        
        List<string> IdentifiedItems { get; }

        IJob CurrentJob { get; }
        List<IJob> Jobs { get; }
        List<string> Slots { get; }
        List<ICulture> Cultures { get; }
        int Size { get; }
        
        IVision VisionProvider { get; }
        FulfillmentData FulfillmentData { get; }
        NeedAIData CurrentTarget { get; set;  }
        IDriver Driver { get; }
        IPathfinder Pathfinder { get; }
        Queue<Vector2Int> PathfindingData { get; }
        
        Vector2Int TargetPoint { get; set; }
        IAbility TargetingAbility { get; set; }
        
        bool PlayerControlled { get; set; }
        bool Sentient { get; }
        
        int VisionMod { get; }
        
        string CreatureType { get; }
        
        bool HasMoved { get; }
        
        bool Alive { get; }
        
        string Description { get; }

        void Tick();
        void AddQuest(IQuest quest);
        IEnumerable<Tuple<string, int>> GetData(IEnumerable<string> tags, params object[] args);
        void AddIdentifiedItem(string nameRef);
        bool RemoveItemFromPerson(IItemInstance item);
        bool RemoveEquipment(string slot, IItemInstance item = null);
        IItemInstance[] SearchBackpackForItemType(IEnumerable<string> tags);
        bool EquipItem(string slotRef, IItemInstance itemRef);
        IItemInstance GetEquipment(string slotRef);
        bool UnequipItem(string slot);

        void DecreaseMana(int value);
        void IncreaseMana(int value);

        void DecreaseComposure(int value);
        void IncreaseComposure(int value);

        void DecreaseConcentration(int value);
        void IncreaseConcentration(int value);

        void AddExperience(int value);

        bool AddJob(IJob job);
        
        void DamageMe(int value, Entity source);
        void DirectDVModification(int value, string index = EntityDerivedValue.HITPOINTS);
    }
}