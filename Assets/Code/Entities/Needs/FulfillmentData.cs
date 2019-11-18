using System.Collections.Generic;

namespace JoyLib.Code.Entities.Needs
{
    public class FulfillmentData
    {
        public FulfillmentData(string name, int counter, JoyObject[] targets)
        {
            this.Name = name;
            this.Counter = counter;
            this.Targets = targets;
        }

        public int DecrementCounter()
        {
            Counter -= 1;
            return Counter;
        }

        public string Name
        {
            get;
            protected set;
        }

        public int Counter
        {
            get;
            protected set;
        }

        public JoyObject[] Targets
        {
            get;
            protected set;
        }
    }
}