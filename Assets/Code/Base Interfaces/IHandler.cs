using System.Collections.Generic;

namespace JoyLib.Code
{
    public interface IHandler<T>
    {
        IEnumerable<T> Values { get; }

        T Get(string name);

        IEnumerable<T> Load();
    }
}