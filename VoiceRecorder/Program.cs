using System;
using NAudio.Wave;
using System.Diagnostics;
using System.IO;
using NAudio.Wave.Compression;

namespace VoiceRecorder
{
    class Program
    {
        public WaveInEvent waveSource = null;
        public WaveFileWriter waveFile = null;

        static void Main(string[] args)
        {
            Program prg = new Program();
            prg.Init();
            prg.Process();
        }

        public void Recording()
        {
            var output = Path.Combine(path, folder, filename + ".wav");
            waveSource = new WaveInEvent();
            waveSource.WaveFormat = new WaveFormat(44100, 1);
            waveSource.DataAvailable += new EventHandler<WaveInEventArgs>(waveSource_DataAvailable);
            waveSource.RecordingStopped += new EventHandler<StoppedEventArgs>(waveSource_RecordingStopped);
            waveFile = new WaveFileWriter(output, waveSource.WaveFormat);

            Logger("Recorder start with filename = " + filename);
            waveSource.StartRecording();
        }

        public void RecordStopped()
        {
            waveSource.StopRecording();
        }

        public void Init()
        {

        }

        void waveSource_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (waveFile != null)
            {
                waveFile.Write(e.Buffer, 0, e.BytesRecorded);
                waveFile.Flush();
            }
        }

        string filename = "";
        string lastname = "";
        void waveSource_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (waveSource != null)
            {
                waveSource = null;
            }

            if (waveFile != null)
            {
                try
                {
                    waveFile.Dispose();
                }
                catch (Exception ex)
                {
                    Logger(ex.ToString());
                }
                waveFile = null;
            }

            wtm(lastname);
        }

        public void wtm(string filename)
        {
            var intput = Path.Combine(path, folder, filename + ".wav");
            var output = Path.Combine(path, folder, filename + ".mp3");
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.FileName = AppDomain.CurrentDomain.BaseDirectory + @"lame.exe";
            psi.Arguments = "-b 128 " + intput + " " + output;
            Process p = System.Diagnostics.Process.Start(psi);
            p.Close();
            p.Dispose();

            Logger("Recorder saved: " + output);
        }

        static string folder = "Record";
        static string path = AppDomain.CurrentDomain.BaseDirectory;
        
        enum Status
        {
            waitRecord = 0,
            waitStop = 1,
        };

        Status status = Status.waitRecord;


        public void Process()
        {
            bool recorder = false;
            while (true)
            {
                Logger("Command = ", false);

                filename = Console.ReadLine();
                if (filename.ToLower().Equals("exit"))
                {
                    if(status == Status.waitStop)
                    {
                        RecordStopped();
                    }
                    return;
                }
                    

                switch (status)
                {
                    case Status.waitRecord:
                        if (filename.ToLower().Equals("start"))
                        {
                            status = Status.waitStop;
                        }
                        break;
                    case Status.waitStop:

                        if (filename.ToLower().Equals("stop"))
                        {
                            RecordStopped();
                            recorder = false;
                        }
                        else if(recorder == false)
                        {
                            Recording();
                            lastname = filename;
                            recorder = true;
                        }
                        break;
                }

            }
        }

        //public void Process()
        //{
        //    bool recorder = false;
        //    while (true)
        //    {

        //        if (recorder)
        //        {
        //            Logger("Recorder started");
        //            recorder = true;
        //            lastname = filename;
        //            Recording();

        //        }

        //        Logger("File name = ", false);
        //        filename = Console.ReadLine();
        //        if (filename.ToLower().Equals("exit"))
        //            return;

        //        if (!filename.Equals(lastname))
        //        {
        //            if (recorder)
        //            {
        //                Logger("Recorder stopped");
        //                RecordStopped();
        //                recorder = false;
        //            }
        //            if (!filename.Equals(""))
        //            {
        //                recorder = true;
        //            }

        //        }
        //        else
        //        {
        //            Logger("Recorder ...");
        //            recorder = false;
        //        }
        //    }
        //}
        public void Logger(string log, bool enter = true)
        {
            if (enter)
                Console.WriteLine(DateTime.Now + " " + log);
            else
                Console.Write(DateTime.Now + " " + log);
        }
    }
}
