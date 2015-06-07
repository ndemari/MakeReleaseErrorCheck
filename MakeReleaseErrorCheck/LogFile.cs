using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace MakeReleaseErrorCheck
{
    class LogFile
    {
        private List<string> _errorsFound = new List<string>();

        private bool _noIssuesFound;
        private int _lineLocation;

        public List<string> ErrorsFound
        {
            get
            {
                return _errorsFound;
            }
            set
            {
                _errorsFound = value;
            }
        }

        public int lineLocation 
        { 
            get 
            {
                return _lineLocation;
            }
            set
            {
                _lineLocation = value;
            }
        }

        public bool noIssuesFound
        {
            get
            {
                return _noIssuesFound;
            }
            set
            {
                _noIssuesFound = value;
            }
        }

        public LogFile()
        {
            this.lineLocation = 0;
            this.noIssuesFound = true;
        }

        public LogFile(int lineLoc, bool noIssues)
        {
            this.lineLocation = lineLoc;
            this.noIssuesFound = noIssues;
        }
        
        private void NoneNumCharBeforeWrite(string vError, List<string> logFileParse)
        {
            noIssuesFound = false;
            if (logFileParse[lineLocation - 2] == System.Environment.NewLine)
            {
                this._errorsFound.Add(vError.ToUpper() + " has been found at line - " + lineLocation + "!" + System.Environment.NewLine + logFileParse[lineLocation - 2]); 
            }
            else
            {
                this._errorsFound.Add(vError.ToUpper() + " has been found at line - " + lineLocation + "!" + System.Environment.NewLine + logFileParse[lineLocation - 1] + logFileParse[lineLocation + 2]);
            }
        }

        public List<string> ErrorSearchFunc(string vError)
        {
            List<string> logFileParse = new List<string>();
            StreamReader inFile = new StreamReader("MakeNSRelease.log"); // LIVE LOG FILE
            //StreamReader inFile = new StreamReader("C:\\MakeNSRelease.log"); // TEST LOG FILE WITH ERRORS
            //StreamReader inFile = new StreamReader("C:\\MakeNSRelease (2).log"); // TEST LOG FILE WITHOUT ERRORS

            string line;
            int errors;
            this.lineLocation = 0;
            while ((line = inFile.ReadLine()) != null) // File being read
            {
                logFileParse.Add(line);
            }

            inFile.DiscardBufferedData();
            inFile.BaseStream.Seek(0, SeekOrigin.Begin);
            inFile.BaseStream.Position = 0;

            while ((line = inFile.ReadLine()) != null) // TODO: File being read again. Change to a for loop that looks at logFileParse..
            {
                lineLocation++;

                int index = line.IndexOf(vError);
                if (index != -1)
                {
                    string charBefore = line.Substring(index - 2, 1);

                    bool parseResult = Int32.TryParse(charBefore, out errors);

                    if (errors > 0 && parseResult)
                    {
                        noIssuesFound = false;
                        this._errorsFound.Add(vError.ToUpper() + " has been found at line - " + lineLocation + "!" + System.Environment.NewLine + line);
                    }
                    else
                    {
                        if (charBefore == ":")
                        {
                            NoneNumCharBeforeWrite(vError, logFileParse);
                        }
                        else if (charBefore == "-")
                        {
                            NoneNumCharBeforeWrite(vError, logFileParse);
                        }
                    }
                }
            }

            return _errorsFound;
        }

        public void ToConsoleLogReport(List<string> foundResults)
        {
            foreach (string item in foundResults)
            {
                Console.WriteLine("\n" + item);
            }
        }

        public void ToLogFileReport(List<string> foundResults, string toLogFileReportFile)
        {
            if (noIssuesFound)
            {
                return;
            }

            StreamWriter outFile = new StreamWriter(toLogFileReportFile, false);

            foreach (string item in foundResults)
            {
                outFile.WriteLine("\n" + item);
            }
            outFile.Close();
        }
    }
}
