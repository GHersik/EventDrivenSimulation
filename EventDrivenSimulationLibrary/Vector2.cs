using System;

namespace EventDrivenSimulationLibrary
{
    /// <summary>
    /// Represents two points x and y as coordinates on a 2D plane.
    /// </summary>
    /// <param>x axis</param>>
    /// <param>y axis</param>>
    public struct Vector2
    {
        public double x;
        public double y;

        public static readonly Vector2 One = new Vector2(1.0, 1.0);
        public static readonly Vector2 Zero = new Vector2(0, 0);

        public Vector2(double x = 0, double y = 0)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Makes this vector have a magnitude of 1 which makes its length 1 as well.
        /// If the vector is too small it will be set to 0.
        /// </summary>
        public void Normalize()
        {
            double magnitude = Math.Sqrt((this.x * this.x) + (this.y * this.y));
            if (magnitude == 0)
            {
                this = Vector2.Zero;
                return;
            }

            this.x /= magnitude;
            this.y /= magnitude;
        }

        /// <summary>
        /// Returns the scalar of a dot product between the two vectors.
        /// </summary>
        /// <param name="a">Vector a</param>
        /// <param name="b">Vector b</param>
        /// <returns></returns>
        public static double Dot(Vector2 a, Vector2 b) => (a.x * b.x) + (a.y * b.y);

        /// <summary>
        /// Adds two vectors to each other.
        /// </summary>
        /// <param name="a">Vector2 to add to.</param>
        /// <param name="b">Vector2 being added.</param>
        /// <returns>New Vector2 as a result of adding x and y axis of one Vector2 to another.</returns>
        public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.x + b.x, a.y + b.y);

        /// <summary>
        /// Subtracts two vectors from each one another.
        /// </summary>
        /// <param name="a">Vector to subtract from.</param>
        /// <param name="b">Vector being subtracted by.</param>
        /// <returns></returns>
        public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.x - b.x, a.y - b.y);

        /// <summary>
        /// Multiplies given vector by a specific scalar.
        /// </summary>
        /// <param name="vectorToMultiply"></param>
        /// <param name="scalar"></param>
        /// <returns>Vector2 multiplied by a specified scalar</returns>
        public static Vector2 operator *(Vector2 vectorToMultiply, double scalar) => new Vector2(vectorToMultiply.x * scalar, vectorToMultiply.y * scalar);

        /// <summary>
        /// Divides given vector by a specific scalar.
        /// </summary>
        /// <param name="vectorToDivide"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vector2 operator /(Vector2 vectorToMultiply, double scalar) => new Vector2(vectorToMultiply.x / scalar, vectorToMultiply.y / scalar);
    }
}