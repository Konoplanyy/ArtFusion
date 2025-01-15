using Microsoft.Win32;
using System.Text;
using System.Windows;
using SixLabors.ImageSharp;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SixLabors.ImageSharp.Processing;
using System.IO;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Bmp;
using System.Drawing;
using System.Windows.Interop;

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

            ShowImage showImage = new ShowImage();

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(inpImg.FileName, UriKind.Absolute);
            bitmapImage.EndInit();

            double newWidth = 20;  // Новий розмір за шириною
            double newHeight = 20; // Новий розмір за висотою

            ScaleTransform scale = new ScaleTransform(newWidth / bitmapImage.PixelWidth, newHeight / bitmapImage.PixelHeight);

            TransformedBitmap transformedBitmap = new TransformedBitmap(bitmapImage, scale);

            RenderOptions.SetBitmapScalingMode(showImage, BitmapScalingMode.NearestNeighbor);

            showImage.Img.Source = transformedBitmap;
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