using JoyLib.Code.Entities;
using JoyLib.Code.Quests;

namespace JoyLib.Code.Scripting
{
    public abstract class AbstractAction : IJoyAction
    {
        public AbstractAction(IQuestTracker questTracker = null)
        {
            if (GlobalConstants.GameManager is null == false)
            {
                QuestTracker = questTracker is null ? GlobalConstants.GameManager.QuestTracker : questTracker;
            }
        }
        
        public abstract bool Execute(IJoyObject[] participants, string[] tags = null, params object[] args);

        public void ClearLastParameters()
        {
            this.LastParticipants = null;
            this.LastTags = null;
            this.LastArgs = null;
            this.Successful = false;
        }

        public virtual void SetLastParameters(IJoyObject[] participants, string[] tags = null, params object[] args)
        {
            this.LastParticipants = participants;
            this.LastTags = tags;
            this.LastArgs = args;
            this.Successful = true;
            
            QuestTracker?.PerformQuestAction(this.LastParticipants[0] as Entity, this);
        }

        public virtual string Name => "abstractaction";
        public virtual string ActionString => "SOMEONE FORGOT TO OVERRIDE THE ACTIONSTRING";
        public IJoyObject[] LastParticipants { get; protected set; }
        public string[] LastTags { get; protected set; }
        public object[] LastArgs { get; protected set; }
        public bool Successful { get; protected set; }
        public static IQuestTracker QuestTracker { get; set; }
    }
}