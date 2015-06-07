/*                                 Change Log

Date     Programmer    CSR   Description of Change
======== ============= ===== =====================================================
04/28/15 ndemari             Initial coding
05/14/15 ndemari             Added output log file if issues are found
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using MakeReleaseErrorCheck;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ConsoleApplication
{
    class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                DateTime nowName = DateTime.Now;

                string filePath = @"C:\somefolderpathhere";
                string fileName = "LogSearchResult_" + nowName.ToString("MM-dd-yy_HHmm") + ".log";
                string toLogFileReportFile = Path.Combine(filePath, fileName);

                LogFile log = new LogFile(0, true);
                
                Stopwatch watch = Stopwatch.StartNew();

                List<string> foundResults = new List<string>();

                List<string> issues = new List<string>();
                issues.Add("error");
                //issues.Add("error(s)");
                issues.Add("skipped");
                issues.Add("failed");
                issues.Add("FAILED");
                issues.Add("warning(s)");
                issues.Add("fatal");

                foreach (string item in issues)
                {
                    foundResults = log.ErrorSearchFunc(item); //TODO: Turn this into a function that passes into ToLogFileReport() and ToConsoleLogReport()
                    Console.WriteLine();
                }

                log.ToLogFileReport(foundResults, toLogFileReportFile);
                log.ToConsoleLogReport(foundResults);

                if (log.noIssuesFound)
                {
                    Console.WriteLine("\nNo issues found...");
                    Console.WriteLine();
                }
                else
                {
                    Process.Start("notepad++.exe", toLogFileReportFile);
                }

                watch.Stop();

                Console.WriteLine("\nThe search took " + watch.ElapsedMilliseconds.ToString() + " milliseconds.");
                Console.WriteLine("\n" + log.lineLocation.ToString() + " lines searched.");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }
    }
}
