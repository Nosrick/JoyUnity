namespace JoyLib.Code.Scripting
{
    public interface IJoyAction
    {
        bool Execute(JoyObject[] participants, string[] tags = null , params object[] args);

        string Name
        {
            get;
        }

        string ActionString
        {
            get;
        }
    }
}