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
                    20)
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
                    20)
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
}
