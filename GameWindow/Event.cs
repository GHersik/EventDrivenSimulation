using SimulationWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationRender
{
    public class Event : IComparable<Event>
    {
        private double time;
        private Particle p1, p2;
        private int countp1, countp2;

        public Event(double time, Particle p1, Particle p2, int countp1, int countp2)
        {


            this.time = time;
            this.p1 = p1;
            this.p2 = p2;
            this.countp1 = countp1;
            this.countp2 = countp2;
        }

        public int CompareTo(Event that)
        {
            if (this.time - that.time > 0)
                return 1;
            if (this.time - that.time == 0)
                return 0;

            return -1;
        }

        public bool isValid()
        {
            return true;
        }
    }
}
