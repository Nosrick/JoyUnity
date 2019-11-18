using System;

namespace JoyLib.Code.Entities.Sexuality
{
    public class AbstractSexuality : ISexuality
    {
        public virtual string Name => throw new NotImplementedException("Someone forgot to override Name.");

        public virtual bool DecaysNeed => throw new NotImplementedException("Someone forgot to override DecaysNeed.");

        public virtual int MatingThreshold
        {
            get => throw new NotImplementedException("Someone forgot to override MatingThreshold.");
            set => throw new NotImplementedException("Someone forgot to override MatingThreshold.");
        }

        public virtual bool WillMateWith(Entity me, Entity them)
        {
            throw new NotImplementedException("Someone forgot to override WillMateWith.");
        }
    }
}
