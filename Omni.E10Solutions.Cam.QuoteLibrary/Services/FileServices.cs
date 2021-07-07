using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    class FileServices
    {
        ImpersonateUser _user;
        DirectoryParameter _dir;

        public FileServices(DirectoryParameter dir)
        {
            _dir = dir;
        }

        public void ImitateUserLogin()
        {
            _user = new ImpersonateUser("epicadmin", "OMNI", "0mni3pic539#");
        }

        public void ImitateUserLogout()
        {
            _user.Dispose();
        }

        public GrabFilesService CreateGrabFilesService()
        {
            return new GrabFilesService(_dir);
        }

        public MoveFilesService CreateMoveFilesService()
        {
            return new MoveFilesService(_dir);
        }

        public ReadFilesService CreateReadFilesService()
        {
            return new ReadFilesService();
        }

        public InvalidateJobFilesService CreateInvalidateFilesService()
        {
            return new InvalidateJobFilesService(_dir);
        }

        public string[] GetFilePaths()
        {
            var directory = new DirectoryInfo(_dir.SourceDirectory);
            var filePaths = directory.GetFiles(_dir.SearchPattern, _dir.SearchOption)
                .Select(file => file.FullName).ToArray();
            return filePaths;
        }

        public RawFileDataDictionary ReadFiles(IEnumerable<string> filePaths)
        {
            var fileDataDictionary = new RawFileDataDictionary();
            foreach (string path in filePaths)
            {
                var pathData = new List<string[]>(); // read in the file lines
                using (var reader = new StreamReader(path))
                {
                    reader.ReadLine(); // skip first line
                    while (!reader.EndOfStream)
                    {
                        string[] fileLineFields = reader.ReadLine().Split(',');
                        pathData.Add(fileLineFields);
                    }
                }
                fileDataDictionary.Add(path, pathData); // match the file to its lines
            }

            return fileDataDictionary;
        }

        public void InvalidateJobs(IEnumerable<CamductJob> jobs)
        {
            if (jobs.Count() == 0) return;

            foreach (var job in jobs)
                InvalidateJob(job);

            // * send email with information regarding invalid files.
        }

        void InvalidateJob(CamductJob job)
        {
            var exes = job.GetExceptions();

            using (var sw = new StreamWriter(Path.Combine(_dir.ErrorDirectory, job.Name + "_ERRORS.txt")))
            {
                foreach (var ex in exes)
                {
                    sw.WriteLine();
                    sw.WriteLine(ex.ToString());
                    sw.WriteLine();
                }
            }

            var filePaths = job.GetFilePaths();
            foreach (var filePath in filePaths)
            {
                if (filePath == null) continue;

                var fileName = Path.GetFileName(filePath);
                var inProcessPath = Path.Combine(_dir.InProcessDirectory, fileName);
                var errorFilePath = Path.Combine(_dir.ErrorDirectory, fileName);

                if (File.Exists(errorFilePath))
                    File.Delete(inProcessPath);
                else
                    File.Move(inProcessPath, errorFilePath);
            }
        }

        public string[] Archive(IEnumerable<string> filePaths)
        {
            return Move(filePaths, _dir.ArchivesDirectory);
        }

        public string[] Process(IEnumerable<string> filePaths)
        {
            return Move(filePaths, _dir.InProcessDirectory);
        }

        string[] Move(IEnumerable<string> filePaths, string destinationDir)
        {
            var newFilePaths = new List<string>();

            foreach (var filePath in filePaths)
            {
                var fileName = Path.GetFileName(filePath);
                var newFilePath = Path.Combine(destinationDir, fileName);

                if (File.Exists(newFilePath))
                    File.Delete(filePath);
                else
                    File.Move(filePath, newFilePath);

                newFilePaths.Add(newFilePath);
            }

            return newFilePaths.ToArray();
        }
    }
}
