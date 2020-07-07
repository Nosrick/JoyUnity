﻿using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Helpers;
using JoyLib.Code.Rollers;
using JoyLib.Code.World;
using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.Quests
{
    public class QuestProvider : MonoBehaviour
    {        
        protected ItemFactory m_ItemFactory;

        protected EntityRelationshipHandler m_EntityRelationshipHandler;

        public void Awake()
        {
            Initialise();
        }

        protected void Initialise()
        {
            m_ItemFactory = new ItemFactory();
            m_EntityRelationshipHandler = GameObject.Find("GameManager").GetComponent<EntityRelationshipHandler>();
        }

        public Quest MakeRandomQuest(Entity questor, Entity provider, WorldInstance overworldRef)
        {
            List<QuestStep> steps = new List<QuestStep>();

            //int numberOfSteps = RNG.instance.Roll(1, 4);
            int numberOfSteps = 1;
            for (int i = 0; i < numberOfSteps; i++)
            {
                int result = RNG.instance.Roll(0, 3);
                switch (result)
                {
                    case (int)QuestAction.Deliver:
                        steps.Add(MakeDeliveryQuest(provider, overworldRef));
                        break;

                    case (int)QuestAction.Retrieve:
                        steps.Add(MakeRetrieveQuest(provider, overworldRef));
                        break;

                    case (int)QuestAction.Destroy:
                        steps.Add(MakeDestroyQuest(provider, overworldRef));
                        break;

                    case (int)QuestAction.Explore:
                        steps.Add(MakeExploreQuest(provider, overworldRef));
                        break;
                }
            }
            
            return new Quest(steps, QuestMorality.Neutral, GetRewards(questor, provider, steps));
        }

        private List<ItemInstance> GetRewards(Entity questor, Entity provider, List<QuestStep> steps)
        {
            List<ItemInstance> rewards = new List<ItemInstance>();
            int reward = ((steps.Count * 100) + (m_EntityRelationshipHandler.GetHighestRelationshipValue(provider, questor)));
            rewards.Add(BagOfGoldHelper.GetBagOfGold(reward));

            return rewards;
        }

        public QuestStep MakeDeliveryQuest(Entity provider, WorldInstance overworldRef)
        {
            QuestAction action = QuestAction.Deliver;

            ItemInstance deliveryItem = null;
            ItemInstance[] backpack = provider.Backpack;
            if (backpack.Length > 0)
            {
                int result = RNG.instance.Roll(0, backpack.Length - 1);

                deliveryItem = backpack[result];
            }
            Entity endPoint = overworldRef.GetRandomSentientWorldWide();
            if(deliveryItem == null)
            {
                deliveryItem = m_ItemFactory.CreateCompletelyRandomItem();
            }

            QuestStep step = new QuestStep(action, new List<ItemInstance>() { deliveryItem }, new List<JoyObject>() { endPoint }, new List<WorldInstance>());
            return step;
        }

        public QuestStep MakeDestroyQuest(Entity provider, WorldInstance overworldRef)
        {
            QuestAction action = QuestAction.Destroy;

            int result = RNG.instance.Roll(0, 1);
            JoyObject target = null;
            if(result == 0)
            {
                int breakout = 0;
                //Destroy an item
                while(breakout < 100)
                {
                    Entity holder = overworldRef.GetRandomSentientWorldWide();
                    ItemInstance[] backpack = holder.Backpack;
                    if (backpack.Length > 0)
                    {
                        result = RNG.instance.Roll(0, backpack.Length - 1);
                        target = backpack[result];
                        break;
                    }
                    breakout += 1;
                }

                if(breakout == 100)
                {
                    target = overworldRef.GetRandomSentientWorldWide();
                }
            }
            else
            {
                //Destroy a creature
                target = overworldRef.GetRandomSentientWorldWide();
            }
            QuestStep step = new QuestStep(action, new List<ItemInstance>(), new List<JoyObject>() { target }, new List<World.WorldInstance>());
            return step;
        }

        public QuestStep MakeRetrieveQuest(Entity provider, WorldInstance overworldRef)
        {
            QuestAction action = QuestAction.Retrieve;

            ItemInstance target = null;
            int breakout = 0;
            while(breakout < 100)
            {
                Entity holder = overworldRef.GetRandomSentientWorldWide();
                ItemInstance[] backpack = holder.Backpack;
                if(backpack.Length > 0)
                {
                    int result = RNG.instance.Roll(0, backpack.Length - 1);
                    target = backpack[result];
                    break;
                }
                breakout += 1;
            }

            if (breakout == 100)
            {
                return MakeExploreQuest(provider, overworldRef);
            }

            QuestStep step = new QuestStep(action, new List<ItemInstance>() { target }, new List<JoyObject>() { provider }, new List<World.WorldInstance>());
            return step;
        }

        public QuestStep MakeExploreQuest(Entity provider, WorldInstance overworldRef)
        {
            QuestAction action = QuestAction.Explore;

            List<WorldInstance> worlds = overworldRef.GetWorlds(overworldRef);
            WorldInstance target = overworldRef;

            WorldInstance currentWorld = provider.MyWorld;

            while (true)
            {
                target = worlds[RNG.instance.Roll(0, worlds.Count - 1)];
                if (target.GUID != currentWorld.GUID && target.GUID != overworldRef.GUID)
                {
                    break;
                }
            }

            QuestStep step = new QuestStep(action, new List<ItemInstance>(), new List<JoyObject>(), new List<WorldInstance>() { target });
            return step;
        }
    }
}
