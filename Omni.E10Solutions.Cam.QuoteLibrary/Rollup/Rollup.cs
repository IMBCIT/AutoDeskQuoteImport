using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class Rollup
    {
        public FileTypeO _oFile;
        protected FileTypeA _aFile;
        protected FileTypeS _sFile;
        protected bool _isBurnDetected;
        private readonly Epicor10Cache _cache;

        public Rollup(FileTypeO oFile, FileTypeA aFile, FileTypeS sFile, bool isBurnDetected, Epicor10Cache cache)
        {
            _oFile = oFile;
            _aFile = aFile;
            _sFile = sFile;
            _isBurnDetected = isBurnDetected;
            _cache = cache;
        }

        public IEnumerable<RollupLine> GetLines()
        {
            var lines = new List<RollupLine>();

            // basically we want to get references to the o-a-s data that go together and put them into the right line.
            var distinctProductGroups = _oFile.Select(o => o.EpicorGroupField.GetTextValue()).Distinct();
            foreach (var group in distinctProductGroups)
            {
                // if it is stock or spiral, skip
                if (IsStockOrSpiral(group)) continue;


                var galvLines = GetGalvLines(group);
                var nonGalvLines = GetNonGalvLines(group);
                var laborLines = GetLaborLines(group);

                if (galvLines.Count() != 0)
                {
                    galvLines = galvLines.Concat(laborLines);
                }
                else if (nonGalvLines.Count() != 0)
                {
                    nonGalvLines = nonGalvLines.Concat(laborLines);
                }
                else
                {
                    throw new Exception("Labor must belong to a rollup group containing non labor lines.");
                }

                // if it is not, collect the related data for the galv and then the non-galv for the collection
                var galvLine = RollupLines(galvLines);
                var nonGalvLine = RollupLines(nonGalvLines);
                if (galvLine != null) lines.Add(galvLine);
                if (nonGalvLine != null) lines.Add(nonGalvLine);
            }

            // Collect the data for each stock or spiral 
            var stockOrSpiralRollupLines = RollupStockOrSpiralLines();
            lines.AddRange(stockOrSpiralRollupLines);

            return lines;
        }

        protected bool IsStockOrSpiral(string group)
        {
            return group == "STCKCAT" || group == "STCKSPL" || group == "POSTTEN";
        }

        protected IEnumerable<RollupLine> RollupStockOrSpiralLines()
        {
            var lines = new List<RollupLine>();

            var stockOrSpiralLines = _oFile.Where(o => IsStockOrSpiral(o.EpicorGroupField.GetTextValue()));

            // new strategy: rolls up stock or spiral
            var distinctParts = stockOrSpiralLines.Select(s => s.ItemAliasField.GetTextValue()).Distinct();
            foreach (var part in distinctParts)
            {
                var catalogPartLines = GetCatalogLinesByPart(part);
                var rolledLine = RollupLines(catalogPartLines);
                lines.Add(rolledLine);
            }

            // original strategy: does not roll up stock or spiral
            //foreach (var ssLine in stockOrSpiralLines)
            //{
            //    var aData = _aFile.Where(a => a.ItemNoField.GetTextValue() == ssLine.ItemNoField.GetTextValue());
            //    var line = new RollupLine(new List<TypeOData>() { ssLine }, aData, _sFile, _isBurnDetected);
            //    lines.Add(line);
            //}

            return lines.Distinct();
        }

        protected IEnumerable<TypeOData> GetNonGalvLines(string group)
        {
            var groupLines = _oFile.Where(o => o.EpicorGroupField.GetValue() == group);
            var nonLaborLines = groupLines.Where(o => !o.ItemAliasField.GetTextValue().ToUpper().Contains("LABOR"));
            var nongalvs = nonLaborLines.Where(o => !string.IsNullOrWhiteSpace(o.NonGalvField.GetTextValue()));

            return nongalvs;
        }

        protected IEnumerable<TypeOData> GetGalvLines(string group)
        {
            var groupLines = _oFile.Where(o => o.EpicorGroupField.GetValue() == group);
            var nonLaborLines = groupLines.Where(o => !o.ItemAliasField.GetTextValue().ToUpper().Contains("LABOR"));
            var galvs = nonLaborLines.Where(o => string.IsNullOrWhiteSpace(o.NonGalvField.GetTextValue()));

            return galvs;
        }

        protected IEnumerable<TypeOData> GetLaborLines(string group)
        {
            var groupLines = _oFile.Where(o => o.EpicorGroupField.GetValue() == group);
            var laborLines = groupLines.Where(o => o.ItemAliasField.GetTextValue().ToUpper().Contains("LABOR"));
            return laborLines;
        }

        protected IEnumerable<TypeOData> GetCatalogLinesByPart(string part)
        {
            return _oFile.Where(o => o.ItemAliasField.GetValue() == part);
        }

        protected RollupLine RollupLines(IEnumerable<TypeOData> lines)
        {
            if (lines.Count() == 0) return null;

            var aData = GetRelatedAData(lines);

            return new RollupLine(lines, aData, _sFile, _isBurnDetected, _cache);
        }

        // DELETE
        //protected RollupLine RollupNonGalvLines(string prodGroup)
        //{
        //    // get group lines
        //    var groupLines = _oFile.Where(o => o.EpicorGroupField.GetValue() == prodGroup);

        //    // ignore labor
        //    var nonLaborLines = groupLines.Where(o => !o.ItemAliasField.GetTextValue().ToUpper().Contains("LABOR"));

        //    var nonGalv_oDataLines = nonLaborLines.Where(o => !string.IsNullOrWhiteSpace(o.NonGalvField.GetTextValue()) && o.EpicorGroupField.GetTextValue() == prodGroup);

        //    if (nonGalv_oDataLines.Count() == 0) return null;

        //    var aData = GetRelatedAData(nonGalv_oDataLines);

        //    return new RollupLine(nonGalv_oDataLines, aData, _sFile, _isBurnDetected);
        //}

        //protected RollupLine RollupGalvLines(I)
        //{
        //    // get group lines
        //    var groupLines = _oFile.Where(o => o.EpicorGroupField.GetValue() == prodGroup);

        //    // ignore labor
        //    var nonLaborLines = groupLines.Where(o => !o.ItemAliasField.GetTextValue().ToUpper().Contains("LABOR"));

        //    var galv_oDataLines = nonLaborLines.Where(o => string.IsNullOrWhiteSpace(o.NonGalvField.GetTextValue()) && o.EpicorGroupField.GetTextValue() == prodGroup);

        //    if (galv_oDataLines.Count() == 0) return null;

        //    var aData = GetRelatedAData(galv_oDataLines);

        //    return new RollupLine(galv_oDataLines, aData, _sFile, _isBurnDetected);
        //}

        protected IEnumerable<TypeAData> GetRelatedAData(IEnumerable<TypeOData> oData)
        {
            var relatedAData = new List<TypeAData>();
            foreach (var o in oData)
            {
                var aData = _aFile.Where(a => a.ItemNoField.GetTextValue() == o.ItemNoField.GetTextValue());
                relatedAData.AddRange(aData);
            }
            return relatedAData;
        }
    }
}
