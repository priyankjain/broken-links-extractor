﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Broken_Links_Extractor
{
    class Link
    {
        public static StreamWriter brokenLinksSW = null;
        private static Regex linkParser = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static System.Windows.Forms.DataGridView OutputTable = null;
        private HashSet<string> externalList = null;
        private string BaseUrl = string.Empty;
        Hashtable ChildUrls = null;
        public static int timeOut = 0;
        public static int depth = 5000;
        private int lastIndex = 0;
        public Link(string baseUrl)
        {
            this.BaseUrl = baseUrl;
            ChildUrls = new Hashtable();
            externalList = new HashSet<string>();
        }

        public void AddChildLink(string url)
        {
            if (url == this.BaseUrl) return;
            if(!ChildUrls.ContainsValue(url))
                ChildUrls[lastIndex++] = url;
        }

        private void GetStatusOnly(string url, out string error_message, out string response_code)
        {
            error_message = string.Empty;
            response_code = string.Empty;
            HttpWebResponse response = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Proxy = null;
                request.Timeout = timeOut;
                request.Method = "GET";
                response = (HttpWebResponse)request.GetResponse();
                error_message = response.StatusDescription;
                response_code = ((int)response.StatusCode).ToString();
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                    error_message = ((HttpWebResponse)ex.Response).StatusDescription;
                else
                    error_message = ex.Message;

                if (ex.Response != null)
                    response_code = ((int)((HttpWebResponse)ex.Response).StatusCode).ToString();

                if (error_message.Contains("The remote name could not be resolved:"))
                {
                    response_code = "1000";
                }
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
            return;
        }
        

        private HashSet<string> GetAllLinksOnUrl(string url,out string error_message,out string response_code)
        {
            error_message = string.Empty;
            response_code = string.Empty;
            HttpWebResponse response = null;
            HashSet<string> linkList = null;
            try
            {
                Uri uri = null;
                Uri.TryCreate(url, UriKind.Absolute, out uri);
                if (uri == null)
                {
                    error_message = "Invalid Url: " + url;
                    return null;
                }
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Proxy = null;
                request.Timeout = timeOut;
                request.Method = "GET";
                response = (HttpWebResponse)request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream());
                string html = sr.ReadToEnd();
                Uri currentUri = new Uri(url.Replace(@"http://www.", @"http://").Replace(@"http://", @"http://www."));
                linkList = new HashSet<string>();
                foreach (Match m in linkParser.Matches(html))
                {
                    Uri muri = null;
                    int index = 0;
                    string temp = m.Value.ToString();
                    #region Url Clean Up

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

                    Uri.TryCreate(currentUri, temp, out muri);
                    if (muri != null)
                    {
                        if (Uri.Compare(muri, currentUri, UriComponents.Host, UriFormat.UriEscaped, StringComparison.CurrentCulture) == 0)
                        {
                            if (muri.ToString().Replace(@"http://", "").Replace(@"https://", "").Split('/').Length - 1 <= depth)
                                linkList.Add(muri.ToString());
                        }
                        else
                        {
                            string external_url = muri.Host;
                            if (!external_url.Contains(@"http://") && !external_url.Contains(@"https://"))
                                external_url = @"http://" + external_url;
                            if (external_url != @"http://")
                                this.externalList.Add(external_url);
                        }
                    }
                }
                error_message = response.StatusDescription;
                response_code = ((int)response.StatusCode).ToString();
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                    error_message = ((HttpWebResponse)ex.Response).StatusDescription;
                else
                    error_message = ex.Message;

                if(ex.Response != null)
                response_code = ((int)((HttpWebResponse)ex.Response).StatusCode).ToString();
                if(error_message == null)
                    error_message = string.Empty;
                if (error_message!=string.Empty && error_message.Contains("The remote name could not be resolved:"))
                {
                    response_code = "1000";
                }
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
        public void StartProcessing()
        {
            try
            {
                //For each link, process the current link by calling GetAllLinksOnUrl
                HashSet<string> currChilds = null;
                string error_message = string.Empty;
                string response_code = string.Empty;
                currChilds = this.GetAllLinksOnUrl(this.BaseUrl,out error_message, out response_code);
                lock (Form1.outputLock)
                {
                    OutputTable.Rows.Insert(0, this.BaseUrl.ToString(), response_code, error_message);
                    if (OutputTable.Rows.Count > 50)
                        OutputTable.Rows.RemoveAt(OutputTable.Rows.Count - 1);
                }
                if (error_message != "OK")
                {
                    lock(Form1.brokenLinksOutputLock)
                    {
                        brokenLinksSW.Write("\"" + BaseUrl + "\",\"" + response_code + "\",\"" + error_message + "\"" + Environment.NewLine);
                    }
                }
                if(error_message == "OK" && currChilds != null)
                foreach (string url in currChilds)
                {
                    this.AddChildLink(url);
                }
                for(int i=0;i<lastIndex;i++)
                {
                    try
                    {
                        currChilds = null;
                        error_message = string.Empty;
                        response_code = string.Empty;
                        currChilds = this.GetAllLinksOnUrl(this.ChildUrls[i].ToString(), out error_message, out response_code);
                        //if (this.ChildUrls.ContainsKey(i) && this.ChildUrls[i] != null)
                        {
                            lock (Form1.outputLock)
                            {
                                OutputTable.Rows.Insert(0, this.ChildUrls[i].ToString(), response_code, error_message);
                                if (OutputTable.Rows.Count > 50)
                                    OutputTable.Rows.RemoveAt(OutputTable.Rows.Count - 1);
                            }
                            if (error_message != "OK")
                            {
                                lock (Form1.brokenLinksOutputLock)
                                {
                                    brokenLinksSW.Write("\"" + this.ChildUrls[i].ToString() + "\",\"" + response_code + "\",\"" + error_message + "\"" + Environment.NewLine);
                                }
                            }
                            if (error_message == "OK" && currChilds != null)
                                foreach (string url in currChilds)
                                {
                                    this.AddChildLink(url);
                                }
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (Form1.brokenLinksOutputLock)
                        {
                            brokenLinksSW.Write("\"" + string.Empty + "\",\"" + ex.Message + "\",\"" + ex.StackTrace + "\"" + Environment.NewLine);
                        }
                    }
                }
                foreach (string url in this.externalList)
                {
                    try
                    {
                        error_message = string.Empty;
                        response_code = string.Empty;
                        this.GetStatusOnly(url, out error_message, out response_code);
                        lock (Form1.outputLock)
                        {
                            OutputTable.Rows.Insert(0, url, response_code, error_message);
                            if (OutputTable.Rows.Count > 50)
                                OutputTable.Rows.RemoveAt(OutputTable.Rows.Count - 1);
                        }
                        if (error_message != "OK")
                        {
                            lock (Form1.brokenLinksOutputLock)
                            {
                                brokenLinksSW.Write("\"" + url + "\",\"" + response_code + "\",\"" + error_message + "\"" + Environment.NewLine);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (Form1.brokenLinksOutputLock)
                        {
                            brokenLinksSW.Write("\"" + string.Empty + "\",\"" + ex.Message + "\",\"" + ex.StackTrace + "\"" + Environment.NewLine);
                        }
                    }
                }   
                ChildUrls.Clear();
                externalList.Clear();
                GC.Collect();
            }
            catch (Exception ex)
            {
                lock (Form1.brokenLinksOutputLock)
                {
                    brokenLinksSW.Write("\"" + string.Empty + "\",\"" + ex.Message + "\",\"" + ex.StackTrace + "\"" + Environment.NewLine);
                }
            }
            return;
        }
    }
}
