using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace WinFormsApp1
{
    public interface IEditor
    {
        Form ColorForm { get; set; }
        Form LightForm { get; set; }
        Form FilterForm { get; set; }
        float ZoomFactor { get; set; }
        System.Drawing.Point StartPoint { get; set; }
        Rectangle SelectionRectangle { get; set; }
        bool IsSelecting { get; set; }
        bool IsSelectActive { get; set; }
        Bitmap OriginalImage { get; set; }
        Bitmap OriginalImageBeforeCrop { get; set; }
        Bitmap CurrentImage { get; set; }
        Bitmap CompareImage { get; set; }
        bool IsTextActive { get; set; }
        string InputText { get; set; }
        System.Drawing.Point TextPosition { get; set; }
        Color TextColor { get; set; }
        int TextSize { get; set; }
        bool IsMarking { get; set; }
        System.Drawing.Point StartMarkupPoint { get; set; }
        System.Drawing.Point EndMarkupPoint { get; set; }
        Color MarkupColor { get; set; }
        int MarkupSize { get; set; }
        string CurrentShape { get; set; }
        System.Drawing.Point DragStartPoint { get; set; }
        bool IsDragging { get; set; }
        bool IsDraggingActive { get; set; }
        float OffsetX { get; set; }
        float OffsetY { get; set; }
        Stack<Bitmap> UndoStack { get; set; }
        Stack<Bitmap> RedoStack { get; set; }

        void LoadImage(string filePath);
        void SaveStateForUndo(Image image);
        System.Drawing.Point GetScaledPoint(System.Drawing.Point mousePoint, PictureBox pictureBox);
        int GetFontSizeBasedOnImageSize(int baseFontSize, PictureBox pictureBox);
        Image CropImage(Rectangle cutArea, Image image);
        Bitmap RotateImage(Bitmap originalBitmap, float angle);
        Image DrawTextOnImage(System.Drawing.Point position, PictureBox image);
        Image DrawShapeOnImage(System.Drawing.Point start, System.Drawing.Point end, Pen pen);
        Image FlipImageHorizontally(Image image);
        Image FlipImageVertically(Image image);
        Image RedrawImage(PictureBox pictureBox);
        Image ApplyLightAdjustments(int brightness, int exposure, int contrast, Image image);
        Image ApplyColorAdjustments(int saturation, int warmth, int tint, Image image);
        ColorMatrix CombineColorMatrices(params ColorMatrix[] matrices);
        ColorMatrix MultiplyColorMatrices(ColorMatrix matrix1, ColorMatrix matrix2);
        Image ApplyFilter(String filterName);
        void ApplyBokeh();
        void ApplyVintage();
        void ApplySepia();
        void ApplyBlackAndWhite();
        void ApplyGlow();
        void ApplyBlur();
        void ApplyVignette();
        void ApplyContrast();
        void ApplySaturation();
    }

}
