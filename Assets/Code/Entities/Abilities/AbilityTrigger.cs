using System;

namespace JoyLib.Code.Entities.Abilities
{
    [Flags]
    public enum AbilityTrigger
    {
        OnAttack = 1,
        OnTakeHit = 2,
        OnHeal = 4,
        OnPickup = 8,
        OnTick = 16,
        OnDeath = 32,
        OnKill = 64,
        OnUse = 128
    }
}