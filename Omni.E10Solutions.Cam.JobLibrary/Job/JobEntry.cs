using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Erp.BO;
using Ice.BO;

namespace Omni.E10Solutions.Cam.JobLibrary
{
    class JobEntry : IJobEntry
    {
        public JobEntry(CamductJob job, TypeOData line)
        {
            Job = job;
            Line = line;
            JobEntryDataSet = new UpdExtJobEntryDataSet();

            if (!JobEntryDataSet.JobHead.Columns.Contains("StockJobId_c"))
            {
                JobEntryDataSet.JobHead.Columns.Add("StockJobId_c", typeof(string));
            }
        }

        public CamductJob Job { get; protected set; }
        public TypeOData Line { get; protected set; }

        private UpdExtJobEntryDataSet JobEntryDataSet;

        public UpdExtJobEntryDataSet.JobHeadRow NewHeadRow()
        {
            return this.JobEntryDataSet.JobHead.NewJobHeadRow();
        }

        public UpdExtJobEntryDataSet.JobAsmblRow NewAsmRow()
        {
            return this.JobEntryDataSet.JobAsmbl.NewJobAsmblRow();
        }

        public UpdExtJobEntryDataSet.JobOperRow NewOprRow()
        {
            return this.JobEntryDataSet.JobOper.NewJobOperRow();
        }

        public UpdExtJobEntryDataSet.JobMtlRow NewMtlRow()
        {
            return this.JobEntryDataSet.JobMtl.NewJobMtlRow();
        }

        public void AddHeadRow(UpdExtJobEntryDataSet.JobHeadRow row)
        {
            this.JobEntryDataSet.JobHead.AddJobHeadRow(row);
        }

        public void AddAsmRow(UpdExtJobEntryDataSet.JobAsmblRow row)
        {
            this.JobEntryDataSet.JobAsmbl.AddJobAsmblRow(row);
        }

        public void AddOprRow(UpdExtJobEntryDataSet.JobOperRow row)
        {
            JobEntryDataSet.JobOper.AddJobOperRow(row);
        }

        public void AddMtlRow(UpdExtJobEntryDataSet.JobMtlRow row)
        {
            JobEntryDataSet.JobMtl.AddJobMtlRow(row);
        }

        public UpdExtJobEntryDataSet GetJobEntryDataSet()
        {
            return JobEntryDataSet;
        }

        UpdExtJobEntryDataSet.JobHeadRow _cachedFirstHead = null;
        public string GetJobNum()
        {
            var hed = _cachedFirstHead ?? this.JobEntryDataSet.JobHead.Rows
                .Cast<UpdExtJobEntryDataSet.JobHeadRow>()
                .FirstOrDefault();

            if (hed != null)
            {
                return hed.JobNum;
            }

            return string.Empty;
        }

        public void SetJobNum(string jobNum)
        {
            var hed = _cachedFirstHead ?? this.JobEntryDataSet.JobHead.Rows
                .Cast<UpdExtJobEntryDataSet.JobHeadRow>()
                .FirstOrDefault();

            if (hed != null)
            {
                hed.JobNum = jobNum;
                SetJobNumOnMtlsAndOprs(jobNum);
            }
            else
            {
                throw new Exception("JobNum could not be set.");
            }
        }

        public void MakeUDCompatible(UdService udService)
        {
            udService.PrepDataSet(JobEntryDataSet);
        }

        public void SetJobNumOnMtlsAndOprs(string jobNum)
        {
            foreach (UpdExtJobEntryDataSet.JobMtlRow mtl in this.JobEntryDataSet.JobMtl.Rows)
            {
                mtl.AssemblySeq = 0;
                mtl.JobNum = jobNum;
            }

            foreach (UpdExtJobEntryDataSet.JobOperRow opr in this.JobEntryDataSet.JobOper.Rows)
            {
                opr.AssemblySeq = 0;
                opr.JobNum = jobNum;
            }
        }

        public UpdExtUD03DataSet GetReportingDataSet()
        {
            var ds = new UpdExtUD03DataSet();
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
            row.Key2 = Line.ItemNoField.GetTextValue();
            row.ShortChar01 = Line.EpicorGroupField.GetValue();
            row.Character01 = Line.ItemAliasField.GetValue();
            row.Character02 = string.Join(" ", new string[] { Line.ItemDescriptionField.GetValue(), Line.SizeEnd1Field.GetValue(), Line.SizeEnd2Field.GetValue(), Line.SizeEnd3Field.GetValue(), Line.SizeEnd4Field.GetValue() });
            row.Number01 = Line.QtyField.GetValue();
            row.Number02 = Line.BaseWeightField.GetValue() ?? 0m;
            row.ShortChar02 = Line.InsulationMaterialField.GetValue();
            row.Number03 = Line.InsulationThicknessField.GetValue() ?? 0m;
            row.Number04 = Line.InsulationAreaField.GetValue() ?? 0m;
            row.Number05 = Line.WireGaugeField.GetValue() ?? 0m;
            row.Number06 = Line.ItemLengthOrAngleField.GetValue() ?? 0m;
            row.ShortChar03 = Line.MaterialField.GetValue();

            row.ShortChar04 = "Job";
            row.ShortChar05 = this.GetJobNum();

            // add the row to the table
            ds.UD03.AddUD03Row(row);

            return ds;
        }
    }
}
