﻿using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Helpers;
using JoyLib.Code.Rollers;
using JoyLib.Code.World;

namespace JoyLib.Code.Entities.Abilities.Conversation.Processors
{
    public class LocalAreaInfoProcessor : TopicData
    {
        protected Entity Listener
        {
            get;
            set;
        }
        
        public LocalAreaInfoProcessor() 
            : base(
                new ITopicCondition[0], 
                "LocalAreaInfo",
                new []{ "Thanks" },
                "",
                0,
                new string[0], 
                Speaker.LISTENER)
        {
        }

        public override ITopic[] Interact(Entity instigator, Entity listener)
        {
            Listener = listener;
            return base.Interact(instigator, listener);
        }

        protected override ITopic[] FetchNextTopics()
        {
            return new ITopic[]
            {
                new TopicData(
                    new ITopicCondition[0], 
                    "LocalAreaInfo",
                    new []{ "Thanks" },
                    GetAreaInfo(Listener),
                    0,
                    new string[0], 
                    Speaker.LISTENER) 
            };
        }

        protected string GetAreaInfo(Entity listener)
        {
            string message = "";

            WorldInstance listenerWorld = listener.MyWorld;
            if (listenerWorld.HasTag("interior"))
            {
                int result = RNG.instance.Roll(0, 100);
                if (result <= 50)
                {
                    int numberOfLevels = 1;
                    numberOfLevels = WorldConversationDataHelper.GetNumberOfFloors(numberOfLevels, listenerWorld);

                    string plural = numberOfLevels > 1 ? "floors" : "floor";
                    message = "This place has " + numberOfLevels + plural + " to it.";
                }
                if (result > 50)
                {
                    int exactNumber = WorldConversationDataHelper.GetNumberOfCreatures(listener.CreatureType, listenerWorld);
                    int roughNumber = 0;
                    if (exactNumber < 10)
                    {
                        roughNumber = exactNumber;
                    }
                    else if (exactNumber % 10 < 6)
                    {
                        roughNumber = exactNumber - (exactNumber % 10);
                    }
                    else
                    {
                        roughNumber = exactNumber + (exactNumber % 10);
                    }

                    string plural = roughNumber > 1 ? "s" : "";
                    message = "There are around " + roughNumber + " " + listener.CreatureType + plural + " here.";
                }
            }
            else
            {
                message = "I don't know much about this place, sorry.";
            }

            return message;
        }
    }
}