using System;

namespace JoyLib.Code.Entities.Sexes
{
    public class Male : IBioSex
    {
        public bool CanBirth
        {
            get
            {
                return false;
            }
        }

        public string Name
        {
            get
            {
                return "male";
            }
        }

        public Entity CreateChild(Entity[] parents)
        {
            throw new NotImplementedException();
        }
    }
}
