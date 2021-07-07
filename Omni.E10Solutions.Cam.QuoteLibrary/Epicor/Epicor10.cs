using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Erp.BO;
using Erp.Proxy.BO;
using Ice.Common;
using Ice.Core;
using Ice.Lib.Framework;
using Ice.Proxy.BO;
using Ice.BO;
using System.Data;
//using static Erp.BO.UpdExtQuoteDataSet;
using Epicor.Data;
using Ice;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class Epicor10
    {
        Session _session;
        QuoteImpl _quoteBo;
        QuoteAsmImpl _quoteAsmBo;
        CustomerImpl _customerBo;
        PartImpl _partBo;
        PartCostSearchImpl _partCostBo;
        UD03Impl _ud03Bo;
        static readonly Regex _xml = new Regex(@"<\w+", RegexOptions.Compiled);

        public Epicor10(EpicorParameter epi)
        {
            _session = new Session("Epicor.One", "esOmni01!", epi.AppServerUrl);
            _session.PlantID = epi.Plant;
            _session.CompanyID = epi.Company;

            _quoteBo = WCFServiceSupport.CreateImpl<QuoteImpl>(_session, QuoteImpl.UriPath);
            _quoteAsmBo = WCFServiceSupport.CreateImpl<QuoteAsmImpl>(_session, QuoteAsmImpl.UriPath);
            _customerBo = WCFServiceSupport.CreateImpl<CustomerImpl>(_session, CustomerImpl.UriPath);
            _partBo = WCFServiceSupport.CreateImpl<PartImpl>(_session, PartImpl.UriPath);
            _partCostBo = WCFServiceSupport.CreateImpl<PartCostSearchImpl>(_session, PartCostSearchImpl.UriPath);
            _ud03Bo = WCFServiceSupport.CreateImpl<UD03Impl>(_session, UD03Impl.UriPath);
        }

        public void DeleteQuote(int quotenum)
        {
            _quoteBo.DeleteByID(quotenum);
        }

        public CustomerDataSet GetCustomerData(string customerId)
        {
            return _customerBo.GetByCustID(customerId, true);
        }

        public PartDataSet GetPartData(string partnum)
        {
            return _partBo.GetByID(partnum);
        }

        public PartListDataSet GetPartsListData(IEnumerable<string> parts)
        {
            var wheres = parts.Select(p => "partnum = '" + p + "'");
            var whereClause = string.Join(" or ", wheres);
            bool b;
            var pds = _partBo.GetList(whereClause, 0, 0, out b);
            return pds;
        }

        public PartDataSet GetPartsDataSet(IEnumerable<string> parts)
        {
            var wheres = parts.Select(p => "partnum = '" + p + "'");
            var whereClause = string.Join(" or ", wheres);
            bool b;
            var ds = _partBo.GetRows(whereClause, "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", 0, 0, out b);
            return ds;
        }

        public PartCostListDataSet GetPartCostsData(IEnumerable<string> parts, string plant)
        {
            var wheres = parts.Select(p => "partnum = '" + p + "'");
            var whereClause = "(" + string.Join(" or ", wheres) + ") and CostID = '" + plant + "'";
            bool b;
            var pcds = _partCostBo.GetList(whereClause, 0, 0, out b);
            return pcds;
        }

        public void MakeDataSetUDCompatible(UpdExtQuoteDataSet qds)
        {
            this.CommitQuote(qds); // silly I know.
        }

        public QuoteDataSet GetQuoteDataSet(int quoteNum)
        {
            return _quoteBo.GetByID(quoteNum);
        }
        //HashSet<Guid> processed = new HashSet<Guid>();
        public string CommitQuote(UpdExtQuoteDataSet qds)
        {
            bool errors = false;
            try
            {
                MapCustomDtlFields(qds);
                MapCustomHedField(qds);

                var quoteBoErrors = _quoteBo.UpdateExt(qds, true, false, out errors);

                if (quoteBoErrors.BOUpdError.Rows.Count > 0)
                {
                    return quoteBoErrors.BOUpdError.Rows[0]["ErrorText"].ToString();
                }
            }
            catch (BusinessObjectException ex)
            {
                return ex.Message;
            }

            return string.Empty;
        }

        private static void MapCustomHedField(UpdExtQuoteDataSet qds)
        {
            foreach (UpdExtQuoteDataSet.QuoteHedRow hed in qds.QuoteHed)
            {
                RestoreQuoteComments(hed);
                var xml = new XElement("Custom",
                                FillXml(hed, "QuoteComment", "CheckBox01", "ShortChar01", "ShortChar06", "ShortChar07",
                                "ShortChar08", "ShortChar09", "ShortChar10", "ShortChar11").ToArray());
                hed.QuoteComment = xml.ToString();
                TruncateWarnLongChar(hed);
            }
        }

        private static void MapCustomDtlFields(UpdExtQuoteDataSet qds)
        {
            foreach (UpdExtQuoteDataSet.QuoteDtlRow dtl in qds.QuoteDtl)
            {
                RestoreQuoteComments(dtl);

                var xml = new XElement("Custom",
                                            FillXml(dtl, "QuoteComment", "CheckBox01", "CheckBox02", "Number07", "Number08",
                                            "Number09", "Number10", "ShortChar01", "ShortChar02", "ShortChar03", "ShortChar04").ToArray());
                dtl.QuoteComment = xml.ToString();
                TruncateWarnLongChar(dtl);
            }
        }

        private static void RestoreQuoteComments(DataRow row)
        {
            var comment = row["QuoteComment"].ToString();
            if (_xml.IsMatch(comment))
                row["QuoteComment"] = XElement.Parse(comment).Attribute("QuoteComment").Value;
        }

        static void TruncateWarnLongChar(DataRow row)
        {
            const int max = 49;
            foreach (DataColumn column in row.Table.Columns)
            {
                if (column.ColumnName.StartsWith("ShortChar", StringComparison.OrdinalIgnoreCase)
                    && row[column.ColumnName].ToString().Length > max)
                {
                    if (row.Table.Columns.Contains("QuoteLine"))
                        Console.WriteLine("QuoteNum:{0}, line:{1} has a {2} larger than 50 chars, it will be truncated",
                            row["QuoteNum"], row["QuoteLine"], column.ColumnName);
                    else
                        Console.WriteLine("QuoteNum:{0}, header has a {1} larger than 50 chars, it will be truncated",
                            row["QuoteNum"], column.ColumnName);
                    row[column.ColumnName] = row[column.ColumnName].ToString().Remove(max);
                }
            }
        }

        static IEnumerable<object> FillXml(DataRow row, params string[] fields)
        {
            foreach (var field in fields)
                yield return new XAttribute(field, row[field]);
        }

        public string CommitQuoteAsm(UpdExtQuoteAsmDataSet qads, int quoteNum = 0)
        {
            if (quoteNum != 0)
                _quoteBo.GetByID(quoteNum);

            var asmErrors = _quoteAsmBo.UpdateExt(qads, true, false, out _);


            return asmErrors.BOUpdError.Rows.Count > 0
                ? string.Join(", ", asmErrors.BOUpdError.Rows.Cast<DataRow>().Select(p => p["ErrorText"]))
                : string.Empty;
        }

        public string UpdateQuoteOperation(IQuote quote)
        {
            int quoteNum = quote.GetQuoteNum();

            var quoteLines = quote.GetDtls();

            var errors = new StringBuilder();
            int processed = 0;
            var lines = from x in quoteLines
                        where x.PartNum.Equals(x.LineDesc, StringComparison.OrdinalIgnoreCase)
                        orderby x.QuoteLine
                        select x.QuoteLine;
            //foreach (var line in quoteLines.Where(x => x.PartNum.Equals(x.LineDesc, StringComparison.OrdinalIgnoreCase)).Select( x => x.QuoteLine))
            foreach (var line in lines)
            {
                var asmDS = _quoteAsmBo.GetDatasetForTree(quoteNum, line, 0, 0, true);

                foreach (var op in quote.GetQuoteAsmDataSet().QuoteOpr.AsEnumerable().Skip(processed))
                {
                    processed++;
                    _quoteAsmBo.GetNewOperation(asmDS, quoteNum, line, 0, false);
                    var newOprRow = asmDS.QuoteOpr[asmDS.QuoteOpr.Count - 1];

                    foreach (DataColumn col in op.Table.Columns)
                    {
                        if (newOprRow.Table.Columns.Contains(col.ColumnName) && !string.IsNullOrEmpty(op[col.ColumnName].ToString()))
                        {
                            newOprRow[col.ColumnName] = op[col.ColumnName];
                        }

                    }

                    try
                    {
                        _quoteAsmBo.Update(asmDS);
                    }
                    catch (Exception ex)
                    {
                        errors.AppendLine(ex.ToString() + Environment.NewLine);
                    }

                    if (!string.IsNullOrEmpty(op["FinalOpr"].ToString()) && (bool)op["FinalOpr"]) break;
                }

            }

            foreach (var l in quoteLines)
            {
                var log = new { l.LineDesc, l.QuoteLine, l.MfgDetail };
            }

            //var nLines = quoteLines.Where(x => x.MfgDetail).ToList();
            var nLines = quoteLines.Where(x => !x.PartNum.Equals(x.LineDesc, StringComparison.OrdinalIgnoreCase) && x.MfgDetail).ToList();
            var nOps = 0;

            foreach (UpdExtQuoteAsmDataSet.QuoteOprRow opr in quote.GetQuoteAsmDataSet().QuoteOpr)
            {
                try
                {
                    if (opr.FinalOpr)
                        nOps++;
                }
                catch { }
            }

            for (int i = 0; i < nLines.Count(); i++)
            {
                var line = nLines[i];

                var asmDS = _quoteAsmBo.GetDatasetForTree(quoteNum, line.QuoteLine, 0, 0, true);

                if (nLines.Count() <= nOps)
                {
                    var iO = nOps - nLines.Count();
                    for (int o = processed; o < quote.GetQuoteAsmDataSet().QuoteOpr.AsEnumerable().Count(); o++)
                    {
                        if (iO == 0)
                        {
                            break;
                        }

                        try
                        {
                            if (quote.GetQuoteAsmDataSet().QuoteOpr[o].FinalOpr)
                            {
                                iO--;
                            }
                        }
                        catch { }

                    }

                    while (processed < quote.GetQuoteAsmDataSet().QuoteOpr.AsEnumerable().Count())
                    {
                        var op = quote.GetQuoteAsmDataSet().QuoteOpr[processed++];

                        _quoteAsmBo.GetNewOperation(asmDS, quoteNum, line.QuoteLine, 0, false);
                        var newOprRow = asmDS.QuoteOpr[asmDS.QuoteOpr.Count - 1];

                        foreach (DataColumn col in op.Table.Columns)
                        {
                            if (newOprRow.Table.Columns.Contains(col.ColumnName) && !string.IsNullOrEmpty(op[col.ColumnName].ToString()))
                            {
                                newOprRow[col.ColumnName] = op[col.ColumnName];
                            }
                        }

                        try
                        {
                            _quoteAsmBo.Update(asmDS);
                        }
                        catch (Exception ex)
                        {
                            errors.AppendLine(ex.ToString() + Environment.NewLine);
                        }

                        if (!string.IsNullOrEmpty(op["FinalOpr"].ToString()) && (bool)op["FinalOpr"]) break;
                    }
                }

            }

            return errors.ToString();
        }

        public string UpdateRelatedOperation(IQuote quote)
        {
            var errors = new StringBuilder();
            var quoteNum = quote.GetQuoteNum();
            var quoteLines = quote.GetDtls();


            foreach (var line in quoteLines.Where(x => x.PartNum.Equals(x.LineDesc, StringComparison.OrdinalIgnoreCase)).Select(x => x.QuoteLine))
            {
                var asmDS = _quoteAsmBo.GetDatasetForTree(quoteNum, line, 0, 0, true);

                foreach (var mtl in quote.GetQuoteAsmDataSet().QuoteMtl.AsEnumerable().Where(x => !(x["QuoteLine"] is DBNull) && !string.IsNullOrEmpty(x["QuoteLine"].ToString()) && Convert.ToInt32(x["QuoteLine"]) == line))
                {
                    _quoteAsmBo.GetNewQuoteMtl(asmDS, quoteNum, line, 0);
                    var newMtlRow = asmDS.QuoteMtl[asmDS.QuoteMtl.Count - 1];

                    foreach (DataColumn col in mtl.Table.Columns)
                        if (newMtlRow.Table.Columns.Contains(col.ColumnName) && !string.IsNullOrEmpty(mtl[col.ColumnName].ToString()))
                            newMtlRow[col.ColumnName] = mtl[col.ColumnName];

                    if (!string.IsNullOrEmpty(newMtlRow["PartNum"].ToString()))
                    {
                        var pInfo = _partBo.GetByID(newMtlRow["PartNum"].ToString());
                        if (pInfo != null && pInfo.Part.Count > 0 && !string.IsNullOrEmpty(pInfo.Part[0].IUM))
                        {
                            newMtlRow["IUM"] = pInfo.Part[0].IUM;
                            newMtlRow["PartNumIUM"] = pInfo.Part[0].IUM;
                            newMtlRow["PartNumSalesUM"] = pInfo.Part[0].IUM;
                        }
                    }


                    newMtlRow["RelatedOperation"] = 10;

                    try
                    {
                        _quoteAsmBo.Update(asmDS);
                    }
                    catch (Exception ex)
                    {
                        errors.AppendLine(ex.ToString() + Environment.NewLine);
                    }
                }
            }

            var nLines = quoteLines.Where(x => !x.PartNum.Equals(x.LineDesc, StringComparison.OrdinalIgnoreCase) && x.MfgDetail).ToList();
            var nMtl = quote.GetQuoteAsmDataSet().QuoteMtl.AsEnumerable();//HACK .Where(x => x["QuoteLine"] is DBNull || string.IsNullOrEmpty(x.ToString())).ToList();

            var matches = from ln in nLines
                          join mtl in nMtl on ln.QuoteLine equals mtl["QuoteLine"]
                          select (ln, mtl);

            foreach (var (ln, mtl) in matches)
            {
                var asmDS = _quoteAsmBo.GetDatasetForTree(quoteNum, ln.QuoteLine, 0, 0, true);
                //var mtl = nMtl.FirstOrDefault(p => Equals(p["QuoteLine"], ln.QuoteLine)); //&& Equals(p["PartNum"], ln.PartNum)
                //if (mtl == null)
                //    continue;

                _quoteAsmBo.GetNewQuoteMtl(asmDS, quoteNum, ln.QuoteLine, 0);
                var newMtlRow = asmDS.QuoteMtl[asmDS.QuoteMtl.Count - 1];

                foreach (DataColumn col in mtl.Table.Columns)
                    if (newMtlRow.Table.Columns.Contains(col.ColumnName) && !string.IsNullOrEmpty(mtl[col.ColumnName].ToString()))
                        newMtlRow[col.ColumnName] = mtl[col.ColumnName];

                if (!string.IsNullOrEmpty(newMtlRow["PartNum"].ToString()))
                {
                    var pInfo = _partBo.GetByID(newMtlRow["PartNum"].ToString());
                    if (pInfo != null && pInfo.Part.Count > 0 && !string.IsNullOrEmpty(pInfo.Part[0].IUM))
                    {
                        newMtlRow["IUM"] = pInfo.Part[0].IUM;
                        newMtlRow["PartNumIUM"] = pInfo.Part[0].IUM;
                        newMtlRow["PartNumSalesUM"] = pInfo.Part[0].IUM;
                    }
                }

                newMtlRow["RelatedOperation"] = 10;

                try
                {
                    _quoteAsmBo.Update(asmDS);
                }
                catch (Exception ex)
                {
                    errors.AppendLine(ex.ToString() + Environment.NewLine);
                }
            }

            return errors.ToString();
        }

        public void UpdateQuoteAsm_LongForm(UpdExtQuoteAsmDataSet qads, IEnumerable<UpdExtQuoteDataSet.QuoteDtlRow> dtls)
        {
            var ds = new QuoteAsmDataSet();
            foreach (UpdExtQuoteAsmDataSet.QuoteAsmRow asm in qads.QuoteAsm.Rows)
            {
                _quoteAsmBo.GetNewQuoteAsm(ds, asm.QuoteNum, asm.QuoteLine);
                var newasm = ds.QuoteAsm.Rows.Cast<QuoteAsmDataSet.QuoteAsmRow>().ToArray()[ds.QuoteAsm.Rows.Count - 1];
                var dtl = dtls.First(d => d.QuoteLine == asm.QuoteLine);
                newasm.PartNum = dtl.PartNum;
                newasm.Description = dtl.LineDesc ?? "something";
                newasm.IUM = "EA";
                newasm.QtyPer = 1m;
                //_quoteAsmBo.Update(ds);
                foreach (UpdExtQuoteAsmDataSet.QuoteOprRow opr in qads.QuoteOpr.Rows.Cast<UpdExtQuoteAsmDataSet.QuoteOprRow>().Where(o => o.QuoteLine == asm.QuoteLine))
                {
                    _quoteAsmBo.GetNewQuoteOpr(ds, opr.QuoteNum, opr.QuoteLine, 0);
                    var newopr = ds.QuoteOpr.Rows.Cast<QuoteAsmDataSet.QuoteOprRow>().ToArray()[ds.QuoteOpr.Rows.Count - 1];
                    newopr.OpCode = opr.OpCode;
                    newopr.OpDesc = opr.OpDesc;
                    newopr.LaborEntryMethod = opr.LaborEntryMethod;
                    newopr.ProdStandard = opr.ProdStandard;
                    newopr.StdFormat = opr.StdFormat;
                    _quoteAsmBo.Update(ds);
                }

                foreach (UpdExtQuoteAsmDataSet.QuoteMtlRow mtl in qads.QuoteMtl.Rows.Cast<UpdExtQuoteAsmDataSet.QuoteMtlRow>().Where(m => m.QuoteLine == asm.QuoteLine))
                {
                    _quoteAsmBo.GetNewQuoteMtl(ds, mtl.QuoteNum, mtl.QuoteLine, 0);
                    var newmtl = ds.QuoteMtl.Rows.Cast<QuoteAsmDataSet.QuoteMtlRow>().ToArray()[ds.QuoteMtl.Rows.Count - 1];
                    newmtl.PartNum = mtl.PartNum;
                    newmtl.QtyPer = mtl.QtyPer;
                    newmtl.IUM = mtl.IUM;
                    newmtl.PartNumPartDescription = mtl.PartNumPartDescription;
                    newmtl.Description = mtl.Description;
                    newmtl.Class = mtl.Class;
                    newmtl.EstUnitCost = mtl.EstUnitCost;
                    if (mtl["EstScrapType"] != DBNull.Value && mtl["EstScrap"] != DBNull.Value)
                    {
                        newmtl.EstScrapType = mtl.EstScrapType;
                        newmtl.EstScrap = mtl.EstScrap;
                    }
                    newmtl.RelatedOperation = 10;
                    _quoteAsmBo.Update(ds);
                }
                _quoteAsmBo.DeleteByID(asm.QuoteNum, asm.QuoteLine, 1);
            }
        }

        public Exception CommitReportingData(UpdExtUD03DataSet ds, string plant, string fileName)
        {
            bool errors = false;
            try
            {
                var ud03Errors = _ud03Bo.UpdateExt(ds, true, false, out errors);
                if (ud03Errors.BOUpdError.Rows.Count > 0)
                {
                    return new Exception(ud03Errors.BOUpdError.Rows[0]["ErrorText"].ToString());
                }
            }
            catch (BusinessObjectException ex)
            {
                return ex;
            }
            catch (Exception ex)
            {
                return new Exception("The file name " + fileName + " for plant " + plant + " is already used in the UD03 table. Choose another file name.", ex);
            }

            return new Exception("");
        }

        public void CloseSession()
        {
            _session.Dispose();
        }
    }
}
