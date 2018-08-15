using JoyLib.Code.Entities;
using JoyLib.Code.Entities.AI;
using JoyLib.Code.Entities.Items;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.Scripting
{
    public class MoonEntity
    {
        protected Entity m_AssociatedEntity;

        public const int NEED_FULFILMENT_DEFAULT = 3;

        [MoonSharpHidden]
        public MoonEntity(Entity associatedEntity)
        {
            m_AssociatedEntity = associatedEntity;
        }

        public void FulfillNeed(string need, int value, int minutes = NEED_FULFILMENT_DEFAULT)
        {
            NeedIndex needIndex = (NeedIndex)Enum.Parse(typeof(NeedIndex), need);

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
            m_AssociatedEntity.RemoveItemFromBackpack(item.ItemInstance);
            Debug.Log(item.ItemInstance.DisplayName + " removed from " + this.m_AssociatedEntity.JoyName + " backpack.");
        }

        public void AddItemToBackpack(MoonItem item)
        {
            m_AssociatedEntity.AddItem(item.ItemInstance);
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

        public List<MoonItem> SearchSurroundingsForObject(string type)
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

        public void Seek(JoyObject joyObject)
        {
            NeedAIData needAIData = new NeedAIData();
            needAIData.intent = Intent.Interact;
            needAIData.searching = false;
            needAIData.target = joyObject;
            needAIData.targetPoint = joyObject.WorldPosition;

            m_AssociatedEntity.CurrentTarget = needAIData;
        }

        public void Wander()
        {
            NeedAIData needAIData = new NeedAIData();
            needAIData.intent = Intent.Interact;
            needAIData.searching = true;

            m_AssociatedEntity.CurrentTarget = needAIData;
        }

        public void EquipItem(MoonItem item, string slot)
        {
            m_AssociatedEntity.EquipItem(item.ItemInstance, slot);
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

        public int GetHitPoints()
        {
            return m_AssociatedEntity.HitPoints;
        }

        public int GetHitPointsRemaining()
        {
            return m_AssociatedEntity.HitPointsRemaining;
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

        public RelationshipStatus IsFamily(int GUID)
        {
            if(m_AssociatedEntity.Family.ContainsKey(GUID))
            {
                return m_AssociatedEntity.Family[GUID];
            }
            else
            {
                return RelationshipStatus.Unrelated;
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
    }
}
