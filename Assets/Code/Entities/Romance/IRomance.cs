using JoyLib.Code.Entities.Relationships;

namespace JoyLib.Code.Entities.Romance
{
    public interface IRomance : ITagged
    {
        bool Compatible(Entity me, Entity them, IRelationship[] relationships);

        string Name { get; }
        
        bool DecaysNeed { get; }
        
        int RomanceThreshold { get; set; }
        
        int BondingThreshold { get; set; }
    }
}