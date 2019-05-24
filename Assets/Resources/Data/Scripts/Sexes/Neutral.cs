using System;

namespace JoyLib.Code.Entities.Sexes
{
    public class Neutral : IBioSex
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
                return "Neutral";
            }
        }

        public Entity CreateChild(Entity[] parents)
        {
            throw new NotImplementedException();
        }
    }
}
