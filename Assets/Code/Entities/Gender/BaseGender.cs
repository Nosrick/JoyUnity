using System;
using Sirenix.OdinSerializer;

namespace JoyLib.Code.Entities.Gender
{
    [Serializable]
    public class BaseGender : IGender
    {
        [OdinSerialize]
        public string Possessive { get; protected set; }
        [OdinSerialize]
        public string PersonalSubject { get; protected set; }
        [OdinSerialize]
        public string PersonalObject { get; protected set; }
        [OdinSerialize]
        public string Reflexive { get; protected set; }

        [OdinSerialize]
        public string PossessivePlural { get; protected set; }

        [OdinSerialize]
        public string ReflexivePlural { get; protected set; }
        [OdinSerialize]
        public string Name { get; protected set; }

        [OdinSerialize]
        public string IsOrAre { get; protected set; }

        public BaseGender()
        {}
        
        public BaseGender(
            string name,
            string possessive,
            string personalSubject,
            string personalObject,
            string reflexive,
            string possessivePlural,
            string reflexivePlural,
            string isOrAre)
        {
            this.Name = name;
            this.Possessive = possessive;
            this.PossessivePlural = possessivePlural;
            this.PersonalSubject = personalSubject;
            this.PersonalObject = personalObject;
            this.Reflexive = reflexive;
            this.ReflexivePlural = reflexivePlural;
            this.IsOrAre = isOrAre;
        }
    }
}