using System;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace Broken_Links_Extractor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            fileList = new List<string>();
            for (int i = 0; i < 100; i++)
                this.Speed.Items.Add(i + 1);
            for (int i = 0; i < 5; i++)
                this.DepthSelector.Items.Add(i + 1);
        }
        private string fileName = string.Empty;
        private List<string> fileList = null;
        private Queue<string> urlQueue = new Queue<string>();
        private int threads = 10;
        private int depth = 2;
        private int maxQueueSize = 1000;
        static private object waitLock = new object();
        static private object emptyLock = new object();
        int runningThreads = 0;
        static private object outputLock = new object();
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void threadMethod()
        {
            for (; ; )
            {
                //Monitor.Enter(waitLock);
                while (urlQueue.Count == 0)
                {
                    Thread.Sleep(100);
                    //Monitor.Exit(waitLock);
                    //Monitor.Wait(emptyLock);
                }
                //else Monitor.Exit(waitLock);
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
                    request.Timeout = 6000;
                    request.Method = "GET";
                    response = (HttpWebResponse)request.GetResponse();
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
            string message = string.Empty;
            message += this.DepthSelector.ToString() + " " + this.Speed.ToString() + " ";
            foreach (string file in fileList)
            {
                message += file + " ";
            }
            MessageBox.Show(message);
            //return;
            StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory()+"/output_"+DateTime.Now.ToString("ddmmyyyyhhMMss")+".txt",true);
            foreach (string file in fileList)
            {
                StreamReader sr = null;
                try
                {
                    sr = new StreamReader(file);
                    for (int i = 0; i < threads; i++)
                    {
                        Thread thread = new Thread(threadMethod);
                        thread.IsBackground = true;
                        thread.Start();
                        MessageBox.Show("Thread " + i + " started");
                    }
                    while (!sr.EndOfStream)
                    {
                        lock(waitLock)
                        {
                            while (urlQueue.Count < maxQueueSize && !sr.EndOfStream)
                            {
                                urlQueue.Enqueue(sr.ReadLine());
                            }
                        }
                        if (sr.EndOfStream)
                            break;
                        Monitor.PulseAll(emptyLock);
                        //Wait till more urls need to be added
                        while (urlQueue.Count > (int)(maxQueueSize/100))
                        {
                            Thread.Sleep(1000);
                        }
                    }
                }
                catch (IOException ex)
                {
                    lock(outputLock)
                    {
                        this.OutputBox.AppendText("Error while opening one of the files: " + ex.Message);
                    }
                }
                finally
                {
                    if(sr!=null)
                        sr.Close();
                }
            }
//            String[] urllist = { "twinpeaks.co.zw",
//"zanupfpub.co.zw",
//"dealgooder.com",
//"touchbaseafrica.co.uk",
//"zwnews.com",
//"fxflare.com",
//"nationalserviceresources.org"};
//            String message = string.Empty;
//            foreach(String url in urllist)
//            {
//                try
//                {
//                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://" + url);
//                    request.Method = "GET";
//                    request.Proxy = null;
//                    request.KeepAlive = false;
//                    request.Timeout = 6000;
//                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
//                }
//                catch (WebException ex)
//                {
//                    message += ex.Response + "\n-" + ex.Message + "\n";
//                }
//            }
//            this.OutputBox.AppendText(message);
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

        //void FinishWebRequest(IAsyncResult result)
        //{
        //    try
        //    {
        //        HttpWebResponse response = (result.AsyncState as HttpWebRequest).EndGetResponse(result) as HttpWebResponse;
        //        Http
        //        this.OutputBox.AppendText(response.StatusDescription + Environment.NewLine);
        //    }
        //    catch (WebException ex)
        //    {
        //        this.OutputBox.AppendText(ex.Message + Environment.NewLine);
        //    }
        //}
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
