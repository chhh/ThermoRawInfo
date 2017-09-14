using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommandLine;
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoFisher.CommonCore.RawFileReader;

namespace ThermoRawInfo
{
    public class AppStart
    {
        public static void Main(string[] args)
        {
            // hack for Windows to disable QuickEdit mode on any console window
            if (Environment.OSVersion.ToString().Contains("Windows"))
            {
                //DisableConsoleQuickEdit.Go();
            }


            Parser.Default.ParseArguments<InfoOpts, IsolationInfoOpts>(args)
                .WithParsed<InfoOpts>(RunDumpInfo)
                .WithParsed<IsolationInfoOpts>(RunIsolationInfo)
                .WithNotParsed(InputErrors);
        }

        private static void InputErrors(IEnumerable<Error> enumerable)
        {
            
        }


        public static void RunDumpInfo(InfoOpts opts)
        {
            try
            {
                ValidateInfoOpts(opts);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Input validation error: ");
                Console.Error.WriteLine(ex.Message);
                //throw;
                return;
            }

            IRawDataPlus rawFile = null;
            string filename = opts.File;
            try
            {
                // Create the IRawDataPlus object for accessing the RAW file
                rawFile = RawFileReaderAdapter.FileFactory(filename);
                ValidateOpenedRawFile(rawFile);

                // RAW file has been validated and in good shape
                Program.DumpSystemInfo();
                Program.DumpFileInfo(rawFile);

            }
            finally
            {
                rawFile?.Dispose();
            }
        }

        public static void ValidateInfoOpts(InfoOpts opts)
        {
            ValidateFile(opts.File);
        }

        public static void RunIsolationInfo(IsolationInfoOpts opts)
        {
            try
            {
                ValidateIsolationOpts(opts);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Input validation errros!");
                Console.Error.WriteLine(ex.Message);
                //throw;
                return;
            }

            IRawDataPlus rawFile = null;
            string filename = opts.File;
            try
            {
                // Create the IRawDataPlus object for accessing the RAW file
                rawFile = RawFileReaderAdapter.FileFactory(filename);
                ValidateOpenedRawFile(rawFile);

                // RAW file has been validated and in good shape
                Program.DumpIsolationInfo(rawFile, opts.ScanNum);
            }
            finally
            {
                rawFile?.Dispose();
            }
        }

        public static void ValidateIsolationOpts(IsolationInfoOpts opts)
        {
            ValidateFile(opts.File);
            if (opts.ScanNum != null)
            {
                int[] scans = opts.ScanNum.ToArray();
                foreach (int scan in scans)
                {
                    if (scan < 1)
                    {
                        FailWithMessage("Scan numbers must be greater than zero.");
                    }
                }
                if (scans.Length == 2 && scans[1] < scans[0])
                {
                    FailWithMessage("Higher bound of scan range must be greater or equal to lower.");
                }
                if (scans.Length > 2)
                {
                    FailWithMessage("Scan range must be defined by 1 or 2 numbers.");
                }
            }
        }

        private static void FailWithMessage(string msg)
        {
            //Console.Error.WriteLine(msg);
            throw new Exception(msg);
        }

        private static void ValidateOpenedRawFile(IRawDataPlus raw)
        {
            string filename = raw.FileName;
            if (!raw.IsOpen)
            {
                FailWithMessage($"Unable to access the RAW file using the RawFileReader class! ('{filename}')");
            }

            // Check for any errors in the RAW file
            if (raw.IsError)
            {
                FailWithMessage($"Error opening ({raw.FileError}) - {filename}");
            }

            // Check if the RAW file is being acquired
            if (raw.InAcquisition)
            {
                FailWithMessage($"RAW file still being acquired: '{filename}'");
            }
        }

        public static void ValidateFile(string file)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                FailWithMessage("File can't be an empty string.");
            }
            if (!File.Exists(file))
            {
                FailWithMessage($"File doesn't exist: '{file}'");
            }
        }

        // This was a working attempt to use Microsoft.Extensions.CommandLineUtils to do command line parsing
        //public static void Main(string[] args)
        //{
        //    var app = new CommandLineApplication { Name = "ThermoRawInfo" };
        //    app.HelpOption("-?|-h|--help");

        //    var file = app.Option("-f|--file", "Path to RAW file.", CommandOptionType.SingleValue);

        //    app.Command("info", cmd =>
        //    {
        //        cmd.Description = "Print information about a file";
        //        cmd.HelpOption("-?|-h|--help");

        //        cmd.OnExecute(() =>
        //        {
        //            Console.WriteLine($"STUB: will print info about file: {file}");
        //            return 0;
        //        });
        //    });

        //    //app.Command("")

        //    app.OnExecute(() =>
        //    {
        //        Console.WriteLine($"Knowledge is power! ({file})");
        //        return 0;
        //    });

        //    app.Execute(args);
        //}
    }
}
