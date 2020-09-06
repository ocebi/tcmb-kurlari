using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace DataToServer
{
    class InnerOperation
    {
        System.Timers.Timer timer = new System.Timers.Timer();
        public void Start()
        {
            Console.WriteLine("On start: ");
            int timeToSleep = 5000;
            timeToSleep = CalculateRemainingTime();
            if (timeToSleep < 0)
            {
                timeToSleep = 50400000; // 14 hours
                //timeToSleep = 10000; //10 seconds for test
            }
            Console.WriteLine("Time to sleep: " + timeToSleep);
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = timeToSleep; //number in ms  
            timer.Enabled = true;
            //timer.Start();
        }
        public void Stop()
        {
            
        }
        
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            //call the program to pull data and store in database
            timer.Enabled = false;
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo directory = new DirectoryInfo(Path.GetFullPath(Path.Combine(currentDir, "..\\..\\..\\..\\ParseXML\\XMLParser\\bin\\Debug\\XMLParser.exe 2")));
            Console.WriteLine("Exe path: " + directory.ToString());

            
            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Normal,
                CreateNoWindow = false, //true
                UseShellExecute = false,
                FileName = "cmd.exe",
                Arguments = @"/C " + directory.ToString()
            };

            var process = Process.Start(processInfo);

            ServiceController service = new ServiceController("CurrencyReader");
            service.Stop();

        }

        public int CalculateRemainingTime()
        {
            
            var client = new TcpClient("time.nist.gov", 13); //to get real time in case local time is not valid
            using (var streamReader = new StreamReader(client.GetStream()))
            {
                var response = streamReader.ReadToEnd();
                var utcDateTimeString = response.Substring(7, 17);
                var localDateTime = DateTime.ParseExact(utcDateTimeString, "dd-MM-yy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                Console.WriteLine("Localdatetime.min: " + localDateTime.Minute);
                DateTime targetTime = new DateTime(localDateTime.Year, localDateTime.Month, localDateTime.Day, 15, 35, 0);
                //if (localDateTime.Hour > 15 || (localDateTime.Hour == 15 && localDateTime.Minute >= 35))
                if (localDateTime.Hour > 15 || (localDateTime.Hour == 15 && localDateTime.Minute >= 35))
                {
                    Console.WriteLine("Calculated: negative");
                    return -1; // sleep for 14 hours
                }
                else
                {
                    Console.WriteLine("Calculated: " + Convert.ToInt32(targetTime.Subtract(localDateTime).TotalMilliseconds));
                    return Convert.ToInt32(targetTime.Subtract(localDateTime).TotalMilliseconds); //time required to pull data
                }
            }
            
        }
    }
}
