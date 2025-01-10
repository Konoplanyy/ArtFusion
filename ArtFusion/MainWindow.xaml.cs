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

            using (Image<Rgba32> image = Image.Load<Rgba32>(inpImg.FileName))
            {
                image.Mutate(x => x.Resize(100, 100));

                using (var ms = new MemoryStream())
                {
                    image.Save(ms, new BmpEncoder()); // Зберігаємо у формат BMP для сумісності
                    ms.Seek(0, SeekOrigin.Begin);

                    // Конвертуємо в BitmapImage для WPF
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = ms;
                    bitmapImage.EndInit();

                    // Присвоюємо ImageSource
                    showImage.Img.Source = bitmapImage;
                    showImage.Width = bitmapImage.Width;
                    showImage.Height = bitmapImage.Height;
                }
            }

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