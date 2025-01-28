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
using System.Collections.Generic;

namespace ArtFusion
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<System.Drawing.Point, System.Drawing.Color> MainImageColors { get; set; }
        private Dictionary<BitmapSource, System.Drawing.Color> tileData {  get; set; }

        public MainWindow()
        {
            InitializeComponent();
            MainImageColors = new Dictionary<System.Drawing.Point, System.Drawing.Color>();
            tileData = new Dictionary<BitmapSource, System.Drawing.Color>();
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            var resImg = GenerateImageFromTiles(MainImageColors, tileData);
            ShowImage(resImg);
        }

        private void MainImageInputBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog inpImg = new OpenFileDialog();
            inpImg.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif";
            inpImg.Multiselect = false;
            inpImg.Title = "Choose image";

            if (inpImg.ShowDialog() == false)
                return;

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

            MainImageColors = ConvertImageToColorArray(transformedBitmap);


        }

        private void ListImageInpBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog inpImgs = new OpenFileDialog();
            inpImgs.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif";
            inpImgs.Multiselect = true;
            inpImgs.Title = "Choose images";

            if (inpImgs.ShowDialog() == false)
                return;

            List<BitmapImage> images = new List<BitmapImage>();

            foreach (var FileName in inpImgs.FileNames)
            {
                try
                {
                    if (File.Exists(FileName))
                    {
                        // Зберігає картинку в bitmap
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.UriSource = new Uri(FileName, UriKind.Absolute);
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad; // Завантажує зображення одразу
                        bitmapImage.EndInit();
                        images.Add(bitmapImage);
                    }
                    else
                    {
                        // Логування або обробка відсутності файлу
                        Console.WriteLine($"Файл не знайдено: {FileName}");
                    }
                }
                catch (Exception ex)
                {
                    // Логування або обробка винятку
                    Console.WriteLine($"Помилка при завантаженні зображення: {ex.Message}");
                }
            }

            foreach (BitmapImage Image in images)
            {
                tileData.Add(Image, GetImageColor(Image));
            }
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

        private static System.Drawing.Color GetImageColor(BitmapSource bitmap)
        {

            ScaleTransform scale = new ScaleTransform(1 / bitmap.PixelWidth, 1 / bitmap.PixelHeight);

            TransformedBitmap transformedBitmap = new TransformedBitmap(bitmap, scale);

            return GetPixelColor(transformedBitmap, 0, 0);
        }

        private static void ShowImage(BitmapSource bitmap, bool smoothing = true)
        {
            ShowImage showImage = new ShowImage();

            if (!smoothing) RenderOptions.SetBitmapScalingMode(showImage, BitmapScalingMode.NearestNeighbor); // виключає зглажування

            showImage.Img.Source = bitmap;
            showImage.Show();
        }

        public static BitmapSource GenerateImageFromTiles(
        Dictionary<System.Drawing.Point, System.Drawing.Color> pixelData,
        Dictionary<BitmapSource, System.Drawing.Color> tileData)
        {
            // Знаходимо розміри результуючого зображення
            int maxX = pixelData.Keys.Max(p => p.X);
            int maxY = pixelData.Keys.Max(p => p.Y);
            int tileWidth = tileData.First().Key.PixelWidth;
            int tileHeight = tileData.First().Key.PixelHeight;

            // Створюємо нове зображення для результату
            var resultBitmap = new WriteableBitmap(
                (maxX + 1) * tileWidth,
                (maxY + 1) * tileHeight,
                96, 96, System.Windows.Media.PixelFormats.Bgra32, null);

            // Проходимо по кожному пікселю в першому словнику
            foreach (var pixel in pixelData)
            {
                System.Drawing.Point position = pixel.Key;
                System.Drawing.Color targetColor = pixel.Value;

                // Знаходимо найближчий колір у другому словнику
                var closestTile = FindClosestTile(targetColor, tileData);

                // Копіюємо відповідну картинку в результуюче зображення
                CopyTileToResult(resultBitmap, closestTile, position, tileWidth, tileHeight);
            }

            return resultBitmap;
        }

        private static BitmapSource FindClosestTile(System.Drawing.Color targetColor, Dictionary<BitmapSource, System.Drawing.Color> tileData)
        {
            BitmapSource closestTile = null;
            double minDistance = double.MaxValue;

            foreach (var tile in tileData)
            {
                double distance = ColorDistance(targetColor, tile.Value);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestTile = tile.Key;
                }
            }

            return closestTile;
        }

        private static double ColorDistance(System.Drawing.Color color1, System.Drawing.Color color2)
        {
            // Обчислюємо евклідову відстань між кольорами
            int rDiff = color1.R - color2.R;
            int gDiff = color1.G - color2.G;
            int bDiff = color1.B - color2.B;
            return Math.Sqrt(rDiff * rDiff + gDiff * gDiff + bDiff * bDiff);
        }

        private static void CopyTileToResult(WriteableBitmap resultBitmap, BitmapSource tile, System.Drawing.Point position, int tileWidth, int tileHeight)
        {
            // Перевірка на нульові або від'ємні розміри плитки
            if (tileWidth <= 0 || tileHeight <= 0)
            {
                throw new ArgumentException("Розміри плитки (tileWidth та tileHeight) повинні бути більше нуля.");
            }

            // Перевірка на нульові посилання
            if (resultBitmap == null || tile == null)
            {
                throw new ArgumentNullException("resultBitmap та tile не можуть бути null.");
            }

            // Визначаємо позицію для вставки
            int startX = position.X * tileWidth;
            int startY = position.Y * tileHeight;

            // Перевірка, чи виходять координати за межі resultBitmap
            if (startX < 0 || startY < 0 || startX + tileWidth > resultBitmap.PixelWidth || startY + tileHeight > resultBitmap.PixelHeight)
            {
                throw new ArgumentOutOfRangeException("Позиція плитки виходить за межі результуючого зображення.");
            }

            // Копіюємо пікселі з плитки в результуюче зображення
            byte[] pixels = new byte[tileWidth * tileHeight * 4];
            tile.CopyPixels(pixels, tileWidth * 4, 0);

            // Вставляємо пікселі
            resultBitmap.WritePixels(
                new System.Windows.Int32Rect(startX, startY, tileWidth, tileHeight),
                pixels, tileWidth * 4, 0);
        }
    }
}