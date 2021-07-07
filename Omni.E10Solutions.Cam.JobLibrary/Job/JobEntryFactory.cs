using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HedRow = Erp.BO.UpdExtJobEntryDataSet.JobHeadRow;
using OprRow = Erp.BO.UpdExtJobEntryDataSet.JobOperRow;
using MtlRow = Erp.BO.UpdExtJobEntryDataSet.JobMtlRow;

namespace Omni.E10Solutions.Cam.JobLibrary
{
    class JobEntryFactory
    {
        Epicor10Cache _cache;
        UdService _udService;

        public JobEntryFactory(Epicor10Cache cache, UdService udService)
        {
            _cache = cache;
            _udService = udService;
        }

        public IEnumerable<IJobEntry> CreateJobEntries(CamductJob camJob)
        {
            List<IJobEntry> jobs = new List<IJobEntry>();

            foreach (var oFileLine in camJob.OFile)
            {
                var oDataConverter = new TypeODataConverter(oFileLine, _cache);

                var job = new JobEntry(camJob, oFileLine);
                job.MakeUDCompatible(_udService);
                var hed = CreateJobHeadRow(job, oDataConverter );
                job.AddHeadRow(hed);

                // create oprs
                CreateOprs(job, oDataConverter);

                // create mtls
                CreateMtls(job, camJob, oDataConverter);

                jobs.Add(job);
            }

            return jobs;
        }

        public IJobEntry CreateJobEntry(CamductJob camJob)
        {
            // deprecated
            var job = new JobEntry(camJob, null);
            job.MakeUDCompatible(_udService);

            // create header
            var hed = CreateJobHeadRow(job, camJob);
            job.AddHeadRow(hed);

            foreach (var oFileLine in camJob.OFile)
            {
                var oDataConverter = new TypeODataConverter(oFileLine, _cache);

                //var asm = job.NewAsmRow();
                //job.AddAsmRow(asm);

                // create oprs
                CreateOprs(job, oDataConverter);

                // create mtls
                CreateMtls(job, camJob, oDataConverter);
            }

            return job;
        }

        HedRow CreateJobHeadRow(JobEntry job, TypeODataConverter oDataConverter)
        {
            var hed = job.NewHeadRow();
            hed.PartNum = oDataConverter.GetPartNumber();
            hed.PartDescription = oDataConverter.GetHeadPartDescription();
            hed.ProdCode = oDataConverter.GetProdCode();
            hed.ProdQty = oDataConverter.GetHeadProdQty();
            hed.IUM = oDataConverter.GetHeadIUM();
            hed.Plant = job.Job.Plant;
            var stockid = job.Job.Plant + "-" + job.Job.Name;
            hed.CommentText = stockid;
            hed["StockJobID_c"] = stockid;
            return hed;
        }

        HedRow CreateJobHeadRow(JobEntry job, CamductJob camJob)
        {
            var hed = job.NewHeadRow();
            hed.Company = camJob.OFile[0].CompanyField.GetValue();

            var firstOLine = camJob.OFile.FirstOrDefault();
            if (firstOLine == null)
            {
                throw new NoFirstOLineException(camJob.Name);
            }

            var oDataConverter = new TypeODataConverter(firstOLine, _cache);

            hed.PartNum = oDataConverter.GetPartNumber();
            hed.PartDescription = oDataConverter.GetHeadPartDescription();
            hed.ProdCode = oDataConverter.GetProdCode();
            hed.ProdQty = oDataConverter.GetHeadProdQty();
            hed.IUM = oDataConverter.GetHeadIUM();
            hed.Plant = camJob.Plant;
            var stockid = camJob.Company + "-" + camJob.Plant + "-" + camJob.Name;
            hed.CommentText = stockid;
            hed["StockJobID_c"] = stockid;
            return hed;
        }

        void CreateOprs(JobEntry job, TypeODataConverter oDataConverter)
        {
            // create oprs
            var oprsData = oDataConverter.GetOprData();
            foreach (var oprData in oprsData)
            {
                if (oprData.IsSkippable()) continue;

                var opr = CreateOprRow(job, oprData);
                job.AddOprRow(opr);
            }

            // create final opr
            var finalOpr = CreateFinalOprRow(job);
            job.AddOprRow(finalOpr);
        }

        OprRow CreateOprRow(JobEntry job, OprData oprData)
        {
            var opr = job.NewOprRow();

            opr.OpCode = oprData.OpCode;
            opr.OpDesc = oprData.OpDesc;
            opr.LaborEntryMethod = "B";
            opr.ProdStandard = oprData.ProdStandard;
            opr.StdFormat = "MP";
            opr.AssemblySeq = 0;

            return opr;
        }

        OprRow CreateFinalOprRow(JobEntry job)
        {
            var finalOpr = job.NewOprRow();

            finalOpr.OpCode = "RPRT";
            finalOpr.OpDesc = "Report Quantity";
            finalOpr.LaborEntryMethod = "Q";
            finalOpr.ProdStandard = 1m;
            finalOpr.StdFormat = "MP";
            finalOpr.FinalOpr = true;
            finalOpr.AssemblySeq = 0;

            finalOpr.AutoReceive = true;

            return finalOpr;
        }

        void CreateMtls(JobEntry job, CamductJob camJob, TypeODataConverter oDataConverter)
        {
            // create mtls from o-file line.
            var mtlsData = oDataConverter.GetMtlData();
            foreach (var mtlData in mtlsData)
            {
                if (mtlData.IsSkippable()) continue;

                var mtl = CreateMtlRow(job, mtlData);
                var scrap = camJob.GetScrap(mtl.PartNum);

                if (this.IsScrapIncluded(camJob, oDataConverter, scrap))
                {
                    var sDataConverter = new TypeSDataConverter(scrap);
                    IncludeScrapOnTheMtl(mtl, sDataConverter);
                }

                job.AddMtlRow(mtl);
            }

            // create mtls from the a-file.
            foreach (var aFile in oDataConverter.GetMatchingAData(camJob.AFile))
            {
                var aDataConverter = new TypeADataConverter(aFile, _cache);
                var mtl = CreateMtlRow(job, aDataConverter);
                job.AddMtlRow(mtl);
            }
        }

        MtlRow CreateMtlRow(JobEntry job, MtlData mtlData)
        {
            var mtl = job.NewMtlRow();

            mtl.PartNum = mtlData.PartNumber;
            mtl.QtyPer = mtlData.QtyPer;
            mtl.IUM = mtlData.IUM;
            mtl.AssemblySeq = 0;

            MapCachedDataToMtlRow(mtl);

            return mtl;
        }

        MtlRow CreateMtlRow(JobEntry job, TypeADataConverter aDataConverter)
        {
            var mtl = job.NewMtlRow();

            mtl.PartNum = aDataConverter.GetPartNumber();
            mtl.QtyPer = aDataConverter.GetQtyPer();
            mtl.AssemblySeq = 0;

            MapCachedDataToMtlRow(mtl);

            return mtl;
        }

        void MapCachedDataToMtlRow(MtlRow mtl)
        {
            var part = _cache.NonDtlPartCache.FirstOrDefault(p => p.PartNum == mtl.PartNum);
            if (part != null)
            {
                mtl.Description = part.PartDescription;
                mtl.IUM = part.IUM;
                //mtl.Description = part.PartDescription;
            }
            else
            {
                throw new NoPartFoundInCacheException(mtl.PartNum);
            }

            var partCost = _cache.PartCostCache.FirstOrDefault(p => p.PartNum == mtl.PartNum);
            if (partCost != null)
            {
                mtl.EstUnitCost = partCost.StdMaterialCost;
            }
            else
            {
                throw new NoPartCostFoundInCacheException(mtl.PartNum);
            }
        }

        bool IsScrapIncluded(CamductJob camJob, TypeODataConverter oDataConverter, TypeSData scrap)
        {
            var isBurnDetected = camJob.HasBurnOperation();
            var isNotRectangularCoil = !oDataConverter.IsRectangularCoil();
            var isScrapPresent = scrap != null;

            return isNotRectangularCoil && isBurnDetected && isScrapPresent;
        }

        void IncludeScrapOnTheMtl(MtlRow mtl, TypeSDataConverter sDataConverter)
        {
            mtl.EstScrapType = "%";
            mtl.EstScrap = sDataConverter.GetEstScrap();
        }
    }
}
