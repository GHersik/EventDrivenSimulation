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

        //private static readonly Random rnd = new Random();
        private static SolidColorBrush blue = (SolidColorBrush)new BrushConverter().ConvertFrom("#4D96FF");
        private static SolidColorBrush transparent = (SolidColorBrush)new BrushConverter().ConvertFrom("#00000000");

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
            StrokeThickness = 2;
            Stroke = blue;
            Fill = transparent;

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

            //X axis
            //if (position.x + radius > 500 || position.x - radius < 0)
            //{
            //    velocity.x = (-velocity.x);
            //    count++;
            //}
        }

        public void HorizontalWallCollision()
        {
            velocity.y = (-velocity.y);
            count++;

            //Y axis
            //if (position.y + radius > 500 || position.y - radius < 0)
            //{
            //    velocity.y = (-velocity.y);
            //    count++;
            //}
        }

        public bool ParticleCollision(Particle p2)
        {
            //No Sqrt
            double distanceBetween = ((this.position.x - p2.position.x) * (this.position.x - p2.position.x))
                + ((this.position.y - p2.position.y) * (this.position.y - p2.position.y));
            double radiusSum = this.radius + p2.radius;

            if (radiusSum * radiusSum < distanceBetween)
                return false;

            //On collision calculate
            distanceBetween = Math.Sqrt(distanceBetween);

            //Angle and offset distance
            double angle = Math.Atan2(p2.position.y - this.position.y, p2.position.x - this.position.x);
            double distanceToMove = (radiusSum - distanceBetween) / 2;

            //Assign distance to move
            p2.position.x += Math.Cos(angle) * distanceToMove;
            p2.position.y += Math.Cos(angle) * distanceToMove;
            this.position.x -= Math.Cos(angle) * distanceToMove;
            this.position.y -= Math.Cos(angle) * distanceToMove;

            //Move accordingly to remove the offset
            p2.RenderTransform = new TranslateTransform(p2.position.x, p2.position.y);
            this.RenderTransform = new TranslateTransform(this.position.x, this.position.y);

            //Calculate Impulse
            distanceBetween = radiusSum;
            Vector2 tangent = new Vector2(p2.position.x - this.position.x, p2.position.y - this.position.y);
            Vector2 relativeVelocity = new Vector2(p2.velocity.x - this.velocity.x, p2.velocity.y - this.velocity.y);

            double impulse = (2 * this.mass * p2.mass * (Vector2.Dot(tangent, relativeVelocity))) / ((this.mass + p2.mass) * distanceBetween);
            Vector2 impulseVector = new Vector2(impulse * tangent.x, impulse * tangent.y) / distanceBetween;

            //Assign velocity, Newton's Second Law
            this.velocity += (impulseVector / this.mass);
            p2.velocity -= (impulseVector / p2.mass);

            count++;

            return true;
        }

        #endregion

        #region Collision Prediction Methods

        public double timeToHitParticle(Particle p1)
        {
            if (this.Equals(p1))
                return double.PositiveInfinity;

            Vector2 relativePosition = this.position - p1.position;
            Vector2 relativeVelocity = this.velocity - p1.velocity;
            double deltaPdeltaV = Vector2.Dot(relativePosition, relativeVelocity);

            if (deltaPdeltaV > 0)
                return double.PositiveInfinity;

            double distanceBetween = ((this.position.x - p1.position.x) * (this.position.x - p1.position.x))
                            + ((this.position.y - p1.position.y) * (this.position.y - p1.position.y));



            return 0;
        }

        public double timeToHitVerticalWall()
        {
            if (velocity.x == 0)
                return double.PositiveInfinity;

            if (this.velocity.x > 0)
                return (500 - this.position.x - this.radius) / this.velocity.x;

            return Math.Abs((this.position.x - this.radius) / this.velocity.x);
        }

        public double timeToHitHorizontalWall()
        {
            if (velocity.y == 0)
                return double.PositiveInfinity;

            if (this.velocity.y > 0)
                return (500 - this.position.y - this.radius) / this.velocity.y;

            return Math.Abs((this.position.y - this.radius) / this.velocity.y);
        }

        #endregion
    }
}