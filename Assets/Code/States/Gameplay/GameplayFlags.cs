using System;
namespace JoyLib.Code.States.Gameplay
{
    [Flags]
    public enum GameplayFlags
    {
        Moving = 0,
        Interacting = 1,
        Giving = 2,
        Attacking = 4,
        Targeting = 8
    }

    [Flags]
    public enum WorldSpaceFlags
    {
        Interior = 0,
        Exterior = 1,
        Overworld = 2
    }
}
