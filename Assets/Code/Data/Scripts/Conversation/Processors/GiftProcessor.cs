﻿using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Rollers;
using JoyLib.Code.Unity.GUI;
using UnityEngine;

namespace JoyLib.Code.Entities.Abilities.Conversation.Processors
{
    public class GiftProcessor : TopicData
    {
        protected IGUIManager GUIManager { get; set; }
        
        public GiftProcessor() 
            : base(
                new ITopicCondition[0], 
                "GiftTopic", 
                new string[] { "ListenerThanks" }, 
                "", 
                0, 
                new string[] { "giveitemaction" }, 
                Speaker.INSTIGATOR,
                new RNG())
        {
            if (GUIManager is null)
            {
                GUIManager = GlobalConstants.GameManager.GUIManager;
            }
        }

        public override ITopic[] Interact(IEntity instigator, IEntity listener)
        {
            GUIManager.OpenGUI("Inventory");
            GUIManager.BringToFront("Inventory");
            
            return base.Interact(instigator, listener);
        }
    }
}