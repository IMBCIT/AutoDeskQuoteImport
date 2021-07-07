﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class CorruptedFileTypeS : FileTypeS
    {
        public readonly Exception Exception;

        public CorruptedFileTypeS(string path, Exception ex)
            : base(path)
        {
            Exception = ex;
        }
    }
}
