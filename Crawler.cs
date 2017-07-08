using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler
{
    class Crawler
    {
        public static void Main(String[] args)
        {
            KaohsiungCrawler.WebCrawler kaohsiung = new KaohsiungCrawler.WebCrawler(); 
            kaohsiung.urlList.Add("http://data.kaohsiung.gov.tw/Opendata/List.aspx");
            kaohsiung.craw();

            NewTaipeiCrawler.WebCrawler newtaipei = new NewTaipeiCrawler.WebCrawler();
            newtaipei.urlList.Add("http://data.ntpc.gov.tw/NTPC/od/tagcloud");
            newtaipei.craw();

            Console.ReadLine();
        }
    }
}
