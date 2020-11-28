using System.Collections.Generic;

namespace JoyLib.Code.Entities
{
    public interface IEntityTemplateHandler
    {
        List<EntityTemplate> Templates { get; }

        EntityTemplate Get(string type);
        EntityTemplate GetRandom();
        
    }
}