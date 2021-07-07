using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Erp.BO;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class TypeADataConverter
    {
        TypeAData _aData;
        Epicor10Cache _cache;
        PartDataSet.PartRow _part;

        public TypeADataConverter(TypeAData aData, Epicor10Cache cache)
        {
            _aData = aData;
            _cache = cache;
            _part = _cache.NonDtlPartCache.FirstOrDefault(p => p.PartNum == this.GetPartNumber());
        }

        public string GetPartNumber()
        {
            return _aData.AncillaryPartNoField.GetTextValue();
        }

        public string GetDescription()
        {
            return _aData.AncillaryNameField.GetTextValue();
        }

        public int GetLineNo()
        {
            return Convert.ToInt32(_aData.ItemNoField.GetTextValue());
        }

        public decimal GetQtyPer()
        {
            if (_part?.IUM == "FT")
                return _aData.AncillaryLengthField.GetValue() ?? 0m;
            else
                return _aData.AncillaryQtyField.GetValue() ?? 0m;
        }
    }
}
