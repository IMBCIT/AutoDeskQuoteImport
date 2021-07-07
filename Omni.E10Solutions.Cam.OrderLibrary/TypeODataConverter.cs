using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.OrderLibrary
{
    class TypeODataConverter
    {
        TypeOData _oData;

        public TypeODataConverter(TypeOData oData)
        {
            _oData = oData;
        }

        public string GetHedPoNum()
        {
            return _oData.ProjectNameField.GetTextValue();
        }

        public string GetDtlProdCode()
        {
            return _oData.EpicorGroupField.GetTextValue();
        }

        public string GetDtlPartNum()
        {
            return _oData.ItemAliasField.GetTextValue();
        }

        public decimal GetDtlSellingQty()
        {
            return _oData.QtyField.GetValue();
        }
    }
}
