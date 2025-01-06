using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace ArtFusion
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MainImageInputBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog inpImg = new OpenFileDialog();
            inpImg.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif";
            inpImg.Multiselect = false;
            inpImg.Title = "Choose image";

            if (inpImg.ShowDialog() == false)
                return;

            BitmapImage bitmapImage = new BitmapImage(new Uri(inpImg.FileName));
            ShowImage showImage = new ShowImage();
            showImage.Img.Source = bitmapImage;

            showImage.Height = bitmapImage.Height;
            showImage.Width = bitmapImage.Width;

            showImage.Show();

        }

        private void ListImageInpBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog inpImgs = new OpenFileDialog();
            inpImgs.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif";
            inpImgs.Multiselect = true;
            inpImgs.Title = "Choose images";

            if (inpImgs.ShowDialog() == false)
                return;


        }
    }
}