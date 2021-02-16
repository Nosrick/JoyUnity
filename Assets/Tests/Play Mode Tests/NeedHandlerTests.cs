using System.Collections;
using System.Collections.Generic;
using JoyLib.Code;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Graphics;
using JoyLib.Code.Helpers;
using JoyLib.Code.Scripting;
using Moq;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Tests
{
    public class NeedHandlerTests
    {
        private ScriptingEngine ScriptingEngine;
        
        private INeedHandler target;
    
        [SetUp]
        public void SetUp()
        {
            GlobalConstants.ActionLog = new ActionLog();
            ScriptingEngine = new ScriptingEngine();

            IGameManager gameManager = Mock.Of<IGameManager>(
                manager => manager.RelationshipHandler == Mock.Of<IEntityRelationshipHandler>()
                && manager.ObjectIconHandler == Mock.Of<IObjectIconHandler>(
                    handler => handler.GetFrame(
                        It.IsAny<string>(), 
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<int>()) 
                               == new SpriteData
                               {
                                   m_Name = "DEFAULT",
                                   m_Parts = new List<SpritePart>
                                   {
                                       new SpritePart
                                       {
                                           m_Frames = 1
                                       }
                                   },
                                   m_State = "DEFAULT"
                               }));

            GlobalConstants.GameManager = gameManager;
            
            target = new NeedHandler();
        }
    
        [UnityTest]
        public IEnumerator Initialise_ShouldHave_ValidData()
        {
            //given
            
            //when
            
            //then
            Assert.That(target.Values, Is.Not.Empty);
            foreach (INeed need in target.Values)
            {
                Assert.That(need.Name, Is.Not.Empty);
                Assert.That(need.Name, Is.Not.EqualTo("DEFAULT"));
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
