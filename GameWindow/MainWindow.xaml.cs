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
//using My2DPhysicsLibrary;

namespace GameWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //SimulationTime
        DispatcherTimer Time = new DispatcherTimer();
        TimeSpan deltaTime = TimeSpan.FromMilliseconds(20);

        Brush customColor;
        Random rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();

            Time.Tick += FixedUpdate;
            Time.Interval = deltaTime;
            Time.Start();

        }

        private void FixedUpdate(object sender, EventArgs e)
        {
            foreach (var particle in Render.Children.OfType<Ellipse>())
            {
                particle.Width += 1;
            }




            //Render.UpdateLayout();
        }

        private void AddOrRemoveParticle(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Ellipse)
            {
                Ellipse activeEllipse = (Ellipse)e.OriginalSource;

                Render.Children.Remove(activeEllipse);
            }
            else
            {
                customColor = new SolidColorBrush(Color.FromRgb((byte)rnd.Next(1, 255), (byte)rnd.Next(1, 255), (byte)rnd.Next(1, 255)));
                //Particle newParticle = new Particle
                //{
                //    Width = 20,
                //    Height = 20,
                //    Fill = customColor,
                //    StrokeThickness = 1,
                //    Stroke = Brushes.Black
                //};

                Ellipse newEllipse = new Ellipse
                {
                    Width = 20,
                    Height = 20,
                    Fill = customColor,
                    StrokeThickness = 1,
                    Stroke = Brushes.Black
                };

                Canvas.SetLeft(newEllipse, Mouse.GetPosition(Render).X);
                Canvas.SetTop(newEllipse, Mouse.GetPosition(Render).Y);
                Render.Children.Add(newEllipse);

                //todo add velocity etc
            }
        }
    }

    /// <summary>
    /// Initializes an instance of a particle.
    /// </summary>
    /// <param name="particlePosition">Position of a particle using x and y axis.</param>
    /// <param name="particleVelocity">Velocity of a particle using x and y axis.</param>
    /// <param name="particleRadius">Radius of a particle.</param>
    /// <param name="particleColor">Color of a particle.</param>
    public class Particle : Shape
    {
        private Vector2 particlePosition;
        private Vector2 particleVelocity;
        private float particleRadius;
        private Color particleColor;
        private Rect _rect = Rect.Empty;

        public Particle(Vector2 position, Vector2 velocity, float radius, Color color)
        {
            this.particlePosition = position;
            this.particleVelocity = velocity;
            this.particleRadius = radius;
            this.particleColor = color;
        }

        public Particle() : this(Vector2.One, Vector2.One, 1, Color.FromRgb((byte)1, (byte)1, (byte)1)) { }

        protected override Geometry DefiningGeometry
        {
            get
            {
                if (_rect.IsEmpty)
                    return Geometry.Empty;

                return new EllipseGeometry(_rect);
            }
        }

        //protected override void OnRender(DrawingContext drawingContext)
        //{
        //    if (!_rect.IsEmpty)
        //    {
        //        Pen pen = GetPen();
        //    }
        //}

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
