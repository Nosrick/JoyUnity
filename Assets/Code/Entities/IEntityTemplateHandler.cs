using System.Collections.Generic;

namespace JoyLib.Code.Entities
{
    public interface IEntityTemplateHandler
    {
        IEnumerable<IEntityTemplate> Templates { get; }

        IEntityTemplate Get(string type);
        IEntityTemplate GetRandom();
        
    }
}