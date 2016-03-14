using System;
using System.Collections.Generic;
using Microsoft.Extensions.PlatformAbstractions;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.OptionsModel;
using TV.Parser.Models;
using System.Text.RegularExpressions;

namespace TV.Parser.Services
{
    public class ChannelNumberService : IChannelNumberService
    {
        private static Configuration configuration;
        public ChannelNumberService(IOptions<Configuration> config)
        {
            configuration = config.Value;
        }


        public List<Multiplex> Multiplexes()
        {
            var channels = Channels();
            var multiplexes = new List<Multiplex>();

            var channelDoc = new HtmlAgilityPack.HtmlDocument();
            channelDoc.LoadHtml(FileToString("Frequencies.html"));
            var trs = channelDoc.DocumentNode.SelectNodes("//table/tr").ToArray();

            var frequencyExpression = @"(?:\<b\>)(?<Frequency>[0-9]+)\s(?<Polarisation>[A-Z]{1})";
            var deliveryExpression = @"(?<DeliverySystem>[A-Z0-9\-]+)(?:\<br\>){0,1}(?<Constellation>[A-Z0-9]{1,}){0,1}(?:\<br\>SR\s)(?<Symbolrate>[0-9]+)(?:\<br\>FEC\s)(?<FEC>[0-9]\/[0-9]){1}";
            var regFrequency = new Regex(frequencyExpression);
            var regDelivery = new Regex(deliveryExpression);


            for (int i=0; i < trs.Length; i++)
            {
                var tr = trs[i];
                var childRows = tr.Descendants("td");
                if (null == childRows) continue;

                var tds = childRows.ToArray();
                int rowSpan = int.Parse(tds[0].GetAttributeValue("rowspan", "0"));
                if (rowSpan == 0) continue;

                int channelNumber;
                if (!int.TryParse(tds[5].InnerText.Trim(), out channelNumber)) continue;

                var fontElements = tds[0].SelectNodes("font/font").ToArray();
                var rawFequency = fontElements[0].InnerHtml;
                var rawDelivery = fontElements[2].InnerHtml;
                MatchCollection matchFrequency = regFrequency.Matches(rawFequency);
                MatchCollection matchDelivery = regDelivery.Matches(rawDelivery);

                var multiplex = new Multiplex();
                if (matchFrequency.Count > 0)
                {
                    int frequency;
                    int.TryParse(matchFrequency[0].Groups[1].Value, out frequency); 
                    multiplex.Fequency = frequency;
                    multiplex.Polarisation = MutexOptions.Polarisations.FirstOrDefault(p => p.StartsWith(matchFrequency[0].Groups[2].Value));
                }

                if(matchDelivery.Count > 0)
                {
                    multiplex.DeliverySystem = matchDelivery[0].Groups[1].Value;
                    multiplex.Constellation= matchDelivery[0].Groups[2].Value;
                    multiplex.Fec = matchDelivery[0].Groups[4].Value;
                    int symbolRate;
                    int.TryParse(matchDelivery[0].Groups[3].Value, out symbolRate);
                    multiplex.Symbolrate = symbolRate;
                }

                var iCount = i + rowSpan;
                int index = 0;
                multiplex.Channels = new List<Channel>();
                while(i<iCount)
                {
                    tr = trs[i];
                    tds = tr.Descendants("td").ToArray();
                    var imageTag = tds[1 - index].Descendants("img").FirstOrDefault();
                    string imageUrl = "";
                    if (null != imageTag)
                    {
                        imageUrl = imageTag.GetAttributeValue("src", "");
                    }

                    ChannelType channelType;
                    if (imageUrl.Contains("radio/"))
                    {
                        channelType = ChannelType.Radio;
                    }
                    else
                    {
                        channelType = ChannelType.Tv;
                    }

                    int sid = 0;
                    int.TryParse(tds[6 - index].InnerText.Trim().Replace("&nbsp;", ""), out sid);

                    if (int.TryParse(tds[5 - index].InnerText.Trim(), out channelNumber))
                    {
                        var muxChannels = channels.Where(c =>
                                            c.Number == channelNumber && ((c.ChannelType & channelType) != 0));
                        foreach(var channel in muxChannels)
                        {
                            channel.Sid = sid;
                        }
                        multiplex.Channels.AddRange(muxChannels);
                    }
                    
                    index = 1;
                    i++;
                }
                multiplexes.Add(multiplex);
            }            
            return multiplexes;

        }
        public List<Channel> Channels()
        {
            var channelDoc = new HtmlAgilityPack.HtmlDocument();
            channelDoc.LoadHtml(FileToString("ChannelNumbers.html"));
            var trs = channelDoc.DocumentNode.SelectNodes("//table/tr");
            var channels = new List<Channel>();
            foreach(var tr in trs)
            {
                var tds = tr.Descendants("td");
                if ((null != tds) && (tds.Count() == 9))
                {
                    int channelNumber;
                    if (int.TryParse(tds.First().InnerText.Trim(), out channelNumber))
                    {
                        var tags = tds.ToArray();
                        var imageTag = tags[4].Descendants("img").FirstOrDefault();
                        string imageUrl = "";
                        if (null != imageTag)
                        {
                            imageUrl = imageTag.GetAttributeValue("src", "");
                        }

                        var channel = new Channel
                        {
                            Number = channelNumber,
                            Name = tags[5].InnerText.Trim(),
                            ImageUrl = imageUrl
                        };


                        if (imageUrl.Contains("radio/"))
                        {
                            channel.ChannelType = ChannelType.Radio;
                        }
                        else
                        {
                            channel.ChannelType = ChannelType.Tv;
                        }

                        var channelInfo = tags[7].InnerText;

                        if (channelInfo.Contains("Videoguard"))
                        {
                            channel.ChannelType |= ChannelType.VideoGaurd;
                        }
                        else
                        {
                            channel.ChannelType |= ChannelType.Open;
                        }

                        if (channelInfo.Contains("HD"))
                        {
                            channel.ChannelType |= ChannelType.Hd;
                        }
                        else
                        {
                            channel.ChannelType |= ChannelType.Standard;
                        }

                        channels.Add(channel);
                    }                   
                }
            }

            return channels;

        }

        private string FileToString(string fileName)
        {
            string path = string.Format(@"{0}\Development\{1}",
                PlatformServices.Default.Application.ApplicationBasePath, fileName);

            return string.Join("",System.IO.File.ReadAllLines(path));
        }

    }
}
