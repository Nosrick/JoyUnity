using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevionGames.InventorySystem;
using JoyLib.Code.Collections;
using JoyLib.Code.Combat;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using JoyLib.Code.Rollers;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class CombatEngineTests
    {
        private ICombatEngine target;

        private INeedHandler needHandler;
        private IEntityStatisticHandler statisticHandler;
        private IEntitySkillHandler skillHandler;
        private IEntityTemplateHandler templateHandler;

        private IItemFactory itemFactory;
        private IDerivedValueHandler derivedValueHandler;
        private ILiveItemHandler itemHandler;
        private IObjectIconHandler objectIconHandler;
        private IMaterialHandler materialHandler;
        private IAbilityHandler abilityHandler;

        private GameObject inventoryManager;

        private IEntityTemplate template;
        private IEntity attacker;
        private IEntity defender;

        private IDictionary<string, EntityStatistic> attackerStats;
        private IDictionary<string, EntityStatistic> defenderStats;

        private IDictionary<string, EntitySkill> attackerSkills;
        private IDictionary<string, EntitySkill> defenderSkills;

        private string[] attackerTags;
        private string[] defenderTags;

        [SetUp]
        public void SetUp()
        {
            this.inventoryManager = new GameObject();
            this.inventoryManager.AddComponent<InventoryManager>();
            
            this.target = new CombatEngine();
            this.needHandler = new NeedHandler();
            this.statisticHandler = new EntityStatisticHandler();
            this.skillHandler = new EntitySkillHandler(this.needHandler);
            this.templateHandler = new EntityTemplateHandler(this.skillHandler);
            this.template = this.templateHandler.Get("human");

            this.abilityHandler = new AbilityHandler();
            this.objectIconHandler = new ObjectIconHandler(new RNG());
            this.materialHandler = new MaterialHandler();
            this.itemHandler = new LiveItemHandler(
                this.objectIconHandler,
                this.materialHandler,
                this.abilityHandler,
                new RNG());
            this.derivedValueHandler = new DerivedValueHandler(this.statisticHandler, this.skillHandler);

            this.itemFactory = new ItemFactory(
                this.itemHandler, 
                this.objectIconHandler, 
                this.derivedValueHandler,
                new RNG());
        }

        public void DefenderAdvantage()
        {
            this.attackerStats = this.template.Statistics;
            this.defenderStats = this.template.Statistics;

            this.attackerSkills = this.skillHandler.GetDefaultSkillBlock(this.needHandler.Needs);
            this.defenderSkills = this.skillHandler.GetDefaultSkillBlock(this.needHandler.Needs);
            
            foreach (string key in attackerStats.Keys)
            {
                attackerStats[key].SetValue(3);
            }
            foreach (string key in attackerSkills.Keys)
            {
                attackerSkills[key].SetValue(3);
            }

            foreach (string key in defenderStats.Keys)
            {
                defenderStats[key].SetValue(5);
                defenderStats[key].SuccessThreshold = 6;
            }
            foreach (string key in defenderSkills.Keys)
            {
                defenderSkills[key].SetValue(5);
                defenderSkills[key].SuccessThreshold = 6;
            }
        }

        public void AttackerAdvantage()
        {
            this.attackerStats = this.template.Statistics;
            this.defenderStats = this.template.Statistics;

            this.attackerSkills = this.skillHandler.GetDefaultSkillBlock(this.needHandler.Needs);
            this.defenderSkills = this.skillHandler.GetDefaultSkillBlock(this.needHandler.Needs);
            
            foreach (string key in attackerStats.Keys)
            {
                attackerStats[key].SetValue(5);
                attackerStats[key].SuccessThreshold = 6;
            }
            foreach (string key in attackerSkills.Keys)
            {
                attackerSkills[key].SetValue(5);
                attackerSkills[key].SuccessThreshold = 6;
            }

            foreach (string key in defenderStats.Keys)
            {
                defenderStats[key].SetValue(3);
            }
            foreach (string key in defenderSkills.Keys)
            {
                defenderSkills[key].SetValue(3);
            }
        }

        public void EvenMatch()
        {
            this.attackerStats = this.template.Statistics;
            this.defenderStats = this.template.Statistics;

            this.attackerSkills = this.skillHandler.GetDefaultSkillBlock(this.needHandler.Needs);
            this.defenderSkills = this.skillHandler.GetDefaultSkillBlock(this.needHandler.Needs);

            foreach (string key in attackerStats.Keys)
            {
                attackerStats[key].SetValue(5);
            }
            foreach (string key in attackerSkills.Keys)
            {
                attackerSkills[key].SetValue(5);
            }
            
            foreach (string key in defenderStats.Keys)
            {
                defenderStats[key].SetValue(5);
            }
            foreach (string key in defenderSkills.Keys)
            {
                defenderSkills[key].SetValue(5);
            }
        }

        [UnityTest]
        public IEnumerator MakeAttack_DefenderAdvantage_Melee_NoEquipment()
        {
            this.DefenderAdvantage();
            
            NonUniqueDictionary<string, IItemInstance> attackerEquipment = new NonUniqueDictionary<string, IItemInstance>();
            NonUniqueDictionary<string, IItemInstance> defenderEquipment = new NonUniqueDictionary<string, IItemInstance>();
            
            this.attacker = Mock.Of<IEntity>(
                entity => entity.Statistics == attackerStats
                          && entity.Skills == attackerSkills
                          && entity.Equipment == attackerEquipment);
            this.defender = Mock.Of<IEntity>(
                entity => entity.Statistics == defenderStats
                          && entity.Skills == defenderSkills
                          && entity.Equipment == defenderEquipment);

            this.attackerTags = new[] {"agility", "strength", "light blades"};
            this.defenderTags = new[] {"agility", "light armour", "evasion"};

            //given
            List<int> results = new List<int>();
            
            //when
            for (int i = 0; i < 10; i++)
            {
                results.Add(this.target.MakeAttack(this.attacker, this.defender, this.attackerTags, this.defenderTags));
            }

            //then
            foreach (int result in results)
            {
                Assert.That(result, Is.Not.NaN);
            }

            Assert.That(results.Sum(), Is.LessThan(0));
            yield return null;
        }

        [UnityTest]
        public IEnumerator MakeAttack_AttackerAdvantage_Melee_NoEquipment()
        {
            this.AttackerAdvantage();
            
            NonUniqueDictionary<string, IItemInstance> attackerEquipment = new NonUniqueDictionary<string, IItemInstance>();
            NonUniqueDictionary<string, IItemInstance> defenderEquipment = new NonUniqueDictionary<string, IItemInstance>();
            
            this.attacker = Mock.Of<IEntity>(
                entity => entity.Statistics == attackerStats
                          && entity.Skills == attackerSkills
                          && entity.Equipment == attackerEquipment);
            this.defender = Mock.Of<IEntity>(
                entity => entity.Statistics == defenderStats
                          && entity.Skills == defenderSkills
                          && entity.Equipment == defenderEquipment);

            this.attackerTags = new[] {"agility", "strength", "light blades"};
            this.defenderTags = new[] {"agility", "light armour", "evasion"};
            
            //given
            List<int> results = new List<int>();
            
            //when
            for (int i = 0; i < 10; i++)
            {
                results.Add(this.target.MakeAttack(
                    this.attacker, 
                    this.defender, 
                    this.attackerTags, 
                    this.defenderTags));
            }

            //then
            foreach (int result in results)
            {
                Assert.That(result, Is.Not.NaN);
            }

            Assert.That(results.Sum(), Is.GreaterThan(0));
            yield return null;
        }

        [UnityTest]
        public IEnumerator MakeAttack_AttackerAdvantage_Melee_Equipment()
        {
            //given
            this.EvenMatch();

            this.attackerTags = new[] {"agility", "strength", "light blades"};
            this.defenderTags = new[] {"agility", "light armour", "evasion"};

            NonUniqueDictionary<string, IItemInstance> attackerEquipment = new NonUniqueDictionary<string, IItemInstance>();
            NonUniqueDictionary<string, IItemInstance> defenderEquipment = new NonUniqueDictionary<string, IItemInstance>();

            attackerEquipment.Add("hand",
                this.itemFactory.CreateRandomItemOfType(new string[] { "weapon", "two-handed" }, true, null));
            
            this.attacker = Mock.Of<IEntity>(
                entity => entity.Statistics == attackerStats
                          && entity.Skills == attackerSkills
                          && entity.Equipment == attackerEquipment);
            this.defender = Mock.Of<IEntity>(
                entity => entity.Statistics == defenderStats
                          && entity.Skills == defenderSkills
                          && entity.Equipment == defenderEquipment);
            
            List<int> results = new List<int>();
            
            //when
            for (int i = 0; i < 10; i++)
            {
                results.Add(this.target.MakeAttack(
                    this.attacker, 
                    this.defender, 
                    this.attackerTags, 
                    this.defenderTags));
            }
            
            //then
            foreach (int result in results)
            {
                Assert.That(result, Is.Not.NaN);
            }

            Assert.That(results.Sum(), Is.GreaterThan(0));
            
            yield return null;
        }

        [UnityTest]
        public IEnumerator MakeAttack_DefenderAdvantage_Melee_Equipment()
        {
            //given
            this.EvenMatch();
            
            this.attackerTags = new[] {"agility", "strength", "light blades"};
            this.defenderTags = new[] {"agility", "light armour", "evasion"};

            NonUniqueDictionary<string, IItemInstance> attackerEquipment = new NonUniqueDictionary<string, IItemInstance>();
            NonUniqueDictionary<string, IItemInstance> defenderEquipment = new NonUniqueDictionary<string, IItemInstance>();

            attackerEquipment.Add("hand",
                this.itemFactory.CreateRandomItemOfType(new string[] { "weapon", "one-handed" }, true, null));
            
            defenderEquipment.Add("torso",
                this.itemFactory.CreateRandomItemOfType(new string[] { "armour", "torso", "metal" }, true, null));
            defenderEquipment.Add("hand",
                this.itemFactory.CreateRandomItemOfType(new string[] { "shield", "metal" }, true, null));

            this.attacker = Mock.Of<IEntity>(
                entity => entity.Statistics == attackerStats
                          && entity.Skills == attackerSkills
                          && entity.Equipment == attackerEquipment);
            this.defender = Mock.Of<IEntity>(
                entity => entity.Statistics == defenderStats
                          && entity.Skills == defenderSkills
                          && entity.Equipment == defenderEquipment);
            
            List<int> results = new List<int>();
            
            //when
            for (int i = 0; i < 10; i++)
            {
                results.Add(this.target.MakeAttack(
                    this.attacker, 
                    this.defender, 
                    this.attackerTags, 
                    this.defenderTags));
            }
            
            //then
            foreach (int result in results)
            {
                Assert.That(result, Is.Not.NaN);
            }

            Assert.That(results.Sum(), Is.LessThanOrEqualTo(0));
            
            yield return null;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(this.inventoryManager);
        }
    }
}