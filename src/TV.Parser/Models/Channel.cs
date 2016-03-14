namespace TV.Parser.Models
{
    public class Channel
    {
        public int Number { get; set; }
        public int Sid { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public ChannelType ChannelType { get; set; }

    }
}
