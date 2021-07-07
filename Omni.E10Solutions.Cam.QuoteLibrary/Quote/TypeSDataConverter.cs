using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    class TypeSDataConverter
    {
        TypeSData _sData;

        public TypeSDataConverter(TypeSData sData)
        {
            _sData = sData;
        }

        public bool IsInsulation()
        {
            return _sData.InsulationField.GetTextValue().ToLower() == "yes";
        }

        public decimal GetEstScrap()
        {
            decimal scrapRatio = 0m;

            if (IsInsulation())
            {
                var scrapSf = GetScrapSquareFootage();
                var cutArea = GetCutArea();
                scrapRatio = cutArea == 0m ? 0m : (scrapSf / cutArea) * 100;
            }
            else
            {
                var scrapWt = GetScrapWeight();
                var blankWeight = GetBlankWeight();
                scrapRatio = blankWeight == 0m ? 0m : (scrapWt / blankWeight) * 100;
            }

            return scrapRatio;
        }

        decimal GetScrapSquareFootage()
        {
            return _sData.ScrapSFField.GetValue() ?? 0m;
        }

        decimal GetCutArea()
        {
            return _sData.CutAreaField.GetValue() ?? 0m;
        }

        decimal GetScrapWeight()
        {
            return _sData.ScrapWtField.GetValue() ?? 0m; // WIP Null ?
        }

        decimal GetBlankWeight()
        {
            return _sData.BlankWeightField.GetValue() ?? 0m; // WIP null is 0?
        }
    }
}
