using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class RollupLine
    {
        IEnumerable<TypeOData> _oData;
        IEnumerable<TypeAData> _aData;
        FileTypeS _sFile;
        bool _isBurnDetected;
        TypeOCollectionQuery _query;
        private readonly Epicor10Cache _cache;

        public RollupLine(IEnumerable<TypeOData> oData, IEnumerable<TypeAData> aData, FileTypeS sFile, bool isBurnDetected, Epicor10Cache cache)
        {
            _oData = oData;
            _aData = aData;
            _sFile = sFile;
            _isBurnDetected = isBurnDetected;
            _query = new TypeOCollectionQuery(oData, cache);
            _cache = cache;
        }

        public TypeOCollectionQuery GetTypeOCollectionQuery()
        {
            return _query;
        }

        public IEnumerable<RollupMtlData> GetMtlsData()
        {
            var rollupMtls = new List<RollupMtlData>();

            var omtls = _query.GetMtlData().Where(m => !m.IsSkippable()).ToList();
            var distinctMtlPartNumbers = omtls.Select(m => m.PartNumber).Distinct();
 
            foreach (var part in distinctMtlPartNumbers)
            {
                var mtls = omtls.Where(m => m.PartNumber == part);
                var qty1 = mtls.Sum(m => m.QtyPer);
                var ium = mtls.First().IUM;

                var rollupMtl = new RollupMtlData(part, qty1, ium);

                // check for scrap and include it if necessary.
                var scrap = _sFile.GetScrap(part);
                var isNotRectangularCoil = _oData.First().EpicorGroupField.GetTextValue() != "RCTCOIL";
                var isNotSpiralFab = _oData.First().EpicorGroupField.GetTextValue() != "SPRLFAB";
                if (_isBurnDetected && isNotRectangularCoil && isNotSpiralFab && scrap != null)
                {
                    var converter = new TypeSDataConverter(scrap);
                    var estScrap = converter.GetEstScrap();
                    rollupMtl = new RollupMtlData(part, qty1, ium, estScrap);
                }

                rollupMtls.Add(rollupMtl);
            }

            return rollupMtls;
        }

        public IEnumerable<TypeAData> GetAncilliaryMtlsData()
        {
            // if (IsStockOrSpiralLine()) return _aData;

            var ancillaries = new List<TypeAData>();

            var qtySum = _query.GetSellingExpectedQty();
            var distinctParts = _aData.Select(a => a.AncillaryPartNoField.GetTextValue()).Distinct();
            foreach (var part in distinctParts)
            {
                var aData = _aData.Where(a => a.AncillaryPartNoField.GetTextValue() == part);

                var rollupData = new string[]
                    {
                        aData.First().JobNameField.GetTextValue(),
                        aData.First().DateField.GetTextValue(),
                        aData.First().TimeField.GetTextValue(),
                        aData.First().ItemNoField.GetTextValue(),
                        aData.First().AncillaryNameField.GetTextValue(),
                        aData.First().AncillaryPartNoField.GetTextValue(),
                        aData.Sum(a=>a.AncillaryLengthField.GetValue() / qtySum).ToString(),
                        aData.Sum(a=>a.AncillaryQtyField.GetValue() / qtySum).ToString(),
                    };
                var ancillary = new TypeAData(rollupData);
                ancillaries.Add(ancillary);
            }

            return ancillaries;
        }

        public IEnumerable<RollupOprData> GetOprsData()
        {
            return _query.GetOprData();
        }
    }


}
