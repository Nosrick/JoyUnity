using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Statistics;

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

        protected IDictionary<string, EntitySkill> DefaultSkillBlock
        {
            get;
            set;
        }

        public SkillParameterProcessor()
        {
            SkillHandler = GlobalConstants.GameManager.SkillHandler;
            NeedHandler = GlobalConstants.GameManager.NeedHandler;
            DefaultSkillBlock = SkillHandler.GetDefaultSkillBlock(this.NeedHandler.Needs);
        }
        
        public bool CanParse(string parameter)
        {
            if (parameter.Equals("skills", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return DefaultSkillBlock.ContainsKey(parameter);
        }

        public string Parse(string parameter, IJoyObject participant)
        {
            if (!(participant is IEntity entity))
            {
                return "";
            }

            if (parameter.Equals("skills", StringComparison.OrdinalIgnoreCase)
                || entity.Skills.ContainsKey(parameter))
            {
                IEnumerable<Tuple<string, int>> values = entity.GetData(new string[] {parameter});

                return values.OrderByDescending(tuple => tuple.Item2)
                    .First()
                    .Item1;
            }

            return "";
        }
    }
}