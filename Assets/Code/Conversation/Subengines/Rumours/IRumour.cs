using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoyLib.Code.Conversation.Subengines.Rumours
{
    public interface IRumour
    {
        JoyObject[] Participants
        {
            get;
        }

        string[] Tags
        {
            get;
        }

        int ViralPotential
        {
            get;
        }

        
    }
}
