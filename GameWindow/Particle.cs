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
        private Vector2 position, velocity;
        private double radius, mass;

        private static readonly Random rnd = new Random();

        #region Constructors

        public Particle(Vector2 position, Vector2 velocity, double radius, double mass)
        {
            //Assign properties
            this.radius = radius;
            this.position = position;
            this.velocity = velocity;
            this.mass = mass;

            this.Width = radius + radius;
            this.Height = radius + radius;
            StrokeThickness = 1;
            Stroke = Brushes.Black;
            Fill = new SolidColorBrush(Color.FromRgb((byte)rnd.Next(1, 255), (byte)rnd.Next(1, 255), (byte)rnd.Next(1, 255)));

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
        public void VerticalWallCollision(ref Canvas Render)
        {
            //X axis
            if (position.x + radius > Render.Width || position.x - radius < 0)
                velocity.x = (-velocity.x);
        }

        public void HorizontalWallCollision(ref Canvas Render)
        {
            //Y axis
            if (position.y + radius > Render.Height || position.y - radius < 0)
                velocity.y = (-velocity.y);
        }

        public bool ParticleCollision(Particle p1, Particle p2)
        {
            //No Sqrt
            double distanceBetween = ((p1.position.x - p2.position.x) * (p1.position.x - p2.position.x))
                + ((p1.position.y - p2.position.y) * (p1.position.y - p2.position.y));
            double radiusSum = p1.radius + p2.radius;

            if (radiusSum * radiusSum <= distanceBetween)
                return false;

            //On collision calculate
            distanceBetween = Math.Sqrt(distanceBetween);

            //Angle and offset distance
            double angle = Math.Atan2(p2.position.y - p1.position.y, p2.position.x - p1.position.x);
            double distanceToMove = (radiusSum - distanceBetween) / 2;

            //Assign distance to move
            p2.position.x += Math.Cos(angle) * distanceToMove;
            p2.position.y += Math.Cos(angle) * distanceToMove;
            p1.position.x -= Math.Cos(angle) * distanceToMove;
            p1.position.y -= Math.Cos(angle) * distanceToMove;

            //Move accordingly to remove the offset
            p2.RenderTransform = new TranslateTransform(p2.position.x, p2.position.y);
            p1.RenderTransform = new TranslateTransform(p1.position.x, p1.position.y);

            //Calculate Impulse
            distanceBetween = radiusSum;
            Vector2 tangent = new Vector2(p2.position.x - p1.position.x, p2.position.y - p1.position.y);
            Vector2 relativeVelocity = new Vector2(p2.velocity.x - p1.velocity.x, p2.velocity.y - p1.velocity.y);

            double impulse = (2 * p1.mass * p2.mass * (Vector2.Dot(tangent, relativeVelocity))) / ((p1.mass + p2.mass) * distanceBetween);
            Vector2 impulseVector = new Vector2(impulse * tangent.x, impulse * tangent.y) / distanceBetween;

            //Assign velocity, Newton's Second Law
            p1.velocity += (impulseVector / p1.mass);
            p2.velocity -= (impulseVector / p2.mass);

            return true;
        }

        #endregion

        #region Collision Prediction Methods

        public double timeToHitParticle(Particle p1)
        {


            return 0;
        }

        public double timeToHitVerticalWall(ref Canvas Render)
        {
            if (velocity.x == 0)
                return double.PositiveInfinity;

            if (this.velocity.x > 0)
                return (Render.Width - this.position.x - this.radius) / this.velocity.x;

            return Math.Abs((this.position.x - this.radius) / this.velocity.x);
        }

        public double timeToHitHorizontalWall(ref Canvas Render)
        {
            if (velocity.y == 0)
                return double.PositiveInfinity;

            if (this.velocity.y > 0)
                return (Render.Width - this.position.y - this.radius) / this.velocity.y;

            return Math.Abs((this.position.y - this.radius) / this.velocity.y);
        }

        #endregion
    }
}