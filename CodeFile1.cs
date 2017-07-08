using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

class WebCrawler
{
    List<String> urlList = new List<String>();

    public static void Main(String[] args)
    {
        WebCrawler crawler = new WebCrawler();
        crawler.urlList.Add("http://data.kaohsiung.gov.tw/Opendata/List.aspx");
        crawler.craw();
    }

    public void craw()
    {
        int urlIdx = 0;
        while (urlIdx < urlList.Count)
        {
            try
            {
                String url = urlList[urlIdx];
                String filePath = "C:/WebClient/" + toFileName(url);
                Console.WriteLine(urlIdx + ":url=" + url + "\nfile=" + filePath);
                urlToFile(url, filePath);
                String html = fileToText(filePath);
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
                    if ( (childUrl.Contains("data.kaohsiung.gov.tw/Opendata/") || childUrl.Contains("List.aspx?Type=O&cidOrOrganid=") ||  childUrl.Contains("DetailList.aspx?")  ) && Already==0)
                    {
                        Console.WriteLine(childUrl);
                        if (childUrl.Contains("http://data.kaohsiung.gov.tw/Opendata/"))
                            urlList.Add(childUrl); 
                        else
                            urlList.Add("http://data.kaohsiung.gov.tw/Opendata/" + childUrl); 
                    }
                }
            }
            catch
            {
                Console.WriteLine("Error:" + urlList[urlIdx] + " fail!");
            }
            urlIdx++;
        }
        Console.WriteLine("\nCompleted");
        Console.ReadLine();
    }

    public static IEnumerable matches(String pPattern, String pText, int pGroupId)
    {
        Regex r = new Regex(pPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        for (Match m = r.Match(pText); m.Success; m = m.NextMatch())
            yield return m.Groups[pGroupId].Value;
    }

    public static String fileToText(String filePath)
    {
        StreamReader file = new StreamReader(filePath);
        String text = file.ReadToEnd();
        file.Close();
        return text;
    }

    public void urlToFile(String url, String file)
    {
        WebClient webclient = new WebClient();
        //        webclient.Proxy = proxy;
        webclient.DownloadFile(url, file);
    }

    public static String toFileName(String url)
    {
        String fileName = url.Replace('?', '_');
        fileName = fileName.Replace('/', '_');
        fileName = fileName.Replace('&', '_');
        fileName = fileName.Replace(':', '_');
        fileName = fileName.ToLower();
        if (!fileName.EndsWith(".htm") && !fileName.EndsWith(".html"))
            fileName = fileName + ".htm";
        return fileName;
    }
}