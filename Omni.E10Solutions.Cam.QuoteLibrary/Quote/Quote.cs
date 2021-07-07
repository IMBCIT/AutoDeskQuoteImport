using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Erp.BO;
using Ice.BO;
using System.Data;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    class Quote : IQuote
    {
        public Quote(CamductJob job)
        {
            Job = job;

            QuoteDataSet = new UpdExtQuoteDataSet();
            QuoteAsmDataSet = new UpdExtQuoteAsmDataSet();

            AsmReferences = new Dictionary<string, UpdExtQuoteAsmDataSet.QuoteAsmRow>();
            MtlReferences = new Dictionary<string, ICollection<UpdExtQuoteAsmDataSet.QuoteMtlRow>>();
            OprReferences = new Dictionary<string, ICollection<UpdExtQuoteAsmDataSet.QuoteOprRow>>();

            if(!QuoteDataSet.QuoteHed.Columns.Contains("CheckBox01"))
            {
                QuoteDataSet.QuoteHed.Columns.Add("CheckBox01", typeof(bool));
                QuoteDataSet.QuoteHed.Columns.Add("ShortChar01", typeof(string));
                QuoteDataSet.QuoteHed.Columns.Add("ShortChar06", typeof(string));
                QuoteDataSet.QuoteHed.Columns.Add("ShortChar07", typeof(string));
                QuoteDataSet.QuoteHed.Columns.Add("ShortChar08", typeof(string));
                QuoteDataSet.QuoteHed.Columns.Add("ShortChar09", typeof(string));
                QuoteDataSet.QuoteHed.Columns.Add("ShortChar10", typeof(string));
                QuoteDataSet.QuoteHed.Columns.Add("ShortChar11", typeof(string));
            }

            if(!QuoteDataSet.QuoteDtl.Columns.Contains("CheckBox01"))
            {
                QuoteDataSet.QuoteDtl.Columns.Add("CheckBox01", typeof(bool));//HACK QuoteDataSet.QuoteDtl.Columns.Add("CheckBox01", typeof(bool));
                QuoteDataSet.QuoteDtl.Columns.Add("CheckBox02", typeof(bool));
                QuoteDataSet.QuoteDtl.Columns.Add("Number07", typeof(decimal));
                QuoteDataSet.QuoteDtl.Columns.Add("Number08", typeof(decimal));
                QuoteDataSet.QuoteDtl.Columns.Add("Number09", typeof(decimal));
                QuoteDataSet.QuoteDtl.Columns.Add("Number10", typeof(decimal));
                QuoteDataSet.QuoteDtl.Columns.Add("ShortChar01", typeof(string));
                QuoteDataSet.QuoteDtl.Columns.Add("ShortChar02", typeof(string));
                QuoteDataSet.QuoteDtl.Columns.Add("ShortChar03", typeof(string));
                QuoteDataSet.QuoteDtl.Columns.Add("ShortChar04", typeof(string));
            }
        }

        public CamductJob Job { get; protected set; }

        private UpdExtQuoteDataSet QuoteDataSet;
        private UpdExtQuoteAsmDataSet QuoteAsmDataSet;

        private Dictionary<string, UpdExtQuoteAsmDataSet.QuoteAsmRow> AsmReferences;
        private Dictionary<string, ICollection<UpdExtQuoteAsmDataSet.QuoteMtlRow>> MtlReferences;
        private Dictionary<string, ICollection<UpdExtQuoteAsmDataSet.QuoteOprRow>> OprReferences;

        public UpdExtQuoteDataSet.QuoteHedRow NewHedRow()
        {
            return this.QuoteDataSet.QuoteHed.NewQuoteHedRow();
        }

        public UpdExtQuoteDataSet.QuoteDtlRow NewDtlRow()
        {
            return this.QuoteDataSet.QuoteDtl.NewQuoteDtlRow();
        }

        public UpdExtQuoteAsmDataSet.QuoteAsmRow NewAsmRow()
        {
            return this.QuoteAsmDataSet.QuoteAsm.NewQuoteAsmRow();
        }

        public UpdExtQuoteAsmDataSet.QuoteOprRow NewOprRow()
        {
            return this.QuoteAsmDataSet.QuoteOpr.NewQuoteOprRow();
        }

        public UpdExtQuoteAsmDataSet.QuoteMtlRow NewMtlRow()
        {
            return this.QuoteAsmDataSet.QuoteMtl.NewQuoteMtlRow();
        }

        public void AddHedRow(UpdExtQuoteDataSet.QuoteHedRow row)
        {
            this.QuoteDataSet.QuoteHed.AddQuoteHedRow(row);
        }

        public void AddDtlRow(UpdExtQuoteDataSet.QuoteDtlRow row)
        {
            this.QuoteDataSet.QuoteDtl.AddQuoteDtlRow(row);
        }

        public void AddAsmRow(string key, UpdExtQuoteAsmDataSet.QuoteAsmRow row)
        {
            this.QuoteAsmDataSet.QuoteAsm.AddQuoteAsmRow(row);
            this.AsmReferences.Add(key, row);
        }

        public void AddOprRow(string key, UpdExtQuoteAsmDataSet.QuoteOprRow row)
        {
            QuoteAsmDataSet.QuoteOpr.AddQuoteOprRow(row);

            // add the row to the references key.
            ICollection<UpdExtQuoteAsmDataSet.QuoteOprRow> oprRows;
            if (OprReferences.TryGetValue(key, out oprRows))
            {
                oprRows.Add(row);
                return;
            }

            // or create a new key-collection.
            oprRows = new List<UpdExtQuoteAsmDataSet.QuoteOprRow>() { row };
            OprReferences.Add(key,  oprRows);     
        }

        public void AddMtlRow(string key, UpdExtQuoteAsmDataSet.QuoteMtlRow row)
        {
            QuoteAsmDataSet.QuoteMtl.AddQuoteMtlRow(row);

            // add the row to the references key.
            ICollection<UpdExtQuoteAsmDataSet.QuoteMtlRow> mtlRows;
            if (MtlReferences.TryGetValue(key, out mtlRows))
            {
                mtlRows.Add(row);
                return;
            }

            // or create a new key-collection.
            mtlRows = new List<UpdExtQuoteAsmDataSet.QuoteMtlRow>() { row };
            MtlReferences.Add(key, mtlRows);
        }

        public UpdExtQuoteDataSet GetQuoteDataSet()
        {
            return QuoteDataSet;
        }

        public UpdExtQuoteAsmDataSet GetQuoteAsmDataSet()
        {
            return QuoteAsmDataSet;
        }

        public IEnumerable<UpdExtQuoteDataSet.QuoteDtlRow> GetDtls()
        {
            return QuoteDataSet.QuoteDtl.Rows.Cast<UpdExtQuoteDataSet.QuoteDtlRow>();
        }

        public IEnumerable<UpdExtQuoteAsmDataSet.QuoteMtlRow> GetMtls(string reference)
        {
            ICollection<UpdExtQuoteAsmDataSet.QuoteMtlRow> mtlRows;
            if (MtlReferences.TryGetValue(reference, out mtlRows))
            {
                return mtlRows;
            }

            return new List<UpdExtQuoteAsmDataSet.QuoteMtlRow>();
        }

        public IEnumerable<UpdExtQuoteAsmDataSet.QuoteOprRow> GetOprs(string reference)
        {
            ICollection<UpdExtQuoteAsmDataSet.QuoteOprRow> oprRows;
            if (OprReferences.TryGetValue(reference, out oprRows))
            {
                return oprRows;
            }

            return new List<UpdExtQuoteAsmDataSet.QuoteOprRow>();
        }

        UpdExtQuoteDataSet.QuoteHedRow _cachedFirstHed = null;
        public int GetQuoteNum()
        {
            var hed = _cachedFirstHed ?? this.QuoteDataSet.QuoteHed.Rows
                .Cast<UpdExtQuoteDataSet.QuoteHedRow>()
                .FirstOrDefault();

            if (hed != null)
            {
                return hed.QuoteNum;
            }

            return 0;
        }
        class Caseless : IEqualityComparer<string>
        {
            public bool Equals(string x, string y) => string.Equals(x, y, StringComparison.OrdinalIgnoreCase);
            public int GetHashCode(string obj) => obj?.ToLower().GetHashCode() ?? 0;
        }
        HashSet<string> _partReferenced = new HashSet<string>(new Caseless());
        public void SetAsmLinks(string referenceKey, int quoteNum, int quoteLine)
        {
            var dtlRow = (UpdExtQuoteDataSet.QuoteDtlRow)QuoteDataSet.QuoteDtl.AsEnumerable().FirstOrDefault(x => x.Field<int>("QuoteNum") == quoteNum && x.Field<int>("QuoteLine") == quoteLine);
            if (_partReferenced.Contains(dtlRow.PartNum))
                return;
            // get asm, set ids.
            if(AsmReferences.TryGetValue(referenceKey, out var asm))
            {
                asm.QuoteNum = quoteNum;
                asm.QuoteLine = quoteLine;
                asm.PartNum = dtlRow.PartNum;
                asm.Description = dtlRow.LineDesc;
                asm.IUM = dtlRow.SellingExpectedUM;
                _partReferenced.Add(asm.PartNum);
            }
            
            // get oprs, iterate, set ids on each.
            if (OprReferences.TryGetValue(referenceKey, out var oprs))
                foreach (var opr in oprs)
                {
                    opr.QuoteNum = quoteNum;
                    opr.QuoteLine = quoteLine;
                }

            // get mtls, iterate, set ids on each.
            if(MtlReferences.TryGetValue(referenceKey, out var mtls))
                foreach (var mtl in mtls)
                {
                    mtl.QuoteNum = quoteNum;
                    mtl.QuoteLine = quoteLine;
                }
        }

        public void MakeUDCompatible(UdService udService)
        {
            udService.PrepDataSet(QuoteDataSet);
        }

        public void MarkQuoteAsImported()
        {
            this.QuoteDataSet.QuoteHed.Rows[0]["CheckBox02"] = true;
        }

        public void MarkLinesEngineered()
        {
            foreach (var dtl in this.GetDtls())
            {
                dtl.ReadyToQuote = true;
                dtl.Engineer = true;
            }
        }

        public void MarkMtlsAsRelatedToFirstOperation()
        {
            var allMtls = MtlReferences.SelectMany(s => s.Value);
            foreach (var mtl in allMtls)
            {
                mtl.RelatedOperation = 10;
            }
        }

        public UpdExtUD03DataSet GetReportingDataSet()
        {
            var ds = new UpdExtUD03DataSet();
            foreach (var line in this.Job.OFile)
            {
                var row = ds.UD03.NewUD03Row();

                /* From the email from Quartz:
                Key1 = Job File Name
                Key2 = Entry
                ShortChar01 = Epicor Group
                Character01 = Item Alias
                Character02 = Item Description + Size - End 1 + Size - End 2 + Size - End 3 + Size - End 4
                Number01 = Qty
                Number02 = Base Weight
                ShortChar02 = Insulation Material
                Number03 = Insulation Thickness
                Number04 = Insulation Area
                Number05 = Wire Gauge
                Number06 = Length
                ShortChar03 = Material
                */

                row.Key1 = this.Job.Plant + "-" + this.Job.Name;
                row.Key2 = line.ItemNoField.GetTextValue();
                row.ShortChar01 = line.EpicorGroupField.GetValue();
                row.Character01 = line.ItemAliasField.GetValue();
                row.Character02 = string.Join(" ", new string[] { line.ItemDescriptionField.GetValue(), line.SizeEnd1Field.GetValue(), line.SizeEnd2Field.GetValue(), line.SizeEnd3Field.GetValue(), line.SizeEnd4Field.GetValue() });
                row.Number01 = line.QtyField.GetValue();
                row.Number02 = line.BaseWeightField.GetValue() ?? 0m;
                row.ShortChar02 = line.InsulationMaterialField.GetValue();
                row.Number03 = line.InsulationThicknessField.GetValue() ?? 0m;
                row.Number04 = line.InsulationAreaField.GetValue() ?? 0m;
                row.Number05 = line.WireGaugeField.GetValue() ?? 0m;
                row.Number06 = line.ItemLengthOrAngleField.GetValue() ?? 0m;
                row.ShortChar03 = line.MaterialField.GetValue();

                row.ShortChar04 = "Quote";
                row.Number07 = this.GetQuoteNum();

                // add the row to the table
                ds.UD03.AddUD03Row(row);
            }

            return ds;
        }

        public void RefreshQuoteDS(QuoteDataSet quoteDataSet)
        {
            this.QuoteDataSet = new UpdExtQuoteDataSet();
            this.QuoteDataSet.Merge(quoteDataSet, false);     
            
        }
    }
}
