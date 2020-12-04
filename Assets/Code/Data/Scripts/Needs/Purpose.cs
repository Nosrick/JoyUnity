﻿using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities.AI;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using JoyLib.Code.Quests;
using JoyLib.Code.Rollers;
using UnityEngine;

namespace JoyLib.Code.Entities.Needs
{
    public class Purpose : AbstractNeed
    {
        public override string Name => "purpose";

        protected const int DECAY_MIN = 4;
        protected const int DECAY_MAX = 128;

        protected const int PRIORITY_MIN = 0;
        protected const int PRIORITY_MAX = 12;

        protected const int HAPPINESS_THRESHOLD_MIN = 0;
        protected const int HAPPINESS_THRESHOLD_MAX = 24;

        protected const int MAX_VALUE_MIN = HAPPINESS_THRESHOLD_MAX;
        protected const int MAX_VALUE_MAX = MAX_VALUE_MIN * 4;

        protected static IEntityRelationshipHandler RelationshipHandler
        {
            get;
            set;
        }

        protected static IQuestProvider QuestProvider
        {
            get;
            set;
        }

        public Purpose()
        : base(
            1,
            1,
            true,
            1,
            1,
            1,
            1,
            new string[0])
        {
        }

        public Purpose(
            int decayRef, 
            int decayCounterRef, 
            bool doesDecayRef, 
            int priorityRef, 
            int happinessThresholdRef, 
            int valueRef, 
            int maxValueRef,
            Sprite fulfillingSprite,
            int averageForDayRef = 0, 
            int averageForWeekRef = 0) 
            : base(
                decayRef, 
                decayCounterRef, 
                doesDecayRef, 
                priorityRef, 
                happinessThresholdRef, 
                valueRef, 
                maxValueRef, 
                new string[0], 
                fulfillingSprite,
                averageForDayRef,
                averageForWeekRef)
        {
        }

        protected void Initialise()
        {
            if (GlobalConstants.GameManager is null == false && RelationshipHandler is null)
            {
                RelationshipHandler = GlobalConstants.GameManager.RelationshipHandler;
                QuestProvider = GlobalConstants.GameManager.QuestProvider;
            }
        }

        //Currently, the questing and employment systems are not (fully) in.
        //This will just seek out a random person and ask for a quest stub.
        public override bool FindFulfilmentObject(IEntity actor)
        {
            IEnumerable<string> tags = actor.Tags.Where(x => x.IndexOf("sentient", StringComparison.OrdinalIgnoreCase) >= 0);

            List<IEntity> possibleListeners = actor.MyWorld.SearchForEntities(actor, tags).ToList();

            IEntity bestMatch = null;
            int bestRelationship = int.MinValue;
            foreach (IEntity possible in possibleListeners)
            {
                List<IJoyObject> participants = new List<IJoyObject> {actor, possible};

                string[] relationshipTags = new[] {"friendship"};
                IEnumerable<IRelationship> relationships = RelationshipHandler?.Get(participants, relationshipTags);

                if (relationships is null)
                {
                    return false;
                }

                foreach (IRelationship relationship in relationships)
                {
                    int thisRelationship = relationship.GetRelationshipValue(actor.GUID, possible.GUID);
                    if (bestRelationship < thisRelationship)
                    {
                        bestRelationship = thisRelationship;
                        bestMatch = possible;
                    }
                }
            }

            if (bestMatch is null)
            {
                m_CachedActions["wanderaction"].Execute(
                    new IJoyObject[] {actor},
                    new[] {"wander", "need", "purpose"});
                return false;
            }

            m_CachedActions["seekaction"].Execute(
                new IJoyObject[] {actor, bestMatch},
                new[] {"need", "seek", "purpose"},
                new object[] {"purpose"});
            return true;
        }

        public override bool Interact(IEntity actor, IJoyObject obj)
        {
            if (!(obj is IEntity listener))
            {
                return false;
            }
            
            //Asking to do something for your friend increases your relationship
            m_CachedActions["fulfillneedaction"].Execute(
                new IJoyObject[] {actor, listener},
                new[] {"need", "friendship", "fulfill"},
                new object[] {"friendship", actor.Statistics[EntityStatistic.PERSONALITY].Value, 0, true});

            if (RelationshipHandler.IsFamily(actor, listener))
            {
                m_CachedActions["fulfillneedaction"].Execute(
                    new IJoyObject[] {actor, listener},
                    new[] {"need", "family", "fulfill"},
                    new object[] {"family", actor.Statistics[EntityStatistic.PERSONALITY].Value, 0, true});
            }

            if (QuestProvider is null == false)
            {
                actor.AddQuest(
                    QuestProvider.MakeRandomQuest(
                        actor, 
                        listener, 
                        actor.MyWorld.GetOverworld()));
            }

            m_CachedActions["fulfillneedaction"].Execute(
                new IJoyObject[] {actor},
                new[] {"need", "purpose", "fulfill"},
                new object[] {"purpose", listener.Statistics[EntityStatistic.PERSONALITY].Value, 0, false});

            return true;
        }

        public override INeed Copy()
        {
            return new Purpose(
                this.m_Decay,
                this.m_DecayCounter,
                this.m_DoesDecay,
                this.m_Priority,
                this.m_HappinessThreshold,
                this.m_Value,
                this.m_MaximumValue,
                this.FulfillingSprite,
                this.AverageForDay);
        }

        public override INeed Randomise()
        {
            int decay = Roller.Roll(DECAY_MIN, DECAY_MAX);
            int decayCounter = Roller.Roll(0, DECAY_MAX);
            int priority = Roller.Roll(PRIORITY_MIN, PRIORITY_MAX);
            int happinessThreshold = Roller.Roll(HAPPINESS_THRESHOLD_MIN, HAPPINESS_THRESHOLD_MAX);
            int value = Roller.Roll(0, HAPPINESS_THRESHOLD_MAX);
            int maxValue = Roller.Roll(MAX_VALUE_MIN, MAX_VALUE_MAX);
            
            return new Purpose(
                decay,
                decayCounter,
                true,
                priority,
                happinessThreshold,
                value,
                maxValue,
                this.FulfillingSprite);
        }
    }
}