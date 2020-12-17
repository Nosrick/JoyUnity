using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevionGames.InventorySystem;
using JoyLib.Code;
using JoyLib.Code.Collections;
using JoyLib.Code.Combat;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Helpers;
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
        private IDerivedValueHandler derivedValueHandler;

        private GameObject inventoryManager;
        private ActionLog logger;

        private IEntity attacker;
        private IEntity defender;

        private IDictionary<string, IRollableValue<int>> attackerStats;
        private IDictionary<string, IRollableValue<int>> defenderStats;

        private IDictionary<string, IEntitySkill> attackerSkills;
        private IDictionary<string, IEntitySkill> defenderSkills;

        private List<IAbility> attackerAbilities;
        private List<IAbility> defenderAbilities;

        private IDictionary<string, IDerivedValue> attackerDVs;
        private IDictionary<string, IDerivedValue> defenderDVs;

        private string[] attackerTags;
        private string[] defenderTags;

        [SetUp]
        public void SetUp()
        {
            this.inventoryManager = new GameObject();
            this.inventoryManager.AddComponent<InventoryManager>();

            this.logger = new ActionLog();
            GlobalConstants.ActionLog = this.logger;
            
            this.target = new CombatEngine(Mock.Of<IRollable>(
                roller => roller.RollSuccesses(
                    It.IsAny<int>(),
                    It.IsAny<int>()) == 1));

            this.needHandler = new NeedHandler();
            this.statisticHandler = new EntityStatisticHandler();
            this.skillHandler = new EntitySkillHandler(this.needHandler);
            this.derivedValueHandler = new DerivedValueHandler(this.statisticHandler, this.skillHandler);
        }

        [SetUp]
        public void InitialiseCollections()
        {
            this.attackerAbilities = new List<IAbility>();
            this.defenderAbilities = new List<IAbility>();

            this.attackerDVs = new Dictionary<string, IDerivedValue>();
            this.defenderDVs = new Dictionary<string, IDerivedValue>();
        }

        public void LiveFireRolling()
        {
            this.target = new CombatEngine();
        }

        public void InitialiseDVs()
        {
            this.attackerDVs.Add(
                ConcreteDerivedIntValue.HITPOINTS, 
                this.derivedValueHandler.Calculate(ConcreteDerivedIntValue.HITPOINTS, this.attackerStats.Values));
            this.defenderDVs.Add(
                ConcreteDerivedIntValue.HITPOINTS,
                this.derivedValueHandler.Calculate(ConcreteDerivedIntValue.HITPOINTS, this.defenderStats.Values));
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
                    && stat.SuccessThreshold == GlobalConstants.DEFAULT_SUCCESS_THRESHOLD),
                ["endurance"] = Mock.Of<IRollableValue<int>>(
                    stat => stat.Name == "endurance"
                            && stat.Value == 3
                            && stat.SuccessThreshold == GlobalConstants.DEFAULT_SUCCESS_THRESHOLD),
                ["cunning"] = Mock.Of<IRollableValue<int>>(
                    stat => stat.Name == "cunning"
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
                            && stat.SuccessThreshold == 6),
                ["endurance"] = Mock.Of<IRollableValue<int>>(
                    stat => stat.Name == "endurance"
                            && stat.Value == 5
                            && stat.SuccessThreshold == 6),
                ["cunning"] = Mock.Of<IRollableValue<int>>(
                    stat => stat.Name == "cunning"
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

            this.InitialiseDVs();
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
                                && stat.SuccessThreshold == 6),
                ["endurance"] = Mock.Of<IRollableValue<int>>(
                    stat => stat.Name == "endurance"
                            && stat.Value == 5
                            && stat.SuccessThreshold == 6),
                ["cunning"] = Mock.Of<IRollableValue<int>>(
                    stat => stat.Name == "cunning"
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
                            && stat.SuccessThreshold == GlobalConstants.DEFAULT_SUCCESS_THRESHOLD),
                ["endurance"] = Mock.Of<IRollableValue<int>>(
                    stat => stat.Name == "endurance"
                            && stat.Value == 3
                            && stat.SuccessThreshold == GlobalConstants.DEFAULT_SUCCESS_THRESHOLD),
                ["cunning"] = Mock.Of<IRollableValue<int>>(
                    stat => stat.Name == "cunning"
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
            
            this.InitialiseDVs();
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
                            && stat.SuccessThreshold == 6),
                ["endurance"] = Mock.Of<IRollableValue<int>>(
                    stat => stat.Name == "endurance"
                            && stat.Value == 5
                            && stat.SuccessThreshold == 6),
                ["cunning"] = Mock.Of<IRollableValue<int>>(
                    stat => stat.Name == "cunning"
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
                            && stat.SuccessThreshold == 6),
                ["endurance"] = Mock.Of<IRollableValue<int>>(
                    stat => stat.Name == "endurance"
                            && stat.Value == 5
                            && stat.SuccessThreshold == 6),
                ["cunning"] = Mock.Of<IRollableValue<int>>(
                    stat => stat.Name == "cunning"
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
            
            this.InitialiseDVs();
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
                          && entity.Equipment == attackerEquipment
                          && entity.Abilities == this.attackerAbilities
                          && entity.DerivedValues == this.attackerDVs);
            this.defender = Mock.Of<IEntity>(
                entity => entity.Statistics == defenderStats
                          && entity.Skills == defenderSkills
                          && entity.Equipment == defenderEquipment
                          && entity.Abilities == this.defenderAbilities
                          && entity.DerivedValues == this.defenderDVs);

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

            Assert.That(results.Sum(), Is.EqualTo(0));
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
                          && entity.Equipment == attackerEquipment
                          && entity.Abilities == this.attackerAbilities
                          && entity.DerivedValues == this.attackerDVs);
            this.defender = Mock.Of<IEntity>(
                entity => entity.Statistics == defenderStats
                          && entity.Skills == defenderSkills
                          && entity.Equipment == defenderEquipment
                          && entity.Abilities == this.defenderAbilities
                          && entity.DerivedValues == this.defenderDVs);

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

            Assert.That(results.Sum(), Is.EqualTo(0));
            yield return null;
        }

        [UnityTest]
        public IEnumerator MakeAttack_AttackerAdvantage_Melee_Equipment()
        {
            //given
            this.EvenMatch();
            this.LiveFireRolling();

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
                          && entity.Equipment == attackerEquipment
                          && entity.Abilities == this.attackerAbilities
                          && entity.DerivedValues == this.attackerDVs);
            this.defender = Mock.Of<IEntity>(
                entity => entity.Statistics == defenderStats
                          && entity.Skills == defenderSkills
                          && entity.Equipment == defenderEquipment
                          && entity.Abilities == this.defenderAbilities
                          && entity.DerivedValues == this.defenderDVs);
            
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
            this.LiveFireRolling();
            
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
                          && entity.Equipment == attackerEquipment
                          && entity.Abilities == this.attackerAbilities
                          && entity.DerivedValues == this.attackerDVs);
            this.defender = Mock.Of<IEntity>(
                entity => entity.Statistics == defenderStats
                          && entity.Skills == defenderSkills
                          && entity.Equipment == defenderEquipment
                          && entity.Abilities == this.defenderAbilities
                          && entity.DerivedValues == this.defenderDVs);

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
            this.AttackerAdvantage();
            this.LiveFireRolling();

            this.attackerTags = new[] {"agility", "strength", "light blades"};
            this.defenderTags = new[] {"agility", "light armour", "evasion"};

            NonUniqueDictionary<string, IItemInstance> attackerEquipment = new NonUniqueDictionary<string, IItemInstance>();
            NonUniqueDictionary<string, IItemInstance> defenderEquipment = new NonUniqueDictionary<string, IItemInstance>();

            IAbility backdraft = new Backdraft();

            this.attackerAbilities.Add(backdraft);
            
            attackerEquipment.Add("hand",
                Mock.Of<IItemInstance>(
                    item => item.Efficiency == 6
                            && item.Tags == new List<string> { "weapon" }));
            
            this.attacker = Mock.Of<IEntity>(
                entity => entity.Statistics == attackerStats
                          && entity.Skills == attackerSkills
                          && entity.Equipment == attackerEquipment
                          && entity.Abilities == this.attackerAbilities
                          && entity.DerivedValues == this.attackerDVs);
            this.defender = Mock.Of<IEntity>(
                entity => entity.Statistics == defenderStats
                          && entity.Skills == defenderSkills
                          && entity.Equipment == defenderEquipment
                          && entity.Abilities == this.defenderAbilities
                          && entity.DerivedValues == this.defenderDVs);

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
        public IEnumerator MakeAttack_DefenderAdvantage_Melee_WithAbilityAndEquipment()
        {
            this.DefenderAdvantage();
            this.LiveFireRolling();

            this.attackerTags = new[] {"agility", "strength", "light blades"};
            this.defenderTags = new[] {"agility", "light armour", "evasion"};

            NonUniqueDictionary<string, IItemInstance> attackerEquipment = new NonUniqueDictionary<string, IItemInstance>();
            NonUniqueDictionary<string, IItemInstance> defenderEquipment = new NonUniqueDictionary<string, IItemInstance>();

            IAbility keenReflexes = new KeenReflexes();
            IAbility uncannyDodge = new UncannyDodge();

            this.defenderAbilities.Add(keenReflexes);
            this.defenderAbilities.Add(uncannyDodge);
            
            attackerEquipment.Add("hand",
                Mock.Of<IItemInstance>(
                    item => item.Efficiency == 6
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
                          && entity.Equipment == attackerEquipment
                          && entity.Abilities == this.attackerAbilities
                          && entity.DerivedValues == this.attackerDVs);
            this.defender = Mock.Of<IEntity>(
                entity => entity.Statistics == defenderStats
                          && entity.Skills == defenderSkills
                          && entity.Equipment == defenderEquipment
                          && entity.Abilities == this.defenderAbilities
                          && entity.DerivedValues == this.defenderDVs);

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
                Assert.That(result, Is.LessThanOrEqualTo(0));
            }

            Assert.That(results.Sum(), Is.LessThan(0));
            
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