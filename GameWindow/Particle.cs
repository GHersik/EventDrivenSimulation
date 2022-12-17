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

namespace GameWindow
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
            this.particlePosition = position;
            this.particleVelocity = velocity;
            this.particleRadius = radius;

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

        public void Move()
        {
            particlePosition.x += particleVelocity.x;
            particlePosition.y += particleVelocity.y;

            this.RenderTransform = new TranslateTransform
                (particlePosition.x, particlePosition.y);
        }

        public void CollisionDetection(ref Canvas Render)
        {
            if (particlePosition.x + particleRadius > Render.Width || particlePosition.x < 0)
                particleVelocity.x = (-particleVelocity.x);

            if (particlePosition.y + particleRadius > Render.Height || particlePosition.y < 0)
                particleVelocity.y = (-particleVelocity.y);
        }
    }
}