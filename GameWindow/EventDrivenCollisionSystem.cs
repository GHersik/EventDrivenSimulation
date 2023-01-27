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
        private readonly Particle[] particles;
        private double time = 0;
        private double quantaTime = 0;
        //public double offsetDeltaTime = 1;

        private long collisionCounter;

        public long CollisionCounter
        {
            get { return collisionCounter; }
            private set { collisionCounter = value; }
        }

        private TimeSpan nextCollsion;
        public TimeSpan NextCollision
        {
            get { return nextCollsion; }
            private set { nextCollsion = value; }
        }

        //private Event? currentEvent;

        public EventDrivenCollisionSystem(Particle[] particles, double quantaTime)
        {
            this.particles = particles;
            this.quantaTime = quantaTime;
            nextCollsion = TimeSpan.Zero;

            for (int i = 0; i < particles.Length; i++)
                PredictCollisions(particles[i]);

            CalculateNextCollision();
        }

        /// <summary>
        /// Calculate the time of each possible collision.
        /// Particle - Particle, Particle - Vertical Wall, Particle - Horizontal Wall
        /// and place it on the priority queue as an event using null logic to specify the type of collision.
        /// </summary>
        /// <param name="p1">Particle to be evaluated</param>
        private void PredictCollisions(Particle p1)
        {
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



        //public void CalculateNextCollisionTime()
        //{
        //    while (collisionQueue.Count > 0)
        //    {
        //        currentEvent = collisionQueue.Dequeue();
        //        if (!currentEvent.isValid()) continue;

        //        //New with solver, need to calculate time.floor and assign
        //        //time = currentEvent.time;
        //        //nextCollsion = TimeSpan.FromMilliseconds(Math.Floor(time) * stepTime);


        //        //updateCount = currentEvent.time - time;
        //        //time = currentEvent.time;




        //        ////double updatesCount = Math.Floor(currentEvent.time - time);
        //        ////double deltaTime = Math.Truncate((currentEvent.time - time) * 1000) / 1000;
        //        ////offsetDeltaTime = (currentEvent.time - time) / Math.Ceiling(currentEvent.time - time);
        //        //offsetDeltaTime = (currentEvent.time - time) - Math.Floor(currentEvent.time - time);
        //        time = currentEvent.time;

        //        //time - time before next collision
        //        nextCollsion = TimeSpan.FromMilliseconds(time * quantaTime - quantaTime);

        //        break;
        //    }
        //}

        //public void ResolveCollision()
        //{
        //    if (currentEvent.p1 != null && currentEvent.p2 != null)
        //        currentEvent.p1.ParticleCollision(currentEvent.p2);
        //    else if (currentEvent.p1 != null && currentEvent.p2 == null)
        //        currentEvent.p1.VerticalWallCollision();
        //    else if (currentEvent.p1 == null && currentEvent.p2 != null)
        //        currentEvent.p2.HorizontalWallCollision();

        //    PredictCollisions(currentEvent.p1);
        //    PredictCollisions(currentEvent.p2);
        //}


        //Different approach

        /// <summary>
        /// Calculates time of another collision on the queue.
        /// </summary>
        public void CalculateNextCollision()
        {
            while (!collisionQueue.Peek().isValid())
                collisionQueue.Dequeue();

            time = collisionQueue.Peek().time;
            nextCollsion = TimeSpan.FromMilliseconds(time * quantaTime);
        }

        /// <summary>
        /// Solves all the possible collisions at a t time, also those that would occur between the new collisions
        /// and places all the particles at their final position at t + 1 time.
        /// </summary>
        /// <param name="t"></param>
        public void Solver()
        {
            double t = Math.Ceiling(time);
            double offsetDeltaTime = Math.Floor(time);

            while (collisionQueue.Peek().time < t)
            {
                Event currentEvent = collisionQueue.Dequeue();
                if (!currentEvent.isValid()) continue;

                //Calculate time and move particles towards collision destination
                //double difference = currentEvent.time - time;
                //double offsetDeltaTime = difference - Math.Floor(difference);
                //double offsetDeltaTime = (currentEvent.time - time) - Math.Floor(currentEvent.time - time);

                //offsetDeltaTime = (currentEvent.time - time) - Math.Floor(currentEvent.time - time);
                offsetDeltaTime = currentEvent.time - offsetDeltaTime;

                for (int i = 0; i < particles.Length; i++)
                    particles[i].Move(offsetDeltaTime);


                //double offsetDeltaTime = Math.Abs(currentEvent.time - t);
                //if (currentEvent.p1 != null)
                //{
                //    currentEvent.p1.Move(offsetDeltaTime);
                //    currentEvent.p1.Draw();
                //}

                //if (currentEvent.p2 != null)
                //{
                //    currentEvent.p2.Move(offsetDeltaTime);
                //    currentEvent.p2.Draw();
                //}

                time = currentEvent.time;
                offsetDeltaTime = currentEvent.time;

                //Resolve collision
                ResolveCollision(currentEvent);

                //Predict future collisions and add those to the queue
                PredictCollisions(currentEvent.p1);
                PredictCollisions(currentEvent.p2);

                CollisionCounter++;
            }

            //Final position of all the particles at t + 1 time
            offsetDeltaTime = t - offsetDeltaTime;
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].Move(offsetDeltaTime);
                particles[i].Draw();
            }
        }

        /// <summary>
        /// Resolves collision based on given event using null logic to evaluate what kind of collision has occured, particle - particle, particle - wall.
        /// </summary>
        /// <param name="collision">Specific collision that occured</param>
        private void ResolveCollision(Event collision)
        {
            if (collision.p1 != null && collision.p2 != null)
                collision.p1.ParticleCollision(collision.p2);
            else if (collision.p1 != null && collision.p2 == null)
                collision.p1.VerticalWallCollision();
            else if (collision.p1 == null && collision.p2 != null)
                collision.p2.HorizontalWallCollision();
        }
    }
}
