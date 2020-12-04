using System.Collections.Generic;
using JoyLib.Code.Collections;

namespace JoyLib.Code.Entities.Relationships
{
    public interface IEntityRelationshipHandler
    {
        bool AddRelationship(IRelationship relationship);
        bool RemoveRelationship(long ID);
        
        IRelationship CreateRelationship(IJoyObject[] participants, string[] tags);
        IRelationship CreateRelationshipWithValue(IJoyObject[] participants, string[] tags, int value);
        IRelationship[] Get(IJoyObject[] participants, string[] tags = null, bool createNewIfNone = false);
        int GetHighestRelationshipValue(IJoyObject speaker, IJoyObject listener, string[] tags = null);
        IRelationship GetBestRelationship(IJoyObject speaker, IJoyObject listener, string[] tags = null);
        IRelationship[] GetAllForObject(IJoyObject actor);
        bool IsFamily(IJoyObject speaker, IJoyObject listener);
        NonUniqueDictionary<long, IRelationship> Relationships { get; }
        
        List<IRelationship> RelationshipTypes { get; }
    }
}