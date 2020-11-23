﻿namespace JoyLib.Code.Entities.Gender
{
    public class BaseGender : IGender
    {
        public string Possessive { get; protected set; }
        public string PersonalSubject { get; protected set; }
        public string PersonalObject { get; protected set; }
        public string Reflexive { get; protected set; }

        public string PossessivePlural { get; protected set; }

        public string ReflexivePlural { get; protected set; }
        public string Name { get; protected set; }

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