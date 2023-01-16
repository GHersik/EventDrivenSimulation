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
        private Particle[] particles;

        public TimeSpan nextCollision;
        private Event? currentEvent;

        public EventDrivenCollisionSystem(Particle[] particles)
        {
            this.particles = particles;

            for (int i = 0; i < particles.Length; i++)
                PredictCollisions(particles[i]);

            collisionQueue.Enqueue(new Event(0, null, null), 0);
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
            //N + 2 events added to the PQ
            if (p1 == null) return;

            //for (int i = 0; i < particles.Length - 1; i++)
            //{
            //    double deltaTime = p1.timeToHitParticle(particles[i]);
            //    collisionQueue.Enqueue(new Event(deltaTime, p1, particles[i]), deltaTime);
            //}

            double deltaTimeVWall = time + p1.timeToHitVerticalWall();
            double deltaTimeHWall = time + p1.timeToHitHorizontalWall();
            collisionQueue.Enqueue(new Event(deltaTimeVWall, p1, null), deltaTimeVWall);
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


                //double updatesCount = Math.Floor(currentEvent.time - time);
                double deltaTime = Math.Truncate((currentEvent.time - time) * 1000) / 1000;

                time = currentEvent.time;


                nextCollision = TimeSpan.FromMilliseconds(time * 20);




                break;
            }
        }

        public void ResolveCollision()
        {
            if (currentEvent.p1 != null && currentEvent.p2 != null)         //Null evaluation logic
                currentEvent.p1.ParticleCollision(currentEvent.p2);         //if both are not null calculate particle - particle collision
            else if (currentEvent.p1 != null && currentEvent.p2 == null)
                currentEvent.p1.VerticalWallCollision();                    //if the first one is not null calculate the vertical wall collision
            else if (currentEvent.p1 == null && currentEvent.p2 != null)
                currentEvent.p2.HorizontalWallCollision();                  //if the second one is not null calculate the horizontal wall collision
            //else if (currentEvent.p1 == null && currentEvent.p2 == null)
            //    return;                                                   //both null, get another event, no collision

            PredictCollisions(currentEvent.p1);                             //Predict future collisions for those two particles
            PredictCollisions(currentEvent.p2);
        }
    }
}
