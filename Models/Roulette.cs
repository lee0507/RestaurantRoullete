namespace Swilago.Models
{
    public class Roulettes
    {
        public Roulettes()
        {
            RouletteList = new List<Roulette>();
        }

        public List<Roulette> RouletteList { get; set; }
    }

    public class Roulette
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public string FillStyle { get; set; }

        public string TextFillStyle { get; set; }
    }
}
