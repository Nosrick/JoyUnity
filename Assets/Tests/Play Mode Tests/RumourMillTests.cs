using System.Collections;
using System.Collections.Generic;
using DevionGames.InventorySystem;
using JoyLib.Code;
using JoyLib.Code.Collections;
using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Conversation.Subengines.Rumours;
using JoyLib.Code.Conversation.Subengines.Rumours.Parameters;
using JoyLib.Code.Cultures;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.AI.Drivers;
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
using JoyLib.Code.Rollers;
using JoyLib.Code.Scripting;
using JoyLib.Code.World;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class RumourMillTests
    {
        private IRumourMill target;
        
        private ScriptingEngine scriptingEngine;
        
        private IEntityRelationshipHandler RelationshipHandler;
        
        private IEntity left;
        private IEntity right;

        [SetUp]
        public void SetUp()
        {
            scriptingEngine = new ScriptingEngine();
            
            target = new ConcreteRumourMill();

            IWorldInstance world = Mock.Of<IWorldInstance>();
            
            INeedHandler needHandler = new NeedHandler();
            IEntitySkillHandler skillHandler = new EntitySkillHandler(needHandler);

            IGameManager gameManager = Mock.Of<IGameManager>(
                manager => manager.NeedHandler == needHandler
                && manager.SkillHandler == skillHandler);

            IGender gender = Mock.Of<IGender>(
                g => g.PersonalSubject == "her");

            BasicValueContainer<IGrowingValue> skills = skillHandler.GetDefaultSkillBlock(
                new BasicValueContainer<INeed>(
                    needHandler.GetManyRandomised(needHandler.NeedNames)));

            left = Mock.Of<IEntity>(
                entity => entity.PlayerControlled == true
                && entity.JoyName == "TEST1"
                && entity.Skills == skills);

            right = Mock.Of<IEntity>(
                entity => entity.JoyName == "TEST2"
                          && entity.Skills == skills);
        }

        [UnityTest]
        public IEnumerator RumourMill_ShouldHave_NonZeroRumourTypeCount()
        {
            //given
            
            //when
            
            //then
            Assert.That(target.RumourTypes, Is.Not.Empty);

            return null;
        }

        [UnityTest]
        public IEnumerator RumourMill_ShouldMake_ValidRumours()
        {
            //given
            IRumour[] rumours = target.GenerateOneRumourOfEachType(new IJoyObject[] {left, right});

            //then
            foreach (IRumour rumour in rumours)
            {
                Assert.That(rumour.Words, Does.Not.Contain("<"));
                Assert.That(rumour.Words, Does.Not.Contain("PARAMETER NUMBER MISMATCH"));
                Debug.Log(rumour.Words);
            }

            return null;
        }

        [TearDown]
        public void TearDown()
        {
        }
    }
}