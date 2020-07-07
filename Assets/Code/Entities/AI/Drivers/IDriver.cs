using JoyLib.Code.World;
using UnityEngine;

namespace JoyLib.Code.Entities.AI.Drivers
{
    public interface IDriver
    {
        void Locomotion(Entity vehicle);

        void Interact();
    }
}