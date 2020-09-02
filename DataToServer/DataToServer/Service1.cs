using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DataToServer
{
    public partial class Service1 : ServiceBase
    {
        Timer timer = new Timer();
        //test if the service works. install with installutil.exe then set the target time to 5 minutes from now.

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            int timeToSleep = CalculateRemainingTime();
            if(timeToSleep < 0)
            {
                timeToSleep = 50400000; // 14 hours
            }
            
            //WriteToFile("Service is started at " + DateTime.Now);
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = timeToSleep; //number in ms  
            timer.Enabled = true;
        }
        protected override void OnStop()
        {
            //WriteToFile("Service is stopped at " + DateTime.Now);
        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            //call the program to pull data and store in database
            string currentDir = Environment.CurrentDirectory;
            DirectoryInfo directory = new DirectoryInfo(Path.GetFullPath(Path.Combine(currentDir, "..\\..\\..\\..\\ParseXML\\XMLParser\\bin\\Debug\\XMLParser.exe")));
            Process.Start(directory.ToString()); //check if it works

        }

        private int CalculateRemainingTime()
        {
            var client = new TcpClient("time.nist.gov", 13); //to get real time in case local time is not valid
            using (var streamReader = new StreamReader(client.GetStream()))
            {
                var response = streamReader.ReadToEnd();
                var utcDateTimeString = response.Substring(7, 17);
                var localDateTime = DateTime.ParseExact(utcDateTimeString, "dd-MM-yy hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

                DateTime targetTime = new DateTime(localDateTime.Year, localDateTime.Month, localDateTime.Day, 15, 35, 0);
                if (localDateTime.Hour > 15 || (localDateTime.Hour == 15 && localDateTime.Minute >= 35))
                {
                    return -1; // sleep for 14 hours
                }
                else
                {
                    return targetTime.Subtract(localDateTime).Milliseconds; //time required to pull data
                }
            }
        }
        
    }
}
