using System.Collections.Generic;

namespace JoyLib.Code.Entities
{
    public interface IEntityTemplateHandler
    {
        List<IEntityTemplate> Templates { get; }

        IEntityTemplate Get(string type);
        IEntityTemplate GetRandom();
        
    }
}