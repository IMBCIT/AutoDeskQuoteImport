using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Erp.BO;
using Ice.BO;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public interface IQuote
    {
        CamductJob Job { get; }

        UpdExtQuoteDataSet GetQuoteDataSet();
        UpdExtQuoteAsmDataSet GetQuoteAsmDataSet();
        IEnumerable<UpdExtQuoteDataSet.QuoteDtlRow> GetDtls();
        int GetQuoteNum();
        void SetAsmLinks(string referenceKey, int quoteNum, int quoteLine);
        void MarkQuoteAsImported();
        void MarkLinesEngineered();
        void MarkMtlsAsRelatedToFirstOperation();
        UpdExtUD03DataSet GetReportingDataSet();
        void RefreshQuoteDS(QuoteDataSet quoteDataSet);
    }
}
