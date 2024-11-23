using Accord.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using System.Runtime.InteropServices;
using OpenCvSharp.Extensions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics.Contracts;

namespace WinFormsApp1
{
    public class Editor : IEditor
    {
        public Form ColorForm { get; set; }
        public Form LightForm { get; set; }
        public Form FilterForm { get; set; }
        public float ZoomFactor { get; set; } = 1.0f;
        public System.Drawing.Point StartPoint { get; set; }
        public Rectangle SelectionRectangle { get; set; }
        public bool IsSelecting { get; set; }
        public bool IsSelectActive { get; set; }
        public Bitmap OriginalImage { get; set; }
        public Bitmap OriginalImageBeforeCrop { get; set; }
        public Bitmap CurrentImage { get; set; }
        public Bitmap CompareImage { get; set; }
        public bool IsTextActive { get; set; }
        public string InputText { get; set; }
        public System.Drawing.Point TextPosition { get; set; }
        public Color TextColor { get; set; } = Color.Red;
        public int TextSize { get; set; }
        public bool IsMarking { get; set; }
        public System.Drawing.Point StartMarkupPoint { get; set; }
        public System.Drawing.Point EndMarkupPoint { get; set; }
        public Color MarkupColor { get; set; } = Color.Red;
        public int MarkupSize { get; set; }
        public string CurrentShape { get; set; }
        public System.Drawing.Point DragStartPoint { get; set; }
        public bool IsDragging { get; set; } = false;
        public bool IsDraggingActive { get; set; } = false;
        public float OffsetX { get; set; } = 0;
        public float OffsetY { get; set; } = 0;
        public Stack<Bitmap> UndoStack { get; set; } = new Stack<Bitmap>();
        public Stack<Bitmap> RedoStack { get; set; } = new Stack<Bitmap>();

        // Funkcija za ucitavanje slike
        public void LoadImage(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    // Ucitavanje originalne slike sa transparentnoscu
                    Bitmap originalBitmap = new Bitmap(filePath);

                    // Provera da li slika ima alfa kanal (transparentnost)
                    if (Image.IsAlphaPixelFormat(originalBitmap.PixelFormat))
                    {
                        // Kreiranje nove slike bez transparentnosti
                        Bitmap bitmapWithoutTransparency = new Bitmap(originalBitmap.Width, originalBitmap.Height, PixelFormat.Format32bppRgb);

                        using (Graphics g = Graphics.FromImage(bitmapWithoutTransparency))
                        {
                            // Crtanje originalne slike na novu sliku
                            g.DrawImage(originalBitmap, new Rectangle(0, 0, originalBitmap.Width, originalBitmap.Height));
                        }

                        OriginalImage = bitmapWithoutTransparency;
                        CompareImage = bitmapWithoutTransparency;
                    }
                    else
                    {
                        OriginalImage = originalBitmap;
                        CompareImage = originalBitmap;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Path is not valid or image does not exist.");
            }
        }

        // Funkcija za skaliranje tacke misa na osnovnu velicinu slike
        public System.Drawing.Point GetScaledPoint(System.Drawing.Point mousePoint, PictureBox pictureBox)
        {
            // Originalne dimenzije slike i prozora PictureBox-a
            var originalImageSize = pictureBox.Image.Size;
            var pictureBoxSize = pictureBox.Size;

            // Izracunavanje faktora skaliranja
            float scaleX = (float)originalImageSize.Width / pictureBoxSize.Width;
            float scaleY = (float)originalImageSize.Height / pictureBoxSize.Height;

            // Vratite prilagodjenu tacku
            return new System.Drawing.Point((int)(mousePoint.X * scaleX), (int)(mousePoint.Y * scaleY));
        }

        // Funkcija za skaliranje velicine linije za iscrtanje elemenata
        public int GetFontSizeBasedOnImageSize(int baseFontSize, PictureBox pictureBox)
        {
            if (pictureBox.Image != null)
            {
                // Originalne dimenzije slike i prozora PictureBox-a
                var originalImageSize = pictureBox.Image.Size;
                var pictureBoxSize = pictureBox.Size;

                // Izračunaj prosečan faktor skaliranja na osnovu dimenzija slike
                float scaleX = (float)originalImageSize.Width / pictureBoxSize.Width;
                float scaleY = (float)originalImageSize.Height / pictureBoxSize.Height;
                float scaleFactor = (scaleX + scaleY) / 2;

                // Vratite prilagođenu veličinu fonta
                return (int)(baseFontSize * scaleFactor);
            }
            // Vratite podrazumevanu veličinu ako slika nije učitana
            return baseFontSize;
        }

        // Funkcija za odsecanje slike na osnovu selekcije
        public Image CropImage(Rectangle cutArea, Image image)
        {
            if (image != null && cutArea.Width > 0 && cutArea.Height > 0)
            {
                if (OriginalImageBeforeCrop == null)
                {
                    // Sacuvaj originalnu sliku pre crop-a
                    OriginalImageBeforeCrop = (Bitmap)image;
                }

                Bitmap originalBitmap = new Bitmap(image);

                // Provera granica odsecanja
                if (cutArea.X >= 0 && cutArea.Y >= 0 &&
                    cutArea.X + cutArea.Width <= originalBitmap.Width &&
                    cutArea.Y + cutArea.Height <= originalBitmap.Height)
                {
                    Bitmap croppedBitmap = new Bitmap(cutArea.Width, cutArea.Height);
                    using (Graphics g = Graphics.FromImage(croppedBitmap))
                    {
                        g.DrawImage(originalBitmap, new Rectangle(0, 0, cutArea.Width, cutArea.Height), cutArea, GraphicsUnit.Pixel);
                    }

                    OriginalImage = croppedBitmap;
                    return croppedBitmap;
                }
            }
            return null;
        }

        // Funkcija za snimanje stanja slike pre svake promene
        public void SaveStateForUndo(Image image)
        {
            if (image != null)
            {
                // Kreiramo kopiju trenutne slike i dodajemo je na undo stek
                UndoStack.Push(new Bitmap(image));

                // Ako smo radili undo, brisemo redo istoriju
                RedoStack.Clear();
            }
        }

        // Funkcija za rotiranje slike
        public Bitmap RotateImage(Bitmap originalBitmap, float angle)
        {
            // Racunanje nove dimenzije rotirane slike
            float radAngle = angle * (float)Math.PI / 180f;
            float cos = Math.Abs((float)Math.Cos(radAngle));
            float sin = Math.Abs((float)Math.Sin(radAngle));

            int newWidth = (int)(originalBitmap.Width * cos + originalBitmap.Height * sin);
            int newHeight = (int)(originalBitmap.Width * sin + originalBitmap.Height * cos);

            // Kreiranje novog bitmap-a sa novim dimenzijama
            Bitmap rotatedBitmap = new Bitmap(newWidth, newHeight);

            using (Graphics g = Graphics.FromImage(rotatedBitmap))
            {
                // Postavite centar rotacije na centar novog bitmap-a
                g.TranslateTransform((float)newWidth / 2, (float)newHeight / 2);

                // Rotacija slike
                g.RotateTransform(angle);

                // Vratite koordinatni sistem nazad pre nego sto nacrtate sliku
                g.TranslateTransform(-(float)originalBitmap.Width / 2, -(float)originalBitmap.Height / 2);

                // Nacrtajte originalnu sliku u centru novog bitmap-a
                g.DrawImage(originalBitmap, new System.Drawing.Point(0, 0));
            }

            return rotatedBitmap;
        }

        // Crtanje teksta na slici
        public Image DrawTextOnImage(System.Drawing.Point position, PictureBox pictureBox)
        {
            if (!string.IsNullOrEmpty(InputText) && pictureBox.Image != null)
            {
                Bitmap originalBitmap = new Bitmap(OriginalImage);
                using (Graphics g = Graphics.FromImage(originalBitmap))
                {
                    // Prilagodite velicinu fonta prema velicini slike
                    float adjustedFontSize = GetFontSizeBasedOnImageSize(TextSize, pictureBox);
                    // Koristi prilagodjenu velicinu fonta
                    using (Font font = new Font("Arial", adjustedFontSize, FontStyle.Bold))
                    {
                        // Koristi izabranu boju
                        using (Brush brush = new SolidBrush(TextColor))
                        {
                            // Dodaj tekst
                            g.DrawString(InputText, font, brush, position);
                        }
                    }
                }
                OriginalImage = originalBitmap;
                return originalBitmap;
            }
            return OriginalImage;
        }

        // Crtanje izabranog oblika
        public Image DrawShapeOnImage(System.Drawing.Point start, System.Drawing.Point end, Pen pen)
        {
            if (OriginalImage != null)
            {
                // Napravite privremenu sliku na osnovu originalne slike
                Bitmap tempImage = new Bitmap(OriginalImage);

                using (Graphics g = Graphics.FromImage(tempImage))
                {
                    switch (CurrentShape)
                    {
                        case "Line":
                            g.DrawLine(pen, start, end);
                            break;
                        case "Rectangle":
                            g.DrawRectangle(pen, Math.Min(start.X, end.X), Math.Min(start.Y, end.Y),
                                                Math.Abs(end.X - start.X), Math.Abs(end.Y - start.Y));
                            break;
                        case "Ellipse":
                            g.DrawEllipse(pen, Math.Min(start.X, end.X), Math.Min(start.Y, end.Y),
                                              Math.Abs(end.X - start.X), Math.Abs(end.Y - start.Y));
                            break;
                    }
                }
                return tempImage;
            }
            return null;
        }

        // Funkcija za ponovno iscrtavanje slike sa zoom-om
        public Image RedrawImage(PictureBox pictureBox)
        {
            if (pictureBox.Image != null)
            {
                // Racunanje novih dimenzija za prikaz
                int displayWidth = (int)(pictureBox.Width * ZoomFactor);
                int displayHeight = (int)(pictureBox.Height * ZoomFactor);

                // Prikazivanje uvecanog dela slike
                Bitmap zoomedImage = new Bitmap(pictureBox.Width, pictureBox.Height);

                using (Graphics g = Graphics.FromImage(zoomedImage))
                {
                    // Izracunavanje nove pozicije slike nakon zumiranja i prevlacenja
                    int imgX = (pictureBox.Width - displayWidth) / 2 + (int)OffsetX;
                    int imgY = (pictureBox.Height - displayHeight) / 2 + (int)OffsetY;

                    // Crtanje slike tako da bude centrirana
                    g.DrawImage(OriginalImage, new Rectangle(imgX, imgY, displayWidth, displayHeight));
                }
                return zoomedImage;
            }
            return null;
        }

        // Okretanje slike horizontalno
        public Image FlipImageHorizontally(Image image)
        {
            // Kreiraj novu bitmap-u
            Bitmap flippedImage = new Bitmap(image.Width, image.Height);
            using (Graphics g = Graphics.FromImage(flippedImage))
            {
                // Crtanje slike horizontalno
                g.DrawImage(image, new Rectangle(0, 0, flippedImage.Width, flippedImage.Height),
                    new Rectangle(image.Width, 0, -image.Width, image.Height), GraphicsUnit.Pixel);
            }
            OriginalImage = flippedImage;
            return flippedImage;
        }

        // Okretanje slike vertikalno
        public Image FlipImageVertically(Image image)
        {
            // Kreiraj novu bitmap-u
            Bitmap flippedImage = new Bitmap(image.Width, image.Height);
            using (Graphics g = Graphics.FromImage(flippedImage))
            {
                // Crtanje slike vertikalno
                g.DrawImage(image, new Rectangle(0, 0, flippedImage.Width, flippedImage.Height),
                    new Rectangle(0, image.Height, image.Width, -image.Height), GraphicsUnit.Pixel);
            }
            OriginalImage = flippedImage;
            return flippedImage;
        }

        // Funkcija za primenu osvetljenja slike
        public Image ApplyLightAdjustments(int brightness, int exposure, int contrast, Image image)
        {
            if (OriginalImageBeforeCrop == null)
            {
                OriginalImageBeforeCrop = (Bitmap)image;
            }
            // Kopiraj originalnu sliku
            Bitmap adjustedImage = new Bitmap(OriginalImage);

            using (Graphics g = Graphics.FromImage(adjustedImage))
            {

                float brightnessFactor = 1.0f + (brightness / 100.0f);
                float contrastFactor = 1.0f + (contrast / 100.0f);
                float exposureFactor = 1.0f + (exposure / 100.0f);

                float conTimesExposure = contrastFactor * exposureFactor;
                float briMinusOne = brightnessFactor - 1;

                // Kreiramo matricu na osnovu vrednosti
                ColorMatrix colorMatrix = new(
                [
                    [conTimesExposure, 0, 0, 0, 0],
                    [0, conTimesExposure, 0, 0, 0],
                    [0, 0, conTimesExposure, 0, 0],
                    [0, 0, 0, 1, 0],
                    [briMinusOne, briMinusOne, briMinusOne, 0, 1]
                ]);

                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(colorMatrix);

                // Crtamo sliku
                g.DrawImage(OriginalImage, new Rectangle(0, 0, adjustedImage.Width, adjustedImage.Height),
                            0, 0, OriginalImage.Width, OriginalImage.Height, GraphicsUnit.Pixel, attributes);
            }

            // Handle za FormClosing event
            LightForm.FormClosing += (s, args) =>
            {
                OriginalImage = adjustedImage;
            };
            return adjustedImage;
        }

        // Funkcija za primenu boja na sliku
        public Image ApplyColorAdjustments(int saturation, int warmth, int tint, Image image)
        {
            if (OriginalImageBeforeCrop == null)
            {
                OriginalImageBeforeCrop = (Bitmap)image;
            }

            // Kopiraj originalnu sliku
            Bitmap adjustedImage = new Bitmap(OriginalImage);

            float saturationFactor = 1f + saturation / 100f;
            float warmthFactor = warmth / 100f;
            float tintFactor = tint / 100f;

            float satPlus05 = 0.5f + saturationFactor;
            float satMinus05 = 0.5f - saturationFactor;

            // Kreiramo matrice na osnovu vrednosti
            ColorMatrix saturationMatrix = new(
                [
                    [satPlus05, satMinus05, satMinus05, 0, 0],
                    [satMinus05, satPlus05, satMinus05, 0, 0],
                    [satMinus05, satMinus05, satPlus05, 0, 0],
                    [0, 0, 0, 1, 0],
                    [0, 0, 0, 0, 1]
                ]);

            float warPlus01 = 1f + warmthFactor;
            float warMinus01 = 1f - warmthFactor;

            ColorMatrix warmthMatrix = new(
                [
                    [warPlus01, 0, 0, 0, 0],
                    [0, 1, 0, 0, 0],
                    [0, 0, warMinus01, 0, 0],
                    [0, 0, 0, 1, 0],
                    [0, 0, 0, 0, 1]
                ]);
            ColorMatrix tintMatrix = new(
                [
                    [1, 0, 0, 0, 0],
                    [0, 1 + tintFactor, 0, 0, 0],
                    [0, 0, 1, 0, 0],
                    [0, 0, 0, 1, 0],
                    [0, 0, 0, 0, 1]
                ]);

            ColorMatrix finalMatrix;

            if (saturation == 0 && warmth == 0 && tint == 0)
            {
                saturationMatrix = new(
                    [
                        [1, 0, 0, 0, 0],
                        [0, 1, 0, 0, 0],
                        [0, 0, 1, 0, 0],
                        [0, 0, 0, 1, 0],
                        [0, 0, 0, 0, 1]
                    ]);
                warmthMatrix = new(
                    [
                        [1, 0, 0, 0, 0],
                        [0, 1, 0, 0, 0],
                        [0, 0, 1, 0, 0],
                        [0, 0, 0, 1, 0],
                        [0, 0, 0, 0, 1]
                    ]);
                saturationMatrix = new(
                    [
                        [1, 0, 0, 0, 0],
                        [0, 1, 0, 0, 0],
                        [0, 0, 1, 0, 0],
                        [0, 0, 0, 1, 0],
                        [0, 0, 0, 0, 1]
                    ]);
                finalMatrix = CombineColorMatrices(saturationMatrix, warmthMatrix, tintMatrix);
            }
            else
            {
                // Kombinujemo sve matrice u jednu
                finalMatrix = CombineColorMatrices(saturationMatrix, warmthMatrix, tintMatrix);
            }

            using (Graphics g = Graphics.FromImage(adjustedImage))
            {
                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(finalMatrix);
                g.DrawImage(OriginalImage, new Rectangle(0, 0, adjustedImage.Width, adjustedImage.Height), 0, 0, adjustedImage.Width, adjustedImage.Height, GraphicsUnit.Pixel, attributes);
            }

            // Handle za FormClosing event
            ColorForm.FormClosing += (s, args) =>
            {
                OriginalImage = adjustedImage;
            };
            return adjustedImage;
        }

        // Pomocna funkcija za kombinovanje matrica
        public ColorMatrix CombineColorMatrices(params ColorMatrix[] matrices)
        {
            // Pravimo jedinicnu matricu
            ColorMatrix result = new ColorMatrix();
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    result[i, j] = (i == j) ? 1 : 0;
                }
            }
            // Prolazimo jednu po jednu matricu
            foreach (var matrix in matrices)
            {
                result = MultiplyColorMatrices(result, matrix);
            }
            return result;
        }

        // Funkcija za mnozenje dve matrice
        public ColorMatrix MultiplyColorMatrices(ColorMatrix matrix1, ColorMatrix matrix2)
        {
            ColorMatrix result = new ColorMatrix();
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    result[i, j] = 0;
                    for (int k = 0; k < 5; k++)
                    {
                        result[i, j] += matrix1[i, k] * matrix2[k, j];
                    }
                }
            }
            return result;
        }
        // Funkcija za primenu filtera
        public Image ApplyFilter(string filterName)
        {
            switch (filterName)
            {
                case "Bokeh":
                    ApplyBokeh();
                    break;
                case "Vintage":
                    ApplyVintage();
                    break;
                case "Sepia":
                    ApplySepia();
                    break;
                case "Black and White":
                    ApplyBlackAndWhite();
                    break;
                case "Glow":
                    ApplyGlow();
                    break;
                case "Blur":
                    ApplyBlur();
                    break;
                case "Vignette":
                    ApplyVignette();
                    break;
                case "Contrast":
                    ApplyContrast();
                    break;
                case "Saturation":
                    ApplySaturation();
                    break;
            }
            return OriginalImage;
        }

        // Funkcija za primenu bokeh filtera
        public void ApplyBokeh()
        {
            if (OriginalImage != null)
            {
                // Ucitavanje originalne slike u OpenCV Mat format
                Mat src = BitmapConverter.ToMat(OriginalImage);

                if (src.Channels() != 1)
                {
                    // Konvertovanje slike u grayscale
                    Mat gray = new Mat();
                    Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

                    // Primena Canny edge detection za detekciju ivica
                    Mat edges = new Mat();
                    Cv2.Canny(gray, edges, 100, 300, 7);

                    // Kreiranje binarne maske za subjekta
                    Mat mask = new Mat();
                    Cv2.Threshold(edges, mask, 128, 255, ThresholdTypes.Binary);

                    // Inverzija maske
                    Mat maskInv = new Mat();
                    Cv2.BitwiseNot(mask, maskInv);

                    // Primena GaussianBlur za postizanje zamagljenja
                    Mat blurred = new Mat();
                    Cv2.GaussianBlur(src, blurred, new OpenCvSharp.Size(41, 41), 0);

                    // Kombinovanje originalne slike i zamagljene slike koristeći masku
                    Mat bokeh = new Mat();
                    src.CopyTo(bokeh, maskInv); // Kopiranje subjekta iz originalne slike
                    blurred.CopyTo(bokeh, mask); // Kopiranje zamagljene pozadine

                    // Konvertovanje rezultata nazad u Bitmap format
                    OriginalImage = bokeh.ToBitmap();
                }
            }
        }

        // Funkcija za primenu vintage filtera
        public void ApplyVintage()
        {
            if (OriginalImage != null)
            {
                // Učitavanje originalne slike u OpenCV Mat format
                Mat src = BitmapConverter.ToMat(OriginalImage);

                // Kreiranje transformacione matrice za vintage efekat
                Mat kernel = new Mat(new OpenCvSharp.Size(3, 3), MatType.CV_32F);
                kernel.Set<float>(0, 0, 0.272f); kernel.Set<float>(0, 1, 0.534f); kernel.Set<float>(0, 2, 0.131f);
                kernel.Set<float>(1, 0, 0.349f); kernel.Set<float>(1, 1, 0.686f); kernel.Set<float>(1, 2, 0.168f);
                kernel.Set<float>(2, 0, 0.393f); kernel.Set<float>(2, 1, 0.769f); kernel.Set<float>(2, 2, 0.189f);

                // Primena vintage efekta koristeći OpenCV
                Mat vintage = new Mat();
                Cv2.Filter2D(src, vintage, -1, kernel);

                // Kreiranje zrnaste matrice
                Mat noise = Mat.Zeros(vintage.Size(), vintage.Type());
                Cv2.Randu(noise, new Scalar(-20, -20, -20), new Scalar(20, 20, 20));

                // Dodavanje zrnastog efekta
                Mat vintageWithNoise = new Mat();
                Cv2.Add(vintage, noise, vintageWithNoise);

                Mat sepiaOverlay = new Mat(vintageWithNoise.Size(), vintageWithNoise.Type(), new Scalar(0, 50, 80));
                Cv2.AddWeighted(vintageWithNoise, 0.3, sepiaOverlay, 0.3, 0, vintageWithNoise);
                // Konvertovanje rezultata nazad u Bitmap format
                OriginalImage = vintageWithNoise.ToBitmap();
            }
        }

        // Funkcija za primenu sepia filtera
        public void ApplySepia()
        {
            if (OriginalImage != null)
            {
                // Učitavanje originalne slike u OpenCV Mat format
                Mat src = BitmapConverter.ToMat(OriginalImage);

                // Kreiranje sepia kernela
                Mat kernel = new Mat(new OpenCvSharp.Size(3, 3), MatType.CV_32F);
                kernel.Set<float>(0, 0, 0.272f); kernel.Set<float>(0, 1, 0.534f); kernel.Set<float>(0, 2, 0.131f);
                kernel.Set<float>(1, 0, 0.349f); kernel.Set<float>(1, 1, 0.686f); kernel.Set<float>(1, 2, 0.168f);
                kernel.Set<float>(2, 0, 0.393f); kernel.Set<float>(2, 1, 0.769f); kernel.Set<float>(2, 2, 0.189f);

                // Kreiranje sepia efekta primenom kernela direktno na originalnu sliku
                Mat sepia = new Mat();
                Cv2.Filter2D(src, sepia, -1, kernel);

                Mat sepiaOverlay = new Mat(sepia.Size(), sepia.Type(), new Scalar(0, 50, 80));
                Cv2.AddWeighted(sepia, 0.3, sepiaOverlay, 0.3, 0, sepia);
                // Konvertovanje rezultata nazad u Bitmap format
                OriginalImage = sepia.ToBitmap();
            }
        }

        // Funkcija za primenu black and white filtera
        public void ApplyBlackAndWhite()
        {
            if (OriginalImage != null)
            {
                // Ucitavanje originalne slike u OpenCV Mat format
                Mat src = BitmapConverter.ToMat(OriginalImage);
                if(src.Channels() != 1)
                {
                    Mat dst = new Mat();
                    Cv2.CvtColor(src, dst, ColorConversionCodes.BGR2GRAY);
                    // Konvertovanje rezultata nazad u Bitmap format
                    OriginalImage = dst.ToBitmap();
                }
            }
        }

        // Funkcija za primenu glow filtera
        public void ApplyGlow()
        {
            if (OriginalImage != null)
            {
                // Ucitavanje originalne slike u OpenCV Mat format
                Mat src = BitmapConverter.ToMat(OriginalImage);

                // Kreiranje transformacione matrice za glow efekat
                Mat kernel = new Mat(new OpenCvSharp.Size(4, 4), MatType.CV_32F);
                kernel.Set<float>(0, 0, 0.0f); kernel.Set<float>(0, 1, 0.05f); kernel.Set<float>(0, 2, 0.05f); kernel.Set<float>(0, 3, 0.0f);
                kernel.Set<float>(1, 0, 0.05f); kernel.Set<float>(1, 1, 0.3f); kernel.Set<float>(1, 2, 0.3f); kernel.Set<float>(1, 3, 0.05f);
                kernel.Set<float>(2, 0, 0.05f); kernel.Set<float>(2, 1, 0.3f); kernel.Set<float>(2, 2, 0.3f); kernel.Set<float>(2, 3, 0.05f);
                kernel.Set<float>(3, 0, 0.0f); kernel.Set<float>(3, 1, 0.05f); kernel.Set<float>(3, 2, 0.05f); kernel.Set<float>(3, 3, 0.0f);

                Mat glow = new Mat();
                Cv2.Filter2D(src, glow, -1, kernel);

                // Konvertovanje rezultata nazad u Bitmap format
                OriginalImage = glow.ToBitmap();
            }
        }

        // Funkcija za primenu blur filtera
        public void ApplyBlur()
        {
            if (OriginalImage != null)
            {
                // Ucitavanje originalne slike u OpenCV Mat format
                Mat src = BitmapConverter.ToMat(OriginalImage);

                // Primena GaussianBlur-a sa OpenCV
                Mat blurred = new Mat();
                Cv2.GaussianBlur(src, blurred, new OpenCvSharp.Size(21, 21), 0);

                // Pretvaranje rezultata nazad u Bitmap
                OriginalImage = blurred.ToBitmap();
            }
        }

        // Funkcija za primenu vignette filtera
        public void ApplyVignette()
        {
            if (OriginalImage != null)
            {
                // Učitavanje originalne slike u OpenCV Mat format
                Mat src = BitmapConverter.ToMat(OriginalImage);

                // Kreiranje maske za vignette efekat
                Mat mask = new Mat(src.Size(), MatType.CV_32F);
                System.Drawing.Point center = new System.Drawing.Point(src.Width / 2, src.Height / 2);
                double maxDistance = Math.Sqrt(center.X * center.X + center.Y * center.Y);

                // Popunjavanje maske sa GaussianBlur
                for (int y = 0; y < mask.Rows; y++)
                {
                    for (int x = 0; x < mask.Cols; x++)
                    {
                        double distance = Math.Sqrt((x - center.X) * (x - center.X) + (y - center.Y) * (y - center.Y));
                        float value = (float)(1 - Math.Pow(distance / maxDistance, 2));
                        mask.Set<float>(y, x, value);
                    }
                }

                // Proširenje maske na tri kanala da odgovara slici
                Mat mask3Channel = new Mat();
                Cv2.Merge(new Mat[] { mask, mask, mask }, mask3Channel);
                mask3Channel.ConvertTo(mask3Channel, MatType.CV_32FC3);

                // Primena GaussianBlur za glatke prelaze
                Cv2.GaussianBlur(mask3Channel, mask3Channel, new OpenCvSharp.Size(0, 0), 10);

                // Direktna manipulacija pikselima
                for (int y = 0; y < src.Rows; y++)
                {
                    for (int x = 0; x < src.Cols; x++)
                    {
                        Vec3b pixelValue = src.At<Vec3b>(y, x); // Promena tipa u Vec3b za 8-bitne vrednosti
                        Vec3f maskValue = mask3Channel.At<Vec3f>(y, x);

                        Vec3b newValue = new Vec3b(
                            (byte)(pixelValue.Item0 * maskValue.Item0),
                            (byte)(pixelValue.Item1 * maskValue.Item1),
                            (byte)(pixelValue.Item2 * maskValue.Item2)
                        );

                        src.Set(y, x, newValue);
                    }
                }
                // Konvertovanje rezultata nazad u Bitmap format
                OriginalImage = src.ToBitmap();
            }

        }

        // Funkcija za primenu contrast filtera
        public void ApplyContrast()
        {
            if (OriginalImage != null)
            {
                // Učitavanje originalne slike u OpenCV Mat format
                Mat src = BitmapConverter.ToMat(OriginalImage);

                if(src.Type() != MatType.CV_8UC3)
                {
                    src.ConvertTo(src, MatType.CV_8UC3);
                }

                // Parametar za pojačavanje kontrasta
                double alpha = 1.1; // Faktor kontrasta
                int beta = 3; // Pomak osvetljenosti

                // Kreiranje izlaznog Mat objekta za rezultat
                Mat contrast = new Mat(src.Size(), src.Type());

                // Direktna manipulacija pikselima
                for (int y = 0; y < src.Rows; y++)
                {
                    for (int x = 0; x < src.Cols; x++)
                    {
                        Vec3b pixelValue = src.At<Vec3b>(y, x);

                        Vec3b newValue = new Vec3b(
                            (byte)Math.Clamp((pixelValue.Item0 * alpha) + beta, 0, 255),
                            (byte)Math.Clamp((pixelValue.Item1 * alpha) + beta, 0, 255),
                            (byte)Math.Clamp((pixelValue.Item2 * alpha) + beta, 0, 255)
                        );

                        contrast.Set(y, x, newValue);
                    }
                }
                // Konvertovanje rezultata nazad u Bitmap format
                OriginalImage = contrast.ToBitmap();
            }
        }

        // Funkcija za primenu saturation filtera
        public void ApplySaturation()
        {
            if (OriginalImage != null)
            {
                // Ucitavanje originalne slike u OpenCV Mat format
                Mat src = BitmapConverter.ToMat(OriginalImage);

                // Parametar za pojačavanje zasicenosti
                double saturationScale = 1.5; // Faktor zasicenosti

                // Kopiranje originalne slike za manipulaciju
                Mat result = src.Clone();

                // Direktna manipulacija pikselima u BGR prostoru
                for (int y = 0; y < result.Rows; y++)
                {
                    for (int x = 0; x < result.Cols; x++)
                    {
                        Vec3b pixelValue = result.At<Vec3b>(y, x);

                        // Preracunavanje zasicenosti u BGR prostoru
                        float blue = pixelValue[0] * (float)saturationScale;
                        float green = pixelValue[1] * (float)saturationScale;
                        float red = pixelValue[2] * (float)saturationScale;

                        // Ogranicavanje vrednosti na opseg od 0 do 255
                        blue = Math.Clamp(blue, 0, 255);
                        green = Math.Clamp(green, 0, 255);
                        red = Math.Clamp(red, 0, 255);

                        // Postavljanje novih vrednosti piksela
                        result.Set(y, x, new Vec3b((byte)blue, (byte)green, (byte)red));
                    }
                }
                // Konvertovanje rezultata nazad u Bitmap format
                OriginalImage = result.ToBitmap();
            }
        }

    }
}
