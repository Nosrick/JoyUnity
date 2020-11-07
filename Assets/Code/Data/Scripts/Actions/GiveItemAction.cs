using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Quests;
using UnityEngine;

namespace JoyLib.Code.Scripting.Actions
{
    public class GiveItemAction : AbstractAction
    {
        public override string Name => "giveitemaction";
        public override string ActionString => "gives";
        
        protected static QuestTracker QuestTracker { get; set; }

        public GiveItemAction()
        {
            if (QuestTracker is null)
            {
                QuestTracker = GameObject.Find("GameManager").GetComponent<QuestTracker>();
            }
        }
        
        public override bool Execute(IJoyObject[] participants, string[] tags = null, params object[] args)
        {
            ClearLastParameters();
            
            if (participants.Length != 2)
            {
                return false;
            }

            if (!(participants[0] is Entity left))
            {
                return false;
            }
            if (!(participants[1] is Entity right))
            {
                return false;
            }

            if (!(args[0] is ItemInstance item))
            {
                return false;
            }

            if (!left.RemoveItemFromBackpack(item))
            {
                return false;
            }

            if (!right.AddContents(item))
            {
                return false;
            }
            
            SetLastParameters(participants, tags, args);

            return true;
        }
    }
}