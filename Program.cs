﻿using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace PlayOnTrimmer
{
    class Program
    {

        public static string ffmpegLocation = @"C:\dev\ffmpeg\bin\ffmpeg.exe";

        private static string[] getVideoFiles(string folder)
        {
            List<string> VideoExt = new List<string> { ".mp4", ".avi" };
            string[] rawFiles = Directory.GetFiles( folder);
            string[] filteredFiles = rawFiles.Where(s => VideoExt.Contains(Path.GetExtension(s))).ToArray();
            Array.Sort(filteredFiles, StringComparer.CurrentCultureIgnoreCase);
            return (filteredFiles);
        }
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (args.Length<3) {
                Console.WriteLine("Usage: targetPath SecAtStart SecAtEnd");
                Console.WriteLine("Example: dotnet run \"c:\\media\\shows\" 11 23");
                Console.WriteLine("That will create a trim.bat file in the \"c:\\media\\shows\" folder that cuts 11 sec from the start and 23 sec from the end of all .mp4 and .avi files in the folder.");
                return;
            }
            string targetPath = Path.GetFullPath(args[0]);
            if (!targetPath.EndsWith("\\")) targetPath +="\\";
            string startCut = args[1];
            string endCut = args[2];

            Console.WriteLine("Processing: " + targetPath);

            int startCutInt = Convert.ToInt16(startCut);
            int endCutInt = Convert.ToInt16(endCut);

            
            // IBM00858 encoding is required for Danish letters to work inside batch file
            string[] fileEntries = getVideoFiles(targetPath);
            String filetext = "";
            filetext += "chcp 65001" + Environment.NewLine; //Ensure the batch is run using UTF-8 encoding
            filetext += "md ok" + Environment.NewLine;

            foreach (string fileName in fileEntries)
            {
                string fileInfo = ffmpegHelper.command(@" -i """ + targetPath + Path.GetFileName(fileName) + "\"");
                int DurationPos = fileInfo.IndexOf("Duration");
                string DurationString = fileInfo.Substring(DurationPos+10, 11);
                TimeSpan Duration;
                TimeSpan.TryParseExact(DurationString, @"hh\:mm\:ss\.ff", null, out Duration);
                Duration = Duration.Subtract(TimeSpan.FromSeconds(endCutInt+startCutInt) );
                String EndDuration = Duration.ToString(@"hh\:mm\:ss");

                if (startCut.Length==1) startCut = "0" + startCut;
                string batchCommand = @" -i """ + targetPath + Path.GetFileName(fileName) + @""" -vcodec copy -acodec copy -ss 00:00:" + startCut + @".000 -t " + EndDuration + @".000 """+targetPath+@"ok\" + Path.GetFileName(fileName) + @"""" + Environment.NewLine;
                filetext += ffmpegLocation + batchCommand + Environment.NewLine;
            }
            File.WriteAllText(targetPath + "trim.bat", filetext);
            Console.WriteLine("trim.bat created");

        }
    }

    public static class ffmpegHelper
    {
        public static string command(this string cmd)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    
                    FileName = Program.ffmpegLocation, 
                    Arguments = cmd, 
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    
                }
            };
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            result += process.StandardError.ReadToEnd();
            process.WaitForExit();
            return result;
        }
    }

}

