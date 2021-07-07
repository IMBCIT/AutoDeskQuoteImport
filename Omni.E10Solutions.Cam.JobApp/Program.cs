using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Omni.E10Solutions.Cam.JobLibrary;

namespace Omni.E10Solutions.Cam.JobApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Main_Production();
        }

        static void Main_Production()
        {
            log4net.Config.XmlConfigurator.Configure(); // set up the logger.

            var logger = LogManager.GetLogger("logger");
            var stopwatch = Stopwatch.StartNew();

            try
            {
                List<Task> tasks = new List<Task>();
                tasks.Add(Task.Run(() => ImportProcess_Main()));
                //tasks.Add(Task.Run(() => PlantANA()));
                //tasks.Add(Task.Run(() => PlantDUC()));
                //tasks.Add(Task.Run(() => PlantEVE()));
                //tasks.Add(Task.Run(() => PlantSAC()));

                var task = Task.WhenAll(tasks);
                task.Wait();

                stopwatch.Stop();
                logger.Info("The program has terminated. Approx. Time: " + stopwatch.Elapsed);
            }
            catch (Exception ex)
            {
                logger.Error("The Quote Import App encountered an error it could not handle. " + ex.Message, ex);
            }
        }

        static void Main_Development()
        {
            log4net.Config.XmlConfigurator.Configure(); // set up the logger.

            var logger = LogManager.GetLogger("logger");
            var stopwatch = Stopwatch.StartNew();

            try
            {
                List<Task> tasks = new List<Task>();
                tasks.Add(Task.Run(() => PlantANA_Development()));
                // tasks.Add(Task.Run(() => PlantDUC()));
                //tasks.Add(Task.Run(() => PlantEVE()));
                //tasks.Add(Task.Run(() => PlantSAC()));

                var task = Task.WhenAll(tasks);
                task.Wait();

                stopwatch.Stop();
                logger.Info("The program has terminated with no Exceptions. Approx. Time: " + stopwatch.Elapsed);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                logger.Error("The Cam Integration (Quote) App encountered an error it could not handle. " + ex.Message, ex);
            }
        }

        static void ImportProcess_Main()
        {
            var logger = LogManager.GetLogger("logger");

            foreach (var company in Directory.GetDirectories(Settings.Default.LoadPath))
            {
                foreach (var plant in Directory.GetDirectories(company))
                {
                    logger.InfoFormat("=> Loading from {0}", plant);
                    string path(string folder) => Path.Combine(plant, folder);
                    var @params = new DirectoryParameter(plant, path("InProcess"), path("Archive"), path("Invalid"), "*.txt", SearchOption.TopDirectoryOnly);
                    JobImportProcess.Run(@params, logger, Settings.Default.AppServerUrl);
                }
            }

            //Console.WriteLine("Press <ENTER> to continue.");
            //Console.ReadLine();
        }

        static void PlantANA_Development()
        {
            var logger = LogManager.GetLogger("logger");

            foreach (var company in Directory.GetDirectories(Settings.Default.LoadPath))
            {
                foreach (var plant in Directory.GetDirectories(company))
                {
                    logger.InfoFormat("=> Loading from {0}", plant);
                    string path(string folder) => Path.Combine(plant, folder);
                    var @params = new DirectoryParameter(plant, path("InProcess"), path("Archive"), path("Invalid"), "*.txt", SearchOption.TopDirectoryOnly);
                    JobImportProcess.Run(@params, logger, Settings.Default.AppServerUrl);
                }
            }

            Console.WriteLine("Press <ENTER> to continue.");
            Console.ReadLine();
        }

        static void PlantANA()
        {
            var logger = LogManager.GetLogger("logger");
            var sourceParameter = new DirectoryParameter(
                Settings.Default.AnaSourceDirectory,
                Settings.Default.AnaInProcessDirectory,
                Settings.Default.AnaArchivesDirectory,
                Settings.Default.AnaErrorDirectory,
                "*.txt",
                SearchOption.TopDirectoryOnly);
            JobImportProcess.Run(sourceParameter, logger, Settings.Default.AppServerUrl);
        }

        static void PlantDUC()
        {
            var logger = LogManager.GetLogger("logger");
            var sourceParameter = new DirectoryParameter(
                Settings.Default.DucSourceDirectory,
                Settings.Default.DucInProcessDirectory,
                Settings.Default.DucArchivesDirectory,
                Settings.Default.DucErrorDirectory,
                "*.txt",
                SearchOption.TopDirectoryOnly);
            JobImportProcess.Run(sourceParameter, logger, Settings.Default.AppServerUrl);
        }

        static void PlantEVE()
        {
            var logger = LogManager.GetLogger("logger");
            var sourceParameter = new DirectoryParameter(
                Settings.Default.EveSourceDirectory,
                Settings.Default.EveInProcessDirectory,
                Settings.Default.EveArchivesDirectory,
                Settings.Default.EveErrorDirectory,
                "*.txt",
                SearchOption.TopDirectoryOnly);
            JobImportProcess.Run(sourceParameter, logger, Settings.Default.AppServerUrl);
        }

        static void PlantSAC()
        {
            var logger = LogManager.GetLogger("logger");
            var sourceParameter = new DirectoryParameter(
                Settings.Default.SacSourceDirectory,
                Settings.Default.SacInProcessDirectory,
                Settings.Default.SacArchivesDirectory,
                Settings.Default.SacErrorDirectory,
                "*.txt",
                SearchOption.TopDirectoryOnly);
            JobImportProcess.Run(sourceParameter, logger, Settings.Default.AppServerUrl);
        }
    }
}
