using Erp.BO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class TypeOCollectionQuery
    {
        IEnumerable<TypeOData> _oData;
        private readonly Epicor10Cache _cache;
        private readonly PartListDataSet.PartListRow _part;

        public TypeOCollectionQuery(IEnumerable<TypeOData> oData, Epicor10Cache cache)
        {
            _oData = oData;
            _cache = cache;
            _part = _cache.DtlPartCache.FirstOrDefault(p => p.PartNum == GetPartNumber());
        }

        public bool IsStockOrSpiral() => _oData.Any(p => p.EpicorGroupField.GetTextValue() == "STCKCAT" || p.EpicorGroupField.GetTextValue() == "STCKSPL" || p.EpicorGroupField.GetTextValue() == "POSTTEN");

        public TypeOData GetFirstOData() => _oData.First();

        public string GetProdCode() => _oData.First().EpicorGroupField.GetTextValue();

        public decimal GetSellingExpectedQty()
        {
            // return _oData.Sum(o => o.QtyField.GetValue());
            var seQty = _oData.Where(o => !o.ItemAliasField.GetTextValue().ToUpper().Contains("LABOR")).Sum(o => o.QtyField.GetValue());
            if (seQty == 0)
                throw new SellingExpectedQtyCannotBeZeroException();
            return seQty;
        }

        public decimal GetSellingExpectedQtyPlusLabor() => _oData.Sum(o => o.QtyField.GetValue());

        public string GetPartNumber()
        {
            if (IsStockOrSpiral())
                return _oData.First().ItemAliasField.GetTextValue();

            return GetProdCode();
        }
        bool IsFileShowingCatalogItem() => _oData.Any(IsShowingCatalog);

        private static bool IsShowingCatalog(TypeOData p) => p.EpicorGroupField.GetValue() == "STCKCAT" || p.EpicorGroupField.GetValue() == "STCKSPL" || p.EpicorGroupField.GetValue() == "POSTTEN";

        bool IsEpicorShowingCatalogItem() => _part.ProdCode == "STCKCAT" || _part.ProdCode == "STCKSPL" || _part.ProdCode == "POSTTEN";

        public bool IsCatalogItem() => _part != null ? IsEpicorShowingCatalogItem() : IsFileShowingCatalogItem();

        public string GetLineDesc()
        {
            if (IsCatalogItem() && _part != null)
                return _part.PartDescription;
            
            var first = _oData.FirstOrDefault(IsShowingCatalog);
            return first != null
                ? string.Join(" - ", first.ItemDescriptionField.GetTextValue(), first.SizeEnd1Field.GetTextValue(), first.SizeEnd2Field.GetTextValue())
                : GetProdCode();
        }

        public string GetSellingExpectedUM() => "EA";

        public string GetPOLine() => _oData.First().ItemNoField.GetTextValue();

        public string GetLineReferenceKey() => GetPOLine();

        public bool GetDtlCheckBox01() => _oData.Any(p => string.IsNullOrWhiteSpace(p.NonGalvField.GetTextValue()));

        public decimal GetNumber07()
        {
            var qtySum = GetSellingExpectedQty();
            var x1 = _oData.Sum(o => o.QtyField.GetValue() * o.ItemLengthOrAngleField.GetValue() ?? 0m);

            var num07 = x1 / qtySum;
            return Math.Round(num07, 2);
        }

        public decimal GetNumber09()
        {
            var qtySum = GetSellingExpectedQty();
            var num09 = _oData.Sum(o => (o.DWSkinWeightField.GetValue() ?? 0m) * o.QtyField.GetValue()) / qtySum;
            return num09;
        }

        public decimal GetNumber10()
        {
            var qtySum = GetSellingExpectedQty();
            var num10 = _oData.Sum(o => (o.InsulationAreaField.GetValue() ?? 0m) * o.QtyField.GetValue()) / qtySum;
            return Math.Round(num10, 2);
        }

        public string GetDtlShortChar01() => _oData.First().PressureClassField.GetTextValue();

        public string GetDtlShortChar02() => _oData.First().MaterialField.GetTextValue() + "-" + _oData.First().WireGaugeField.GetTextValue();

        public string GetDtlShortChar03() => _oData.First().InsulationMaterialField.GetTextValue() + "-" + _oData.First().InsulationThicknessField.GetTextValue();

        public string GetDtlShortChar04() => _oData.First().DWSkinMaterialField.GetTextValue() + "-" + _oData.First().DWSkinGaugeField.GetTextValue();

        public List<MtlData> GetMtlData()
        {
            var omtls = new List<MtlData>();
            var seQty = GetSellingExpectedQty();

            foreach (var oData in _oData)
            {
                var oqty = oData.QtyField.GetValue();
                omtls.Add(
                        MtlData.NewRollupMtl(oData.MaterialField.GetTextValue() + "-" + oData.WireGaugeField.GetTextValue(), oData.BaseWeightField, oqty, seQty, "LB")
                        );

                omtls.Add(
                        MtlData.NewRollupMtl(oData.InsulationMaterialField.GetTextValue() + "-" + oData.InsulationThicknessField.GetTextValue(), oData.InsulationAreaField, oqty, seQty, "SF")
                        );

                omtls.Add(
                        MtlData.NewRollupMtl(oData.DWSkinMaterialField.GetTextValue() + "-" + oData.DWSkinGaugeField.GetTextValue(), oData.DWSkinWeightField, oqty, seQty, "LB")
                        );
            }

            return omtls;
        }

        public List<RollupOprData> GetOprData()
        {
            var seQty = GetSellingExpectedQty();
            var oprData = new List<RollupOprData>()
                    {
                        new RollupOprData("WELD", "Weld", _oData.Sum(o=>SumOfOpr(o.WELD_Time_minsField.GetValue(), o.QtyField.GetValue(), seQty))),
                        new RollupOprData("PRFB",  "Pre-Fabrication", _oData.Sum(o=>SumOfOpr(o.PRFB_Time_minsField.GetValue(), o.QtyField.GetValue(), seQty))),
                        new RollupOprData("BURN",  "Burn", _oData.Sum(o=>SumOfOpr(o.BURN_Time_minsField.GetValue(), o.QtyField.GetValue(),  seQty))),
                        new RollupOprData("COIL",  "Coil Line", _oData.Sum(o=>SumOfOpr(o.COIL_Time_minsField.GetValue(), o.QtyField.GetValue(), seQty))),
                        new RollupOprData("RECT",  "Rectangular", _oData.Sum(o=>SumOfOpr(o.RECT_Time_minsField.GetValue(), o.QtyField.GetValue(), seQty))),
                        new RollupOprData("ROND",  "Round", _oData.Sum(o=>SumOfOpr(o.ROND_Time_minsField.GetValue(), o.QtyField.GetValue(), seQty))),
                        new RollupOprData("SPIR",  "Spiral", _oData.Sum(o=>SumOfOpr(o.SPIR_Time_minsField.GetValue(), o.QtyField.GetValue(), seQty))),
                        new RollupOprData("PSTT", "Post Tensioning", _oData.Sum(o=>SumOfOpr(o.PSTT_Time_minsField.GetValue(), o.QtyField.GetValue(), seQty))),
                    };

            return oprData.Where(o => o.ProdStandard != 0m).ToList();
        }

        decimal SumOfOpr(decimal? oprTime, decimal oprQty, decimal seQty) => ((oprTime ?? 0m) * oprQty) / seQty;
        // return (value ?? 0m) / qtySum;
        // return (value ?? 0m) /oQty;
    }
}
