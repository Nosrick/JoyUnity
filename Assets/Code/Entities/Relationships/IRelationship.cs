﻿using System.Collections.Generic;

namespace JoyLib.Code.Entities.Relationships
{
    public interface IRelationship
    {
        Dictionary<long, int> GetValuesOfParticipant(long GUID);

        int GetRelationshipValue(long left, long right);

        string[] GetTags();
        
        JoyObject GetParticipant(long GUID);
        JoyObject[] GetParticipants();

        bool AddTag(string tag);

        bool RemoveTag(string tag);

        int ModifyValueOfParticipant(long actor, long observer, int value);

        bool AddParticipant(JoyObject newParticipant);
        
        bool RemoveParticipant(long currentGUID);

        string GenerateHash();

        string Name
        {
            get;
        }
    }
}