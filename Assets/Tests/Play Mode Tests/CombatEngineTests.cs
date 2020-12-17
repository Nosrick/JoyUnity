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

        private NonUniqueDictionary<string, IItemInstance> attackerEquipment;
        private NonUniqueDictionary<string, IItemInstance> defenderEquipment;

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
            this.attackerDVs = this.derivedValueHandler.GetEntityStandardBlock(this.attackerStats.Values);
            this.defenderDVs = this.derivedValueHandler.GetEntityStandardBlock(this.defenderStats.Values);
        }

        public void SetStatsAndSkills(
            int attackerValue = 3, 
            int attackerThreshold = GlobalConstants.DEFAULT_SUCCESS_THRESHOLD, 
            int defenderValue = 3, 
            int defenderThreshold = GlobalConstants.DEFAULT_SUCCESS_THRESHOLD)
        {
            this.attackerStats = new Dictionary<string, IRollableValue<int>>();
            this.defenderStats = new Dictionary<string, IRollableValue<int>>();
            foreach (string name in this.statisticHandler.StatisticNames)
            {
                this.attackerStats.Add(name, Mock.Of<IRollableValue<int>>(
                    stat => stat.Name == name
                            && stat.Value == attackerValue
                            && stat.SuccessThreshold == attackerThreshold));

                this.defenderStats.Add(name, Mock.Of<IRollableValue<int>>(
                    stat => stat.Name == name
                            && stat.Value == defenderValue
                            && stat.SuccessThreshold == defenderThreshold));
            }

            this.attackerSkills = new Dictionary<string, IEntitySkill>();
            this.defenderSkills = new Dictionary<string, IEntitySkill>();
            foreach (string name in this.skillHandler.SkillsNames)
            {
                this.attackerSkills.Add(name, Mock.Of<IEntitySkill>(
                    skill => skill.Name == name
                             && skill.Value == attackerValue
                             && skill.SuccessThreshold == attackerThreshold));
                this.defenderSkills.Add(name, Mock.Of<IEntitySkill>(
                    skill => skill.Name == name
                             && skill.Value == defenderValue
                             && skill.SuccessThreshold == defenderThreshold));
            }
            
            this.InitialiseDVs();
        }

        public void SetUpEquipment(
            string attackTypeTag, 
            int attackerValue = 2, 
            int attackerQuantity = 1, 
            int defenderValue = 2,
            int defenderQuantity = 1)
        {
            this.attackerEquipment = new NonUniqueDictionary<string, IItemInstance>();
            this.defenderEquipment = new NonUniqueDictionary<string, IItemInstance>();

            for (int i = 0; i < attackerQuantity; i++)
            {
                this.attackerEquipment.Add("hand",
                    Mock.Of<IItemInstance>(
                        item => item.Efficiency == attackerValue
                                && item.Tags == new List<string> { "weapon", attackTypeTag }));
            }

            for (int i = 0; i < defenderQuantity; i++)
            {
                this.defenderEquipment.Add("torso",
                    Mock.Of<IItemInstance>(
                        item => item.Efficiency == defenderValue
                        && item.Tags == new List<string> { "armour", attackTypeTag }));
            }
        }

        public void MakeEntities()
        {
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
        }

        public void SetPhysicalTags()
        {
            this.attackerTags = new[] {"light blades", "strength", "agility", "physical", "attack"};
            this.defenderTags = new[] {"agility", "grit", "evasion", "physical", "defend"};
        }

        public void SetMentalTags()
        {
            this.attackerTags = new[] {"chaos magic", "intellect", "focus", "mental", "attack"};
            this.defenderTags = new[] {"focus", "willpower", "evasion", "mental", "defend"};
        }

        [UnityTest]
        public IEnumerator MakeAttack_DefenderAdvantage_Physical_NoEquipment()
        {
            this.SetStatsAndSkills(3, 7, 5, 6);
            this.SetPhysicalTags();
            this.SetUpEquipment(
                "physical",
                0,
                0,
                0,
                0);
            this.MakeEntities();

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
        public IEnumerator MakeAttack_AttackerAdvantage_Physical_NoEquipment()
        {
            this.SetStatsAndSkills(5, 6, 3, 7);
            this.SetPhysicalTags();
            this.SetUpEquipment(
                "physical",
                0,
                0,
                0,
                0);
            this.MakeEntities();
            
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
        public IEnumerator MakeAttack_AttackerAdvantage_Physical_Equipment()
        {
            //given
            this.SetStatsAndSkills(5, 6, 5, 6);
            this.LiveFireRolling();
            this.SetPhysicalTags();
            this.SetUpEquipment(
                "physical",
                6,
                1,
                0,
                0);
            this.MakeEntities();
            
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
        public IEnumerator MakeAttack_DefenderAdvantage_Physical_Equipment()
        {
            //given
            this.SetStatsAndSkills(3, 7, 5, 6);
            this.LiveFireRolling();
            this.SetPhysicalTags();
            this.SetUpEquipment(
                "physical",
                2,
                1,
                6,
                2);
            this.MakeEntities();

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
        public IEnumerator MakeAttack_AttackerAdvantage_Physical_WithAbilityAndEquipment()
        {
            this.SetStatsAndSkills(5, 6, 3, 7);
            this.LiveFireRolling();
            this.SetPhysicalTags();
            this.SetUpEquipment(
                "physical",
                6,
                1,
                0,
                0);

            IAbility backdraft = new Backdraft();

            this.attackerAbilities.Add(backdraft);
            this.MakeEntities();

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
        public IEnumerator MakeAttack_DefenderAdvantage_Physical_WithAbilityAndEquipment()
        {
            this.SetStatsAndSkills(3, 7, 5, 6);
            this.LiveFireRolling();
            this.SetPhysicalTags();
            this.SetUpEquipment(
                "physical",
                2,
                1,
                6,
                2);

            IAbility keenReflexes = new KeenReflexes();
            IAbility uncannyDodge = new UncannyDodge();

            this.defenderAbilities.Add(keenReflexes);
            this.defenderAbilities.Add(uncannyDodge);
            this.MakeEntities();

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

            Assert.That(results.Sum(), Is.LessThanOrEqualTo(0));
            
            yield return null;
        }

        [UnityTest]
        public IEnumerator MakeAttack_EvenMatch_Physical_NonPhysicalEquipmentNotAdded()
        {
            this.SetStatsAndSkills();
            this.SetPhysicalTags();
            this.SetUpEquipment(
                "mental",
                2,
                2,
                0,
                0);
            this.MakeEntities();

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
                Assert.That(result, Is.EqualTo(0));
            }

            Assert.That(results.Sum(), Is.EqualTo(0));
            
            yield return null;
        }

        [UnityTest]
        public IEnumerator MakeAttack_DefenderAdvantage_Mental_NoEquipment()
        {
            this.SetStatsAndSkills(3, 7, 5, 6);
            this.SetMentalTags();
            this.SetUpEquipment(
                "mental",
                0,
                0,
                0,
                0);
            
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

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(this.inventoryManager);
            GlobalConstants.ActionLog = null;
        }
    }
}