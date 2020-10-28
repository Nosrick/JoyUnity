using System;
using System.Linq;
using JoyLib.Code.Collections;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Statistics;
using UnityEngine;

namespace JoyLib.Code.Conversation.Subengines.Rumours
{
    public class SkillParameterProcessor : IParameterProcessor
    {
        protected EntitySkillHandler SkillHandler
        {
            get;
            set;
        }

        protected NeedHandler NeedHandler
        {
            get;
            set;
        }

        protected BasicValueContainer<INeed> DefaultNeeds
        {
            get;
            set;
        }

        public SkillParameterProcessor()
        {
            if (SkillHandler is null)
            {
                SkillHandler = GameObject.Find("GameManager").GetComponent<EntitySkillHandler>();
                NeedHandler = GameObject.Find("GameManager").GetComponent<NeedHandler>();
                DefaultNeeds = new BasicValueContainer<INeed>(NeedHandler.Needs.Values);
            }
        }
        
        public bool CanParse(string parameter)
        {
            if (parameter.Equals("skills", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            BasicValueContainer<IGrowingValue> skills = SkillHandler.GetDefaultSkillBlock(DefaultNeeds);
            return skills.Has(parameter);
        }

        public string Parse(string parameter, JoyObject participant)
        {
            if (!(participant is Entity entity))
            {
                return "";
            }

            if (parameter.Equals("skills", StringComparison.OrdinalIgnoreCase)
                || entity.Skills.Has(parameter))
            {
                Tuple<string, int>[] values = entity.GetData(new string[] {parameter});
                foreach (Tuple<string, int> value in values)
                {
                    Debug.Log(value);
                }

                return values.OrderByDescending(tuple => tuple.Item2)
                    .First()
                    .Item1;
            }

            return "";
        }
    }
}