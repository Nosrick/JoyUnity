using UnityEngine;

namespace JoyLib.Code.Entities.AI.LOS.Providers
{
    public interface IVisionProviderHandler
    {
        bool AddVision(string name,
            Color darkColour,
            Color lightColour,
            IFOVHandler algorithm,
            int minimumLightLevel = 0,
            int minimumComfortLevel = 0,
            int maximumLightLevel = 32, int maximumComfortLevel = 32);

        bool AddVision(IVision vision);
        
        IVision GetVision(string name);

        bool HasVision(string name);
    }
}