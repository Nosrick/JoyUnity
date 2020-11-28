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
        protected IEntitySkillHandler SkillHandler
        {
            get;
            set;
        }

        protected INeedHandler NeedHandler
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
            SkillHandler = GlobalConstants.GameManager.SkillHandler;
            NeedHandler = GlobalConstants.GameManager.NeedHandler;
            DefaultNeeds = new BasicValueContainer<INeed>(NeedHandler.Needs.ToArray());
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

        public string Parse(string parameter, IJoyObject participant)
        {
            if (!(participant is Entity entity))
            {
                return "";
            }

            if (parameter.Equals("skills", StringComparison.OrdinalIgnoreCase)
                || entity.Skills.Has(parameter))
            {
                Tuple<string, int>[] values = entity.GetData(new string[] {parameter});

                return values.OrderByDescending(tuple => tuple.Item2)
                    .First()
                    .Item1;
            }

            return "";
        }
    }
}