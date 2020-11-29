using JoyLib.Code.Quests;

namespace JoyLib.Code.Scripting
{
    public interface IJoyAction
    {
        bool Execute(IJoyObject[] participants, string[] tags = null, params object[] args);
        void SetLastParameters(IJoyObject[] participants, string[] tags = null, params object[] args);

        void ClearLastParameters();

        string Name
        {
            get;
        }

        string ActionString
        {
            get;
        }

        IJoyObject[] LastParticipants
        {
            get;
        }

        string[] LastTags
        {
            get;
        }

        object[] LastArgs
        {
            get;
        }

        bool Successful
        {
            get;
        }
    }
}