using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Rollers;
using UnityEngine;

namespace JoyLib.Code.Entities.Needs
{
    public interface INeed : IBasicValue
    {

        bool FindFulfilmentObject(Entity actor);

        bool Interact(Entity actor, IJoyObject obj);

        INeed Copy();

        INeed Randomise();

        bool Tick(Entity actor);

        int Fulfill(int value);

        int Decay(int value);

        //Name and Value come from IBasicValue

        int Priority
        {
            get;
        }

        bool ContributingHappiness
        {
            get;
        }

        int AverageForDay
        {
            get;
        }

        int AverageForWeek
        {
            get;
        }

        int AverageForMonth
        {
            get;
        }

        Sprite FulfillingSprite
        {
            get;
        }

        int HappinessThreshold
        {
            get;
        }
        
        RNG Roller { get; }
    }
}
