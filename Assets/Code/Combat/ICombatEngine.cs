using JoyLib.Code.Entities;

namespace JoyLib.Code.Combat
{
    public interface ICombatEngine
    {
        int MakeAttack(IEntity attacker,
            IEntity defender,
            string[] attackerTags,
            string[] defenderTags);
    }
}