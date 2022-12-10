using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My2DPhysicsLibrary
{
    readonly public struct Vector2
    {
        readonly public float x;
        readonly public float y;

        public static readonly Vector2 One = new Vector2(1.0f, 1.0f);
        public static readonly Vector2 Zero = new Vector2(0, 0);

        public Vector2(float x = 0, float y = 0)
        {
            this.x = x;
            this.y = y;
        }
    }
}
