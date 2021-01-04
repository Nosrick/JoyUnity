using UnityEngine;

namespace JoyLib.Code.Entities.AI.LOS.Providers
{
    public interface IVisionProviderHandler
    {
        bool AddVision(
            string name,
            Color darkColour,
            Color lightColour,
            IFOVHandler algorithm,
            int minimumLightLevel = 0,
            int maximumLightLevel = GlobalConstants.MAX_LIGHT);

        bool AddVision(IVision vision);
        
        IVision GetVision(string name);

        bool HasVision(string name);
    }
}