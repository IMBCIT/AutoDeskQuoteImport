using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HedRow = Erp.BO.UpdExtQuoteDataSet.QuoteHedRow;
using DtlRow = Erp.BO.UpdExtQuoteDataSet.QuoteDtlRow;
using OprRow = Erp.BO.UpdExtQuoteAsmDataSet.QuoteOprRow;
using MtlRow = Erp.BO.UpdExtQuoteAsmDataSet.QuoteMtlRow;
using System.Data;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class RolledUpQuoteFactory : IQuoteFactory
    {
        Epicor10Cache _cache;
        UdService _udService;

        public RolledUpQuoteFactory(Epicor10Cache cache, UdService udService)
        {
            _cache = cache;
            _udService = udService;
        }

        public IQuote CreateQuote(CamductJob job)
        {
            var quote = new Quote(job);
            quote.MakeUDCompatible(_udService);

            // create header
            var hed = CreateQuoteHedRow(quote, job);
            quote.AddHedRow(hed);

            var rollup = job.GetRollup(_cache);
            int lineNUmber = 0;
            foreach (var line in rollup.GetLines())
            {
                // create dtl
                var dtl = CreateQuoteDtlRow(quote, line.GetTypeOCollectionQuery());
                quote.AddDtlRow(dtl);


                string referenceKey = line.GetTypeOCollectionQuery().GetLineReferenceKey();

                // create asm
                var asm = quote.NewAsmRow();
                quote.AddAsmRow(referenceKey, asm);

                // create oprs
                CreateOprs(quote, line.GetOprsData(), referenceKey, line);

                // create mtls
                var dtlProdCode = dtl.ProdCode;
                CreateMtls(quote, line, dtlProdCode, referenceKey, ++lineNUmber);

                // set mfgdetail and dtl.number08: 
                dtl.MfgDetail = HasMfgDetails(quote, referenceKey);
                dtl["Number08"] = GetSumOfMtlWeight(quote.GetMtls(referenceKey));
            }

            /*
            foreach (var mtlRow in quote.GetQuoteAsmDataSet().QuoteMtl.AsEnumerable().Where(x => !string.IsNullOrEmpty(x["QuoteLine"].ToString())))
            {
                var item = rollup._oFile.FirstOrDefault(x => x.ItemNoField.GetTextValue().Equals(mtlRow["QuoteLine"].ToString()));

                for (int lineNum = 0; lineNum < quote.GetQuoteDataSet().QuoteDtl.Rows.Count; lineNum++)
                {
                    if(quote.GetQuoteDataSet().QuoteDtl.Rows[lineNum]["PartNum"].ToString().Equals(item.EpicorGroupField.GetTextValue(), StringComparison.OrdinalIgnoreCase))
                    {
                        mtlRow["QuoteLine"] = lineNum +1;
                    }                    
                }
                
                
            }
            */
            return quote;
        }

        void CreateOprs(Quote quote, IEnumerable<RollupOprData> oprsData, string referenceKey, RollupLine line)
        {
            // create oprs
            bool didWeAddAnyOprs = false;
            foreach (var oprData in oprsData)
            {
                var opr = CreateOprRow(quote, oprData);
                quote.AddOprRow(referenceKey, opr);
                didWeAddAnyOprs = true;
            }

            if (didWeAddAnyOprs)
            {
                // create final opr
                var finalOpr = CreateFinalOprRow(quote);
                quote.AddOprRow(referenceKey, finalOpr);
            }
        }

        void CreateMtls(Quote quote, RollupLine line, string dtlProdCode, string referenceKey, int relatedLine)
        {
            //var relatedLine = 0;
            //for(int i = 0; i < quote.GetQuoteDataSet().QuoteDtl.Rows.Count; i++)
            //{
            //    if(quote.GetQuoteDataSet().QuoteDtl[i].PartNum.Equals(dtlProdCode, StringComparison.OrdinalIgnoreCase))
            //    {
            //        relatedLine = i + 1;
            //        break;
            //    }
            //}

            // create mtls from o-file line.
            foreach (var mtlData in line.GetMtlsData())
            {
                var mtl = CreateMtlRow(quote, mtlData, dtlProdCode);
                if (relatedLine != 0)
                {
                    mtl.QuoteLine = relatedLine;
                }
                quote.AddMtlRow(referenceKey, mtl);
            }

            // create mtls from the a-file.
            foreach (var aData in line.GetAncilliaryMtlsData())
            {
                var aDataConverter = new TypeADataConverter(aData, _cache);
                var mtl = CreateMtlRow(quote, aDataConverter, dtlProdCode);
                if (relatedLine != 0)
                {
                    mtl.QuoteLine = relatedLine;
                }
                quote.AddMtlRow(referenceKey, mtl);
            }
        }

        HedRow CreateQuoteHedRow(Quote quote, CamductJob job)
        {
            var hed = quote.NewHedRow();
            //hed.Company = job.OFile[0].CompanyField.GetValue();//FX

            var firstOLine = job.OFile.FirstOrDefault();
            if (firstOLine == null)
            {
                throw new NoFirstOLineException(job.Name);
            }

            var oDataConverter = new TypeODataConverter(firstOLine, _cache);

            hed.CustomerCustID = job.GetCustomerId();
            // hed.QuoteComment = job.Plant + " " + job.Name; // 12/2/2016 removed w/UD fields
            hed["CheckBox01"] = true;
            hed["ShortChar01"] = job.Company + "-" + job.Plant + "-" + job.Name;
            hed["ShortChar06"] = oDataConverter.GetHedShortChar06();
            hed["ShortChar07"] = oDataConverter.GetHedShortChar07();
            hed["ShortChar08"] = oDataConverter.GetHedShortChar08();
            hed["ShortChar09"] = oDataConverter.GetHedShortChar09();
            hed["ShortChar10"] = oDataConverter.GetHedShortChar10();
            hed["ShortChar11"] = oDataConverter.GetHedShortChar11();

            var customer = _cache.CustomerCache.FirstOrDefault(c => c.CustID == hed.CustomerCustID);
            if (customer != null)
            {
                hed.CustNum = customer.CustNum;
                hed.SalesRepCode = customer.SalesRepCode;
                hed.TerritoryID = customer.TerritoryID;
                hed.TerritoryTerritoryDesc = customer.TerritoryTerritoryDesc;
                hed.TermsCode = customer.TermsCode;
                hed.ShipViaCode = customer.ShipViaCode;
            }
            else
            {
                throw new NoCustomerFoundInCacheException(job.GetCustomerId());
            }

            var shipto = _cache.ShipToCache.FirstOrDefault(s => s.CustNum == hed.CustNum);
            if (shipto != null)
            {
                hed.ShipToCustNum = shipto.CustNum;
                hed.ShipToCustID = shipto.CustNumCustID;
            } // skips these, they are automatically filled out by Epicor using the customer if not present.

            return hed;
        }

        DtlRow CreateQuoteDtlRow(Quote quote, TypeOCollectionQuery oQuery)
        {
            //if (oQuery.IsStockOrSpiral())
            //{
            //    var converter = new TypeODataConverter(oQuery.GetFirstOData(), _cache);
            //    return CreateQuoteDtlRow(quote, converter);
            //}

            var dtl = quote.NewDtlRow();
            //FX //dtl.Company = oQuery.Company;           
            dtl.ProdCode = oQuery.GetProdCode();
            dtl.SellingExpectedQty = oQuery.GetSellingExpectedQty();
            dtl.PartNum = oQuery.GetPartNumber();
            dtl.LineDesc = oQuery.GetLineDesc();
            dtl.PartNumPartDescription = dtl.LineDesc;
            dtl.SellingExpectedUM = oQuery.GetSellingExpectedUM();
            dtl.POLine = oQuery.GetPOLine();
            dtl["CheckBox01"] = oQuery.GetDtlCheckBox01();//HACK dtl["CheckBox01"] = oQuery.GetDtlCheckBox01();
            dtl["Number07"] = oQuery.GetNumber07();
            dtl["Number09"] = oQuery.GetNumber09();
            dtl["Number10"] = oQuery.GetNumber10();
            dtl["ShortChar01"] = oQuery.GetDtlShortChar01();
            dtl["ShortChar02"] = oQuery.GetDtlShortChar02();
            dtl["ShortChar03"] = oQuery.GetDtlShortChar03();
            dtl["ShortChar04"] = oQuery.GetDtlShortChar04();
            dtl.MfgDetail = true;

            return dtl;
        }

        DtlRow CreateQuoteDtlRow(Quote quote, TypeODataConverter oDataConverter)
        {
            var dtl = quote.NewDtlRow();

            //FX //dtl.Company = oDataConverter.Company;
            dtl.ProdCode = oDataConverter.GetProdCode();
            dtl.SellingExpectedQty = oDataConverter.GetSellingExpectedQty();
            dtl.PartNum = oDataConverter.GetPartNumber();
            dtl.LineDesc = oDataConverter.GetLineDesc();
            dtl.PartNumPartDescription = dtl.LineDesc;
            dtl.SellingExpectedUM = oDataConverter.GetSellingExpectedUM();
            dtl.POLine = oDataConverter.GetPOLine();
            dtl["CheckBox01"] = oDataConverter.GetDtlCheckBox01();//HACK dtl["CheckBox01"] = oDataConverter.GetDtlCheckBox01();
            dtl["Number07"] = oDataConverter.GetDtlNumber07();
            dtl["Number08"] = oDataConverter.GetDtlNumber08();
            dtl["Number09"] = oDataConverter.GetDtlNumber09();
            dtl["Number10"] = oDataConverter.GetDtlNumber10();
            dtl["ShortChar01"] = oDataConverter.GetDtlShortChar01();
            dtl["ShortChar02"] = oDataConverter.GetDtlShortChar02();
            dtl["ShortChar03"] = oDataConverter.GetDtlShortChar03();
            dtl["ShortChar04"] = oDataConverter.GetDtlShortChar04();
            dtl.MfgDetail = true;

            return dtl;
        }

        OprRow CreateOprRow(Quote quote, RollupOprData oprData)
        {
            var opr = quote.NewOprRow();
            opr.OpCode = oprData.OpCode;
            opr.OpDesc = oprData.OpDesc;
            opr.LaborEntryMethod = "B";
            opr.ProdStandard = oprData.ProdStandard;
            opr.StdFormat = "MP";

            return opr;
        }

        OprRow CreateFinalOprRow(Quote quote)
        {
            var finalOpr = quote.NewOprRow();

            finalOpr.OpCode = "RPRT";
            finalOpr.OpDesc = "Report Quantity";
            finalOpr.LaborEntryMethod = "Q";
            finalOpr.ProdStandard = 1m;
            finalOpr.StdFormat = "MP";
            finalOpr.FinalOpr = true;

            return finalOpr;
        }

        MtlRow CreateMtlRow(Quote quote, RollupMtlData mtlData, string dtlProdCode)
        {
            var mtl = quote.NewMtlRow();

            mtl.PartNum = mtlData.PartNumber;
            mtl.QtyPer = mtlData.QtyPer;
            mtl.IUM = mtlData.IUM;
            mtl.BuyIt = false;
            if (!string.IsNullOrWhiteSpace(mtlData.EstScrapType))
            {
                mtl.EstScrapType = mtlData.EstScrapType;
                mtl.EstScrap = mtlData.EstScrap;
            }

            MapCachedDataToMtlRow(mtl, dtlProdCode);

            return mtl;
        }

        MtlRow CreateMtlRow(Quote quote, TypeADataConverter aDataConverter, string dtlProdCode)
        {
            var mtl = quote.NewMtlRow();

            mtl.PartNum = aDataConverter.GetPartNumber();
            mtl.QtyPer = aDataConverter.GetQtyPer();
            mtl.BuyIt = false;
            mtl.QuoteLine = aDataConverter.GetLineNo();

            MapCachedDataToMtlRow(mtl, dtlProdCode);

            return mtl;
        }

        void MapCachedDataToMtlRow(MtlRow mtl, string dtlProdCode)
        {
            var part = _cache.NonDtlPartCache.FirstOrDefault(p => p.PartNum == mtl.PartNum);
            if (part != null)
            {
                mtl.PartNumPartDescription = part.PartDescription;
                mtl.IUM = part.IUM;
                mtl.Class = part.ClassID;
                mtl.Description = part.PartDescription;
                if (dtlProdCode == "RCTCOIL" || dtlProdCode == "SPRLFAB")
                {
                    mtl.EstScrap = Convert.ToDecimal(part["DefaultScrap_c"]);
                }
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

        decimal GetSumOfMtlWeight(IEnumerable<MtlRow> mtls)
        {
            var sum = 0m;
            foreach (var mtl in mtls)
            {
                var part = _cache.NonDtlPartCache.FirstOrDefault(p => p.PartNum == mtl.PartNum);
                if (part != null)
                {
                    sum += mtl.QtyPer * part.GrossWeight;
                }
                else
                {
                    throw new NoPartFoundInCacheException(mtl.PartNum);
                }
            }

            return sum;
        }

        bool HasMfgDetails(Quote quote, string referenceKey)
        {
            var mtls = quote.GetMtls(referenceKey);
            var oprs = quote.GetOprs(referenceKey);

            return mtls.Count() > 0 || oprs.Count() > 0;
        }
    }
}
