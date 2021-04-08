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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Drawing;
using System.Text.RegularExpressions;


namespace ASCII_Art_Attempt
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

        private void txtBox_FileDir_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btn_OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if(openFileDialog.ShowDialog() == true)
            {
                txtBox_FileDir.Text = openFileDialog.FileName;
            }
        }

        private void btn_ConvertToAscii_Click(object sender, RoutedEventArgs e)
        {
            string fileExt = System.IO.Path.GetExtension(txtBox_FileDir.Text).ToLower();
            
            if (txtBox_FileDir.Text == "")
            {
                MessageBox.Show("Please enter a file directory first!", "Error!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
             else if (fileExt == ".png" || fileExt == ".jpeg" || fileExt == ".jpg")
            {
                btn_ConvertToAscii.IsEnabled = false;
                Bitmap image = new Bitmap(txtBox_FileDir.Text, true);
                image = GetReSizedImage(image, (int)this.sldr_ImageSizer.Value);
                string _Content = ConvertToAscii(image);

                txtBox_FinalAscii.Text = _Content;
                btn_ConvertToAscii.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Supported file types are .jpg, .jpeg, and .png", "Error!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }
        private Bitmap GetReSizedImage(Bitmap inputBitmap, int asciiWidth)
        {
            int asciiHeight;
            // Calc height of ASCII art
            asciiHeight = (int)Math.Ceiling((double)inputBitmap.Height * asciiWidth / inputBitmap.Width);

            Bitmap result = new Bitmap(asciiWidth, asciiHeight);
            Graphics g = Graphics.FromImage((System.Drawing.Image)result);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(inputBitmap, 0, 0, asciiWidth, asciiHeight);
            g.Dispose();
            return result;
        }
        private string ConvertToAscii(Bitmap image)
        {
            // Actual RGB Conversions
            bool toggle = false;
            StringBuilder sb = new StringBuilder();
            for(int h = 0; h < image.Height; h++)
            {
                for(int w = 0; w < image.Width; w++)
                {
                    Color pixelColor = image.GetPixel(w, h);
                    int red = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                    int green = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                    int blue = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                    Color grayColor = Color.FromArgb(red, green, blue);

                    if(!toggle)
                    {
                        int index = (grayColor.R * 10) / 255;
                        sb.Append(_AsciiChars[index]);
                    }
                }
                if(!toggle)
                {
                    sb.Append("\n");
                    toggle = true;
                }
                else
                {
                    toggle = false;
                }
            }
            return sb.ToString();
        }

        private readonly string[] _AsciiChars = { "#", "#", "@", "%", "=", "+", "*", ":", "-", ".", "\u00A0" };

        private void txtBox_FontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool CheckNumber = double.TryParse(txtBox_FontSize.Text, out double result);
            if(CheckNumber && result >= 1)
            {
                txtBox_FinalAscii.FontSize = result;
            }
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btn_SaveFile_Click(object sender, RoutedEventArgs e)
        {
            string _Content = txtBox_FinalAscii.Text;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text File (*.txt)|.txt";
            saveFileDialog.FileName = "ASCII_Art";
            if (saveFileDialog.ShowDialog() == true)
            {
                StreamWriter sw = new StreamWriter(saveFileDialog.FileName);
                sw.Write(_Content);
                sw.Close();
            }
        }
    }
}
