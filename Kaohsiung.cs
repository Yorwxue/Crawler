using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace KaohsiungCrawler
{
    class WebCrawler
    {
        public List<String> urlList = new List<String>();
        
        public void craw()
        {
            int urlIdx = 0;
            while (urlIdx < urlList.Count)
            {
                try
                {
                    String url = urlList[urlIdx];
                    String filePath = "Kaohsiung/WebPage/" + toFileName(url,0);  //0表示這是保留網頁檔(.html)
                    //Console.WriteLine(urlIdx + ":url=" + url + "\nfile=" + filePath);
                    urlToFile(url, filePath);
                    String html = fileToText(filePath);
                    //尋找連結
                    foreach (String childUrl in matches("\\shref\\s*=\\s*'(.*?)'", html, 1))
                    {
                        int Already = 0;
                        foreach (String UrlinList in urlList)
                        {
                            if (UrlinList.Contains(childUrl))
                            {
                                Already = 1;
                            }
                        }
                        if ((childUrl.Contains("data.kaohsiung.gov.tw/Opendata/") || childUrl.Contains("List.aspx?Type=O&cidOrOrganid=") || childUrl.Contains("DetailList.aspx?")) && Already == 0)
                        {
                            //Console.WriteLine(childUrl);
                            if (childUrl.Contains("http://data.kaohsiung.gov.tw/Opendata/"))
                                urlList.Add(childUrl);
                            else
                                urlList.Add("http://data.kaohsiung.gov.tw/Opendata/" + childUrl);
                        }
                    }
                    //尋找資料
                    foreach (String OpenData in matches("<div><h3>(?:\\s|\\S)*</li></ul></p></div></div>", html, 0)) 
                    {
                        //資料所在頁面
                        //filePath = "DataPage/" + toFileName(url,1);  //1表示存為文字檔.txt
                        //urlToFile(url, filePath);  

                        //資料內容
                        //Console.WriteLine(OpenData);
                        String UnitName = SubString(OpenData,'3','/');;  //子字串起點與終點
                        Console.WriteLine(UnitName);
                        filePath = "Kaohsiung/Data/" + toFileName(UnitName,1);

                        String CleanData=TagCleaner(OpenData);
                        StreamWriter Sw = new StreamWriter(filePath);
                        Sw.WriteLine(CleanData);
                        Sw.Close();
                    }
                }
                catch
                {
                    Console.WriteLine("Error:" + urlList[urlIdx] + " fail!");
                }
                urlIdx++;
            }
            Console.WriteLine("Kaohsiung Completed");
            //Console.ReadLine();
        }


        public static IEnumerable matches(String pPattern, String pText, int pGroupId)   //尋找網址
        {
            Regex r = new Regex(pPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            for (Match m = r.Match(pText); m.Success; m = m.NextMatch())
            {
                yield return m.Groups[pGroupId].Value;
            }
        }

        public static String fileToText(String filePath)      //讀取網頁的HTML，存到text
        {
            StreamReader file = new StreamReader(filePath);
            String text = file.ReadToEnd();
            file.Close();
            return text;
        }

        public void urlToFile(String url, String file)     //下載網頁
        {
            WebClient webclient = new WebClient();
            //        webclient.Proxy = proxy;
            webclient.DownloadFile(url, file);
        }

        public static String toFileName(String url,int type)   //type決定存檔格式，0：htm,1：txt
        {
            String fileName = url.Replace('?', ' ');
            fileName = fileName.Replace('/', ' ');
            fileName = fileName.Replace('&', ' ');
            fileName = fileName.Replace(':', ' ');
            fileName = fileName.Replace('<', ' ');
            fileName = fileName.Replace('>', ' ');
            fileName = fileName.ToLower();
            if (type == 0)                     //存為網頁檔
            {
                if (!fileName.EndsWith(".htm") && !fileName.EndsWith(".html"))
                    fileName = fileName + ".html";
            }
            else 
            {
                fileName = fileName + ".txt";
            }
            return fileName;
        }

        public static String SubString(String Str, char Strat, char End) 
        {
            String SubStr;
            int Sublength;
            SubStr=Str.Substring(Str.IndexOf(Strat) + 2);//起點處理
            //終點處理
            Sublength=SubStr.Substring(SubStr.IndexOf(End) - 1).Length;
            Sublength = SubStr.Length - Sublength;
            //整合
            SubStr = SubStr.Substring(0, Sublength);
            return SubStr;
        }
        public static String TagCleaner(String Str)
        {
            //只清除tag
            String CleanStr=Str.Replace("<div>","");
            CleanStr = CleanStr.Replace("</div>", "");
            CleanStr = CleanStr.Replace("<h3>","");
            CleanStr = CleanStr.Replace("<li>", "");
            CleanStr = CleanStr.Replace("<ul>", "");
            CleanStr = CleanStr.Replace("</ul>", "");
            CleanStr = CleanStr.Replace("<p>", "");
            CleanStr = CleanStr.Replace("</p>", "");
            CleanStr = CleanStr.Replace("<tr>", "");
            CleanStr = CleanStr.Replace("</tr>", "");
            CleanStr = CleanStr.Replace("<td>", "");
            CleanStr = CleanStr.Replace("</td>", "");
            CleanStr = CleanStr.Replace(" ", ""); //這邊殺掉空白了!
            CleanStr = CleanStr.Replace("/<a", "");
            CleanStr = CleanStr.Replace("<a", "");  //必須放在/<a之後
            CleanStr = CleanStr.Replace("<tablestyle='background-color:lightgray'>以URL存取資料", "");
            //清除並換行
            CleanStr = CleanStr.Replace("</h3>", "\r\n");
            CleanStr = CleanStr.Replace("</li>", "\r\n");
            CleanStr = CleanStr.Replace("</a>", "\r\n");
            CleanStr = CleanStr.Replace("</table>","\r\n");
            //換成其他符號
            CleanStr = CleanStr.Replace("href='", "href='http://data.kaohsiung.gov.tw/Opendata/");
            CleanStr = CleanStr.Replace("target=_Blank>", " : ");
            return CleanStr;
        }
    }
}
