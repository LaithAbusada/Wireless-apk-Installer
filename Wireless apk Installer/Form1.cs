

using MaterialSkin.Controls;
using SharpAdbClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Windows.Forms;
using WK.Libraries.BetterFolderBrowserNS;
using System.Net;
using System.Threading.Tasks;
using AltoHttp;

namespace bulk_apk_installer
{
    public partial class Form1 : MaterialForm
    {

        StreamWriter stdin = null;
        String path = null;
        String formname = "Wirless apk Installer";
        String pathwithot = "";
        int install_count = 0;
        private Process cmdProcess;

        public Form1()
        {
            InitializeComponent();
            this.Text = formname;

        }

        public void installadb()
        {

            string solutionDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;

            // Combine solution directory with "Downloads" folder and adb.exe
            string adbPath = Path.Combine(solutionDirectory, "Downloads", "adb.exe");

            Console.WriteLine(adbPath);
            if (File.Exists(adbPath))
            {
                AdbServer server = new AdbServer();
                var result = server.StartServer(adbPath, restartServerIfNewer: false);
                Console.WriteLine("Adb file exists, and success");
                
            }
            else
            {
                Console.WriteLine("ADB executable not found. Please make sure it is located in the 'Downloads' folder within your solution directory.");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            installadb();
            loadtext();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
          //                 
            
        }


        private void StartCmdProcess()
        {
            ProcessStartInfo pStartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "start /WAIT",
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                RedirectStandardOutput = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true,

            };

             cmdProcess = new Process
            {

                StartInfo = pStartInfo,
                EnableRaisingEvents = true,
            };

            cmdProcess.Start();
            materialMultiLineTextBox1.Text = string.Empty;
            cmdProcess.BeginErrorReadLine();
            cmdProcess.BeginOutputReadLine();
            stdin = cmdProcess.StandardInput;

            cmdProcess.OutputDataReceived += (s, evt) =>
            {
                if (evt.Data != null)
                {
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        materialMultiLineTextBox1.AppendText(evt.Data + Environment.NewLine);
                        materialMultiLineTextBox1.ScrollToCaret();
                    }));
                }
            };

            cmdProcess.ErrorDataReceived += (s, evt) =>
            {
                if (evt.Data != null)
                {
                    BeginInvoke(new Action(() =>
                    {
                        //rtbStdErr.AppendText(evt.Data + Environment.NewLine);
                        //rtbStdErr.ScrollToCaret();
                    }));
                }
            };

            cmdProcess.Exited += (s, evt) =>
            {
                // cmdProcess?.Dispose();
            };
        }

     
            
            
    

        private void materialMultiLineTextBox1_TextChanged(object sender, EventArgs e)
        {

            try
            {

                replace();

                int maxLines = materialMultiLineTextBox1.Lines.Length;
                string lastLine = materialMultiLineTextBox1.Lines[maxLines - 2];
                string lastWord = lastLine.Split(' ').Last();

                if (lastLine == "Success")
                {
                    install_count++;
                    progresstext();
                }
            }
            catch
            {

            }
            
        

        }

        
        void progresstext()
        {
            materialMultiLineTextBox1.AppendText("----------------------------------------------" + Environment.NewLine+ "-- Installed number of apk "+install_count+" --" + Environment.NewLine + "----------------------------------------------" + Environment.NewLine);
        }
        void loadtext()
        {
            materialMultiLineTextBox1.AppendText("..... (¯`v´¯)♥" + Environment.NewLine + ".......•.¸.•´" + Environment.NewLine + "....¸.•´" + Environment.NewLine + "... (" + Environment.NewLine + "☻/" + Environment.NewLine + "/▌♥♥" + Environment.NewLine + "/ \u005C ♥♥" + Environment.NewLine );

        }
        void replace()
        {
            String Directory = System.Windows.Forms.Application.StartupPath;
            materialMultiLineTextBox1.Text = materialMultiLineTextBox1.Text.Replace(Directory, "");
            materialMultiLineTextBox1.Text = materialMultiLineTextBox1.Text.Replace(">", "");
            materialMultiLineTextBox1.Text = materialMultiLineTextBox1.Text.Replace("for %e in", "");
            materialMultiLineTextBox1.Text = materialMultiLineTextBox1.Text.Replace("%", "");
            materialMultiLineTextBox1.Text = materialMultiLineTextBox1.Text.Replace("(", "");
            materialMultiLineTextBox1.Text = materialMultiLineTextBox1.Text.Replace(path, "");
            materialMultiLineTextBox1.Text = materialMultiLineTextBox1.Text.Replace("*.apk)", "");
            materialMultiLineTextBox1.Text = materialMultiLineTextBox1.Text.Replace("do", "");
            materialMultiLineTextBox1.Text = materialMultiLineTextBox1.Text.Replace("adb install", "");
            materialMultiLineTextBox1.Text = materialMultiLineTextBox1.Text.Replace("@echo", "");
            materialMultiLineTextBox1.Text = materialMultiLineTextBox1.Text.Replace("off", "");
            materialMultiLineTextBox1.Text = materialMultiLineTextBox1.Text.Replace("-r", "");
            materialMultiLineTextBox1.Text = materialMultiLineTextBox1.Text.Replace("\"%e\"", "");
        }
       

        private async void button3_Click_1Async(object sender, EventArgs e)
        {
            string ipAddress = textBoxIP.Text;
            int port = int.Parse(textBoxPort.Text);
            string connect = "adb connect " + ipAddress + ":" + port;
            MessageBox.Show("Connecting to device...");
            StartCmdProcess();
            try
            {
                stdin.Write("\u0040echo off" + Environment.NewLine);
                stdin.Write("---------------------------------------------------------------" + Environment.NewLine + "-- Trying to establish connection --" + Environment.NewLine + "---------------------------------------------------------------" + Environment.NewLine);
                stdin.Write(connect + Environment.NewLine);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // Add a delay to allow time for the connection process
            await Task.Delay(5000);

            // Now check if the connection was successful by searching for the confirmation message
            string[] lines = materialMultiLineTextBox1.Lines;
            string searchWord = "connected"; // Update this with the confirmation message
            bool wordFound = false;

            foreach (string line in lines)
            {
                if (line.Contains(searchWord))
                {
                    // Word found in this line
                    wordFound = true;
                    break; // Exit the loop since we found the word
                }
            }

            if (wordFound)
            {
                MessageBox.Show("Device connected successfully");
                listView1.Items.Add(ipAddress + ':' + port);
            }
            else
            {
                MessageBox.Show("Device unable to connect, please check IP and port");
            }

            cmdProcess.CloseMainWindow();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            string ipAddress = textBoxIP.Text;
            int port = int.Parse(textBoxPort.Text);
            string disconnect = "adb disconnect " + ipAddress + ":" + port;
            StartCmdProcess();
            try
            {
                stdin.Write("\u0040echo off" + Environment.NewLine);
                stdin.Write("---------------------------------------------------------------" + Environment.NewLine + "-- Disconnecting Device --" + Environment.NewLine + "---------------------------------------------------------------" + Environment.NewLine);
                stdin.Write(disconnect + Environment.NewLine);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            MessageBox.Show("Device disconnected successfully");
            listView1.Items.Clear();
            cmdProcess.CloseMainWindow();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

            string websiteUrl = "https://innovo.net/repo/TP4/nice-latest.apk";
            string downloadPath = "E:\\Wireless apk Installer\\Wireless apk Installer\\Downloads";
            listView1.Items.Add("1.1.1.1:5555");

            try
            {
                HttpDownloader httpDownloader;
                httpDownloader = new HttpDownloader(websiteUrl,$"{Application.StartupPath}\\{Path.GetFileName("nice-latest.apk")}");
                httpDownloader.Start();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading file: {ex.Message}");
            }

            if (listView1.Items.Count == 0)
            {
                MessageBox.Show("No Device has connected to this PC, Please connect the device and press the refresh button", "Error..", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else


            {
                string path = "";
                string installcom = $"for %%f in ({path}) do adb install -g -r -d %%f";


                StartCmdProcess();

                try
                {
                    stdin.Write("\u0040echo off" + Environment.NewLine);
                    stdin.Write("---------------------------------------------------------------" + Environment.NewLine + "-- Wait a minute the installation started --" + Environment.NewLine + "---------------------------------------------------------------" + Environment.NewLine);
                    stdin.Write(installcom + Environment.NewLine);

                }
                catch
                {

                }
                finally
                {
                    cmdProcess.CloseMainWindow();
                }



            }
        }

 
    }
}
