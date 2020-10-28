using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace JoyLib.Code.Entities.Relationships
{
    public interface IRelationship
    {
        Dictionary<long, int> GetValuesOfParticipant(long GUID);

        int GetRelationshipValue(long left, long right);

        int GetHighestRelationshipValue(long GUID);

        string[] GetTags();
        
        JoyObject GetParticipant(long GUID);
        JoyObject[] GetParticipants();

        bool AddTag(string tag);

        bool RemoveTag(string tag);

        int ModifyValueOfParticipant(long actor, long observer, int value);

        int ModifyValueOfOtherParticipants(long actor, int value);

        int ModifyValueOfAllParticipants(int value);

        bool AddParticipant(JoyObject newParticipant);
        
        bool RemoveParticipant(long currentGUID);

        long GenerateHash(long[] participants);
        
        IRelationship Create(IEnumerable<JoyObject> participants);
        IRelationship CreateWithValue(IEnumerable<JoyObject> participants, int value);

        string Name
        {
            get;
        }

        ReadOnlyCollection<string> Tags
        {
            get;
        }
    }
}
