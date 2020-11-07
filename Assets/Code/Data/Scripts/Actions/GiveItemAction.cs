using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Quests;
using UnityEngine;

namespace JoyLib.Code.Scripting.Actions
{
    public class GiveItemAction : IJoyAction
    {
        public string Name => "giveitemaction";
        public string ActionString => "gives";
        public IJoyObject[] LastParticipants { get; protected set; }
        public string[] LastTags { get; protected set; }
        public object[] LastArgs { get; protected set; }
        public bool Successful { get; protected set; }
        
        protected static QuestTracker QuestTracker { get; set; }

        public GiveItemAction()
        {
            if (QuestTracker is null)
            {
                QuestTracker = GameObject.Find("GameManager").GetComponent<QuestTracker>();
            }
        }
        
        public bool Execute(IJoyObject[] participants, string[] tags = null, params object[] args)
        {
            Successful = false;
            
            if (participants.Length != 2)
            {
                Debug.Log("WRONG NUMBER OF PARTICIPANTS");
                return false;
            }

            if (!(participants[0] is Entity left))
            {
                Debug.Log("FIRST PARTICIPANT IS NOT AN ENTITY");
                return false;
            }
            if (!(participants[1] is Entity right))
            {
                Debug.Log("SECOND PARTICIPANT IS NOT AN ENTITY");
                return false;
            }

            if (!(args[0] is ItemInstance item))
            {
                Debug.Log("ARGUMENT IS NOT AN ITEMINSTANCE");
                return false;
            }

            if (!left.RemoveItemFromBackpack(item))
            {
                Debug.Log("CANNOT REMOVE ITEM FROM FIRST PARTICIPANT'S BACKPACK");
                return false;
            }

            if (!right.AddContents(item))
            {
                Debug.Log("CANNOT ADD ITEM TO SECOND PARTICIPANT");
                return false;
            }
            
            SetLastParameters(participants, tags, args);
            
            QuestTracker.PerformQuestAction(left, this);
            
            Debug.Log("SUCCESSFUL");

            return true;
        }
        
        public void SetLastParameters(IJoyObject[] participants, string[] tags = null, params object[] args)
        {
            this.LastParticipants = participants;
            this.LastTags = tags;
            this.LastArgs = args;
            Successful = true;
        }
    }
}