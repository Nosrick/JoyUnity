using System.Collections.Generic;

namespace JoyLib.Code.Cultures
{
    public interface ICultureHandler
    {
        ICulture GetByCultureName(string name);
        List<ICulture> GetByCreatureType(string type);
        
        ICulture[] Cultures { get; }
    }
}