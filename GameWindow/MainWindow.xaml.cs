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
using My2DPhysicsLibrary;

namespace GameWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Brush customColor;
        Random r = new Random();

        public MainWindow()
        {
            InitializeComponent();
            GameWindowDraw();
        }

        private void GameWindowDraw()
        {



            ////Initialize n particles
            //Particle[] particles = new Particle[2];
            //for (int i = 0; i < 2; i++)
            //{
            //    particles[0] = new Particle();
            //}

            //while (true)
            //{

            //}
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
                customColor = new SolidColorBrush(Color.FromRgb((byte)r.Next(1, 255), (byte)r.Next(1, 255), (byte)r.Next(1, 255)));

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
            }
        }
    }
}
