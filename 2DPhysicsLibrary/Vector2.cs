using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DPhysicsLibrary
{
    public struct Vector2
    {
        public float x;
        public float y;

        public static float one = 1;

        public Vector2(float x = 0, float y = 0)
        {
            this.x = x;
            this.y = y;
        }
    }
}
