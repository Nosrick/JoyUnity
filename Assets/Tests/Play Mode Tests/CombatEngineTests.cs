using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        private List<string> attackerTags;
        private List<string> defenderTags;

        [SetUp]
        public void SetUp()
        {

            this.logger = new ActionLog();
            GlobalConstants.ActionLog = this.logger;
            
            this.target = new CombatEngine(Mock.Of<IRollable>(
                roller => roller.RollSuccesses(
                    It.IsAny<int>(),
                    It.IsIn<int>(7)) == 1
                && roller.RollSuccesses(
                    It.IsAny<int>(),
                    It.IsNotIn(7)) == 2));

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

        public void InitialiseDVs()
        {
            this.attackerDVs = this.derivedValueHandler.GetEntityStandardBlock(this.attackerStats.Values);
            this.defenderDVs = this.derivedValueHandler.GetEntityStandardBlock(this.defenderStats.Values);
        }

        public void SetStatsAndSkills(
            int attackerValue = 3, 
            int attackerThreshold = 6, 
            int defenderValue = 3, 
            int defenderThreshold = 7)
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
            this.attackerTags = new List<string> {"light blades", "strength", "agility", "physical", "attack"};
            this.defenderTags = new List<string> {"agility", "grit", "evasion", "physical", "defend"};
        }

        public void SetMentalTags()
        {
            this.attackerTags = new List<string> {"chaos magic", "intellect",  "mental", "attack"};
            this.defenderTags = new List<string> {"focus", "willpower", "mental", "defend"};
        }

        public void SetSocialTags()
        {
            this.attackerTags = new List<string> {"intimidate", "personality", "social", "attack"};
            this.defenderTags = new List<string> {"poise", "wit", "social", "defend"};
        }

        [UnityTest]
        public IEnumerator MakeAttack_EvenMatch_Physical_NoEquipment()
        {
            this.SetStatsAndSkills(3, 7, 3, 7);
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
            Debug.Log(results.Sum());
            yield return null;
        }

        [UnityTest]
        public IEnumerator MakeAttack_AttackerAdvantage_Physical_Equipment()
        {
            //given
            this.SetStatsAndSkills();
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

            Assert.That(results.Sum(), Is.EqualTo(70));
            Debug.Log(results.Sum());
            
            yield return null;
        }

        [UnityTest]
        public IEnumerator MakeAttack_DefenderAdvantage_Physical_Equipment()
        {
            //given
            this.SetStatsAndSkills();
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

            Assert.That(results.Sum(), Is.EqualTo(0));
            Debug.Log(results.Sum());
            
            yield return null;
        }

        [UnityTest]
        public IEnumerator MakeAttack_AttackerAdvantage_Physical_WithAbilityAndEquipment()
        {
            this.SetStatsAndSkills();
            this.SetPhysicalTags();
            this.SetUpEquipment(
                "physical",
                6,
                1,
                0,
                0);

            IAbility backdraft = new Backdraft();

            this.attackerAbilities.Add(backdraft);
            this.attackerTags.Add("backdraft");
            
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

            Assert.That(results.Sum(), Is.EqualTo(90));
            Debug.Log(results.Sum());
            
            yield return null;
        }

        [UnityTest]
        public IEnumerator MakeAttack_DefenderAdvantage_Physical_WithAbilityAndEquipment()
        {
            this.SetStatsAndSkills();
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
            Debug.Log(results.Sum());
            
            yield return null;
        }

        [UnityTest]
        public IEnumerator MakeAttack_EvenMatch_Physical_NonPhysicalEquipmentNotAdded()
        {
            this.SetStatsAndSkills(3, 7, 3, 7);
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
            Debug.Log(results.Sum());
            
            yield return null;
        }

        [UnityTest]
        public IEnumerator MakeAttack_EvenMatch_Physical_NonPhysicalAbilitiesNotAdded()
        {
            this.SetStatsAndSkills(3, 7, 3, 7);
            this.SetPhysicalTags();
            this.SetUpEquipment(
                "mental",
                0,
                0,
                0,
                0);
            this.attackerAbilities.Add(new Distraction());
            this.attackerTags.Add("distraction");
            this.defenderAbilities.Add(new IronWill());
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
            Debug.Log(results.Sum());
            
            yield return null;
        }

        [UnityTest]
        public IEnumerator MakeAttack_EvenMatch_Mental_NoEquipment()
        {
            this.SetStatsAndSkills(3, 7, 3, 7);
            this.SetMentalTags();
            this.SetUpEquipment(
                "mental",
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
            Debug.Log(results.Sum());

            yield return null;
        }

        [UnityTest]
        public IEnumerator MakeAttack_AttackerAdvantage_Mental_WithEquipment()
        {
            this.SetStatsAndSkills();
            this.SetMentalTags();
            this.SetUpEquipment(
                "mental",
                6,
                1,
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

            Assert.That(results.Sum(), Is.EqualTo(70));
            Debug.Log(results.Sum());
            
            yield return null;
        }

        [UnityTest]
        public IEnumerator MakeAttack_DefenderAdvantage_Mental_WithEquipment()
        {
            this.SetStatsAndSkills();
            this.SetMentalTags();
            this.SetUpEquipment(
                "mental",
                2,
                1,
                6,
                2);
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
            Debug.Log(results.Sum());
            
            yield return null;
        }

        [UnityTest]
        public IEnumerator MakeAttack_AttackerAdvantage_Mental_WithAbilityAndEquipment()
        {
            this.SetStatsAndSkills();
            this.SetMentalTags();
            this.SetUpEquipment(
                "mental",
                6,
                1,
                0,
                0);
            
            this.attackerAbilities.Add(new Distraction());
            this.attackerTags.Add("distraction");
            
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

            Assert.That(results.Sum(), Is.EqualTo(70));
            Debug.Log(results.Sum());
            
            yield return null;
        }

        [UnityTest]
        public IEnumerator MakeAttack_DefenderAdvantage_Mental_WithAbilityAndEquipment()
        {
            this.SetStatsAndSkills();
            this.SetMentalTags();
            this.SetUpEquipment(
                "mental",
                2,
                1,
                6,
                2);
            
            this.defenderAbilities.Add(new IronWill());
            
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
            Debug.Log(results.Sum());
            
            yield return null;
        }

        [UnityTest]
        public IEnumerator MakeAttack_EvenMatch_Social_NoEquipment()
        {
            //given
            this.SetStatsAndSkills(3, 7, 3, 7);
            this.SetUpEquipment("social", 0, 0, 0, 0);
            this.SetSocialTags();
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

            Assert.That(results.Sum(), Is.EqualTo(0));
            Debug.Log(results.Sum());
            
            yield return null;
        }

        [UnityTest]
        public IEnumerator MakeAttack_AttackerAdvantage_Social_Equipment()
        {
            //given
            this.SetStatsAndSkills();
            this.SetUpEquipment("social", 6, 1, 0, 0);
            this.SetSocialTags();
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

            Assert.That(results.Sum(), Is.EqualTo(70));
            Debug.Log(results.Sum());
            
            yield return null;
        }

        [UnityTest]
        public IEnumerator MakeAttack_DefenderAdvantage_Social_Equipment()
        {
            //given
            this.SetStatsAndSkills();
            this.SetUpEquipment("social", 2, 1, 6, 2);
            this.SetSocialTags();
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

            Assert.That(results.Sum(), Is.EqualTo(0));
            Debug.Log(results.Sum());
            
            yield return null;
        }

        [UnityTest]
        public IEnumerator MakeAttack_AttackerAdvantage_Social_WithAbilityAndEquipment()
        {
            //given
            this.SetStatsAndSkills();
            this.SetUpEquipment("social", 6, 1, 0, 0);
            this.SetSocialTags();
            
            this.attackerAbilities.Add(new PiercingGaze());

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

            Assert.That(results.Sum(), Is.EqualTo(70));
            Debug.Log(results.Sum());
            
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator MakeAttack_DefenderAdvantage_Social_WithAbilityAndEquipment()
        {
            //given
            this.SetStatsAndSkills();
            this.SetUpEquipment("social", 2, 1, 6, 2);
            this.SetSocialTags();
            
            this.defenderAbilities.Add(new Indomitable());

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

            Assert.That(results.Sum(), Is.EqualTo(0));
            Debug.Log(results.Sum());
            
            yield return null;
        }

        [TearDown]
        public void TearDown()
        {
            GlobalConstants.ActionLog = null;
        }
    }
}