using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevionGames.InventorySystem;
using JoyLib.Code.Combat;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Statistics;
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

        private GameObject inventoryManager;

        private IEntityTemplate template;
        private IEntity attacker;
        private IEntity defender;

        private string[] attackerTags;
        private string[] defenderTags;

        [SetUp]
        public void SetUpEntities()
        {
            this.inventoryManager = new GameObject();
            this.inventoryManager.AddComponent<InventoryManager>();
            
            this.target = new CombatEngine();
            this.needHandler = new NeedHandler();
            this.statisticHandler = new EntityStatisticHandler();
            this.skillHandler = new EntitySkillHandler(this.needHandler);
            this.templateHandler = new EntityTemplateHandler(this.skillHandler);
            this.template = this.templateHandler.Get("human");
        }

        [UnityTest]
        public IEnumerator MakeAttack_With_DefenderAdvantageMelee()
        {
            IDictionary<string, EntityStatistic> attackerStats = this.template.Statistics;
            IDictionary<string, EntityStatistic> defenderStats = this.template.Statistics;

            IDictionary<string, EntitySkill> attackerSkills = this.skillHandler.GetDefaultSkillBlock(this.needHandler.Needs);
            IDictionary<string, EntitySkill> defenderSkills = this.skillHandler.GetDefaultSkillBlock(this.needHandler.Needs);
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

            this.attacker = Mock.Of<IEntity>(
                entity => entity.Statistics == attackerStats
                          && entity.Skills == attackerSkills);
            this.defender = Mock.Of<IEntity>(
                entity => entity.Statistics == defenderStats
                          && entity.Skills == defenderSkills);

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
        public IEnumerator MakeAttack_With_AttackerAdvantageMelee()
        {
            IDictionary<string, EntityStatistic> attackerStats = this.template.Statistics;
            IDictionary<string, EntityStatistic> defenderStats = this.template.Statistics;

            IDictionary<string, EntitySkill> attackerSkills = this.skillHandler.GetDefaultSkillBlock(this.needHandler.Needs);
            IDictionary<string, EntitySkill> defenderSkills = this.skillHandler.GetDefaultSkillBlock(this.needHandler.Needs);
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

            this.attacker = Mock.Of<IEntity>(
                entity => entity.Statistics == attackerStats
                          && entity.Skills == attackerSkills);
            this.defender = Mock.Of<IEntity>(
                entity => entity.Statistics == defenderStats
                          && entity.Skills == defenderSkills);

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

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(this.inventoryManager);
        }
    }
}