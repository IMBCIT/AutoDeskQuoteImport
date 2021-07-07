using System;
using System.Collections.Generic;
using System.Linq;
using Erp.BO;
using Erp.Proxy.BO;
using Ice.BO;
using Ice.Common;
using Ice.Core;
using Ice.Lib.Framework;
using Ice.Proxy.BO;

namespace Omni.E10Solutions.Cam.JobLibrary
{
    public class Epicor10
    {
        Session _session;
        JobEntryImpl _jobEntryBo;
        CustomerImpl _customerBo;
        PartImpl _partBo;
        PartCostSearchImpl _partCostBo;
        UD03Impl _ud03Bo;

        public Epicor10(EpicorParameter epi)
        {
            _session = new Session("Epicor.One", "esOmni01!", epi.AppServerUrl);
            _session.PlantID = epi.Plant;
            _session.CompanyID = epi.Company;

            _jobEntryBo = WCFServiceSupport.CreateImpl<JobEntryImpl>(_session, JobEntryImpl.UriPath);
            _customerBo = WCFServiceSupport.CreateImpl<CustomerImpl>(_session, CustomerImpl.UriPath);
            _partBo = WCFServiceSupport.CreateImpl<PartImpl>(_session, PartImpl.UriPath);
            _partCostBo = WCFServiceSupport.CreateImpl<PartCostSearchImpl>(_session, PartCostSearchImpl.UriPath);
            _ud03Bo = WCFServiceSupport.CreateImpl<UD03Impl>(_session, UD03Impl.UriPath);
        }

        public void DeleteJob(string jobNum)
        {
            _jobEntryBo.DeleteByID(jobNum);
        }

        public string GetNextJobNum()
        {
            string nextJobNum = string.Empty;
            _jobEntryBo.GetNextJobNum(out nextJobNum);
            return nextJobNum;
        }

        public CustomerDataSet GetCustomerData(string customerId)
        {
            return _customerBo.GetByCustID(customerId, true);
        }

        public PartDataSet GetPartData(string partnum)
        {
            return _partBo.GetByID(partnum);
        }

        public PartListDataSet GetPartsData(IEnumerable<string> parts)
        {
            var wheres = parts.Select(p => "partnum = '" + p + "'");
            var whereClause = string.Join(" or ", wheres);
            bool b;
            var pds = _partBo.GetList(whereClause, 0, 0, out b);
            return pds;
        }

        public PartCostListDataSet GetPartCostsData(IEnumerable<string> parts, string plant)
        {
            var wheres = parts.Select(p => "partnum = '" + p + "'");
            var whereClause = "(" + string.Join(" or ", wheres) + ") and CostID = '" + plant + "'";
            bool b;
            var pcds = _partCostBo.GetList(whereClause, 0, 0, out b);
            return pcds;
        }

        public void MakeDataSetUDCompatible(UpdExtJobEntryDataSet jds)
        {
            this.CommitJob(jds); // silly I know.
        }

        public string CommitJob(UpdExtJobEntryDataSet jds)
        {
            bool errors = false;
            try
            {
                var jobEntryBoErrors = _jobEntryBo.UpdateExt(jds, true, false, out errors);
                if (jobEntryBoErrors.BOUpdError.Rows.Count > 0)
                {
                    return jobEntryBoErrors.BOUpdError.Rows[0]["ErrorText"].ToString();
                }
            }
            catch (BusinessObjectException ex)
            {
            }

            return string.Empty;
        }

        public string CommitJob_Long(UpdExtJobEntryDataSet jds)
        {
            try
            {
                var ds = new JobEntryDataSet();
                _jobEntryBo.GetNewJobHead(ds);
                var hed = ds.JobHead.Rows.Cast<JobEntryDataSet.JobHeadRow>().First();
                var extHed = jds.JobHead.Rows.Cast<UpdExtJobEntryDataSet.JobHeadRow>().First();
                hed.JobNum = extHed.JobNum;
                hed.PartNum = extHed.PartNum;
                hed.PartDescription = extHed.PartDescription;
                hed.RevisionNum = "A";
                hed.ProdCode = extHed.ProdCode;
                hed.ReqDueDate = DateTime.Now.GetNextBusinesDay().Date;
                //hed.ProdQty = extHed.ProdQty; //[!] move this down to the make to stock
                hed.IUM = extHed.IUM;
                hed.Plant = extHed.Plant;
                hed.CommentText = extHed.CommentText;
                hed["StockJobID_c"] = extHed["StockJobID_c"];

                _jobEntryBo.Update(ds);

                ds = _jobEntryBo.GetDatasetForTree(hed.JobNum, 0, 0, false, "MFG, PRJ");

                // [!] do make to stock
                var plantCode = "INV";// TODO Change back once warehouse codes are finalized //GetWarehouseCode(extHed.Plant); 
                _jobEntryBo.PreCheckNewJobProd(extHed.JobNum, extHed.PartNum, "STOCK");
                _jobEntryBo.GetNewJobProd(ds, extHed.JobNum, extHed.PartNum, 0, 0, 0, plantCode, "", 0);
                var prod = ds.JobProd.Rows.Cast<JobEntryDataSet.JobProdRow>().First();
                _jobEntryBo.CheckJobProdMakeToType("STOCK", ds);
                _jobEntryBo.ChangeJobProdMakeToType(ds);
                prod.MakeToStockQty = extHed.ProdQty;
                _jobEntryBo.ChangeJobProdMakeToStockQty(ds);
                _jobEntryBo.Update(ds);

                var currentIndex = 0; // to determine last oper
                var lastOprSeq = 0;
                foreach (var extOpr in jds.JobOper.Rows.Cast<UpdExtJobEntryDataSet.JobOperRow>())
                {
                    currentIndex++;
                    var count = jds.JobOper.Rows.Count;
                    var isLast = currentIndex == count;
                    _jobEntryBo.GetNewJobOper(ds, extHed.JobNum, 0);
                    var opr = ds.JobOper.Rows.Cast<JobEntryDataSet.JobOperRow>().Last();
                    opr.OpCode = extOpr.OpCode;
                    opr.OpDesc = extOpr.OpDesc;
                    opr.LaborEntryMethod = isLast ? "Q" : "B";
                    opr.ProdStandard = extOpr.ProdStandard;
                    opr.StdFormat = "MP";
                    opr.FinalOpr = isLast;
                    if (isLast) opr.AutoReceive = extOpr.AutoReceive;
                    _jobEntryBo.Update(ds);
                    lastOprSeq = opr.OprSeq;
                }

                foreach (var extMtl in jds.JobMtl.Rows.Cast<UpdExtJobEntryDataSet.JobMtlRow>())
                {
                    _jobEntryBo.GetNewJobMtl(ds, extHed.JobNum, 0);
                    var mtl = ds.JobMtl.Rows.Cast<JobEntryDataSet.JobMtlRow>().Last();
                    mtl.PartNum = extMtl.PartNum;
                    mtl.QtyPer = extMtl.QtyPer;
                    mtl.IUM = extMtl.IUM;
                    mtl.Description = extMtl.Description;
                    mtl.EstUnitCost = extMtl.EstUnitCost;
                    mtl.RelatedOperation = 10;
                    mtl.Plant = extHed.Plant;
                    mtl.WarehouseCode = plantCode;
                    mtl.BackFlush = true;
                    mtl.BuyIt = false;
                    mtl.dspBuyIt = false;
                    if (extMtl["EstScrapType"] != DBNull.Value && extMtl["EstScrap"] != DBNull.Value)
                    {
                        mtl.EstScrapType = extMtl.EstScrapType;
                        mtl.EstScrap = extMtl.EstScrap;
                    }
                    _jobEntryBo.Update(ds);
                }

                var finalhed = ds.JobHead.Rows.Cast<JobEntryDataSet.JobHeadRow>().First();
                finalhed.JobEngineered = true;
                finalhed.JobReleased = true;
                //finalhed.ChangeDescription = "not implemented";
                _jobEntryBo.Update(ds);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return string.Empty;
        }

        private string GetWarehouseCode(string plant)
        {
            return plant + "1";
        }

        public string CommitReportingData(UpdExtUD03DataSet ds)
        {
            bool errors = false;
            try
            {
                var ud03Errors = _ud03Bo.UpdateExt(ds, true, false, out errors);
                if (ud03Errors.BOUpdError.Rows.Count > 0)
                {
                    return ud03Errors.BOUpdError.Rows[0]["ErrorText"].ToString();
                }
            }
            catch (BusinessObjectException ex)
            {
            }

            return string.Empty;
        }

        public void CloseSession()
        {
            _session.Dispose();
        }
    }
}
