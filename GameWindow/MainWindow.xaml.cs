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
using My2DPhysicsLibrary;
//using My2DPhysicsLibrary;

namespace GameWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //SimulationTime
        private DispatcherTimer Time = new DispatcherTimer();
        private TimeSpan deltaTime = TimeSpan.FromMilliseconds(20);

        private readonly Random rnd = new Random();

        public MainWindow()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();

            //Setup simulation
            Time.Tick += Update;
            Time.Interval = deltaTime;
            Time.Start();

            //Spawn given amount at the center
            foreach (var particle in SpawnParticles(10))
            {
                Render.Children.Add(particle);
            }
        }

        private void Update(object sender, EventArgs e)
        {



            foreach (var particle in Render.Children.OfType<Particle>())
            {
                particle.CollisionDetection(ref Render);
                particle.Move();



            }
        }

        IEnumerable<Particle> SpawnParticles(int amountToSpawn)
        {
            for (int i = 0; i < amountToSpawn; i++)
            {
                Particle newP = new Particle(
                    new Vector2(Render.Width / 2, Render.Height / 2),
                    new Vector2(rnd.Next(-10, 10) * rnd.NextDouble(), rnd.Next(-10, 10) * rnd.NextDouble()),
                    10)
                {
                    Width = 20,
                    Height = 20,
                    Fill = new SolidColorBrush(Color.FromRgb((byte)rnd.Next(1, 255), (byte)rnd.Next(1, 255), (byte)rnd.Next(1, 255))),
                    StrokeThickness = 1,
                    Stroke = Brushes.Black
                };

                yield return newP;
            }
        }

        private void AddOrRemoveParticle(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Particle)
            {
                //Remove particle
                Particle activeParticle = (Particle)e.OriginalSource;
                Render.Children.Remove(activeParticle);
            }
            else
            {
                //Get the specific position on click
                Point mousePos = new Point(Mouse.GetPosition(Render).X, Mouse.GetPosition(Render).Y);

                //Initiliaze particle at a given point
                Particle newParticle = new Particle(
                    new Vector2(mousePos.X, mousePos.Y),
                    new Vector2(rnd.Next(-10, 10) * rnd.NextDouble(), rnd.Next(-10, 10) * rnd.NextDouble()),
                    10)
                {
                    Width = 20,
                    Height = 20,
                    Fill = new SolidColorBrush(Color.FromRgb((byte)rnd.Next(1, 255), (byte)rnd.Next(1, 255), (byte)rnd.Next(1, 255))),
                    StrokeThickness = 1,
                    Stroke = Brushes.Black
                };

                //Add to canvas
                Render.Children.Add(newParticle);
            }
        }
    }

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
            if (particlePosition.x + particleRadius > Render.Width || particlePosition.x + particleRadius < 0)
                particleVelocity.x = (-particleVelocity.x);

            if (particlePosition.y + particleRadius > Render.Height || particlePosition.y + particleRadius < 0)
                particleVelocity.y = (-particleVelocity.y);
        }
    }

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
    }
}
