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
using EventDrivenSimulationLibrary;
using System.Drawing;

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

        private readonly Random rnd = new Random();

        //UI elements
        Line distanceBetweenParticles = new Line() { Stroke = green, StrokeThickness = 2 };
        TextBlock p1text = new TextBlock() { Text = "p1", Foreground = yellow, FontSize = 14, FontWeight = FontWeights.Bold, Visibility = Visibility.Hidden };
        TextBlock p2text = new TextBlock() { Text = "p2", Foreground = yellow, FontSize = 14, FontWeight = FontWeights.Bold, Visibility = Visibility.Hidden };
        Line velocityVectorp1 = new Line() { Stroke = red, StrokeThickness = 2 };


        //Simulation time
        private readonly DispatcherTimer Time = new DispatcherTimer();
        private static double deltaTime = 0.020;
        private TimeSpan fixedTime = TimeSpan.FromMilliseconds(deltaTime * 1000);
        private TimeSpan totalTimeElapsed = TimeSpan.Zero;

        //Particles
        private Particle[] particles;
        private Particle? p1 = null;
        private Particle? p2 = null;

        //Event driven simulation logic
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
            Time.Interval = fixedTime;
            Time.Start();


            //Spawn particles
            //foreach (var particle in SpawnParticles(11, 16, 1, new Vector2(150, 150), TimeSpan.Zero))
            //{
            //    Render.Children.Add(particle);
            //    particles.Add(particle);
            //}


            particles = IntializeParticles();
            collisionSystem = new EventDrivenCollisionSystem(particles);
        }

        private void Update(object sender, EventArgs e)
        {
            //Event Driven Simulation O(n log n), initialization O(N^2)
            if (collisionSystem.nextCollision <= totalTimeElapsed)
            {
                collisionSystem.ResolveCollision();
                collisionSystem.CalculateNextCollision();
            }
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].Move(1);
                particles[i].Draw();
            }

            //Time Driven Simulation O(n^2), no initialization
            //for (int i = 0; i < particles.Length; i++)
            //{
            //    particles[i].VerticalWallCollision();
            //    particles[i].HorizontalWallCollision();

            //    for (int j = i; j < particles.Length - 1; j++)
            //        particles[i].ParticleCollision(particles[j + 1]);

            //    particles[i].Move(1);
            //    particles[i].Draw();
            //}

            //Track each particle
            TrackParticle1Stats();
            TrackParticle2Stats();
            DrawVector(p1);

            //both Particles
            DrawDistanceLine();

            totalTimeElapsed += fixedTime;
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
        private void TrackParticle1Stats()
        {
            //p1 position
            if (p1 != null)
            {
                p1Position.Text = $"({p1.position.x.ToString("00.00")} , {p1.position.y.ToString("00.00")})";
                particleVelocity.Text = $"({p1.velocity.x.ToString("00.00")} , {p1.velocity.y.ToString("00.00")})";
                particleRadius.Text = p1.radius.ToString();
                particleMass.Text = p1.mass.ToString();

                p1text.RenderTransform = new TranslateTransform(p1.position.x - (p1.Width), p1.position.y - (p1.Height + p1.Height));
            }
        }
        private void TrackParticlep2(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Particle)
            {
                p2 = (Particle)e.OriginalSource;
                p2text.RenderTransform = new TranslateTransform(p2.position.x - (p2.Width), p2.position.y - (p2.Height + p2.Height));
                DrawDistanceLine();

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
        private void TrackParticle2Stats()
        {
            //p2 position
            if (p2 != null)
            {
                particle2Position.Text = $"({p2.position.x.ToString("00.00")} , {p2.position.y.ToString("00.00")})";
                particle2Velocity.Text = $"({p2.velocity.x.ToString("00.00")} , {p2.velocity.y.ToString("00.00")})";
                particle2Radius.Text = p2.radius.ToString();
                particle2Mass.Text = p2.mass.ToString();

                p2text.RenderTransform = new TranslateTransform(p2.position.x - (p2.Width), p2.position.y - (p2.Height + p2.Height));
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
            velocityVectorp1.X2 = particle.position.x + particle.velocity.x * 6;
            velocityVectorp1.Y2 = particle.position.y + particle.velocity.y * 6;

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
        #endregion

        //IEnumerable<Particle> SpawnParticles(int amountToSpawn, double size, double mass, Vector2 position, TimeSpan interval)
        //{
        //    for (int i = 0; i < amountToSpawn; i++)
        //    {
        //        TimeSpan timeBetween = totalTimeElapsed + interval;

        //        if (timeBetween == totalTimeElapsed)
        //            yield return new Particle(position,
        //            new Vector2(rnd.Next(-10, 10) * rnd.NextDouble(), rnd.Next(-10, 10) * rnd.NextDouble()),
        //            size / 2, mass);
        //    }

        //    ParticlesCounter.Text = Convert.ToString(particles.Count());
        //}

        private Particle[] IntializeParticles()
        {
            //Spawn 100 particles
            Particle[] particles = new Particle[100];
            int x = 25;
            int y = 25;

            //max 16
            int randomOffSet = 16;

            for (int i = 0; i < 100; i += 10)
            {
                for (int j = 0; j < 10; j++)
                {
                    particles[i + j] = new Particle(new Vector2(x + rnd.Next(-randomOffSet, randomOffSet), y + rnd.Next(-randomOffSet, randomOffSet)),
                        new Vector2(rnd.Next(-10, 10) * rnd.NextDouble(), rnd.Next(-10, 10) * rnd.NextDouble()), 14 / 2, 1);
                    Render.Children.Add(particles[i + j]);
                    x += 50;
                }

                y += 50;
                x = 25;
            }

            ParticlesCounter.Text = Convert.ToString(particles.Length);
            return particles;
        }

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
    }
}
