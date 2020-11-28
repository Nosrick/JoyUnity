using System.Collections.Generic;

namespace JoyLib.Code.Entities.Gender
{
    public interface IGenderHandler
    {
        HashSet<IGender> Genders { get; }

        IGender Get(string name);
    }
}