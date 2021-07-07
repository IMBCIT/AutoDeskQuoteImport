using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.JobLibrary
{
    public class InvalidateJobFilesService
    {
        DirectoryParameter _dir;

        public InvalidateJobFilesService(DirectoryParameter dir)
        {
            _dir = dir;
        }

        public void InvalidateJobs(IEnumerable<CamductJob> jobs)
        {
            if (jobs.Count() == 0) return;

            using (new ImpersonateUser("epicadmin", "OMNI", "0mni3pic539#"))
            {
                foreach (var job in jobs)
                    InvalidateJob(job);
            }

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
    }
}
