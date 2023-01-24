using SimulationWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationRender
{
    public class EventDrivenCollisionSystem
    {
        private PriorityQueue<Event, double> collisionQueue = new PriorityQueue<Event, double>();
        private double time = 0;
        private double stepTime = 0;
        public double offsetDeltaTime = 1;
        private readonly Particle[] particles;

        public double updateCount = 0;

        public TimeSpan nextCollision = TimeSpan.Zero;
        private Event? currentEvent;

        public EventDrivenCollisionSystem(Particle[] particles, double stepTime)
        {
            this.particles = particles;
            this.stepTime = stepTime;

            for (int i = 0; i < particles.Length; i++)
                PredictCollisions(particles[i]);

            //collisionQueue.Enqueue(new Event(0, null, null), 0);
            CalculateNextCollision();
        }

        /// <summary>
        /// Compute the time of each possible collision.
        /// Particle - Particle, Particle - Vertical Wall, Particle - Horizontal Wall
        /// and place it on the priority queue as an event using null logic to specify the type of collision.
        /// </summary>
        /// <param name="p1">Particle to be evaluated</param>
        private void PredictCollisions(Particle p1)
        {
            //N + 2 events added to the PQ at worst
            if (p1 == null) return;

            for (int i = 0; i < particles.Length; i++)
            {
                double deltaTimeParticleCollision = time + p1.timeToHitParticle(particles[i]);
                if (deltaTimeParticleCollision != double.PositiveInfinity)
                    collisionQueue.Enqueue(new Event(deltaTimeParticleCollision, p1, particles[i]), deltaTimeParticleCollision);
            }

            double deltaTimeVWall = time + p1.timeToHitVerticalWall();
            if (deltaTimeVWall != double.PositiveInfinity)
                collisionQueue.Enqueue(new Event(deltaTimeVWall, p1, null), deltaTimeVWall);

            double deltaTimeHWall = time + p1.timeToHitHorizontalWall();
            if (deltaTimeHWall != double.PositiveInfinity)
                collisionQueue.Enqueue(new Event(deltaTimeHWall, null, p1), deltaTimeHWall);
        }

        //public void Simulation()
        //{
        //    while (collisionQueue.Count > 0)
        //    {
        //        Event currentEvent = collisionQueue.Dequeue();
        //        if (currentEvent.isValid()) continue;

        //        //todo
        //        for (int i = 0; i < particles.Length; i++)
        //            particles[i].Move(currentEvent.time - time);

        //        time = currentEvent.time;

        //        if (currentEvent.p1 != null && currentEvent.p2 != null)         //Null evaluation logic
        //            currentEvent.p1.ParticleCollision(currentEvent.p2);         //if both are not null calculate particle - particle collision
        //        else if (currentEvent.p1 != null && currentEvent.p2 == null)
        //            currentEvent.p1.VerticalWallCollision();                    //if the first one is not null calculate the vertical wall collision
        //        else if (currentEvent.p1 == null && currentEvent.p2 != null)
        //            currentEvent.p2.HorizontalWallCollision();                  //if the second one is not null calculate the horizontal wall collision
        //        else if (currentEvent.p1 == null && currentEvent.p2 == null)
        //            continue;                                                   //both null, get another event, no collision

        //        PredictCollisions(currentEvent.p1);                             //Predict future collisions for those two particles
        //        PredictCollisions(currentEvent.p2);
        //    }
        //}

        public void CalculateNextCollision()
        {
            while (collisionQueue.Count > 0)
            {
                currentEvent = collisionQueue.Dequeue();
                if (!currentEvent.isValid()) continue;



                //updateCount = currentEvent.time - time;
                //time = currentEvent.time;




                ////double updatesCount = Math.Floor(currentEvent.time - time);
                ////double deltaTime = Math.Truncate((currentEvent.time - time) * 1000) / 1000;
                ////offsetDeltaTime = (currentEvent.time - time) / Math.Ceiling(currentEvent.time - time);
                offsetDeltaTime = (currentEvent.time - time) - Math.Floor(currentEvent.time - time);
                time = currentEvent.time;

                //time - time before next collision
                nextCollision = TimeSpan.FromMilliseconds(time * stepTime - stepTime);

                break;
            }
        }

        public void ResolveCollision()
        {
            if (currentEvent.p1 != null && currentEvent.p2 != null)
                currentEvent.p1.ParticleCollision(currentEvent.p2);
            else if (currentEvent.p1 != null && currentEvent.p2 == null)
                currentEvent.p1.VerticalWallCollision();
            else if (currentEvent.p1 == null && currentEvent.p2 != null)
                currentEvent.p2.HorizontalWallCollision();

            PredictCollisions(currentEvent.p1);
            PredictCollisions(currentEvent.p2);
        }
    }
}
