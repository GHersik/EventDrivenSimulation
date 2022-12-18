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

namespace SimulationWindow
{
    /// <summary>
    /// Initializes an instance of a particle.
    /// </summary>
    /// <param name="particlePosition">Position of a particle using x and y axis.</param>
    /// <param name="particleVelocity">Velocity of a particle using x and y axis.</param>
    /// <param name="particleRadius">Radius of a particle.</param>
    public class Particle : Shape
    {
        private Vector2 particlePosition;
        private Vector2 particleVelocity;
        private double particleRadius;

        public Particle(Vector2 position, Vector2 velocity, double radius)
        {
            //Assign properties
            this.particleRadius = radius;
            this.particlePosition = new Vector2(position.x - particleRadius, position.y - particleRadius);
            this.particleVelocity = velocity;


            //Position accordingly
            this.RenderTransform = new TranslateTransform
                (particlePosition.x, particlePosition.y);
        }

        public Particle() : this(Vector2.Zero, Vector2.One, 9) { }

        protected override Geometry DefiningGeometry
        {
            get
            {
                return new EllipseGeometry(new Rect(0, 0, this.Width - 2, this.Height - 2));
            }
        }

        public void Move(double deltaTime)
        {
            particlePosition += new Vector2(particleVelocity.x * deltaTime, particleVelocity.y * deltaTime);

            this.RenderTransform = new TranslateTransform
                (particlePosition.x, particlePosition.y);
        }

        public void CollisionDetection(ref Canvas Render)
        {
            //Wall
            //X axis
            if (particlePosition.x + this.Width > Render.Width || particlePosition.x < 0)
                particleVelocity.x = (-particleVelocity.x);

            //Y axis
            if (particlePosition.y + this.Width > Render.Height || particlePosition.y < 0)
                particleVelocity.y = (-particleVelocity.y);
        }

        public void ParticleCollision(Particle p1, Particle p2)
        {
            double radiusSum = p1.particleRadius + p2.particleRadius;
            double distanceBetween = Math.Sqrt(Math.Pow(p1.particlePosition.x - p2.particlePosition.x, 2)
                + Math.Pow(p1.particlePosition.y - p2.particlePosition.y, 2));

            if (radiusSum < distanceBetween)
                return;

            //Angle and offset distance
            double angle = Math.Atan2(p2.particlePosition.y - p1.particlePosition.y,
                p2.particlePosition.x - p1.particlePosition.x);
            double distanceToMove = radiusSum - distanceBetween;

            //Assign distance to move
            p2.particlePosition.x += Math.Cos(angle) * distanceToMove;
            p2.particlePosition.y += Math.Cos(angle) * distanceToMove;

            //Move
            p2.RenderTransform = new TranslateTransform(p2.particlePosition.x, p2.particlePosition.y);

            //Vector perpendicular to (x, y) is (-y, x)
            Vector2 tangentVector = new Vector2(p2.particlePosition.y - p1.particlePosition.y, -(p2.particlePosition.x - p1.particlePosition.x));

            tangentVector.Normalize();
        }
    }
}