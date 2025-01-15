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

            //зберігає картинку в bitmap
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(inpImg.FileName, UriKind.Absolute);
            bitmapImage.EndInit();

            //maybe temp
            double newWidth = 20; 
            double newHeight = 20;

            //перетворює змінює розмір
            ScaleTransform scale = new ScaleTransform(newWidth / bitmapImage.PixelWidth, newHeight / bitmapImage.PixelHeight);

            TransformedBitmap transformedBitmap = new TransformedBitmap(bitmapImage, scale);

            RenderOptions.SetBitmapScalingMode(showImage, BitmapScalingMode.NearestNeighbor); // виключає зглажування

            

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

        private static Dictionary<System.Drawing.Point, System.Drawing.Color> ConvertImageToColorArray(BitmapSource image)
        {
            //створює масив кольорів
            WriteableBitmap writeableBitmap = new WriteableBitmap(image);

            Dictionary<System.Drawing.Point, System.Drawing.Color> imageColors = new Dictionary<System.Drawing.Point, System.Drawing.Color>();

            for (int i = 0; i < writeableBitmap.PixelWidth; i++)
            {
                for (int j = 0; j < writeableBitmap.PixelHeight; j++)
                {
                    System.Drawing.Color pixelColor = GetPixelColor(writeableBitmap, i, j);
                    imageColors.Add(new System.Drawing.Point(i, j), pixelColor);
                }
            }

            return imageColors;
        }

        private static System.Drawing.Color GetPixelColor(BitmapSource bitmap, int x, int y)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));
            if (x < 0 || x >= bitmap.PixelWidth || y < 0 || y >= bitmap.PixelHeight)
                throw new ArgumentOutOfRangeException("Координати виходять за межі зображення.");

            int bytesPerPixel = (bitmap.Format.BitsPerPixel + 7) / 8;
            byte[] pixelData = new byte[bytesPerPixel];

            // Витягуємо один піксель
            var rect = new Int32Rect(x, y, 1, 1);
            bitmap.CopyPixels(rect, pixelData, bytesPerPixel, 0);

            // Припускаємо формат Bgra32
            byte blue = pixelData[0];
            byte green = pixelData[1];
            byte red = pixelData[2];
            byte alpha = pixelData[3];

            return System.Drawing.Color.FromArgb(alpha, red, green, blue);
        }
    }
}