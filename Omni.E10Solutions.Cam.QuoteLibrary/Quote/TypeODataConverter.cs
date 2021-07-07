using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Erp.BO;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class TypeODataConverter
    {
        TypeOData _oData;
        Epicor10Cache _cache;
        PartListDataSet.PartListRow _part;

        public TypeODataConverter(TypeOData oData, Epicor10Cache cache)
        {
            _oData = oData;
            _cache = cache;

            // go ahead and cache this part number's data.
            _part = _cache.DtlPartCache.FirstOrDefault(p => p.PartNum == this.GetPartNumber());
        }

        public string GetProdCode()
        {
            if (_part != null)
                return _part.ProdCode;
            else return _oData.EpicorGroupField.GetValue();
        }

        public decimal GetSellingExpectedQty()
        {

            return _oData.QtyField.GetValue();
        }

        public string GetPartNumber()
        {
            return _oData.ItemAliasField.GetValue();
        }

        public bool IsCatalogItem()
        {
            if (_part != null)
                return IsEpicorShowingCatalogItem();
            else return IsFileShowingCatalogItem();
        }

        bool IsFileShowingCatalogItem()
        {
            return _oData.EpicorGroupField.GetValue() == "STCKCAT" || _oData.EpicorGroupField.GetValue() == "STCKSPL" || _oData.EpicorGroupField.GetValue() == "POSTTEN";
        }

        bool IsEpicorShowingCatalogItem()
        {
            return _part.ProdCode == "STCKCAT" || _part.ProdCode == "STCKSPL" || _part.ProdCode == "POSTTEN";
        }

        public bool IsRectangularCoil()
        {
            return _oData.EpicorGroupField.GetValue() == "RCTCOIL";
        }

        public string GetLineDesc()
        {
            if (this.IsCatalogItem() && _part != null)
            {
                return _part.PartDescription;
            }

            return _oData.ItemDescriptionField.GetTextValue() + " - " + _oData.SizeEnd1Field.GetTextValue() + " - " + _oData.SizeEnd2Field.GetTextValue();
        }

        public string GetSellingExpectedUM()
        {
            if (_part != null)
                return _part.IUM;

            return "EA";
        }

        public string GetPOLine()
        {
            return _oData.ItemNoField.GetTextValue();
        }

        public string GetDtlReferenceKey()
        {
            return _oData.ItemNoField.GetTextValue();
        }

        public int GetDtlNumber01()
        {
            return _oData.ItemNoField.GetValue();
        }

        public decimal GetDtlNumber07()
        {
            return _oData.ItemLengthOrAngleField.GetValue() ?? 0m;
        }

        public decimal GetDtlNumber08()
        {
            return _oData.BaseWeightField.GetValue() ?? 0m;
        }

        public decimal GetDtlNumber09()
        {
            return _oData.DWSkinWeightField.GetValue() ?? 0m;
        }

        public decimal GetDtlNumber10()
        {
            return _oData.InsulationAreaField.GetValue() ?? 0m;
        }

        public bool GetDtlCheckBox01()
        {
            return string.IsNullOrWhiteSpace(_oData.NonGalvField.GetTextValue());
        }

        public string GetDtlShortChar01()
        {
            return _oData.PressureClassField.GetTextValue();
        }

        public string GetDtlShortChar02()
        {
            return _oData.MaterialField.GetTextValue() + "-" + _oData.WireGaugeField.GetTextValue(); 
        }

        public string GetDtlShortChar03()
        {
            return _oData.InsulationMaterialField.GetTextValue() + "-" + _oData.InsulationThicknessField.GetTextValue();
        }

        public string GetDtlShortChar04()
        {
            return _oData.DWSkinMaterialField.GetTextValue() + "-" + _oData.DWSkinGaugeField.GetTextValue();
        }

        public string GetHedShortChar06()
        {
            return _oData.NumberCutSheetsField.GetTextValue();
        }

        public string GetHedShortChar07()
        {
            return _oData.DrawingNumberField.GetTextValue();
        }

        public string GetHedShortChar08()
        {
            return _oData.SpecSectionField.GetTextValue();
        }

        public string GetHedShortChar09()
        {
            return _oData.AddendumsField.GetTextValue();
        }

        public string GetHedShortChar10()
        {
            return _oData.MAJOtherFileNameField.GetTextValue();
        }

        public string GetHedShortChar11()
        {
            return _oData.SentViaField.GetTextValue();
        }

        public List<OprData> GetOprData()
        {
            var oprData = new List<OprData>()
                    {
                        new OprData("WELD", "Weld", _oData.WELD_Time_minsField),
                        new OprData("PRFB",  "Pre-Fabrication", _oData.PRFB_Time_minsField),
                        new OprData("BURN",  "Burn", _oData.BURN_Time_minsField),
                        new OprData("COIL",  "Coil Line", _oData.COIL_Time_minsField),
                        new OprData("RECT",  "Rectangular", _oData.RECT_Time_minsField),
                        new OprData("ROND",  "Round", _oData.ROND_Time_minsField),
                        new OprData("SPIR",  "Spiral", _oData.SPIR_Time_minsField),
                        new OprData("PSTT", "Post Tensioning", _oData.PSTT_Time_minsField),
                    };

            return oprData;
        }

        public IEnumerable<MtlData> GetMtlData()
        {
            yield return MtlData.NewLineItemMtl(_oData.MaterialField.GetTextValue() + "-" + _oData.WireGaugeField.GetTextValue(), 
                _oData.BaseWeightField, "LB", _oData.ItemNoField.GetTextValue());
            yield return MtlData.NewLineItemMtl(_oData.InsulationMaterialField.GetTextValue() + "-" + _oData.InsulationThicknessField.GetTextValue(), 
                _oData.InsulationAreaField, "SF", _oData.ItemNoField.GetTextValue());
            yield return MtlData.NewLineItemMtl(_oData.DWSkinMaterialField.GetTextValue() + "-" + _oData.DWSkinGaugeField.GetTextValue(),
                _oData.DWSkinWeightField, "LB", _oData.ItemNoField.GetTextValue());
        }

        public IEnumerable<TypeAData> GetMatchingAData(FileTypeA aFile)
        {
            return aFile.Where(a => a.ItemNoField.GetTextValue() == _oData.ItemNoField.GetTextValue());
        }
    }
}
