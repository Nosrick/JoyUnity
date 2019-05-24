using JoyLib.Code.Entities;
using JoyLib.Code.Helpers;
using JoyLib.Code.World;
using System.Collections.Generic;

namespace JoyLib.Code.Conversation.Subengines
{
    public static class RumourMill
    {
        private static List<string> s_Rumours;

        public static void GenerateRumours(WorldInstance worldRef)
        {
            s_Rumours = new List<string>();
            for (int i = 0; i < 12; i++)
            {
                s_Rumours.Add(CreateRumour(worldRef));
            }
        }

        public static void AddRumour(Entity left, Entity right, RumourType typeRef)
        {
            switch(typeRef)
            {
                case RumourType.Argument:
                    GenerateArgument(left, right);
                    break;

                case RumourType.Scandal:
                    GenerateScandal(left, right);
                    break;

                case RumourType.Theft:
                    GenerateTheft(left, right);
                    break;
            }
        }

        public static void AddRumour(Entity entityRef, RumourType typeRef)
        {
            switch(typeRef)
            {
                case RumourType.Skill:
                    GenerateSkillRumour(entityRef);
                    break;
            }
        }

        public static string FetchRumour()
        {
            return s_Rumours[RNG.Roll(0, s_Rumours.Count - 1)];
        }

        public static string CreateRumour(WorldInstance worldRef)
        {
            int result = RNG.Roll(0, 100);

            if(result >= 0 && result < 25)
            {
                return GenerateScandal(worldRef.GetRandomSentient(), worldRef.GetRandomSentient());
            }
            else if(result >= 25 && result < 50)
            {
                return GenerateSkillRumour(worldRef.GetRandomSentient());
            }
            else if(result >= 50 && result < 75)
            {
                return GenerateArgument(worldRef.GetRandomSentient(), worldRef.GetRandomSentient());
            }
            else if(result >= 75 && result < 100)
            {
                return GenerateTheft(worldRef.GetRandomSentient(), worldRef.GetRandomSentient());
            }

            return "Sorry, I don't have any juicy gossip for you.";
        }

        private static string GenerateSkillRumour(Entity entityRef)
        {
            List<string> skills = new List<string>();

            //Choose either best skills or worst skills
            int result = RNG.Roll(0, 1);
            if(result == 0)
            {
                int worstSkill = int.MaxValue;
                foreach(KeyValuePair<string, EntitySkill> skillPair in entityRef.Skills)
                {
                    if (skillPair.Value.value <= worstSkill)
                    {
                        worstSkill = skillPair.Value.value;
                        skills.Add(skillPair.Key);
                    }
                }
                return "I heard " + entityRef.JoyName + " is terrible at " + skills[RNG.Roll(0, skills.Count - 1)] + ", how embarrassing!";
            }
            else
            {
                int bestSkill = int.MinValue;
                foreach (KeyValuePair<string, EntitySkill> skillPair in entityRef.Skills)
                {
                    if (skillPair.Value.value >= bestSkill)
                    {
                        bestSkill = skillPair.Value.value;
                        skills.Add(skillPair.Key);
                    }
                }
                return entityRef.JoyName + " is really good at " + skills[RNG.Roll(0, skills.Count - 1)] + ", I'm pretty envious.";
            }
        }

        private static string GenerateScandal(Entity left, Entity right)
        {
            string rumour = "Did you hear about " + left.JoyName + " and " + right.JoyName + "? It's scandalous!";
            return rumour;
        }

        private static string GenerateArgument(Entity left, Entity right)
        {
            return "Apparently " + left.JoyName + " and " + right.JoyName + 
                " are embroiled in some sort of feud? I don't know what it's about, though.";
        }

        private static string GenerateTheft(Entity left, Entity right)
        {
            string itemName = "something";
            if(left.Backpack.Length > 0)
            {
                itemName = left.Backpack[RNG.Roll(0, left.Backpack.Length - 1)].JoyName;
            }
            return "I heard " + left.JoyName + " stole " + itemName + " from " + right.JoyName + ".";
        }
    }
}
