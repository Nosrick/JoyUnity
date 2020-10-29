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
        
        IJoyObject GetParticipant(long GUID);
        IJoyObject[] GetParticipants();

        bool AddTag(string tag);

        bool RemoveTag(string tag);

        int ModifyValueOfParticipant(long actor, long observer, int value);

        int ModifyValueOfOtherParticipants(long actor, int value);

        int ModifyValueOfAllParticipants(int value);

        bool AddParticipant(IJoyObject newParticipant);
        
        bool RemoveParticipant(long currentGUID);

        long GenerateHash(long[] participants);
        
        IRelationship Create(IEnumerable<IJoyObject> participants);
        IRelationship CreateWithValue(IEnumerable<IJoyObject> participants, int value);

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
