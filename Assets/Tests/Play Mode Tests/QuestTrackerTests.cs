using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JoyLib.Code;
using JoyLib.Code.Collections;
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
using JoyLib.Code.Quests;
using JoyLib.Code.Rollers;
using JoyLib.Code.Scripting;
using JoyLib.Code.World;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class QuestTrackerTests
    {
        private IQuestTracker target;

        private IQuestProvider questProvider;
        
        private ScriptingEngine scriptingEngine;

        private INeedHandler NeedHandler;
        private IEntitySkillHandler SkillHandler;

        private WorldInstance world;
        
        private IEntity left;
        private IEntity right;
        
        [SetUp]
        public void SetUp()
        {
            scriptingEngine = new ScriptingEngine();
            
            target = new QuestTracker();
        }
        
        [SetUp]
        public void SetUpEntities()
        {
            left = Mock.Of<IEntity>(
                entity => entity.GUID == 1
                && entity.JoyName == "TEST1"
                && entity.PlayerControlled == true);

            right = Mock.Of<IEntity>(
                entity => entity.JoyName == "TEST2"
                && entity.GUID == 2);
        }

        [UnityTest]
        public IEnumerator QuestTracker_Should_AddQuest()
        {
            //given
            IQuest quest = Mock.Of<IQuest>();
            
            //when
            target.AddQuest(left.GUID, quest);

            //then
            Assert.That(target.GetQuestsForEntity(left.GUID), Is.Not.Empty);

            return null;
        }

        [UnityTest]
        public IEnumerator QuestTracker_Should_AdvanceOrCompleteQuest()
        {
            //given
            IJoyAction action = Mock.Of<IJoyAction>();
            IQuest quest = Mock.Of<IQuest>(
                q => q.AdvanceStep() == true
                && q.FulfilsRequirements(left, action) == true
                && q.CompleteQuest(left) == true
                && q.IsComplete == true);
            
            target.AddQuest(left.GUID, quest);
            quest.StartQuest(left);
            
            //when
            target.PerformQuestAction(left, quest, action);

            //then
            Assert.That(target.GetQuestsForEntity(left.GUID), Is.Empty);

            return null;
        }

        [TearDown]
        public void TearDown()
        {
            GlobalConstants.GameManager = null;
        }
    }
}