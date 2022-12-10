using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Windows.Media;
using System.Drawing;

namespace My2DPhysicsLibrary
{
    /// <summary>
    /// Initializes an instance of a particle.
    /// </summary>
    /// <param name="particlePosition">Position of a particle using x and y axis.</param>
    /// <param name="particleVelocity">Velocity of a particle using x and y axis.</param>
    /// <param name="particleRadius">Radius of a particle.</param>
    /// <param name="particleColor">Color of a particle.</param>
    public class Particle //: DrawingVisual
    {
        private Vector2 particlePosition;
        private Vector2 particleVelocity;
        private float particleRadius;
        private Color particleColor;

        public Particle(Vector2 position, Vector2 velocity, float radius, Color color)
        {
            this.particlePosition = position;
            this.particleVelocity = velocity;
            this.particleRadius = radius;
            this.particleColor = color;
        }

        public Particle() : this(Vector2.One, Vector2.One, 1, Color.Black) { }

        //to do
        public void Move()
        {
            throw new NotImplementedException();
        }

        //to do
        public void Draw()
        {
            throw new NotImplementedException();
        }
    }
}