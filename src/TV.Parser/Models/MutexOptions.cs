using System.Collections.Generic;

namespace TV.Parser.Models
{
    public static class MutexOptions
    {
        public static List<string> Fecs = new List<string>
        {
            "None",
            "Auto",
            "1/2",
            "2/3",
            "3/4",
            "4/5",
            "5/6",
            "6/7",
            "7/8",
            "8/9",
            "9/10"
        };

        public static List<string> DeilverySystems =  new List<string>
        {
            "DVB_T",
            "DVB_S",
            "DVB_S2"
        };

        public static List<string> Constellations = new List<string>
        {
            "QPSK",
            "PSK_8",
            "APSK_16",
            "APSK_32"
        };

        public static List<string> Polarisations = new List<string>
        {
            "Horizontal",
            "Vertical",
            "Circular Left",
            "Circular Right"
        };
    }
}
