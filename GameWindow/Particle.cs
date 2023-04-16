using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SimulationWindow
{
    /// <summary>
    /// Initializes an instance of a particle.
    /// </summary>
    /// <param name="position">Position of a particle using x and y axis.</param>
    /// <param name="velocity">Velocity of a particle using x and y axis.</param>
    /// <param name="radius">Radius of a particle.</param>
    /// <param name="mass">Mass of a particle.</param>
    /// <param name="count">"Collisions counter"</param>
    public class Particle : Shape
    {
        public Vector2 position, velocity;      //position and velocity
        public double radius, mass;             //radius and mass
        public int count;                       //collision counter

        #region Constructors

        public Particle(Vector2 position, Vector2 velocity, double radius, double mass)
        {
            //Assign properties
            this.radius = radius;
            this.position = position;
            this.velocity = velocity;
            this.mass = mass;
            this.count = 0;

            this.Width = radius + radius;
            this.Height = radius + radius;

            //Position accordingly
            this.RenderTransform = new TranslateTransform
                (this.position.x - radius, this.position.y - radius);
        }

        public Particle() : this(Vector2.Zero, Vector2.One, 8, 1) { }

        #endregion

        protected override Geometry DefiningGeometry
        {
            get
            {
                return new EllipseGeometry(new Rect(0, 0, this.Width - 2, this.Height - 2));
            }
        }

        /// <summary>
        /// Moves the particle by a given velocity and deltaTime offset.
        /// </summary>
        /// <param name="deltaTime">Time to move the particle by.</param>
        public void Move(double deltaTime) => position += velocity * deltaTime;

        /// <summary>
        /// Draws the particle at a given position with it's center in the middle.
        /// </summary>
        public void Draw() => this.RenderTransform = new TranslateTransform(position.x - radius, position.y - radius);

        #region Collisions Calculations

        /// <summary>
        /// Applies velocity to a particle based on the vertical wall collision. 
        /// </summary>
        public void VerticalWallCollision()
        {
            velocity.x = (-velocity.x);
            count++;
        }

        /// <summary>
        /// Applies velocity to a particle based on the horizontal wall collision. 
        /// </summary>
        public void HorizontalWallCollision()
        {
            velocity.y = (-velocity.y);
            count++;
        }

        /// <summary>
        /// Calculates the impulse and velocity between the two particles based on the laws of elastic collisions.
        /// </summary>
        /// <param name="p2">Particle to collide with.</param>
        public void ParticleCollision(Particle p2)
        {
            //***************************************************************************************
            //*    Title: <Event-Driven Simulation>
            //*    Author: <Robert Sedgewick and Kevin Wayne>
            //*    Date: <2000-2019>
            //*    Availability: <https://algs4.cs.princeton.edu/61event/>

            //Calculate Impulse
            Vector2 relativePos = p2.position - this.position;
            Vector2 relativeVel = p2.velocity - this.velocity;
            double deltaVelDeltaPos = Vector2.Dot(relativePos, relativeVel);
            double distanceBetween = this.radius + p2.radius;

            double impulse = 2 * this.mass * p2.mass * deltaVelDeltaPos / ((this.mass + p2.mass) * distanceBetween);
            Vector2 impulseVector = relativePos * impulse / distanceBetween;

            //Assign velocity
            this.velocity += impulseVector / this.mass;
            p2.velocity -= impulseVector / p2.mass;

            this.count++;
            p2.count++;

            //***************************************************************************************
        }

        #endregion

        #region Collision Prediction Methods

        /// <summary>
        /// Calculates the time of next collision between the two particles.
        /// </summary>
        /// <param name="p1">Particle to calculate time with</param>
        /// <returns>Time of collision between two particles</returns>
        public double timeToHitParticle(Particle p1)
        {
            //***************************************************************************************
            //*    Title: <Event-Driven Simulation>
            //*    Author: <Robert Sedgewick and Kevin Wayne>
            //*    Date: <2000-2019>
            //*    Availability: <https://algs4.cs.princeton.edu/61event/>

            if (this.Equals(p1)) return double.PositiveInfinity;

            Vector2 relativePosition = this.position - p1.position;
            Vector2 relativeVelocity = this.velocity - p1.velocity;
            double deltaPosdeltaVel = Vector2.Dot(relativePosition, relativeVelocity);

            if (deltaPosdeltaVel > 0) return double.PositiveInfinity;

            double deltaPosS = Vector2.Dot(relativePosition, relativePosition);
            double deltaVelS = Vector2.Dot(relativeVelocity, relativeVelocity);
            double sigma = this.radius + p1.radius;
            double distance = (deltaPosdeltaVel * deltaPosdeltaVel) - deltaVelS * (deltaPosS - sigma * sigma);

            if (distance < 0) return double.PositiveInfinity;

            return -(deltaPosdeltaVel + Math.Sqrt(distance)) / deltaVelS;

            //***************************************************************************************
        }

        /// <summary>
        /// Calculates time of collision with a vertical wall.
        /// </summary>
        /// <returns>Time of collison with a wall.</returns>
        public double timeToHitVerticalWall()
        {
            if (this.velocity.x == 0)
                return double.PositiveInfinity;
            else if (this.velocity.x > 0)
                return (500 - this.position.x - this.radius) / this.velocity.x;

            return Math.Abs((0 + this.position.x - this.radius) / this.velocity.x);
        }

        /// <summary>
        /// Calculates time of collision with a horizontal wall.
        /// </summary>
        /// <returns>Time of collison with a wall.</returns>
        public double timeToHitHorizontalWall()
        {
            if (this.velocity.y == 0)
                return double.PositiveInfinity;
            else if (this.velocity.y > 0)
                return (500 - this.position.y - this.radius) / this.velocity.y;

            return Math.Abs((0 + this.position.y - this.radius) / this.velocity.y);
        }

        #endregion
    }
}

/***************************************************************************************
*    Title: <EventDrivenSimulation>
*    Author: <Gregory Werner>
*    Date: <16.04.2023>
*    Code version: <1.0.0>
*    Availability: <https://github.com/GHersik/EventDrivenSimulation>
*
***************************************************************************************/