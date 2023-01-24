using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Windows.Ink;
using System.Reflection;
using EventDrivenSimulationLibrary;

namespace SimulationWindow
{
    /// <summary>
    /// Initializes an instance of a particle.
    /// </summary>
    /// <param name="position">Position of a particle using x and y axis.</param>
    /// <param name="velocity">Velocity of a particle using x and y axis.</param>
    /// <param name="radius">Radius of a particle.</param>
    /// <param name="mass">Mass of a particle.</param>
    public class Particle : Shape
    {
        public Vector2 position, velocity;      //position and velocity
        public double radius, mass;             //radius and mass
        public int count;                       //collision counter

        private static SolidColorBrush blue = (SolidColorBrush)new BrushConverter().ConvertFrom("#4D96FF");

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
            Fill = blue;

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

        public void Move(double deltaTime) => position += velocity * deltaTime;

        public void Draw() => this.RenderTransform = new TranslateTransform(position.x - radius, position.y - radius);

        #region Collisions Calculations
        public void VerticalWallCollision()
        {
            velocity.x = (-velocity.x);
            count++;
        }

        public void HorizontalWallCollision()
        {
            velocity.y = (-velocity.y);
            count++;
        }

        public void ParticleCollision(Particle p2)
        {
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
        }

        #endregion

        #region Collision Prediction Methods

        public double timeToHitParticle(Particle p1)
        {
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
        }

        public double timeToHitVerticalWall()
        {
            if (this.velocity.x == 0)
                return double.PositiveInfinity;
            else if (this.velocity.x > 0)
                return (500 - this.position.x - this.radius) / this.velocity.x;

            return Math.Abs((0 + this.position.x - this.radius) / this.velocity.x);
        }

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