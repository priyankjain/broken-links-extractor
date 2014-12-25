using System;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Configuration;
using HtmlAgilityPack;

namespace Broken_Links_Extractor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            fileList = new List<string>();
            for (int i = 0; i < max_threads; i++)
                this.Speed.Items.Add(i + 1);
            for (int i = 0; i < max_depth; i++)
                this.DepthSelector.Items.Add(i + 1);
        }
        private string fileName = string.Empty;
        private List<string> fileList = null;
        private Queue<string> urlQueue = new Queue<string>();
        private int threads = int.Parse(ConfigurationManager.AppSettings["Default_Number_Of_Threads"].ToString());
        private int depth = int.Parse(ConfigurationManager.AppSettings["Default_Depth"].ToString());
        private int max_depth = int.Parse(ConfigurationManager.AppSettings["Max_Depth"].ToString());
        private int max_threads = int.Parse(ConfigurationManager.AppSettings["Max_Number_Of_Threads"].ToString());
        //private int maxQueueSize = 1000;
        static private object waitLock = new object();
        static private object emptyLock = new object();
        int runningThreads = 0;
        static private object outputLock = new object();
        static private object ouputFileLock = new object();
        private bool allLinksProcessed = false;
        static private object[] linkFilesLock = null;
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        StreamWriter[] sw = null;
        private void threadMethod()
        {
            for (; ; )
            {
                if (allLinksProcessed == true && urlQueue.Count == 0) return;
                while (urlQueue.Count == 0)
                {
                    Thread.Sleep(100);
                }
                HttpWebResponse response = null;
                string url = string.Empty;
                lock (waitLock)
                {
                    url = urlQueue.Dequeue();
                }
                try
                {
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                    request.Proxy = null;
                    request.Timeout = int.Parse(ConfigurationManager.AppSettings["Timeout_In_Seconds"])*1000;
                    request.Method = "GET";
                    response = (HttpWebResponse)request.GetResponse();
                    StreamReader sr = new StreamReader(response.GetResponseStream());
                    string html = sr.ReadToEnd();
                    Uri currentUri = new Uri(url);
                    Regex linkParser = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    HashSet<string> linkList = new HashSet<string>(); 
                    foreach(Match m in linkParser.Matches(html))
                    {
                        Uri muri = null;
                        lock (outputLock)
                        {
                            this.OutputBox.AppendText("Child Link for " + url + ": " + m.Value.ToString() + Environment.NewLine);
                        }
                        Uri.TryCreate(m.Value.ToString(),UriKind.RelativeOrAbsolute,out muri);
                        if(Uri.Compare(muri,currentUri,UriComponents.Host,UriFormat.SafeUnescaped,StringComparison.CurrentCulture) == 0)
                        {
                            linkList.Add(m.Value.ToString());
                        }
                    }
                    for(int i=1;i<=this.depth;i++)
                    {
                        HashSet<string> links_Of_Current_Depth = new HashSet<string>();
                        foreach(string matchedUrls in linkList)
                        {
                            if(matchedUrls.Replace(@"http\:\/\/","").Replace(@"https\:\/\/","").Split('/').Length - 1 == i )
                            {
                                links_Of_Current_Depth.Add(matchedUrls);
                            }
                        }
                        lock(linkFilesLock[i])
                        {
                            foreach(string final_Matches in links_Of_Current_Depth)
                            {
                                sw[i].WriteLine(final_Matches);
                            }
                        }
                    }
                    lock (outputLock)
                    {
                        this.OutputBox.AppendText(url +" " + Thread.CurrentThread.ManagedThreadId.ToString() + " " + response.StatusDescription + Environment.NewLine);
                    }
                }
                catch (WebException ex)
                {
                    lock (outputLock)
                    {
                        this.OutputBox.AppendText(url + " " + Thread.CurrentThread.ManagedThreadId.ToString() + " " + ex.Message + Environment.NewLine);
                    }
                }
                finally
                {
                    if (response != null)
                    {
                        response.Close();
                        response.Dispose();
                    }
                    
                }
            }
        }
        private void StartButton_Click(object sender, EventArgs e)
        {
            allLinksProcessed = false; // A flag which will make all the threads stop when the main thread ends

            #region Spawn the child consumer threads
            List<Thread> threadList = new List<Thread>();
            //Spawn the threads
            for (int i = 0; i < threads; i++)
            {
                Thread thread = new Thread(threadMethod);
                threadList.Add(thread);
                thread.IsBackground = true;
                thread.Start();
            }
            #endregion

            int counter_RunTimeFilesAdded = 0;
            while (true)
            {
                int inputFileCountBefore = fileList.Count;

                #region Create Shared Objects
                sw = new StreamWriter[this.depth+1];
                linkFilesLock = new object[this.depth+1];
                for (int i = 0; i<=this.depth;i++)
                {//Open File Handles for the different depths
                    sw[i] = new StreamWriter(Directory.GetCurrentDirectory()+"/links_depth_" + counter_RunTimeFilesAdded + "_" + i + ".txt",false);
                    linkFilesLock[i] = new object();
                }
                #endregion
                
                for(int i=0;i<=this.depth;i++)
                {
                    #region Collect links of depth 0
                    if(i == 0) // For the first depth, collect links from the various input files
                    {
                        HashSet<String> linkSet = new HashSet<string>();
                        foreach(string file in fileList)
                        {
                            StreamReader sr = null;
                            try
                            {
                                sr = new StreamReader(file);
                                while(!sr.EndOfStream)
                                {
                                    string url = sr.ReadLine();
                                    Uri uri = null;
                                    Uri.TryCreate(url,UriKind.Absolute,out uri);
                                    if(uri == null)
                                    {
                                        lock(outputLock)
                                        {
                                            this.OutputBox.AppendText("Invalid URL: "+url + " in file: " + file);
                                        }
                                    }
                                    else
                                    {
                                        linkSet.Add(url);
                                    }
                                }
                            }
                            catch(Exception ex)
                            {
                                lock(outputLock)
                                {
                                    this.OutputBox.AppendText("Error while opening one of the files: " + ex.Message);
                                }
                            }
                            finally
                            {
                                if (sr != null)
                                    sr.Close();
                            }
                        }
                        foreach(string url in linkSet)
                        {
                            sw[i].WriteLine(url);
                        }
                        lock(waitLock)
                        {
                            foreach(string url in linkSet)
                            {
                                urlQueue.Enqueue(url);
                            }
                        }
                        sw[i].Close();
                    }//All links of depth 0 written to file
                    #endregion
                    
                }   
                //foreach (string file in fileList)
                //{
                //    StreamReader sr = null;
                //    try
                //    {
                //        sr = new StreamReader(file);

                //        while (!sr.EndOfStream)
                //        {
                //            lock (waitLock)
                //            {
                //                while (urlQueue.Count < maxQueueSize && !sr.EndOfStream)
                //                {
                //                    urlQueue.Enqueue(sr.ReadLine());
                //                }
                //            }
                //            if (sr.EndOfStream)
                //                break;
                //            //Monitor.PulseAll(emptyLock);
                //            //Wait till more urls need to be added
                //            while (urlQueue.Count > (int)(maxQueueSize / 100))
                //            {
                //                Thread.Sleep(1000);
                //            }
                //        }
                //    }
                //    catch (IOException ex)
                //    {
                //        lock (outputLock)
                //        {
                //            this.OutputBox.AppendText("Error while opening one of the files: " + ex.Message);
                //        }
                //    }
                //    finally
                //    {
                //        if (sr != null)
                //            sr.Close();
                //    }
                //}

                #region Logic to terminate the main thread
                int inputFileCountAfter = fileList.Count;
                if (inputFileCountBefore == inputFileCountAfter) break;
                else
                {
                    for (int i = 0; i < inputFileCountBefore; i++)
                    {
                        fileList.RemoveAt(i);
                    }
                }
                #endregion

            } 
            #region Logic to terminate and wait for child threads and close shared files
            allLinksProcessed = true;
            foreach(Thread t in threadList)
            {
                t.Join();
            }
            for(int i=1;i<=this.depth;i++)
            {
                if(sw[i]!=null)
                sw[i].Close();
            }
            #endregion
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
        }
        private void OutputBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void UploadButton_Click(object sender, EventArgs e)
        {
            DialogResult result = this.openFileDialog1.ShowDialog();
            if(result == DialogResult.OK)
            {
                fileList.Add(this.openFileDialog1.FileName.ToString());
            }
        }
        private void Speed_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.threads = int.Parse(this.Speed.SelectedItem.ToString());
        }

        private void Depth_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.depth = int.Parse(this.DepthSelector.SelectedItem.ToString());
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter_1(object sender, EventArgs e)
        {

        }
    }
}
