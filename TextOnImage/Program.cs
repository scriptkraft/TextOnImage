using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace TextOnImage
{
    class TextFormat
    {
        public string Text { get; set; }
        public string Font { get; set; }
        public int Size { get; set; }
        public string Style { get; set; }
        public string ColorHex { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var imagePath = args[0];
            var outPath = args[1];
            var json = args[2];
            var bitmap = new Bitmap(imagePath);
            var graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var fontStore = new PrivateFontCollection();
            var fontFiles = Directory.EnumerateFiles("fonts").Where(f => Path.GetFileName(f)[0] != '.'); // TODO: can load font files dynamically
            foreach (var fontFile in fontFiles)
            {
                fontStore.AddFontFile(fontFile);
            }
            var fontMap = new Dictionary<string, FontFamily>();
            foreach (var family in fontStore.Families)
            {
                fontMap.Add(family.Name, family);
            }
            var textFormats = JsonConvert.DeserializeObject<TextFormat[]>(json);
            foreach (var tf in textFormats)
            {
                var font = new Font(fontMap[tf.Font], tf.Size, (FontStyle)Enum.Parse(typeof(FontStyle), tf.Style, true));
                var brush = new SolidBrush(ColorTranslator.FromHtml($"#{tf.ColorHex}"));
                if (tf.Width != 0 && tf.Height != 0)
                    graphics.DrawString(tf.Text, font, brush, new Rectangle(tf.X, tf.Y, tf.Width, tf.Height));
                else
                    graphics.DrawString(tf.Text, font, brush, new Point(tf.X, tf.Y));
            }
            bitmap.Save(outPath, ImageFormat.Png);
            bitmap.Dispose();
            graphics.Dispose();
        }
    }
}
