using JoyLib.Code.Entities;
using JoyLib.Code.Entities.AI;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.States;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.Scripting
{
    public class MoonEntity : MoonObject
    {
        protected Entity m_AssociatedEntity;

        public const int NEED_FULFILMENT_DEFAULT = 3;

        [MoonSharpHidden]
        public MoonEntity(Entity associatedEntity) : base(associatedEntity)
        {
            m_AssociatedEntity = associatedEntity;
        }

        public long GUID()
        {
            return m_AssociatedEntity.GUID;
        }

        public bool CanSee(int x, int y)
        {
            return m_AssociatedEntity.Vision[x, y];
        }

        public void FulfillNeed(string need, int value, int minutes = NEED_FULFILMENT_DEFAULT)
        {
            NeedIndex needIndex = (NeedIndex)Enum.Parse(typeof(NeedIndex), need, true);

            m_AssociatedEntity.FulfillNeed(needIndex, value, minutes);
        }

        public void InfluenceMe(int GUID, int value)
        {
            m_AssociatedEntity.InfluenceMe(GUID, value);
        }

        public int HasRelationship(int GUID)
        {
            return m_AssociatedEntity.HasRelationship(GUID);
        }

        public void RemoveItemFromBackpack(MoonItem item)
        {
            m_AssociatedEntity.RemoveItemFromBackpack(item.GetItemInstance());
            Debug.Log(item.GetItemInstance().DisplayName + " removed from " + this.m_AssociatedEntity.JoyName + "'s backpack.");
        }

        public void AddItemToBackpack(MoonItem item)
        {
            m_AssociatedEntity.AddItem(item.GetItemInstance());
        }

        public void InteractWithItem(MoonItem item)
        {
            item.GetItemInstance().Interact(this.m_AssociatedEntity);
            Debug.Log(m_AssociatedEntity.JoyName + " interacted with " + item.GetName() + " at " + m_AssociatedEntity.WorldPosition.ToString());
        }

        public void RemoveItemFromWorld(MoonItem item)
        {
            m_AssociatedEntity.MyWorld.RemoveObject(item.GetAssociatedObject().WorldPosition);
            Debug.Log(m_AssociatedEntity.JoyName + " removed " + item.GetName() + " from world at " + item.GetAssociatedObject().WorldPosition.ToString());
        }

        public void RemoveItemFromWorld(int x, int y)
        {
            m_AssociatedEntity.MyWorld.RemoveObject(new Vector2Int(x, y));
        }

        public List<MoonItem> SearchBackpack(string itemType)
        {
            List<ItemInstance> items = new List<ItemInstance>();
            foreach(ItemInstance item in m_AssociatedEntity.Backpack)
            {
                if(item.BaseType == itemType)
                {
                    items.Add(item);
                }
            }

            List<MoonItem> moonItems = new List<MoonItem>();
            foreach(ItemInstance item in items)
            {
                moonItems.Add(new MoonItem(item));
            }

            return moonItems;
        }

        public List<MoonItem> SearchForObject(string type)
        {
            List<NeedAIData> items = m_AssociatedEntity.MyWorld.SearchForObjects(m_AssociatedEntity, type, Intent.Interact);

            List<MoonItem> moonItems = new List<MoonItem>();
            foreach(NeedAIData data in items)
            {
                ItemInstance item = (ItemInstance)data.target;
                moonItems.Add(new MoonItem(item));
            }
            return moonItems;
        }

        public List<MoonEntity> SearchForEntity(string type, string sentience)
        {
            List<MoonEntity> entities = m_AssociatedEntity.MyWorld.SearchForEntities(this, type, sentience);

            Debug.Log(m_AssociatedEntity.JoyName + " is searching for entities of type " + type + " of sentience " + sentience);

            return entities;
        }

        public List<MoonEntity> SearchForMate()
        {
            List<MoonEntity> mates = m_AssociatedEntity.MyWorld.SearchForMate(this.m_AssociatedEntity);

            Debug.Log(m_AssociatedEntity.JoyName + " is searching for mates");

            return mates;
        }

        public void Seek(MoonObject moonObject, string need)
        {
            NeedAIData needAIData = new NeedAIData();
            needAIData.intent = Intent.Interact;
            needAIData.searching = false;
            needAIData.target = moonObject.GetAssociatedObject();
            needAIData.targetPoint = new Vector2Int(-1, -1);
            needAIData.need = (NeedIndex)Enum.Parse(typeof(NeedIndex), need, true);

            m_AssociatedEntity.CurrentTarget = needAIData;

            Debug.Log(m_AssociatedEntity.JoyName + " is seeking " + needAIData.target.JoyName);
        }

        public void Wander()
        {
            NeedAIData needAIData = new NeedAIData();
            needAIData.intent = Intent.Interact;
            needAIData.searching = true;

            m_AssociatedEntity.CurrentTarget = needAIData;

            Debug.Log(m_AssociatedEntity.JoyName + " is wandering");
        }

        public void EquipItem(MoonItem item, string slot)
        {
            m_AssociatedEntity.EquipItem(item.GetItemInstance(), slot);
        }

        public void UnequipItem(string slot)
        {
            m_AssociatedEntity.UnequipItem(slot);
        }

        public void DecreaseMana(int value)
        {
            m_AssociatedEntity.DecreaseMana(value);
        }

        public void IncreaseMana(int value)
        {
            m_AssociatedEntity.IncreaseMana(value);
        }

        public int GetMana()
        {
            return m_AssociatedEntity.Mana;
        }

        public int GetManaRemaining()
        {
            return m_AssociatedEntity.ManaRemaining;
        }

        public void DecreaseConcentration(int value)
        {
            m_AssociatedEntity.DecreaseConcentration(value);
        }

        public void IncreaseConcentration(int value)
        {
            m_AssociatedEntity.IncreaseConcentration(value);
        }

        public int GetConcentration()
        {
            return m_AssociatedEntity.Concentration;
        }

        public int GetConcentrationRemaining()
        {
            return m_AssociatedEntity.ConcentrationRemaining;
        }

        public void DecreaseComposure(int value)
        {
            m_AssociatedEntity.DecreaseComposure(value);
        }

        public void IncreaseComposure(int value)
        {
            m_AssociatedEntity.IncreaseComposure(value);
        }

        public int GetComposure()
        {
            return m_AssociatedEntity.Composure;
        }

        public int GetComposureRemaining()
        {
            return m_AssociatedEntity.ComposureRemaining;
        }

        public void AddExperience(float value)
        {
            m_AssociatedEntity.AddExperience(value);
        }

        public void DamageMe(int value, MoonEntity source)
        {
            m_AssociatedEntity.DamageMe(value, source.m_AssociatedEntity);
        }

        public void DirectDamage(int value)
        {
            m_AssociatedEntity.DirectDamage(value);
        }

        public MoonItem GetEquipment(string slot)
        {
            ItemInstance item = m_AssociatedEntity.GetEquipment(slot);

            if(item != null)
            {
                return new MoonItem(item);
            }

            return null;
        }

        public string GetCreatureType()
        {
            return m_AssociatedEntity.CreatureType;
        }

        public Sex GetSex()
        {
            return m_AssociatedEntity.Sex;
        }

        public Sexuality GetSexuality()
        {
            return m_AssociatedEntity.Sexuality;
        }

        public List<MoonEntity> GetRelationships()
        {
            List<MoonEntity> relationships = new List<MoonEntity>();
            foreach(KeyValuePair<long, int> pair in m_AssociatedEntity.Relationships)
            {
                Entity entity = WorldState.EntityHandler.Get(pair.Key);
                MoonEntity moon = new MoonEntity(entity);
                relationships.Add(moon);
            }

            return relationships;
        }

        public MoonEntity GetSpouse()
        {
            Dictionary<long, RelationshipStatus> relationships = m_AssociatedEntity.Family;

            foreach(KeyValuePair<long, RelationshipStatus> relationship in relationships)
            {
                if(relationship.Value == RelationshipStatus.Spouse)
                {
                    return new MoonEntity(WorldState.EntityHandler.Get(relationship.Key));
                }
            }

            return null;
        }

        public string IsFamily(long GUID)
        {
            if(m_AssociatedEntity.Family.ContainsKey(GUID))
            {
                return m_AssociatedEntity.Family[GUID].ToString();
            }
            else
            {
                return RelationshipStatus.Unrelated.ToString();
            }
        }
        
        public int GetStatistic(string statistic)
        {
            try
            {
                StatisticIndex statisticIndex = (StatisticIndex)Enum.Parse(typeof(StatisticIndex), statistic, true);
                return m_AssociatedEntity.Statistics[statisticIndex].Value;
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        public int GetSkill(string skill)
        {
            if(m_AssociatedEntity.Skills.ContainsKey(skill))
            {
                return m_AssociatedEntity.Skills[skill].value;
            }
            else
            {
                return -1;
            }
        }

        public string GetJobName()
        {
            return m_AssociatedEntity.JobName;
        }

        public bool GetSentient()
        {
            return m_AssociatedEntity.Sentient;
        }

        public int GetSize()
        {
            return m_AssociatedEntity.Size;
        }

        public MoonCulture GetCulture()
        {
            return new MoonCulture(m_AssociatedEntity.Culture);
        }
    }
}
