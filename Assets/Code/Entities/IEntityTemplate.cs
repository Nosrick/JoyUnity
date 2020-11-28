using JoyLib.Code.Collections;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.AI.LOS.Providers;
using JoyLib.Code.Entities.Statistics;

namespace JoyLib.Code.Entities
{
    public interface IEntityTemplate : ITagged
    {
        string[] Slots { get; }
        BasicValueContainer<IRollableValue> Statistics { get; }
        BasicValueContainer<IGrowingValue> Skills { get; }
        string[] Needs { get; }
        IAbility[] Abilities { get; }
        int Size { get; }
        bool Sentient { get; }
        IVision VisionType { get; }
        string CreatureType { get; }
        string JoyType { get; }
        string Tileset { get; }
    }
}