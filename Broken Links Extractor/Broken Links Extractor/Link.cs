using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;

namespace Broken_Links_Extractor
{
    class Link
    {
        private static object LogFileLock = new object();
        public static StreamWriter LogFile = new StreamWriter(Directory.GetCurrentDirectory() + "/logfile.txt",false);

        private static string Url_Cleanup(string url)
        {
            #region Url Clean Up
            //Remove any query part
            int index = url.IndexOf('?');
            string temp = url.ToString();
            if (index != -1)
                temp = temp.Remove(index);

            //Remove any leftover part starting with "
            index = temp.IndexOf('"');
            if (index != -1)
                temp = temp.Remove(index);

            //Remove any leftover part starting with #
            index = temp.IndexOf('#');
            if (index != -1)
                temp = temp.Remove(index);

            //Remove any leftover part starting with '
            index = temp.IndexOf('\'');
            if (index != -1)
                temp = temp.Remove(index);

            //Remove trailing slashes
            if (temp[temp.Length - 1] == '/')
                temp = temp.Substring(0, temp.Length - 1);

            #endregion

            return temp;
        }

        private static string Remove_Trailing_Slash(string url)
        {
            string temp = url;
            if (temp[temp.Length - 1] == '/')
                temp = temp.Substring(0, temp.Length - 1);
            return temp;
        }
        private string BaseUrl = string.Empty;
        Hashtable ChildUrls = null;
        public static int timeOut = 0;
        public static int depth = 5000;
        private int lastIndex = 0;
        public Link(string baseUrl)
        {
            this.BaseUrl = baseUrl;
            ChildUrls = new Hashtable();
        }

        public void AddChildLink(string url)
        {
            if (url == this.BaseUrl) return;
            if(!ChildUrls.ContainsValue(url))
                ChildUrls[lastIndex++] = url;
        }


        private HashSet<string> GetAllLinksOnUrl(string url,out string error_message)
        {
            error_message = string.Empty;
            HttpWebResponse response = null;
            HashSet<string> linkList = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Proxy = null;
                request.Timeout = timeOut;
                request.Method = "GET";
                response = (HttpWebResponse)request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream());
                string html = sr.ReadToEnd();
                Uri currentUri = new Uri(url.Replace(@"http://www.", @"http://").Replace(@"http://", @"http://www."));
                #region Extract Links Using HTMLAgilityPack
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);
                if (doc.DocumentNode.SelectSingleNode("//body") != null)
                {
                    HtmlNodeCollection collection = doc.DocumentNode.SelectNodes("//a[@href]");
                    linkList = new HashSet<string>();
                    if (collection != null)
                        foreach (HtmlNode node in collection)
                        {
                            Uri muri = null;
                            Uri.TryCreate(currentUri, node.GetAttributeValue("href", "").ToString(), out muri);
                            if (muri != null)
                                if (Uri.Compare(muri, currentUri, UriComponents.Host, UriFormat.UriEscaped, StringComparison.CurrentCulture) == 0)
                                {
                                    if (muri.ToString().Replace(@"http://", "").Replace(@"https://", "").Split('/').Length - 1 <= depth)
                                        linkList.Add(Url_Cleanup(muri.ToString()));
                                }
                        }
                }
                #endregion
                error_message = "OK";
            }
            catch (WebException ex)
            {
                error_message = ex.Message;
            }
            catch (HtmlAgilityPack.HtmlWebException ex)
            {
                error_message = ex.Message;
            }
            catch (Exception ex)
            {
                error_message = ex.Message + " " + ex.StackTrace;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response.Dispose();
                }
            }
            return linkList;
        }
        public string StartProcessing()
        {
            string broken_list = string.Empty;
            //For each link, process the current link by calling GetAllLinksOnUrl
            HashSet<string> currChilds = null;
            string error_message = string.Empty;
            currChilds = this.GetAllLinksOnUrl(this.BaseUrl,out error_message);
            if (error_message != "OK")
            {
                broken_list += BaseUrl + " " + error_message + Environment.NewLine;
            }
            if(error_message == "OK" && currChilds != null)
            foreach (string url in currChilds)
            {
                this.AddChildLink(url);
            }
            for(int i=0;i<lastIndex;i++)
            {
                currChilds = null;
                error_message = string.Empty;
                currChilds = this.GetAllLinksOnUrl(this.ChildUrls[i].ToString(), out error_message);
                if (error_message != "OK")
                {
                    broken_list += BaseUrl + " " + error_message + Environment.NewLine;
                }
                if (error_message == "OK" && currChilds != null)
                foreach (string url in currChilds)
                {
                    this.AddChildLink(url);
                }
            }
            lock (LogFileLock)
            {
                foreach(DictionaryEntry pair in this.ChildUrls)
                {
                    LogFile.WriteLine(this.BaseUrl + " " + pair.Value.ToString());
                }
            }
            GC.Collect();
            ChildUrls.Clear();
            return broken_list;
        }
    }
}
