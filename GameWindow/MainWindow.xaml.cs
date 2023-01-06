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

namespace SimulationWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //SimulationTime
        private DispatcherTimer Time = new DispatcherTimer();
        private static double deltaTime = 0.15;
        private TimeSpan fixedTime = TimeSpan.FromMilliseconds(15);
        private TimeSpan time = TimeSpan.Zero;

        //Particles
        private List<Particle> particles = new List<Particle>();
        private readonly Random rnd = new Random();

        public MainWindow()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();

            //Setup simulation
            Time.Tick += Update;
            Time.Interval = fixedTime;
            Time.Start();

            //Spawn particles
            foreach (var particle in SpawnParticles(1, 52, 16, new Vector2(50, 50), TimeSpan.Zero))
            {
                Render.Children.Add(particle);
                particles.Add(particle);
            }
        }

        private void Update(object sender, EventArgs e)
        {
            time += fixedTime;

            //Time Driven Simulation O(n^2)
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].WallCollision(ref Render);

                for (int j = i; j < particles.Count - 1; j++)
                    particles[i].ParticleCollision(particles[i], particles[j + 1]);

                particles[i].Move(deltaTime);
            }
        }

        IEnumerable<Particle> SpawnParticles(int amountToSpawn, double size, double mass, Vector2 position, TimeSpan interval)
        {
            for (int i = 0; i < amountToSpawn; i++)
            {
                TimeSpan timeBetween = time + interval;

                Particle newParticle = new Particle(position, new Vector2(rnd.Next(-10, 10) * rnd.NextDouble(), rnd.Next(-10, 10) * rnd.NextDouble()), size / 2, mass)
                { Fill = new SolidColorBrush(Color.FromRgb((byte)rnd.Next(1, 255), (byte)rnd.Next(1, 255), (byte)rnd.Next(1, 255))) };

                if (timeBetween == time)
                    yield return newParticle;
            }
        }

        private void AddOrRemoveParticle(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Particle)
            {
                //Remove particle
                Particle activeParticle = (Particle)e.OriginalSource;
                particles.Remove(activeParticle);
                Render.Children.Remove(activeParticle);
            }
            else
            {
                //Add particle
                //Initiliaze particle at a given point
                Particle newParticle = new Particle(new Vector2(Mouse.GetPosition(Render).X, Mouse.GetPosition(Render).Y),
                    new Vector2(rnd.Next(-20, 20) * rnd.NextDouble(), rnd.Next(-20, 20) * rnd.NextDouble()), 6, 1)
                { Fill = new SolidColorBrush(Color.FromRgb((byte)rnd.Next(1, 255), (byte)rnd.Next(1, 255), (byte)rnd.Next(1, 255))), };

                //Add to Render
                particles.Add(newParticle);
                Render.Children.Add(newParticle);
            }
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

        private void DragParticle(object sender, MouseButtonEventArgs e)
        {
            foreach (var particle in SpawnParticles(10, 12, 1, new Vector2(Mouse.GetPosition(Render).X, Mouse.GetPosition(Render).Y), TimeSpan.Zero))
            {
                Render.Children.Add(particle);
                particles.Add(particle);
            }
        }
    }
}
