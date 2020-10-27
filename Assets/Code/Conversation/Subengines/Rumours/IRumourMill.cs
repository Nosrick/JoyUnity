﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoyLib.Code.Conversation.Subengines.Rumours
{
    public interface IRumourMill
    {
        List<IRumour> Rumours
        {
            get;
        }

        List<IRumour> RumourTypes
        {
            get;
        }

        IRumour GenerateRandomRumour(JoyObject[] participants);

        IRumour GenerateRumourFromTags(JoyObject[] participants, string[] tags);
    }
}
