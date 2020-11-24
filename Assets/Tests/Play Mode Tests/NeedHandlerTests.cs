﻿using System.Collections;
using JoyLib.Code;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Graphics;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class NeedHandlerTests
    {
        private GameObject container;
    
        private NeedHandler target;

        private ObjectIconHandler objectIconHandler;
    
        [SetUp]
        public void SetUp()
        {
            container = new GameObject("GameManager");
            GlobalConstants.GameManager = container;
            
            objectIconHandler = container.AddComponent<ObjectIconHandler>();
            target = container.AddComponent<NeedHandler>();
        }
    
        [UnityTest]
        public IEnumerator Initialise_ShouldHave_ValidData()
        {
            //given
            
            //when
            
            //then
            Assert.That(target.Needs, Is.Not.Empty);
            foreach (INeed need in target.Needs.Values)
            {
                Assert.That(need.Name, Is.Not.Empty);
                Assert.That(need.Name, Is.Not.EqualTo("DEFAULT"));
            }
            
            return null;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(container);
        }
    }
}