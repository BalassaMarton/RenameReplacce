using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RenameReplace
{
    public class RenameReplaceContext
    {
        public string WorkingDir { get;set; }
        public string Glob { get; set; }

        public string OldText { get; set; }
        public string NewText { get; set; }

        public List<string> FilesToRename { get; } = new List<string>();
        public List<string> FilesToScan { get; } = new List<string>();
    }
}