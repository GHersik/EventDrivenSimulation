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
        private double particleMass;

        public Particle(Vector2 position, Vector2 velocity, double radius, double mass)
        {
            //Assign properties
            this.particleRadius = radius;
            this.particlePosition = new Vector2(position.x - particleRadius, position.y - particleRadius);
            this.particleVelocity = velocity;
            this.particleMass = mass;

            //Position accordingly
            this.RenderTransform = new TranslateTransform
                (particlePosition.x, particlePosition.y);
        }

        public Particle() : this(Vector2.Zero, Vector2.One, 9, 1) { }

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

        public void WallCollision(ref Canvas Render)
        {
            //Wall
            //X axis
            if (particlePosition.x + this.Width > Render.Width || particlePosition.x < 0)
                particleVelocity.x = (-particleVelocity.x);

            //Y axis
            if (particlePosition.y + this.Width > Render.Height || particlePosition.y < 0)
                particleVelocity.y = (-particleVelocity.y);
        }

        public bool ParticleCollision(Particle p1, Particle p2)
        {
            double radiusSum = p1.particleRadius + p2.particleRadius;
            //No Sqrt
            double distanceBetween = ((p1.particlePosition.x - p2.particlePosition.x) * (p1.particlePosition.x - p2.particlePosition.x))
                + ((p1.particlePosition.y - p2.particlePosition.y) * (p1.particlePosition.y - p2.particlePosition.y));

            if (radiusSum * radiusSum < distanceBetween)
                return false;

            //On collision calculate
            //Angle and offset distance
            double angle = Math.Atan2(p2.particlePosition.y - p1.particlePosition.y,
                p2.particlePosition.x - p1.particlePosition.x);
            double distanceToMove = radiusSum - Math.Sqrt(distanceBetween);

            //Assign distance to move
            p2.particlePosition.x += Math.Cos(angle) * distanceToMove;
            p2.particlePosition.y += Math.Cos(angle) * distanceToMove;

            //Move
            p2.RenderTransform = new TranslateTransform(p2.particlePosition.x, p2.particlePosition.y);

            //Linear algebra
            //Vector perpendicular to (x, y) is (-y, x)
            Vector2 tangentVector = new Vector2(-(p2.particlePosition.y - p1.particlePosition.y),
                (p2.particlePosition.x - p1.particlePosition.x));
            tangentVector.Normalize();

            Vector2 relativeVelocity = new Vector2(p2.particleVelocity.x - p1.particleVelocity.x,
                p2.particleVelocity.y - p1.particleVelocity.y);
            double length = Vector2.Dot(tangentVector, relativeVelocity);

            Vector2 velocityComponentOnTangent = tangentVector * length;
            Vector2 velocityComponentPerpendicularToTangent = relativeVelocity - velocityComponentOnTangent;

            p2.particleVelocity.x = (-velocityComponentPerpendicularToTangent.x);
            p2.particleVelocity.y = (-velocityComponentPerpendicularToTangent.y);
            p1.particleVelocity = velocityComponentPerpendicularToTangent;


            //Newton's second law


            return true;
        }
    }
}