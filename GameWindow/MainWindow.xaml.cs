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
    public partial class MainWindow : Window
    {
        #region Color palette
        private static SolidColorBrush green = (SolidColorBrush)new BrushConverter().ConvertFrom("#6BCB77");
        private static SolidColorBrush yellow = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFD93D");
        private static SolidColorBrush red = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF6B6B");
        private static SolidColorBrush blue = (SolidColorBrush)new BrushConverter().ConvertFrom("#4D96FF");
        private static RadialGradientBrush transparentToRed = new RadialGradientBrush()
        {
            GradientOrigin = new System.Windows.Point(0.5, 0.5),
            Center = new System.Windows.Point(0.5, 0.5),
            RadiusX = 0.5,
            RadiusY = 0.5,
            GradientStops = { new GradientStop(red.Color, 0), new GradientStop(Colors.Transparent, 2) }
        };

        #endregion

        #region UI elements
        Line distanceBetweenParticles = new Line() { Stroke = green, StrokeThickness = 2, Visibility = Visibility.Hidden };
        TextBlock p1text = new TextBlock() { Text = "p1", Foreground = yellow, FontSize = 14, FontWeight = FontWeights.Bold, Visibility = Visibility.Hidden };
        TextBlock p2text = new TextBlock() { Text = "p2", Foreground = yellow, FontSize = 14, FontWeight = FontWeights.Bold, Visibility = Visibility.Hidden };
        //Velocity Vectors p1
        Line vectorVelp1 = new Line() { StrokeThickness = 2, Stroke = transparentToRed }; //Visibility = Visibility.Visible};
        Line vectorVelp1Right = new Line() { StrokeThickness = 2, Stroke = red };
        Line vectorVelp1Left = new Line() { StrokeThickness = 2, Stroke = red };
        //p2
        Line vectorVelp2 = new Line() { StrokeThickness = 2, Stroke = transparentToRed };
        Line vectorVelp2Right = new Line() { StrokeThickness = 2, Stroke = red };
        Line vectorVelp2Left = new Line() { StrokeThickness = 2, Stroke = red };
        #endregion

        private static readonly Random rnd = new Random();

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

            #region Add particle UI elements
            //p1
            Render.Children.Add(p1text);
            Render.Children.Add(vectorVelp1);
            Render.Children.Add(vectorVelp1Left);
            Render.Children.Add(vectorVelp1Right);
            //p2
            Render.Children.Add(p2text);
            Render.Children.Add(vectorVelp2);
            Render.Children.Add(vectorVelp2Left);
            Render.Children.Add(vectorVelp2Right);

            Render.Children.Add(distanceBetweenParticles);
            #endregion

            //Setup simulation
            Time.Tick += Update;
            Time.Interval = deltaTime;

            //Intialize collision system
            particles = IntializeParticles(600);
            collisionSystem = new EventDrivenCollisionSystem(particles, quantaTime);
        }

        private void Update(object sender, EventArgs e)
        {
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
            UpdateVectors();

            //TrackCollisions
            UpdateCollisionsPerSecond();

            totalTimeElapsed += deltaTime;
        }

        #region Update UI Elements
        /// <summary>
        /// Shows or hides particle statistics along with the distance between the two.
        /// </summary>
        private void ShowHideParticlesUI()
        {
            if (p1 != null)
            {
                Particle1.Visibility = Visibility.Visible;
                p1text.Visibility = Visibility.Visible;

                vectorVelp1.Visibility = Visibility.Visible;
                vectorVelp1Right.Visibility = Visibility.Visible;
                vectorVelp1Left.Visibility = Visibility.Visible;
            }
            else
            {
                Particle1.Visibility = Visibility.Hidden;
                p1text.Visibility = Visibility.Hidden;

                vectorVelp1.Visibility = Visibility.Hidden;
                vectorVelp1Right.Visibility = Visibility.Hidden;
                vectorVelp1Left.Visibility = Visibility.Hidden;
            }

            if (p2 != null)
            {
                Particle2.Visibility = Visibility.Visible;
                p2text.Visibility = Visibility.Visible;

                vectorVelp2.Visibility = Visibility.Visible;
                vectorVelp2Right.Visibility = Visibility.Visible;
                vectorVelp2Left.Visibility = Visibility.Visible;
            }
            else
            {
                Particle2.Visibility = Visibility.Hidden;
                p2text.Visibility = Visibility.Hidden;

                vectorVelp2.Visibility = Visibility.Hidden;
                vectorVelp2Right.Visibility = Visibility.Hidden;
                vectorVelp2Left.Visibility = Visibility.Hidden;
            }

            if (p1 == null || p2 == null)
                distanceBetweenParticles.Visibility = Visibility.Hidden;
            else
                distanceBetweenParticles.Visibility = Visibility.Visible;

        }

        /// <summary>
        /// Track particles statistics and the distance between them.
        /// </summary>
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

                p2text.RenderTransform = new TranslateTransform(p2.position.x - (p2.radius + 10), p2.position.y - (p2.radius + 20));
            }

            if (p1 != null && p2 != null)
            {
                distanceBetweenParticles.X1 = p1.position.x;
                distanceBetweenParticles.Y1 = p1.position.y;

                distanceBetweenParticles.X2 = p2.position.x;
                distanceBetweenParticles.Y2 = p2.position.y;
            }

            if (p1 == p2)
                p2 = null;
        }

        /// <summary>
        ///On click track the first particle.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrackParticlep1(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Particle)
                p1 = (Particle)e.OriginalSource;
            else
                p1 = null;

            TrackParticles();
            UpdateVectors();
            ShowHideParticlesUI();
        }

        /// <summary>
        /// On click track the second particle.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrackParticlep2(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Particle)
                p2 = (Particle)e.OriginalSource;
            else
                p2 = null;

            TrackParticles();
            UpdateVectors();
            ShowHideParticlesUI();
        }

        /// <summary>
        /// Updates the velocity vectors's position.
        /// </summary>
        private void UpdateVectors()
        {
            if (p1 != null)
            {
                Vector2 p1Normalized = p1.velocity;
                p1Normalized.Normalize();
                p1Normalized *= (p1.radius + 20);

                //main line
                vectorVelp1.X1 = p1.position.x; vectorVelp1.Y1 = p1.position.y;
                vectorVelp1.X2 = p1.position.x + p1Normalized.x; vectorVelp1.Y2 = p1.position.y + p1Normalized.y;

                //Get position
                p1Normalized *= 0.2;
                Vector2 rightVector = new Vector2(vectorVelp1.X2 + p1Normalized.y - p1Normalized.x, vectorVelp1.Y2 - p1Normalized.x - p1Normalized.y);
                Vector2 leftVector = new Vector2(vectorVelp1.X2 - p1Normalized.y - p1Normalized.x, vectorVelp1.Y2 + p1Normalized.x - p1Normalized.y);
                //right
                vectorVelp1Right.X1 = vectorVelp1.X2; vectorVelp1Right.Y1 = vectorVelp1.Y2;
                vectorVelp1Right.X2 = rightVector.x; vectorVelp1Right.Y2 = rightVector.y;

                //left
                vectorVelp1Left.X1 = vectorVelp1.X2; vectorVelp1Left.Y1 = vectorVelp1.Y2;
                vectorVelp1Left.X2 = leftVector.x; vectorVelp1Left.Y2 = leftVector.y;
            }

            if (p2 != null)
            {
                Vector2 p2Normalized = p2.velocity;
                p2Normalized.Normalize();
                p2Normalized *= (p2.radius + 20);

                //main line
                vectorVelp2.X1 = p2.position.x; vectorVelp2.Y1 = p2.position.y;
                vectorVelp2.X2 = p2.position.x + p2Normalized.x; vectorVelp2.Y2 = p2.position.y + p2Normalized.y;

                //Get position
                p2Normalized *= 0.2;
                Vector2 rightVector = new Vector2(vectorVelp2.X2 + p2Normalized.y - p2Normalized.x, vectorVelp2.Y2 - p2Normalized.x - p2Normalized.y);
                Vector2 leftVector = new Vector2(vectorVelp2.X2 - p2Normalized.y - p2Normalized.x, vectorVelp2.Y2 + p2Normalized.x - p2Normalized.y);
                //right
                vectorVelp2Right.X1 = vectorVelp2.X2; vectorVelp2Right.Y1 = vectorVelp2.Y2;
                vectorVelp2Right.X2 = rightVector.x; vectorVelp2Right.Y2 = rightVector.y;

                //left
                vectorVelp2Left.X1 = vectorVelp2.X2; vectorVelp2Left.Y1 = vectorVelp2.Y2;
                vectorVelp2Left.X2 = leftVector.x; vectorVelp2Left.Y2 = leftVector.y;
            }

            //Vector2 particleNormalized = particle.velocity;
            //particleNormalized.Normalize();
            //particleNormalized *= (particle.radius + 20);

            //Line main = new Line()
            //{
            //    StrokeThickness = 2,
            //    Stroke = transparentToRed,
            //    X1 = particle.position.x,
            //    Y1 = particle.position.y,
            //    X2 = particle.position.x + particleNormalized.x,
            //    Y2 = particle.position.y + particleNormalized.y
            //};

            //particleNormalized *= 0.2;
            //Vector2 leftV = new Vector2(main.X2 - particleNormalized.y - particleNormalized.x, main.Y2 + particleNormalized.x - particleNormalized.y);
            //Vector2 rightV = new Vector2(main.X2 + particleNormalized.y - particleNormalized.x, main.Y2 - particleNormalized.x - particleNormalized.y);

            //Line left = new Line()
            //{
            //    Stroke = red,
            //    StrokeThickness = 2,
            //    X1 = main.X2,
            //    Y1 = main.Y2,

            //    X2 = leftV.x,
            //    Y2 = leftV.y
            //};

            //Line right = new Line()
            //{
            //    Stroke = red,
            //    StrokeThickness = 2,
            //    X1 = main.X2,
            //    Y1 = main.Y2,

            //    X2 = rightV.x,
            //    Y2 = rightV.y
            //};

            //Render.Children.Add(main);
            //Render.Children.Add(left);
            //Render.Children.Add(right);
        }

        /// <summary>
        /// Calculates and displays collisions per second.
        /// </summary>
        private void UpdateCollisionsPerSecond() => CollisionsCounter.Text = (collisionSystem.CollisionCounter / totalTimeElapsed.TotalSeconds).ToString("00.00");
        #endregion

        #region Simulations

        /// <summary>
        /// Resets the entire simulation to a point where no particles are on a scene.
        /// </summary>
        private void ResetSimulation()
        {
            Time.Stop();
            TimeButton.Content = "Start Time";

            p1 = null;
            p2 = null;
            ShowHideParticlesUI();

            totalTimeElapsed = TimeSpan.Zero;
            UpdateCollisionsPerSecond();

            for (int i = 0; i < particles.Length; i++)
                Render.Children.Remove(particles[i]);

            particles = null;
            collisionSystem = null;
        }

        /// <summary>
        /// Initializes one big and lots of small particles with increased velocity.
        /// </summary>
        /// <returns></returns>
        private Particle[] BrownianMotion()
        {
            Particle[] particles = new Particle[240];

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
                Render.Children.Add(particles[i]);

                x += 12;
            }

            //Big one
            particles[100] = new Particle(new Vector2(250, 250), new Vector2(rnd.Next(0, 2) * 2 - 1 * rnd.NextDouble(), rnd.Next(0, 2) * 2 - 1 * rnd.NextDouble()), 26, 10) { Fill = yellow };
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
                Vector2 velocity = new Vector2(rnd.Next(0, 2) * 2 - 1 * rnd.NextDouble() * 1, rnd.Next(0, 2) * 2 - 1 * rnd.NextDouble() * 1);
                particles[i] = new Particle(position, velocity, 2, 1) { Fill = blue, Stroke = blue, StrokeThickness = 2 };
                Render.Children.Add(particles[i]);

                x += 12;
            }

            ParticlesCounter.Text = Convert.ToString(particles.Length);
            return particles;
        }

        #endregion

        #region Buttons

        /// <summary>
        /// Button which either stops or starts the time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        #endregion
    }
}
