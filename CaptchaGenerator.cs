using System;
using System.Drawing;
using System.Linq;

namespace MagazinTechniki
{
    public class CaptchaGenerator
    {
        private static readonly Random _random = new Random();
        private const string _characters = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const int _captchaLength = 4;

        public string GenerateCaptcha()
        {
            var captcha = new string(Enumerable.Range(0, _captchaLength)
                .Select(_ => _characters[_random.Next(_characters.Length)]).ToArray());
            return captcha;
        }

        public Bitmap RenderCaptcha(string captcha)
        {
            var bitmap = new Bitmap(150, 50);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.White);
                graphics.DrawString(captcha, new Font("Monserat", 24), new SolidBrush(Color.Black), 10, 10);

                for (int i = 0; i < 50; i++)
                {
                    graphics.DrawLine(new Pen(Color.Gray, 1), _random.Next(bitmap.Width), _random.Next(bitmap.Height),
                        _random.Next(bitmap.Width), _random.Next(bitmap.Height));
                }
            }
            return bitmap;
        }
    }
}