﻿using System.Collections;
using System.Collections.Generic;
using JoyLib.Code;
using JoyLib.Code.Collections;
using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Conversation.Subengines.Rumours;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Gender;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using JoyLib.Code.Helpers;
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
            ActionLog actionLog = new ActionLog();
            GlobalConstants.ActionLog = actionLog;
            scriptingEngine = new ScriptingEngine();
            
            target = new ConcreteRumourMill();

            IWorldInstance world = Mock.Of<IWorldInstance>();

            IGameManager gameManager = Mock.Of<IGameManager>(
                manager => manager.NeedHandler == Mock.Of<INeedHandler>(
                               handler => handler.GetManyRandomised(It.IsAny<IEnumerable<string>>())
                               == new List<INeed>())
                && manager.SkillHandler == Mock.Of<IEntitySkillHandler>(
                    handler => handler.GetDefaultSkillBlock(It.IsAny<IEnumerable<INeed>>())
                    == new Dictionary<string, IEntitySkill>
                    {
                        {
                            "light blades", 
                            new EntitySkill(
                                "light blades", 
                                5, 
                                7, 
                                new NonUniqueDictionary<INeed, float>())
                        }
                    })
                && manager.RelationshipHandler == Mock.Of<IEntityRelationshipHandler>()
                           && manager.ObjectIconHandler == Mock.Of<IObjectIconHandler>());

            GlobalConstants.GameManager = gameManager;

            IGender gender = Mock.Of<IGender>(
                g => g.PersonalSubject == "her");

            IDictionary<string, IEntitySkill> skills = gameManager.SkillHandler.GetDefaultSkillBlock(
                gameManager.NeedHandler.GetManyRandomised(gameManager.NeedHandler.NeedNames));

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
            GlobalConstants.GameManager = null;
            GlobalConstants.ActionLog.Dispose();
        }
    }
}