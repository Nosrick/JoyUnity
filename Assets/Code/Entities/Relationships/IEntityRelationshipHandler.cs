using JoyLib.Code.Collections;

namespace JoyLib.Code.Entities.Relationships
{
    public interface IEntityRelationshipHandler
    {
        IRelationship CreateRelationship(IJoyObject[] participants, string type = "friendship");
        IRelationship CreateRelationshipWithValue(IJoyObject[] participants, string type, int value);
        IRelationship[] Get(IJoyObject[] participants, string[] tags = null, bool createNewIfNone = false);
        int GetHighestRelationshipValue(IJoyObject speaker, IJoyObject listener, string[] tags = null);
        IRelationship GetBestRelationship(IJoyObject speaker, IJoyObject listener, string[] tags = null);
        IRelationship[] GetAllForObject(IJoyObject actor);
        bool IsFamily(IJoyObject speaker, IJoyObject listener);
        NonUniqueDictionary<long, IRelationship> Relationships { get; }
    }
}