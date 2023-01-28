﻿using SimulationWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationRender
{
    /// <summary>
    /// In other words collision at a t time between particles or particles and wall along with n collisions stored for each at the time of calculating this event.
    /// </summary>
    public class Event
    {
        public double time = 0;
        public Particle? p1, p2;
        private int countp1 = 0, countp2 = 0;

        public Event(double time, Particle p1, Particle p2)
        {
            this.time = time;
            this.p1 = p1;
            this.p2 = p2;

            if (p1 != null) countp1 = p1.count;
            if (p2 != null) countp2 = p2.count;
        }

        /// <summary>
        /// Checks if no collisions have happened between queuing and dequeuing this event.
        /// </summary>
        /// <returns>True if no collisions have happened.</returns>
        public bool isValid()
        {
            if (p1 != null && p2 != null) return (p1.count == countp1 && p2.count == countp2);
            else if (p1 != null) return (p1.count == countp1);
            else if (p2 != null) return (p2.count == countp2);

            return false;
        }
    }
}
