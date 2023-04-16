using SimulationWindow;
using System;
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
            int randomOffSet = 16;

            //Position particles
            for (int i = 0; i < particles.Length; i++)
            {
                if (i % 12 == 0)
                {
                    y += 36;
                    x = 48;
                }
                SolidColorBrush customColor = new SolidColorBrush(Color.FromRgb((byte)rnd.Next(80, 255), (byte)rnd.Next(80, 255), (byte)rnd.Next(80, 255)));
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
                if (i % 20 == 0)
                {
                    y += 16;
                    x = 24;
                }
                Vector2 position = new Vector2(x + rnd.Next(-randomOffSet, randomOffSet), y + rnd.Next(-randomOffSet, randomOffSet));
                Vector2 velocity = new Vector2((rnd.Next(0, 2) * 2 - 1 * rnd.NextDouble()) * 1.5, (rnd.Next(0, 2) * 2 - 1 * rnd.NextDouble()) * 1.5);
                particles[i] = new Particle(position, velocity, 3, 1) { Fill = blue, Stroke = blue, StrokeThickness = 2 };
                x += 24;
            }
            particles[240] = new Particle(new Vector2(250, 400), new Vector2(rnd.Next(0, 2) * 2 - 1 * rnd.NextDouble(),
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
                Vector2 velocity = new Vector2((rnd.Next(0, 2) - 1 * rnd.NextDouble()) * 10, (rnd.Next(0, 2) - 1 * rnd.NextDouble()) * 10);
                particles[i] = new Particle(position, velocity, 6, 1) { Fill = red, Stroke = red, StrokeThickness = 2 };
                x += 50;
            }

            return particles;
        }

        public static Particle[] BilliardSample()
        {
            Particle[] particles = new Particle[11];
            double x = 220;
            double y = 100;
            int a = 4;
            int b = 4;
            double xoffset = 10;

            for (int i = 1; i < particles.Length; i++)
            {
                SolidColorBrush customColor = new SolidColorBrush(Color.FromRgb((byte)rnd.Next(1, 255), (byte)rnd.Next(1, 255), (byte)rnd.Next(1, 255)));

                Vector2 position = new Vector2(x, y);
                Vector2 velocity = new Vector2(0, 0);
                particles[i] = new Particle(position, velocity, 10, 1) { Fill = customColor, Stroke = customColor, StrokeThickness = 2 };
                x += 20.01;
                if (i == a)
                {
                    y += 18.1;
                    x = 220 + xoffset;
                    xoffset += 10.01;
                    b = b - 1;
                    a = a + b;
                }

            }

            //The bill to end them all
            Vector2 positionx = new Vector2(250, 440);
            Vector2 velocityx = new Vector2(rnd.Next(0, 2) - 1 * rnd.NextDouble(), rnd.Next(8, 15) * -1);
            particles[0] = new Particle(positionx, velocityx, 10, 1) { Fill = blue, Stroke = blue, StrokeThickness = 2 };

            return particles;
        }

        public static Particle[] Particles1600()
        {
            Particle[] particles = new Particle[1600];

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
                SolidColorBrush customColor = new SolidColorBrush(Color.FromRgb((byte)rnd.Next(80, 255), (byte)rnd.Next(80, 255), (byte)rnd.Next(80, 255)));
                Vector2 position = new Vector2(x + rnd.Next(-randomOffSet, randomOffSet), y + rnd.Next(-randomOffSet, randomOffSet));
                Vector2 velocity = new Vector2((rnd.Next(0, 2) - 1 * rnd.NextDouble()) * 0.3, (rnd.Next(0, 2) - 1 * rnd.NextDouble()) * 0.3);
                particles[i] = new Particle(position, velocity, 2, 1) { Fill = customColor, Stroke = customColor, StrokeThickness = 2 };
                x += 12;
            }

            return particles;
        }
    }
}

/***************************************************************************************
*    Title: <EventDrivenSimulation>
*    Author: <Gregory Werner>
*    Date: <16.04.2023>
*    Code version: <1.0.0>
*    Availability: <https://github.com/GHersik/EventDrivenSimulation>
*
***************************************************************************************/