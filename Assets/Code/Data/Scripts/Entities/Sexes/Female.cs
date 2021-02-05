using System;

namespace JoyLib.Code.Entities.Sexes
{
    [Serializable]
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
                return "female";
            }
        }

        public Entity CreateChild(Entity[] parents)
        {
            throw new NotImplementedException();
        }
    }
}
