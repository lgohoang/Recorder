using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Media;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Recorder
{
    class Program
    {

        static void Main(string[] args)
        {
            Program prg = new Program();
            prg.Process();
           
            Console.WriteLine("Main thread will exited");
            Thread.Sleep(2000);
        }



        public void Recording(string filename)
        {
            int err = 0;
            err = mciSendString("open new Type waveaudio Alias recsound", "", 0, 0);
            if(err != 0)
            {
                Console.WriteLine("open new Type waveaudio Alias recsound " + err);
                err = 0;
            }

            err = mciSendString("record recsound", "", 0, 0);
            if (err != 0)
            {
                Console.WriteLine("record recsounde " + err);
                err = 0;
            }
        }

        

        public void RecordStopped(string filename)
        {
            var output = Path.Combine(path, folder, filename + ".wav");
            int err = 0;
            mciSendString(@"save recsound " + output, "", 0, 0);
            if (err != 0)
            {
                Console.WriteLine("save recsound " + err);
                err = 0;
            }
            mciSendString("close recsound ", "", 0, 0);
            if (err != 0)
            {
                Console.WriteLine("close recsound " + err);
                err = 0;
            }
            wtm(filename);
        }


        public void wtm(string filename)
        {
            var intput = Path.Combine(path, folder, filename + ".wav");
            var output = Path.Combine(path, folder, filename + ".mp3");
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.FileName = AppDomain.CurrentDomain.BaseDirectory + @"lame\lame.exe";
            psi.Arguments = "-b 128 " + intput + " " + output;
            Process p = System.Diagnostics.Process.Start(psi);
            p.Close();
            p.Dispose();
        }

        static string folder = "Record";
        static string path = AppDomain.CurrentDomain.BaseDirectory;
        static string filename = "";
        public void Process()
        {
            string lastname = "";
            bool recorder = false;
            while (true)
            {
                
                if (recorder)
                {
                    Logger("Recorder started");
                    recorder = true;
                    lastname = filename;
                    Recording(filename);

                }

                Logger("File name = ", false);
                filename = Console.ReadLine();
                if (filename.ToLower().Equals("exit"))
                    return;

                if (!filename.Equals(lastname))
                {
                    if (recorder)
                    {
                        Logger("Recorder stopped");
                        RecordStopped(filename);
                        recorder = false;
                    }
                    if (!filename.Equals(""))
                    {
                        recorder = true;
                    }
                    
                }
                else
                {
                    Logger("Recorder ...");
                    recorder = false;
                }

                
            }
        }
        public void Dispose()
        {
            Console.WriteLine("Dispose"); 
        }

        [DllImport("winmm.dll", EntryPoint = "mciSendStringA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int mciSendString(string lpstrCommand, string lpstrReturnString, int uReturnLength, int hwndCallback);

        public void Logger(string log, bool enter = true)
        {
            if (enter)
                Console.WriteLine(DateTime.Now + " " + log);
            else
                Console.Write(DateTime.Now + " " + log);
        }
    }
}
