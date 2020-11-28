using System.Collections.Generic;

namespace JoyLib.Code.Entities.Needs
{
    public interface INeedHandler
    {
        INeed Get(string name);
        ICollection<INeed> GetMany(IEnumerable<string> names);
        ICollection<INeed> GetManyRandomised(IEnumerable<string> names);
        INeed GetRandomised(string name);
        
        IEnumerable<INeed> Needs { get; }
        IEnumerable<string> NeedNames { get; }
    }
}