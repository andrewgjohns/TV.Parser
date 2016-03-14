namespace TV.Parser.Models
{
    [System.Flags()]
    public enum ChannelType : int
    {
        None = 0,
        Open = 1,
        VideoGaurd = 2,
        Tv= 4,
        Radio = 8,
        Hd = 16,
        Standard = 32
    }
}
