using SimulationWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SimulationRender
{
    public delegate Particle[] ParticleDelegate();

    /// <summary>
    /// Contains all of the simulations for the EventDrivenSimulation app to choose from.
    /// </summary>
    public static class Simulations
    {
        private static readonly Random rnd = new Random();

        private static SolidColorBrush blue = (SolidColorBrush)new BrushConverter().ConvertFrom("#4D96FF");
        private static SolidColorBrush yellow = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFD93D");
        private static SolidColorBrush red = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF6B6B");

        public static Particle[] Ambient()
        {
            Particle[] particles = new Particle[144];

            //Spawn each n-th position
            double x = 0;
            double y = 20;

            //Position randomly off the center
            int randomOffSet = 15;

            //Position particles
            for (int i = 0; i < particles.Length; i++)
            {
                if (i % 12 == 0)
                {
                    y += 36;
                    x = 48;
                }
                SolidColorBrush customColor = new SolidColorBrush(Color.FromRgb((byte)rnd.Next(1, 255), (byte)rnd.Next(1, 255), (byte)rnd.Next(1, 255)));
                Vector2 position = new Vector2(x + rnd.Next(-randomOffSet, randomOffSet), y + rnd.Next(-randomOffSet, randomOffSet));
                Vector2 velocity = new Vector2(rnd.Next(0, 2) - 1 * rnd.NextDouble(), rnd.Next(0, 2) - 1 * rnd.NextDouble());
                particles[i] = new Particle(position, velocity, 3, 1) { Fill = customColor, Stroke = customColor, StrokeThickness = 2 };
                x += 36;
            }

            return particles;
        }

        public static Particle[] BrownianMotion()
        {
            Particle[] particles = new Particle[241];

            //Spawn each n-th position
            double x = 0;
            double y = 6;

            //Position randomly off the center
            int randomOffSet = 0;

            //Position particles
            for (int i = 0; i < particles.Length; i++)
            {
                if (i % 40 == 0)
                {
                    y += 12;
                    x = 18;
                }
                Vector2 position = new Vector2(x + rnd.Next(-randomOffSet, randomOffSet), y + rnd.Next(-randomOffSet, randomOffSet));
                Vector2 velocity = new Vector2(rnd.Next(0, 2) * 2 - 1 * rnd.NextDouble() * 2, rnd.Next(0, 2) * 2 - 1 * rnd.NextDouble() * 2);
                particles[i] = new Particle(position, velocity, 3, 1) { Fill = blue, Stroke = blue, StrokeThickness = 2 };
                x += 12;
            }
            particles[240] = new Particle(new Vector2(250, 250), new Vector2(rnd.Next(0, 2) * 2 - 1 * rnd.NextDouble(),
               rnd.Next(0, 2) * 2 - 1 * rnd.NextDouble()), 26, 10)
            { Fill = yellow, Stroke = yellow, StrokeThickness = 2 };

            return particles;
        }

        public static Particle[] FastParticles()
        {
            Particle[] particles = new Particle[81];

            //Spawn each n-th position
            double x = 0;
            double y = 0;

            //Position randomly off the center
            int randomOffSet = 20;

            //Position particles
            for (int i = 0; i < particles.Length; i++)
            {
                if (i % 9 == 0)
                {
                    y += 50;
                    x = 50;
                }

                Vector2 position = new Vector2(x + rnd.Next(-randomOffSet, randomOffSet), y + rnd.Next(-randomOffSet, randomOffSet));
                Vector2 velocity = new Vector2(rnd.Next(0, 2) - 1 * rnd.NextDouble(), rnd.Next(9, 16) * rnd.NextDouble());
                particles[i] = new Particle(position, velocity, 6, 1) { Fill = red, Stroke = red, StrokeThickness = 2 };
                x += 50;
            }

            return particles;
        }
    }
}
