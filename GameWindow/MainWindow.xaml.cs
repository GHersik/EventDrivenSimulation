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
        private Particle p1 = null;
        private Particle p2 = null;
        //UIElements
        Line distanceBetweenParticles = new Line()
        {
            Stroke = green,
            StrokeThickness = 2
        };
        TextBlock p1text = new TextBlock()
        {
            Text = "p1",
            Foreground = yellow,
            FontSize = 14,
            FontWeight = FontWeights.Bold,
            Visibility = Visibility.Hidden
        };
        TextBlock p2text = new TextBlock()
        {
            Text = "p2",
            Foreground = yellow,
            FontSize = 14,
            FontWeight = FontWeights.Bold,
            Visibility = Visibility.Hidden
        };

        //Collision prediction
        private PriorityQueue<Event, double> collisionQueue = new PriorityQueue<Event, double> { };

        //Color palette
        static SolidColorBrush green = (SolidColorBrush)new BrushConverter().ConvertFrom("#6BCB77");
        static SolidColorBrush blue = (SolidColorBrush)new BrushConverter().ConvertFrom("#4D96FF");
        static SolidColorBrush white = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFFFF");
        static SolidColorBrush yellow = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFD93D");
        static SolidColorBrush red = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF6B6B");

        private readonly Random rnd = new Random();


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
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();

            //Setup simulation
            Time.Tick += Update;
            Time.Interval = fixedTime;
            Time.Start();

            //Spawn big particles
            foreach (var particle in SpawnParticles(11, 16, 1, new Vector2(150, 150), TimeSpan.Zero))
            {
                Render.Children.Add(particle);
                particles.Add(particle);
            }

            //ParticleUIElements
            Render.Children.Add(distanceBetweenParticles);
            Render.Children.Add(p1text);
            Render.Children.Add(p2text);
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


            //p1
            if (p1 != null)
            {
                p1Position.Text = $"({p1.position.x.ToString("00.00")} , {p1.position.x.ToString("00.00")})";
                particleVelocity.Text = $"({p1.velocity.x.ToString("00.00")} , {p1.velocity.x.ToString("00.00")})";
                particleRadius.Text = p1.radius.ToString();
                particleMass.Text = p1.mass.ToString();

                p1text.RenderTransform = new TranslateTransform(p1.position.x - (p1.Width), p1.position.y - (p1.Height + p1.Height));
            }


            //p2
            if (p2 != null)
            {
                particle2Position.Text = $"({p2.position.x.ToString("00.00")} , {p2.position.x.ToString("00.00")})";
                particle2Velocity.Text = $"({p2.velocity.x.ToString("00.00")} , {p2.velocity.x.ToString("00.00")})";
                particle2Radius.Text = p2.radius.ToString();
                particle2Mass.Text = p2.mass.ToString();

                p2text.RenderTransform = new TranslateTransform(p2.position.x - (p2.Width), p2.position.y - (p2.Height + p2.Height));
            }

            //bothParticles
            DrawDistanceLine();
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

        //private void AddOrRemoveParticle(object sender, MouseButtonEventArgs e)
        //{
        //    if (e.OriginalSource is Particle)
        //    {
        //        p1 = (Particle)e.OriginalSource;


        //        //Remove particle
        //        //Particle activeParticle = (Particle)e.OriginalSource;
        //        //particles.Remove(activeParticle);
        //        //Render.Children.Remove(activeParticle);
        //        //ParticlesCounter.Text = Convert.ToString(particles.Count());
        //    }
        //    else
        //    {
        //        //ToDo
        //        //Vector2 point = new Vector2(Mouse.GetPosition(Render).X, Mouse.GetPosition(Render).Y);



        //        ////Add particle
        //        ////Initiliaze particle at a given point
        //        //Particle newParticle = new Particle(new Vector2(Mouse.GetPosition(Render).X, Mouse.GetPosition(Render).Y),
        //        //    new Vector2(rnd.Next(-20, 20) * rnd.NextDouble(), rnd.Next(-20, 20) * rnd.NextDouble()), 4, 1);

        //        ////Add to Render
        //        //particles.Add(newParticle);
        //        //Render.Children.Add(newParticle);
        //        //ParticlesCounter.Text = Convert.ToString(particles.Count());
        //    }
        //}

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

        //private void DragParticle(object sender, MouseButtonEventArgs e)
        //{
        //    foreach (var particle in SpawnParticles(10, 12, 1, new Vector2(Mouse.GetPosition(Render).X, Mouse.GetPosition(Render).Y), TimeSpan.Zero))
        //    {
        //        Render.Children.Add(particle);
        //        particles.Add(particle);
        //    }
        //}

        private void TrackParticlep1(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Particle)
            {
                p1 = (Particle)e.OriginalSource;
                p1text.RenderTransform = new TranslateTransform(p1.position.x - (p1.Width), p1.position.y - (p1.Height + p1.Height));
                p1text.Visibility = Visibility.Visible;
                DrawDistanceLine();

                //Show UI
                Particle1.Visibility = Visibility.Visible;
            }
            else
            {
                p1 = null;
                DrawDistanceLine();

                //Hide UI
                Particle1.Visibility = Visibility.Hidden;
                p1text.Visibility = Visibility.Hidden;
            }

        }

        private void TrackParticlep2(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Particle)
            {
                p2 = (Particle)e.OriginalSource;
                p2text.RenderTransform = new TranslateTransform(p2.position.x - (p2.Width), p2.position.y - (p2.Height + p2.Height));
                p2text.Visibility = Visibility.Visible;
                DrawDistanceLine();

                //Show UI
                Particle2.Visibility = Visibility.Visible;
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
    }
}
