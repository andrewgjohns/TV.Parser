using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TV.Parser.Models;
using TV.Parser.Services;
namespace TV.Parser.Controllers.Api
{
    public class ChannelController : Controller
    {
        IChannelNumberService channelService;
        public ChannelController(IChannelNumberService service)
        {
            channelService = service;
        }

        public List<Multiplex> Multiplexes()
        {
            return channelService.Multiplexes();
        }

        public List<Channel> Channels()
        {
            return channelService.Channels();
        }
    }
}
