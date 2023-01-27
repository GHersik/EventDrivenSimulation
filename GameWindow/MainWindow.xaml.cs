global using EventDrivenSimulationLibrary;
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
using System.Threading;
using SimulationRender;
using SimulationRender.Properties;
using System.Globalization;
using System.Drawing;
using System.Reflection;

namespace SimulationWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Color palette
        static SolidColorBrush green = (SolidColorBrush)new BrushConverter().ConvertFrom("#6BCB77");
        static SolidColorBrush yellow = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFD93D");
        static SolidColorBrush red = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF6B6B");
        private static SolidColorBrush blue = (SolidColorBrush)new BrushConverter().ConvertFrom("#4D96FF");

        private readonly Random rnd = new Random();

        //UI elements
        Line distanceBetweenParticles = new Line() { Stroke = green, StrokeThickness = 2 };
        TextBlock p1text = new TextBlock() { Text = "p1", Foreground = yellow, FontSize = 14, FontWeight = FontWeights.Bold, Visibility = Visibility.Hidden };
        TextBlock p2text = new TextBlock() { Text = "p2", Foreground = yellow, FontSize = 14, FontWeight = FontWeights.Bold, Visibility = Visibility.Hidden };
        Line velocityVectorp1 = new Line() { Stroke = red, StrokeThickness = 2 };


        //Simulation time
        private readonly DispatcherTimer Time = new DispatcherTimer();
        private static double quantaTime = 30;
        private TimeSpan deltaTime = TimeSpan.FromMilliseconds(quantaTime);
        private TimeSpan totalTimeElapsed = TimeSpan.Zero;

        //Particles
        private Particle[] particles;
        private Particle? p1 = null;
        private Particle? p2 = null;

        //Event driven collision logic
        EventDrivenCollisionSystem collisionSystem;

        public MainWindow()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();

            //ParticleUIElements
            Render.Children.Add(distanceBetweenParticles);
            Render.Children.Add(p1text);
            Render.Children.Add(p2text);
            Render.Children.Add(velocityVectorp1);

            //Setup simulation
            Time.Tick += Update;
            Time.Interval = deltaTime;
            //Time.Start();

            //max 1600
            particles = IntializeParticles(20);
            collisionSystem = new EventDrivenCollisionSystem(particles, quantaTime);
        }

        private void Update(object sender, EventArgs e)
        {
            //Solver approach
            if (collisionSystem.NextCollision <= totalTimeElapsed)
            {
                collisionSystem.Solver();
                collisionSystem.CalculateNextCollision();
            }
            else
            {
                for (int i = 0; i < particles.Length; i++)
                {
                    particles[i].Move(1);
                    particles[i].Draw();
                }
            }

            //Track each particle
            TrackParticles();
            DrawVector(p1);

            //both Particles
            DrawDistanceLine();

            //TrackCollisions
            CollisionsPerSecond();

            totalTimeElapsed += deltaTime;
        }

        #region Update UI Elements
        private void TrackParticlep1(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Particle)
            {
                p1 = (Particle)e.OriginalSource;
                p1text.RenderTransform = new TranslateTransform(p1.position.x - (p1.Width), p1.position.y - (p1.Height + p1.Height));
                DrawDistanceLine();
                DrawVector(p1);
                TrackParticles();

                //Show UI
                Particle1.Visibility = Visibility.Visible;
                p1text.Visibility = Visibility.Visible;
            }
            else
            {
                p1 = null;
                DrawDistanceLine();
                DrawVector(p1);

                //Hide UI
                Particle1.Visibility = Visibility.Hidden;
                p1text.Visibility = Visibility.Hidden;
            }

        }
        private void TrackParticles()
        {
            if (p1 != null)
            {
                p1Position.Text = $"({p1.position.x.ToString("00.00")} , {p1.position.y.ToString("00.00")})";
                particleVelocity.Text = $"({p1.velocity.x.ToString("00.00")} , {p1.velocity.y.ToString("00.00")})";
                particleRadius.Text = p1.radius.ToString();
                particleMass.Text = p1.mass.ToString();

                p1text.RenderTransform = new TranslateTransform(p1.position.x - (p1.radius + 10), p1.position.y - (p1.radius + 20));
            }

            if (p2 != null)
            {
                particle2Position.Text = $"({p2.position.x.ToString("00.00")} , {p2.position.y.ToString("00.00")})";
                particle2Velocity.Text = $"({p2.velocity.x.ToString("00.00")} , {p2.velocity.y.ToString("00.00")})";
                particle2Radius.Text = p2.radius.ToString();
                particle2Mass.Text = p2.mass.ToString();

                p2text.RenderTransform = new TranslateTransform(p2.position.x - (p2.Width + 10), p2.position.y - (p2.Height + 20));
            }
        }
        private void TrackParticlep2(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Particle)
            {
                p2 = (Particle)e.OriginalSource;
                p2text.RenderTransform = new TranslateTransform(p2.position.x - (p2.Width), p2.position.y - (p2.Height + p2.Height));
                DrawDistanceLine();
                TrackParticles();

                //Show UI
                Particle2.Visibility = Visibility.Visible;
                p2text.Visibility = Visibility.Visible;
            }
            else
            {
                p2 = null;
                DrawDistanceLine();

                //Hide UI
                Particle2.Visibility = Visibility.Hidden;
                p2text.Visibility = Visibility.Hidden;
            }
        }
        private void DrawDistanceLine()
        {
            if (p1 != null && p2 != null)
            {
                distanceBetweenParticles.X1 = p1.position.x;
                distanceBetweenParticles.Y1 = p1.position.y;

                distanceBetweenParticles.X2 = p2.position.x;
                distanceBetweenParticles.Y2 = p2.position.y;
            }
            else
            {
                distanceBetweenParticles.X1 = 0;
                distanceBetweenParticles.Y1 = 0;

                distanceBetweenParticles.X2 = 0;
                distanceBetweenParticles.Y2 = 0;
            }
        }
        private void DrawVector(Particle particle)
        {
            if (particle == null)
            {
                velocityVectorp1.X1 = 0;
                velocityVectorp1.Y1 = 0;
                velocityVectorp1.X2 = 0;
                velocityVectorp1.Y2 = 0;

                return;
            }

            velocityVectorp1.X1 = particle.position.x;
            velocityVectorp1.Y1 = particle.position.y;
            velocityVectorp1.X2 = particle.position.x + particle.velocity.x * 2 * particle.radius;
            velocityVectorp1.Y2 = particle.position.y + particle.velocity.y * 2 * particle.radius;

            //Line main = new Line()
            //{
            //    Stroke = red,
            //    StrokeThickness = 2,
            //    X1 = particle.position.x,
            //    Y1 = particle.position.y,
            //    X2 = particle.position.x + particle.velocity.x * 6,
            //    Y2 = particle.position.y + particle.velocity.y * 6
            //};

            //Line left = new Line()
            //{
            //    Stroke = red,
            //    StrokeThickness = 2,
            //    X1 = main.X2,
            //    Y1 = main.Y1,

            //    X2 = main.X2 * 6,
            //    Y2 = main.Y2 * 6
            //};

            //RotateTransform rotateTransform2 = new RotateTransform(45);
            //rotateTransform2.CenterX = 0;
            //rotateTransform2.CenterY = 0;
            //left.RenderTransform = rotateTransform2;

            ////Line right = new Line() { Stroke = red, StrokeThickness = 2 };

            //Render.Children.Add(main);
            //Render.Children.Add(left);
        }
        private void CollisionsPerSecond() => CollisionsCounter.Text = (collisionSystem.CollisionCounter / totalTimeElapsed.TotalSeconds).ToString("00.00");
        #endregion


        /// <summary>
        /// Initializes one big and lots of small particles with increased velocity.
        /// </summary>
        /// <returns></returns>
        private Particle[] BrownianMotion()
        {
            Particle[] particles = new Particle[400];

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
                Vector2 velocity = new Vector2(rnd.NextDouble() * 2, rnd.NextDouble() * 2);
                particles[i] = new Particle(position, velocity, 2, 1) { Fill = blue, Stroke = blue, StrokeThickness = 2 };
                Render.Children.Add(particles[i]);

                x += 12;
            }

            //Big one
            particles[100] = new Particle(new Vector2(250, 250), new Vector2(-1 * rnd.NextDouble(), 1 * rnd.NextDouble()), 26, 10) { Fill = yellow };
            Render.Children.Remove(particles[100]);
            Render.Children.Add(particles[100]);

            ParticlesCounter.Text = Convert.ToString(particles.Length);
            return particles;
        }

        private Particle[] IntializeParticles(int amountToSpawn)
        {
            //Max 1600
            Particle[] particles = new Particle[amountToSpawn];

            //Spawn each n-th position
            double x = 0;
            double y = 6;

            //Position randomly off the center, max 16
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
                Vector2 velocity = new Vector2(rnd.NextDouble() * 1, rnd.NextDouble() * 1);
                particles[i] = new Particle(position, velocity, 2, 1) { Fill = blue, Stroke = blue, StrokeThickness = 2 };
                Render.Children.Add(particles[i]);

                x += 12;
            }

            ParticlesCounter.Text = Convert.ToString(particles.Length);
            return particles;
        }

        #region Buttons
        private void TimeButton_Click(object sender, RoutedEventArgs e)
        {
            if (Time.IsEnabled)
            {
                TimeButton.Content = "Start Time";
                Time.Stop();
            }
            else
            {
                TimeButton.Content = "Stop Time";
                Time.Start();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            ResetSimulation();


            particles = BrownianMotion();
            collisionSystem = new EventDrivenCollisionSystem(particles, quantaTime);
        }

        /// <summary>
        /// Resets the entire simulation to a point where no particles are on a scene.
        /// </summary>
        private void ResetSimulation()
        {
            Time.Stop();
            p1 = null;
            p2 = null;


            totalTimeElapsed = TimeSpan.Zero;
            for (int i = 0; i < particles.Length; i++)
                Render.Children.Remove(particles[i]);

            particles = null;
            collisionSystem = null;
        }
        #endregion
    }
}
