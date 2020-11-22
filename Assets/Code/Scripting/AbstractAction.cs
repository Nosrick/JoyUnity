using JoyLib.Code.Entities;
using JoyLib.Code.Quests;
using UnityEngine;

namespace JoyLib.Code.Scripting
{
    public abstract class AbstractAction : IJoyAction
    {
        public AbstractAction()
        {
            if (QuestTracker is null)
            {
                QuestTracker = GameObject.Find("GameManager").GetComponent<QuestTracker>();
            }
        }
        
        public abstract bool Execute(IJoyObject[] participants, string[] tags = null, params object[] args);

        public void ClearLastParameters()
        {
            LastParticipants = null;
            LastTags = null;
            LastArgs = null;
            Successful = false;
        }

        public virtual void SetLastParameters(IJoyObject[] participants, string[] tags = null, params object[] args)
        {
            LastParticipants = participants;
            LastTags = tags;
            LastArgs = args;
            Successful = true;
            
            QuestTracker.PerformQuestAction(LastParticipants[0] as Entity, this);
        }

        public virtual string Name => "abstractaction";
        public virtual string ActionString => "SOMEONE FORGOT TO OVERRIDE THE ACTIONSTRING";
        public IJoyObject[] LastParticipants { get; protected set; }
        public string[] LastTags { get; protected set; }
        public object[] LastArgs { get; protected set; }
        public bool Successful { get; protected set; }
        
        protected static QuestTracker QuestTracker { get; set; }
    }
}