using System;
namespace Chessharp.Core.Structures
{
    public class Move : ICloneable
    {
        private string color;
        private string from;
        private string to;
        private int flags;
        private string piece;
        private string promotion;
        private string captured;
        private string san;

        public string Color { get => color; set => color = value; }
        public string From { get => from; set => from = value; }
        public string To { get => to; set => to = value; }
        public int Flags { get => flags; set => flags = value; }
        public string Piece { get => piece; set => piece = value; }
        public string Promotion { get => promotion; set => promotion = value; }
        public string Captured { get => captured; set => captured = value; }
        public string San { get => san; set => san = value; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public void Set(string key, string value)
        {
            key = key.ToLower();
            switch (key)
            {
                case "color":
                    this.Color = value;
                    break;
                case "from":
                    this.From = value;
                    break;
                case "to":
                    this.From = value;
                    break;
                case "piece":
                    this.Piece = value;
                    break;
                case "promotion":
                    this.Promotion = value;
                    break;
                case "captured":
                    this.Captured = value;
                    break;
                case "san":
                    this.San = value;
                    break;
                case "flags":
                    this.Flags = Convert.ToInt32(value);
                    break;
            }
        }

        public void Set(string key, int value)
        {
            key = key.ToLower();
            switch (key)
            {
                case "flags":
                    Flags = value;
                    break;
            }
        }
    }
}
