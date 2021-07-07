using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    class RawJobDataDictionary : ReadOnlyDictionary<string, RawFileDataDictionary>
    {
        public RawJobDataDictionary(IDictionary<string, RawFileDataDictionary> dictionary)
            : base(dictionary) { }

        public static RawJobDataDictionary Create(RawFileDataDictionary rawData)
        {
            var jobDictionary = new Dictionary<string, RawFileDataDictionary>();

            // [?] do we want to verify there are three files? do we want to verify an o and a? etc.?
            // [?] are these rules absolute?
            var jobNames = rawData.Where(d => !(d.Key.Contains(".S") || d.Key.Contains(".A")))
                .Select(d => System.IO.Path.GetFileNameWithoutExtension(d.Key));

            foreach (var jobName in jobNames)
            {
                var relatedFileData = rawData.Where(d => d.Key.Contains(jobName));
                var relatedFileDataDictionary = new RawFileDataDictionary(relatedFileData);
                jobDictionary.Add(jobName, relatedFileDataDictionary);
            }

            return new RawJobDataDictionary(jobDictionary);
        }

        public CamductJob BuildCamductJob(string key)
        {
            RawFileDataDictionary fileDataDictionary;
            if (!TryGetValue(key, out fileDataDictionary))
                throw new Exception("Could not find any raw data for key " + key + ".");

            var camductJobBuilder = new CamductJobBuilder();
            return camductJobBuilder.BuildCamductJob(key, fileDataDictionary);
        }
    }
}
