using SimulationWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationRender
{
    /// <summary>
    /// Collision based system which uses Priority Queue at it's core to store every future collision between every particle and wall on a scene.
    /// As of this moment the collision based system always has to have at least one moving particle.
    /// </summary>
    public class EventDrivenCollisionSystem
    {
        private PriorityQueue<Event, double> collisionQueue = new PriorityQueue<Event, double>();
        private readonly Particle[] particles;
        private double time = 0;
        private double quantaTime = 0;

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

        /// <summary>
        /// Calculates time of another collision on the queue, if given particles have no velocity then we dequeue an event with no collision time.
        /// </summary>
        public void CalculateNextCollision()
        {
            while (true)
            {
                Event Peek = collisionQueue.Peek();
                if (!Peek.isValid() || Peek.time is double.NaN) { collisionQueue.Dequeue(); continue; }

                time = collisionQueue.Peek().time;
                nextCollsion = TimeSpan.FromMilliseconds(time * quantaTime);
                break;
            }
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

                offsetDeltaTime = currentEvent.time - offsetDeltaTime;

                for (int i = 0; i < particles.Length; i++)
                    particles[i].Move(offsetDeltaTime);

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

            //Can be further optimized
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
