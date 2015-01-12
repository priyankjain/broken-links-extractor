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

namespace Broken_Links_Extractor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            for (int i = 0; i < max_threads; i++)
                this.Speed.Items.Add(i + 1);
            for (int i = 0; i < max_depth; i++)
                this.DepthSelector.Items.Add(i + 1);
        }
        private int classFileCounter = 0;
        private Queue<string> urlQueue = new Queue<string>();
        private string inputFileName = string.Empty;
        private int threads = int.Parse(ConfigurationManager.AppSettings["Default_Number_Of_Threads"].ToString());
        private int depth = int.Parse(ConfigurationManager.AppSettings["Default_Depth"].ToString());
        private int max_depth = int.Parse(ConfigurationManager.AppSettings["Max_Depth"].ToString());
        private int max_threads = int.Parse(ConfigurationManager.AppSettings["Max_Number_Of_Threads"].ToString());
        static private object waitLock = new object();
        static public object outputLock = new object();
        private string countFilePath = string.Empty;
        private int count = 1;
        static private object brokenLinksOutputLock = new object();
        private List<Thread> threadList = null;
        StreamWriter brokenLinksSW = null;
        private int leeWay = int.Parse(ConfigurationManager.AppSettings["Leeway"].ToString());
        private bool allLinksProcessed = false;
        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private int queueCounter = 0;
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
                    queueCounter++;
                    if (queueCounter % leeWay == 0)
                    {
                        count += leeWay;
                        WriteCount(count);
                        this.PercentageBox.Text = "Completed: " + Math.Round((((float)count)*100.0f / classFileCounter),2).ToString() + "%";
                    }
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

        private void WriteCount(int count)
        {
            StreamWriter countSW = new StreamWriter(countFilePath, false);
            countSW.Write(count);
            countSW.Close();
        }
        private void StartButton_Click(object sender, EventArgs e)
        {
            #region Actual Processing
            
            try
            {
                brokenLinksSW = new StreamWriter(Directory.GetCurrentDirectory() + "/broken_links.csv", true);
                this.OutputTable.Dock = DockStyle.Fill;
                Link.OutputTable = this.OutputTable;
                
                Link.depth = this.depth;
                Link.timeOut = int.Parse(ConfigurationManager.AppSettings["Timeout_In_Seconds"]) * 1000;
                
                countFilePath = Directory.GetCurrentDirectory() + "/count.txt";
                if (File.Exists(countFilePath)) // If count file exists, read it's value and ask the user whether he needs to start reading the file from current count?
                {
                    StreamReader countSR = new StreamReader(countFilePath);
                    while (!countSR.EndOfStream)
                    {
                        int.TryParse(countSR.ReadLine(),out count);
                    }
                    if (countSR != null)
                    {
                        countSR.Close();
                        countSR = null;
                    }
                    DialogResult choice = MessageBox.Show("Do you want to start processing the input file from line number " + count + "?", "Skip already processed urls", MessageBoxButtons.YesNo);
                    if (choice == DialogResult.No)
                        count = 1;
                }
                WriteCount(count);
                FileInfo inputFileInfo = new FileInfo(inputFileName);
                DateTime dt = inputFileInfo.LastWriteTime;
                int fileCounter = 1;
                while (true)
                {
                    allLinksProcessed = false; // A flag which will make all the threads stop when the main thread ends
                    #region Spawn the child consumer threads
                    threadList = new List<Thread>();
                    //Spawn the threads
                    for (int i = 0; i < threads; i++)
                    {
                        Thread thread = new Thread(NewThreadMethod);
                        threadList.Add(thread);
                        thread.IsBackground = true;
                        thread.Start();
                    }
                    #endregion
                    if (count >= leeWay)
                        count -= leeWay;
                    StreamReader sr = null;
                    try
                    {
                        sr = new StreamReader(inputFileName);
                        fileCounter = 1;
                        lock (waitLock)
                        {
                            while (!sr.EndOfStream)
                            {
                                string url = sr.ReadLine();
                                if(fileCounter >= count)
                                    urlQueue.Enqueue(url);
                                fileCounter++;
                            }
                        }
                        classFileCounter = fileCounter;
                    }
                    catch (Exception ex)
                    {
                        lock (outputLock)
                        {
                            this.OutputTable.Rows.Insert(0, string.Empty, string.Empty, "Error while opening one of the files: " + ex.Message);
                            if (this.OutputTable.Rows.Count > 50)
                                this.OutputTable.Rows.RemoveAt(this.OutputTable.Rows.Count - 1);
                        }
                    }
                    finally
                    {
                        if (sr != null)
                            sr.Close();
                    }
                    while (urlQueue.Count > 0)
                    {
                        Application.DoEvents();
                    }

                    allLinksProcessed = true;
                    foreach (Thread t in threadList)
                    {
                        t.Join();
                    }
                    WriteCount(fileCounter);
                    #region Logic to terminate the main thread
                    if (dt != (new FileInfo(inputFileName)).LastWriteTime)//If more links have been added to the input file
                    {
                        dt = (new FileInfo(inputFileName)).LastWriteTime;
                    }
                    else break;
                    #endregion

                }
                this.PercentageBox.Text = "Completed: 100%";
                MessageBox.Show("All links processed");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (brokenLinksSW != null)
                {
                    brokenLinksSW.Close();
                    brokenLinksSW = null;
                }
            }
            #endregion
            
        }

      
        private void UploadButton_Click(object sender, EventArgs e)
        {
            DialogResult result = this.openFileDialog1.ShowDialog();
            if(result == DialogResult.OK)
            {
                this.inputFileName = this.openFileDialog1.FileName.ToString();
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

        private void form1_Closed(object sender, FormClosingEventArgs e)
        {
            if(threadList != null)
                foreach (Thread t in threadList)
                {
                    t.Abort();
                }
        }
    }
}
