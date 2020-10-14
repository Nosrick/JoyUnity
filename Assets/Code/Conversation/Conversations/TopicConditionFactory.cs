using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Scripting;

namespace JoyLib.Code.Conversation.Conversations
{
    public class TopicConditionFactory
    {
        protected static List<Type> s_ConditionTypes;

        public TopicConditionFactory()
        {
            if (s_ConditionTypes is null)
            {
                s_ConditionTypes = ScriptingEngine.instance.FetchTypeAndChildren(typeof(ITopicCondition)).ToList();
            }
        }
        
        public ITopicCondition Create(string condition, string operand, int value)
        {
            switch (operand)
            {
                case "!":
                    return new NotEqualToCondition(condition, value);
                    break;

                case "=":
                    return new EqualToCondition(condition, value);
                    break;

                case ">":
                    return new GreaterThanCondition(condition, value);
                    break;

                case "<":
                    return new LessThanCondition(condition, value);
                    break;

                default:
                    throw new InvalidOperationException("Could not find the condition for operand " + operand);
            }
        }
    }
}