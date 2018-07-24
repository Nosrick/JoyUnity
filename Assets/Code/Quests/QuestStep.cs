using JoyLib.Code.Entities.Items;
using JoyLib.Code.World;
using System.Collections.Generic;
using System.Linq;

namespace JoyLib.Code.Quests
{
    public class QuestStep
    {
        protected QuestAction m_Action;
        protected List<ItemInstance> m_Objects;
        protected List<JoyObject> m_Actors;
        protected List<WorldInstance> m_Areas;

        public QuestStep(QuestAction action, List<ItemInstance> objects, List<JoyObject> actors, List<WorldInstance> areas)
        {
            m_Action = action;
            m_Objects = objects;
            m_Actors = actors;
            m_Areas = areas;
        }

        public override string ToString()
        {
            string questString = "";
            string actionString = "";
            string objectString = "";
            string actorString = "";
            string areaString = "";

            switch (action)
            {
                case QuestAction.Deliver:
                    actionString = "Deliver ";
                    actorString = " to ";
                    for (int i = 0; i < actors.Count; i++)
                    {
                        actorString += actors[i].JoyName;
                        if (i < actors.Count - 2)
                        {
                            actorString += ", ";
                        }
                        else if (actors.Count > 1)
                        {
                            actorString += "or ";
                        }
                    }
                    break;

                case QuestAction.Destroy:
                    actionString = "Destroy ";
                    for(int i = 0; i < objects.Count; i++)
                    {
                        actionString += objects[i].JoyName;
                        if(i < objects.Count - 2)
                        {
                            actionString += ", ";
                        }
                        else if(objects.Count > 1)
                        {
                            actionString += "and ";
                        }
                    }

                    for(int i = 0; i < actors.Count; i++)
                    {
                        actionString += actors[i].JoyName;
                        if (i < actors.Count - 2)
                        {
                            actionString += ", ";
                        }
                        else if (actors.Count > 1)
                        {
                            actionString += "and ";
                        }

                    }
                    break;

                case QuestAction.Explore:
                    actionString = "Explore ";
                    for (int i = 0; i < areas.Count; i++)
                    {
                        areaString += areas[i].Name;
                        if (i < areas.Count - 2)
                        {
                            areaString += ", ";
                        }
                        else if (areas.Count > 1)
                        {
                            areaString += "and ";
                        }
                    }
                    break;

                case QuestAction.Retrieve:
                    actionString = "Bring ";
                    actorString = " to ";
                    for (int i = 0; i < actors.Count; i++)
                    {
                        actorString += actors[i].JoyName;
                        if (i < actors.Count - 2)
                        {
                            actorString += ", ";
                        }
                        else if (actors.Count > 1)
                        {
                            actorString += "or ";
                        }
                    }
                    break;
            }

            for (int i = 0; i < objects.Count; i++)
            {
                objectString += objects[i].JoyName;
                if (i < objects.Count - 2)
                {
                    objectString += ", ";
                }
                else if (objects.Count > 1)
                {
                    objectString += "and ";
                }
            }

            questString = actionString + objectString + actorString + areaString + ". ";
            return questString;
        }

        public QuestAction action
        {
            get
            {
                return m_Action;
            }
        }

        public List<ItemInstance> objects
        {
            get
            {
                return m_Objects.ToList();
            }
        }

        public List<JoyObject> actors
        {
            get
            {
                return m_Actors.ToList();
            }
        }

        public List<WorldInstance> areas
        {
            get
            {
                return m_Areas.ToList();
            }
        }
    }
}
