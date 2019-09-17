using System;

namespace JoyLib.Code.Entities.Sexes
{
    public class Female : IBioSex
    {
        public bool CanBirth
        {
            get
            {
                return true;
            }
        }

        public string Name
        {
            get
            {
                return "Female";
            }
        }

        public Entity CreateChild(Entity[] parents)
        {
            throw new NotImplementedException();
        }
    }
}
