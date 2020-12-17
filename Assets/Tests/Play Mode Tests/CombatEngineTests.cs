using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevionGames.InventorySystem;
using JoyLib.Code;
using JoyLib.Code.Collections;
using JoyLib.Code.Combat;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Helpers;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class CombatEngineTests
    {
        private ICombatEngine target;

        private GameObject inventoryManager;
        private ActionLog logger;

        private IEntity attacker;
        private IEntity defender;

        private IDictionary<string, IRollableValue<int>> attackerStats;
        private IDictionary<string, IRollableValue<int>> defenderStats;

        private IDictionary<string, IEntitySkill> attackerSkills;
        private IDictionary<string, IEntitySkill> defenderSkills;

        private string[] attackerTags;
        private string[] defenderTags;

        [SetUp]
        public void SetUp()
        {
            this.inventoryManager = new GameObject();
            this.inventoryManager.AddComponent<InventoryManager>();

            this.logger = new ActionLog();
            GlobalConstants.ActionLog = this.logger;
            
            this.target = new CombatEngine();
        }

        public void DefenderAdvantage()
        {
            this.attackerStats = new Dictionary<string, IRollableValue<int>>
            {
                ["strength"] = Mock.Of<IRollableValue<int>>(
                    stat => stat.Name == "strength"
                    && stat.Value == 3
                    && stat.SuccessThreshold == GlobalConstants.DEFAULT_SUCCESS_THRESHOLD),
                ["agility"] = Mock.Of<IRollableValue<int>>(
                    stat => stat.Name == "agility"
                    && stat.Value == 3
                    && stat.SuccessThreshold == GlobalConstants.DEFAULT_SUCCESS_THRESHOLD)
            };
            this.defenderStats = new Dictionary<string, IRollableValue<int>>
            {
                ["strength"] = Mock.Of<IRollableValue<int>>(
                    stat => stat.Name == "strength"
                            && stat.Value == 5
                            && stat.SuccessThreshold == 6),
                ["agility"] = Mock.Of<IRollableValue<int>>(
                    stat => stat.Name == "agility"
                            && stat.Value == 5
                            && stat.SuccessThreshold == 6)
            };

            this.attackerSkills = new Dictionary<string, IEntitySkill>
            {
                ["light blades"] = Mock.Of<IEntitySkill>(
                    skill => skill.Name == "light blades"
                            && skill.Value == 3
                            && skill.SuccessThreshold == GlobalConstants.DEFAULT_SUCCESS_THRESHOLD)
            };
            this.defenderSkills = new Dictionary<string, IEntitySkill>
            {
                ["light armour"] = Mock.Of<IEntitySkill>(
                    skill => skill.Name == "light armour"
                             && skill.Value == 5
                             && skill.SuccessThreshold == 6),
                ["evasion"] = Mock.Of<IEntitySkill>(
                    skill => skill.Name == "evasion"
                             && skill.Value == 5
                             && skill.SuccessThreshold == 6)
            };
        }

        public void AttackerAdvantage()
        {
            this.attackerStats = new Dictionary<string, IRollableValue<int>>
            {
                ["strength"] = Mock.Of<IRollableValue<int>>(
                    stat => stat.Name == "strength"
                            && stat.Value == 5
                            && stat.SuccessThreshold == 6),
                ["agility"] = Mock.Of<IRollableValue<int>>(
                        stat => stat.Name == "agility"
                                && stat.Value == 5
                                && stat.SuccessThreshold == 6)
            };
            this.defenderStats = new Dictionary<string, IRollableValue<int>>
            {
                ["strength"] = Mock.Of<IRollableValue<int>>(
                    stat => stat.Name == "strength"
                            && stat.Value == 3
                            && stat.SuccessThreshold == GlobalConstants.DEFAULT_SUCCESS_THRESHOLD),
                ["agility"] = Mock.Of<IRollableValue<int>>(
                    stat => stat.Name == "agility"
                            && stat.Value == 3
                            && stat.SuccessThreshold == GlobalConstants.DEFAULT_SUCCESS_THRESHOLD)
            };

            this.attackerSkills = new Dictionary<string, IEntitySkill>
            {
                ["light blades"] = Mock.Of<IEntitySkill>(
                    skill => skill.Name == "light blades"
                            && skill.Value == 5
                            && skill.SuccessThreshold == 6)
            };
            this.defenderSkills = new Dictionary<string, IEntitySkill>
            {
                ["light armour"] = Mock.Of<IEntitySkill>(
                    skill => skill.Name == "light armour"
                             && skill.Value == 3
                             && skill.SuccessThreshold == GlobalConstants.DEFAULT_SUCCESS_THRESHOLD),
                ["evasion"] = Mock.Of<IEntitySkill>(
                    skill => skill.Name == "evasion"
                             && skill.Value == 3
                             && skill.SuccessThreshold == GlobalConstants.DEFAULT_SUCCESS_THRESHOLD)
            };
        }

        public void EvenMatch()
        {
            this.attackerStats = new Dictionary<string, IRollableValue<int>>
            {
                ["strength"] = Mock.Of<IRollableValue<int>>(
                    stat => stat.Name == "strength"
                            && stat.Value == 5
                            && stat.SuccessThreshold == 6),
                ["agility"] = Mock.Of<IRollableValue<int>>(
                    stat => stat.Name == "agility"
                            && stat.Value == 5
                            && stat.SuccessThreshold == 6)
            };
            this.defenderStats = new Dictionary<string, IRollableValue<int>>
            {
                ["strength"] = Mock.Of<IRollableValue<int>>(
                    stat => stat.Name == "strength"
                            && stat.Value == 5
                            && stat.SuccessThreshold == 6),
                ["agility"] = Mock.Of<IRollableValue<int>>(
                    stat => stat.Name == "agility"
                            && stat.Value == 5
                            && stat.SuccessThreshold == 6)
            };

            this.attackerSkills = new Dictionary<string, IEntitySkill>
            {
                ["light blades"] = Mock.Of<IEntitySkill>(
                    skill => skill.Name == "light blades"
                             && skill.Value == 5
                             && skill.SuccessThreshold == 6)
            };
            this.defenderSkills = new Dictionary<string, IEntitySkill>
            {
                ["light armour"] = Mock.Of<IEntitySkill>(
                    skill => skill.Name == "light armour"
                             && skill.Value == 3
                             && skill.SuccessThreshold == GlobalConstants.DEFAULT_SUCCESS_THRESHOLD),
                ["evasion"] = Mock.Of<IEntitySkill>(
                    skill => skill.Name == "evasion"
                             && skill.Value == 3
                             && skill.SuccessThreshold == GlobalConstants.DEFAULT_SUCCESS_THRESHOLD)
            };
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
                Mock.Of<IItemInstance>(
                    item => item.Efficiency == 6
                            && item.Tags == new List<string> { "weapon" }));
            
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
                Mock.Of<IItemInstance>(
                    item => item.Efficiency == 2
                    && item.Tags == new List<string> { "weapon" }));
            
            defenderEquipment.Add("torso",
                Mock.Of<IItemInstance>(
                    item => item.Efficiency == 6
                            && item.Tags == new List<string> { "armour" }));
            defenderEquipment.Add("hand",
                Mock.Of<IItemInstance>(
                    item => item.Efficiency == 6
                            && item.Tags == new List<string> { "armour" }));

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

        [UnityTest]
        public IEnumerator MakeAttack_AttackerAdvantage_Melee_WithAbilityAndEquipment()
        {
            
            
            yield return null;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(this.inventoryManager);
            GlobalConstants.ActionLog = null;
        }
    }
}