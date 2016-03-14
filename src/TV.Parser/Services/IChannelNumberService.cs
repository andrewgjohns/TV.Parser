using System.Collections.Generic;
using TV.Parser.Models;

namespace TV.Parser.Services
{
    public interface IChannelNumberService
    {
        List<Channel> Channels();
        List<Multiplex> Multiplexes();
    }
}