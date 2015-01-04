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
        public static StreamWriter logFileCSV = new StreamWriter(Directory.GetCurrentDirectory() + "/logfile.csv",false);
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
        static private object waitLock = new object();
        static public object outputLock = new object();
        static private object ouputFileLock = new object();

        static private object brokenLinksOutputLock = new object();

        StreamWriter brokenLinksSW = new StreamWriter(Directory.GetCurrentDirectory() + "/broken_links.csv",false);

        private bool allLinksProcessed = false;
        private void Form1_Load(object sender, EventArgs e)
        {
           
        }


        private void NewThreadMethod()
        {
            for (; ; )
            {
                if (allLinksProcessed == true && urlQueue.Count == 0) Thread.CurrentThread.Abort();
                while (urlQueue.Count == 0)
                {
                    if (allLinksProcessed == true) Thread.CurrentThread.Abort();
                    Thread.Sleep(100);
                }
                string url = string.Empty;

                lock (waitLock)
                {
                    if (urlQueue.Count == 0) continue;
                    url = urlQueue.Dequeue();
                }
                Link linkObject = new Link(url);
                string broken_links = linkObject.StartProcessing();
                lock (brokenLinksOutputLock)
                {
                    brokenLinksSW.Write(broken_links);
                    brokenLinksSW.Flush();
                }
            }
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            this.OutputTable.Dock = DockStyle.Fill;
            Link.OutputTable = this.OutputTable;
            allLinksProcessed = false; // A flag which will make all the threads stop when the main thread ends
            Link.depth = this.depth;
            Link.timeOut = int.Parse(ConfigurationManager.AppSettings["Timeout_In_Seconds"]) * 1000;
            #region Spawn the child consumer threads
            List<Thread> threadList = new List<Thread>();
            //Spawn the threads
            for (int i = 0; i < threads; i++)
            {
                Thread thread = new Thread(NewThreadMethod);
                threadList.Add(thread);
                thread.IsBackground = true;
                thread.Start();
            }
            #endregion
            int counter_RunTimeFilesAdded = 0;
            while (true)
            {
                int inputFileCountBefore = fileList.Count;

                        HashSet<String> linkSet = new HashSet<string>();
                        foreach (string file in fileList)
                        {
                            StreamReader sr = null;
                            try
                            {
                                sr = new StreamReader(file);
                                while (!sr.EndOfStream)
                                {
                                    string url = sr.ReadLine();
                                    Uri uri = null;
                                    Uri.TryCreate(url, UriKind.Absolute, out uri);
                                    if (uri == null)
                                    {
                                        lock (outputLock)
                                        {
                                            //this.OutputTable.SuspendLayout();
                                            this.OutputTable.Rows.Insert(0,string.Empty,string.Empty,"Invalid URL: " + url + " in file: " + file);
                                            logFileCSV.WriteLine("\"\",\"\",\"" + "Invalid URL: " + url + " in file: " + file + "\"");
                                            if (this.OutputTable.Rows.Count > 50)
                                                this.OutputTable.Rows.RemoveAt(this.OutputTable.Rows.Count - 1);
                                            //this.OutputTable.ResumeLayout();
                                        }
                                    }
                                    else
                                    {
                                        linkSet.Add(url);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                lock (outputLock)
                                {
                                    //this.OutputTable.SuspendLayout();
                                    this.OutputTable.Rows.Insert(0,string.Empty, string.Empty, "Error while opening one of the files: " + ex.Message);
                                    logFileCSV.WriteLine("\"\",\"\",\"" + "Error while opening one of the files: " + ex.Message + "\"");
                                    if (this.OutputTable.Rows.Count > 50)
                                        this.OutputTable.Rows.RemoveAt(this.OutputTable.Rows.Count - 1);
                                    //this.OutputTable.ResumeLayout();
                                }
                            }
                            finally
                            {
                                if (sr != null)
                                    sr.Close();
                            }
                        }
                        lock (waitLock)
                        {
                            foreach (string url in linkSet)
                            {
                                urlQueue.Enqueue(url);
                            }
                        }
                    
                    while (urlQueue.Count > 0)
                    {
                        Application.DoEvents();
                    }
                #region Logic to terminate the main thread
                int inputFileCountAfter = fileList.Count;
                if (inputFileCountBefore == inputFileCountAfter) break;
                else
                {
                    for (int i = 0; i < inputFileCountBefore; i++)
                    {
                        fileList.RemoveAt(i);
                    }
                    counter_RunTimeFilesAdded++;
                }
                #endregion

            }
            #region Logic to terminate and wait for child threads and close shared files
            allLinksProcessed = true;
            MessageBox.Show("Flag for termination set");
            foreach (Thread t in threadList)
            {
                t.Join();
            }
            
            brokenLinksSW.Close();
            MessageBox.Show("Application exit");
            Link.LogFile.Close();
            #endregion
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

        private void OutputTable_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        private void OutputTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
