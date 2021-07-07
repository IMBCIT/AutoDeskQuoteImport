using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Erp.BO;
using HedRow = Erp.BO.UpdExtQuoteDataSet.QuoteHedRow;
using DtlRow = Erp.BO.UpdExtQuoteDataSet.QuoteDtlRow;
using OprRow = Erp.BO.UpdExtQuoteAsmDataSet.QuoteOprRow;
using MtlRow = Erp.BO.UpdExtQuoteAsmDataSet.QuoteMtlRow;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class QuoteFactory : IQuoteFactory
    {
        Epicor10Cache _cache;
        UdService _udService;

        public QuoteFactory(Epicor10Cache cache, UdService udService)
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

            foreach (var oFileLine in job.OFile)
            {
                // create dtl
                var oDataConverter = new TypeODataConverter(oFileLine, _cache);
                var dtl = CreateQuoteDtlRow(quote, oDataConverter);
                quote.AddDtlRow(dtl);

                string referenceKey = oDataConverter.GetDtlReferenceKey();

                // create asm
                var asm = quote.NewAsmRow();
                quote.AddAsmRow(referenceKey, asm);

                // create oprs
                CreateOprs(quote, oDataConverter, referenceKey, oDataConverter.IsCatalogItem());

                // create mtls
                CreateMtls(quote, job, oDataConverter, referenceKey);

                // set dtl.number08: 
                dtl.MfgDetail = HasMfgDetails(quote, referenceKey);
                dtl["Number08"] = GetSumOfMtlWeight(quote.GetMtls(referenceKey));
            }

            return quote;
        }

        void CreateOprs(Quote quote, TypeODataConverter oDataConverter, string referenceKey, bool isCatalog)
        {
            // create oprs
            var oprsData = oDataConverter.GetOprData();
            var didWeAddAnyOprs = false;
            foreach (var oprData in oprsData)
            {
                if (oprData.IsSkippable()) continue;

                var opr = CreateOprRow(quote, oprData);
                quote.AddOprRow(referenceKey, opr);
                didWeAddAnyOprs = true;
            }

            var isCatalogWithOprs = isCatalog && didWeAddAnyOprs;

            if (!isCatalog || isCatalogWithOprs)
            {
                // create final opr
                var finalOpr = CreateFinalOprRow(quote);
                quote.AddOprRow(referenceKey, finalOpr);
            }
        }

        void CreateMtls(Quote quote, CamductJob job, TypeODataConverter oDataConverter, string referenceKey)
        {
            // create mtls from o-file line.
            foreach (var mtlData in oDataConverter.GetMtlData())
            {
                if (mtlData.IsSkippable())
                    continue;

                var mtl = CreateMtlRow(quote, mtlData);
                var scrap = job.SFile.GetScrap(mtl.PartNum);

                if (this.IsScrapIncluded(job, oDataConverter, scrap))
                {
                    var sDataConverter = new TypeSDataConverter(scrap);
                    IncludeScrapOnTheMtl(mtl, sDataConverter);
                }

                quote.AddMtlRow(referenceKey, mtl);
            }

            // create mtls from the a-file.
            foreach (var aData in oDataConverter.GetMatchingAData(job.AFile))
            {
                var aDataConverter = new TypeADataConverter(aData, _cache);
                var mtl = CreateMtlRow(quote, aDataConverter);
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
            hed["CheckBox01"] = true;//HACK hed["CheckBox01"] = true;
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

        DtlRow CreateQuoteDtlRow(Quote quote, TypeODataConverter oDataConverter)
        {
            var dtl = quote.NewDtlRow();
            dtl.PartNumIUM = oDataConverter.GetSellingExpectedUM();


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

        OprRow CreateOprRow(Quote quote, OprData oprData)
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

        MtlRow CreateMtlRow(Quote quote, MtlData mtlData)
        {
            var mtl = quote.NewMtlRow();

            mtl.PartNum = mtlData.PartNumber;
            mtl.QtyPer = mtlData.QtyPer;
            mtl.IUM = mtlData.IUM;
            mtl.QuoteLine = Convert.ToInt32(mtlData.Line);

            MapCachedDataToMtlRow(mtl);

            return mtl;
        }

        MtlRow CreateMtlRow(Quote quote, TypeADataConverter aDataConverter)
        {
            var mtl = quote.NewMtlRow();

            mtl.PartNum = aDataConverter.GetPartNumber();
            mtl.QtyPer = aDataConverter.GetQtyPer();
            // mtl.Description = aDataConverter.GetDescription(); // 11/21/2016 Changed to come from Epicor.
            mtl.QuoteLine = aDataConverter.GetLineNo();

            MapCachedDataToMtlRow(mtl);

            return mtl;
        }

        void MapCachedDataToMtlRow(MtlRow mtl)
        {
            var part = _cache.NonDtlPartCache.FirstOrDefault(p => p.PartNum == mtl.PartNum);
            if (part != null)
            {
                mtl.PartNumPartDescription = part.PartDescription;
                mtl.IUM = part.IUM;
                mtl.Class = part.ClassID;
                mtl.Description = part.PartDescription;
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

        bool IsScrapIncluded(CamductJob job, TypeODataConverter oDataConverter, TypeSData scrap)
        {
            var isBurnDetected = job.HasBurnOperation();
            var isNotRectangularCoil = !oDataConverter.IsRectangularCoil();
            var isScrapPresent = scrap != null;

            return isNotRectangularCoil && isBurnDetected && isScrapPresent;
        }

        void IncludeScrapOnTheMtl(MtlRow mtl, TypeSDataConverter sDataConverter)
        {
            mtl.EstScrapType = "%";
            mtl.EstScrap = sDataConverter.GetEstScrap();
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
