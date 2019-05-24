using JoyLib.Code.Entities.Relationships;
using System;
using System.Collections.Generic;

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

        public virtual bool FindMate(Entity me)
        {
            List<Entity> possibleMates = me.MyWorld.SearchForEntities(entity => me.CanSee(entity.WorldPosition) && WillMateWith(me, entity));

            if (possibleMates.Count == 0)
            {
                return false;
            }

            Entity bestMate = null;
            int bestRelationship = MatingThreshold;
            foreach (Entity mate in possibleMates)
            {
                List<long> participants = new List<long>();
                participants.Add(me.GUID);
                participants.Add(mate.GUID);
                List<string> tags = new List<string>() { "sexual" };
                List<IRelationship> relationships = EntityRelationshipHandler.Get(participants.ToArray(), tags.ToArray());

                foreach (IRelationship relationship in relationships)
                {
                    int thisRelationship = relationship.GetRelationshipValue(me.GUID, mate.GUID);
                    if (thisRelationship >= bestRelationship)
                    {
                        bestRelationship = thisRelationship;
                        bestMate = mate;
                    }
                }
            }

            if (bestMate != null)
            {
                me.Seek(bestMate, "Sex");
                return true;
            }

            return false;
        }

        public virtual bool WillMateWith(Entity me, Entity them)
        {
            throw new NotImplementedException("Someone forgot to override WillMateWith.");
        }
    }
}
