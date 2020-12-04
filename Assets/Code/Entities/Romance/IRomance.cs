using JoyLib.Code.Entities.Relationships;

namespace JoyLib.Code.Entities.Romance
{
    public interface IRomance : ITagged
    {
        bool WillRomance(IEntity me, IEntity them, IRelationship[] relationships);
        bool Compatible(IEntity me, IEntity them);

        string Name { get; }
        
        bool DecaysNeed { get; }
        
        int RomanceThreshold { get; set; }
        
        int BondingThreshold { get; set; }
    }
}