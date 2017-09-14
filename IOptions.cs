﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace ThermoRawInfo
{
    public interface IOptions
    {
        [Option('f', "file", Required = true, HelpText = "Path to RAW file.")]
        string File { get; set; }
    }
}
