using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;
using gdRead.Data;
using gdRead.Data.Models;
using HtmlAgilityPack;

namespace gdRead.Console
{
    class Program
    {
        static gdReadContext ctx = new gdReadContext();

        static void Main(string[] args)
        {
            var test = new HtmlWeb();
            var doc = test.Load(@"http://www.gavindraper.co.uk");
            var node = doc.DocumentNode.SelectNodes("/html/head/link[@type='application/rss+xml']/@href");
            if (node.Count > 0)
            {
                var url = node[0].Attributes["href"];
            }


        }
    }
}
