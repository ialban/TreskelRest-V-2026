using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Entidades.modelos
{
    public class mReservacion
    {
        public string reseNid { get; set; }
        public string nidNid { get; set; }
        public string nidNbr { get; set; }
        public int reseCnt { get; set; }
        public string reseFec { get; set; }
        public string reseDes { get; set; }
        public string reseHas { get; set; }
        public string reseMot { get; set; }
        public string reseNot { get; set; }
        public Image<Rgba32> imagen { get; set; }
        public string reseMes { get; set; }
    }
}
