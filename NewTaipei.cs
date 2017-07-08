using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace NewTaipeiCrawler
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
                    //Console.WriteLine("urlIdx=" + urlIdx + ", urlList.Count=" + urlList.Count);
                    String url = urlList[urlIdx];
                    String filePath = "NewTaipei/WebPage/" + toFileName(url, 0);  //0表示這是保留網頁檔(.html)
                    //Console.WriteLine(urlIdx + ":url=" + url);
                    urlToFile(url, filePath);
                    String html = fileToText(filePath);
                    //尋找連結
                    foreach (String childUrl in matches("(?:\\s|\\S)'?href'?\\s*.\\s*(?:'|\")(.*?)(?:'|\")", html, 1))
                    {
                       int Already = 0;
                        foreach (String UrlinList in urlList)
                        {
                            if (UrlinList.Contains(childUrl))
                            {
                                Already = 1;
                            }
                        }
                        if (childUrl.Contains("/NTPC/od/") && Already == 0) 
                        {
                            //Console.WriteLine(childUrl);
                            if (childUrl.Contains("http://data.ntpc.gov.tw"))
                            {
                                urlList.Add(childUrl);
                            }
                            else
                            {
                                urlList.Add("http://data.ntpc.gov.tw" + childUrl);
                            }
                        }
                    }
                    //尋找資料
                    foreach (String OpenData in matches("<div class=\"title\" id=\"title\">\\s*新北市(?:\\s|\\S)*分　　類(?:\\s|\\S)*<div style=\"margin-left:20px;margin-right:00px\">", html, 0))
                    {
                        //資料所在頁面
                        //filePath = "DataPage/" + toFileName(url,1);  //1表示存為文字檔.txt
                        //urlToFile(url, filePath);  

                        //資料內容
                        //Console.WriteLine(OpenData);
                        String UnitName = SubString(OpenData, '新'); ;  //子字串起點與終點(都是從頭數起第一個符號)
                        Console.WriteLine(UnitName);
                        filePath = "NewTaipei/Data/" + toFileName(UnitName, 1);

                        String CleanData = TagCleaner(OpenData);
                        StreamWriter Sw = new StreamWriter(filePath);
                        Sw.WriteLine(UnitName + "\r\n" + CleanData);
                        Sw.Close();
                    }
                }
                catch
                {
                    Console.WriteLine("Error:" + urlList[urlIdx] + " fail!");
                }
                urlIdx++;
            }
            Console.WriteLine("NewTaipei Completed");
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

        public static String toFileName(String url, int type)   //type決定存檔格式，0：htm,1：txt
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
                    fileName = fileName +".html";
            }
            else
            {
                fileName = fileName + ".txt";
            }
            return fileName;
        }

        public static String SubString(String Str, char Strat)
        {
            String SubStr;
            SubStr = Str.Replace(" ", "");
            SubStr = SubStr.Substring(Str.IndexOf(Strat)-2);//起點處理
            StringReader StrLine = new StringReader(SubStr);
            SubStr = StrLine.ReadLine();

            return SubStr;
        }
        public static String TagCleaner(String Str)
        {
            String CleanStr = Str;
            //只清除tag
            CleanStr = CleanStr.Substring(CleanStr.IndexOf('分'));
            CleanStr = CleanStr.Replace("</td><td>", "");
            CleanStr = CleanStr.Replace("</tr>", "");
            CleanStr = CleanStr.Replace("<tr>", "");
            CleanStr = CleanStr.Replace("<td>", "");
            CleanStr = CleanStr.Replace("&nbsp;", " ");
            CleanStr = CleanStr.Replace("target=\"_blank\">", "");
            CleanStr = CleanStr.Replace("<a", "");
            CleanStr = CleanStr.Replace("<br/>", "");
            CleanStr = CleanStr.Replace("<table>", "");
            CleanStr = CleanStr.Replace("<td style=\"vertical-align:top;\">", "");
            CleanStr = CleanStr.Replace("<table style=\"border: 1px double rgb(0, 0, 0);\" frame=\"border\" rules=\"all\">", "");
            CleanStr = CleanStr.Replace("<td style=\"background-color: rgb(153, 153, 153);\">", "");
            CleanStr = CleanStr.Replace("<!-- span1 -->", "");
            CleanStr = CleanStr.Replace("<!-- span2 -->", "");
            CleanStr = CleanStr.Replace("<!-- span3 -->", "");
            CleanStr = CleanStr.Replace("<!-- span4 -->", "");
            CleanStr = CleanStr.Replace("<div style=\"margin-left:20px;margin-right:00px\">", "");
            CleanStr = CleanStr.Replace(" ", "");
            CleanStr = CleanStr.Replace("	", "");
            CleanStr = CleanStr.Replace("\r", "").Replace("\n", "");
            //清除並換行
            CleanStr = CleanStr.Replace("</div>", "\r\n");
            CleanStr = CleanStr.Replace("</td>", "\r\n");
            CleanStr = CleanStr.Replace("</a>", "\r\n");
            CleanStr = CleanStr.Replace("</table>", "\r\n");
            CleanStr = CleanStr.Replace("</ul>", "\r\n");
            //換成其他符號
            CleanStr = CleanStr.Replace("\">", " : ");
            CleanStr = CleanStr.Trim();
            return CleanStr;
        }
    }
}
