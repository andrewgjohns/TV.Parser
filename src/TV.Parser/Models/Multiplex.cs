using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TV.Parser.Models
{
    public class Multiplex
    {
        public int Sid { get; set; }
        public int Fequency { get; set; }
        public int Symbolrate { get; set; }
        public string Fec { get; set; }
        public string DeliverySystem { get; set; }
        public string Constellation { get; set; }
        public string Polarisation { get; set; }
        public List<Channel> Channels { get; set; }
    }
}
