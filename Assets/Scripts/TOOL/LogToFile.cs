using System;
using System.IO;
using UnityEngine;


namespace LogToFile
{
    public class FileLog
    {
        private String LogFilePath;


        public FileLog(String logFileName)
        {
            LogFilePath = "Logs/logs_" + logFileName + ".log";
            File.CreateText(LogFilePath).Dispose();
        }

        public void Log(String line)
        {
            StreamWriter logWriter = File.AppendText(LogFilePath);
            logWriter.WriteLine(line);
            logWriter.Close();
        }
    }
}