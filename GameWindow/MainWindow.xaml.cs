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

namespace SimulationWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //SimulationTime
        private readonly DispatcherTimer Time = new DispatcherTimer();
        private static double deltaTime = 0.020;
        private TimeSpan fixedTime = TimeSpan.FromMilliseconds(deltaTime * 1000);
        private TimeSpan time = TimeSpan.Zero;

        //Particles
        private List<Particle> particles = new List<Particle>();
        private readonly Random rnd = new Random();

        //Collision prediction
        private PriorityQueue<Event, double> collisionQueue = new PriorityQueue<Event, double> { };

        //Color palette
        SolidColorBrush blue = (SolidColorBrush)new BrushConverter().ConvertFrom("#4D96FF");
        SolidColorBrush white = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFFFF");
        SolidColorBrush yellow = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFD93D");
        SolidColorBrush green = (SolidColorBrush)new BrushConverter().ConvertFrom("#6BCB77");
        SolidColorBrush red = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF6B6B");
        

        private void PredictCollisions(Particle p1)
        {
            if (p1 == null) return;

            for (int i = 0; i < particles.Count; i++)
            {

            }

            //collisionQueue.Enqueue(new Event());
        }

        public MainWindow()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();

            //Setup simulation
            Time.Tick += Update;
            Time.Interval = fixedTime;
            Time.Start();

            //Spawn big particles
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
                particles[i].VerticalWallCollision(ref Render);
                particles[i].HorizontalWallCollision(ref Render);

                for (int j = i; j < particles.Count - 1; j++)
                    particles[i].ParticleCollision(particles[i], particles[j + 1]);

                particles[i].Move(deltaTime);
                particles[i].Draw();
            }
        }

        IEnumerable<Particle> SpawnParticles(int amountToSpawn, double size, double mass, Vector2 position, TimeSpan interval)
        {
            for (int i = 0; i < amountToSpawn; i++)
            {
                TimeSpan timeBetween = time + interval;

                if (timeBetween == time)
                    yield return new Particle(position,
                    new Vector2(rnd.Next(-10, 10) * rnd.NextDouble(), rnd.Next(-10, 10) * rnd.NextDouble()),
                    size / 2, mass);
            }

            ParticlesCounter.Text = Convert.ToString(particles.Count());
        }

        private void AddOrRemoveParticle(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Particle)
            {
                //Remove particle
                Particle activeParticle = (Particle)e.OriginalSource;
                particles.Remove(activeParticle);
                Render.Children.Remove(activeParticle);
                ParticlesCounter.Text = Convert.ToString(particles.Count());
            }
            else
            {
                //Add particle
                //Initiliaze particle at a given point
                Particle newParticle = new Particle(new Vector2(Mouse.GetPosition(Render).X, Mouse.GetPosition(Render).Y),
                    new Vector2(rnd.Next(-20, 20) * rnd.NextDouble(), rnd.Next(-20, 20) * rnd.NextDouble()), 4, 1);

                //Add to Render
                particles.Add(newParticle);
                Render.Children.Add(newParticle);
                ParticlesCounter.Text = Convert.ToString(particles.Count());
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
