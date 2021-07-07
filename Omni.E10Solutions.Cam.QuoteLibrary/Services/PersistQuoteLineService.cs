using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class PersistQuoteLineService : BasePersistQuote, IPersistQuoteService
    {
        //Epicor10 _store;
        //ILog _logger;

        public PersistQuoteLineService(Epicor10 store, ILog logger) : base(store, logger)
        {
            //_store = store;
            //_logger = logger;
        }

        public override bool PersistQuote(IQuote quote)
        {
            // assume if this fails, no UD.
            return PersistQuote_RelatedOperation(quote);
        }
        protected override bool BeforeCommitAsm(IQuote quote, Dictionary<string, object> bag)
        {
            foreach (var dtl in quote.GetDtls())
                quote.SetAsmLinks(dtl.POLine, dtl.QuoteNum, dtl.QuoteLine);
            return true;
        }
        //public bool PersistQuote_SlowWay(IQuote quote)
        //{
        //    bool isCommited;

        //    isCommited = CommitHedAndDtl(quote, "Quote Data");
        //    if (!isCommited) return false;

        //    foreach (var dtl in quote.GetDtls())
        //    {
        //        quote.SetAsmLinks(dtl.POLine, dtl.QuoteNum, dtl.QuoteLine);
        //    }


        //    quote.MarkMtlsAsRelatedToFirstOperation();
        //    _store.UpdateQuoteAsm_LongForm(quote.GetQuoteAsmDataSet(), quote.GetDtls());

        //    quote.MarkQuoteAsImported();
        //    quote.MarkLinesEngineered();

        //    isCommited = CommitHedAndDtl(quote, "Final Quote Data");
        //    if (!isCommited) return false;

        //    return true;
        //}
        //protected override bool PersistQuote_RelatedOperation(IQuote quote)
        //{
        //    bool isCommited;

        //    isCommited = CommitHedAndDtl(quote, "Quote Data");

        //    if (!isCommited) return false;

        //    foreach (var dtl in quote.GetDtls())
        //        quote.SetAsmLinks(dtl.POLine, dtl.QuoteNum, dtl.QuoteLine);

        //    isCommited = CommitAsm(quote, "Asm Data");
        //    if (!isCommited) return false;

        //    isCommited = UpdateRelatedOperation(quote, "Related Operation Data");
        //    if (!isCommited) return false;

        //    RefreshQuoteDs(quote);

        //    quote.MarkQuoteAsImported();
        //    quote.MarkLinesEngineered();

        //    isCommited = CommitHedAndDtl(quote, "Final Quote Data");
        //    if (!isCommited) return false;

        //    return true;
        //}

        //void RefreshQuoteDs(IQuote quote)
        //{
        //    var newDs = _store.GetQuoteDataSet(quote.GetQuoteNum());
        //    quote.RefreshQuoteDS(newDs);
        //    _store.MakeDataSetUDCompatible(quote.GetQuoteDataSet());
        //}


        //public bool PersistQuote_Original(IQuote quote)
        //{
        //    bool isCommited;

        //    isCommited = CommitHedAndDtl(quote, "Quote Data");
        //    if (!isCommited) return false;

        //    foreach (var dtl in quote.GetDtls())
        //    {
        //        quote.SetAsmLinks(dtl.POLine, dtl.QuoteNum, dtl.QuoteLine);
        //    }

        //    isCommited = CommitAsm(quote, "Asm Data");
        //    if (!isCommited) return false;

        //    quote.MarkQuoteAsImported();
        //    quote.MarkLinesEngineered();

        //    isCommited = CommitHedAndDtl(quote, "Final Quote Data");
        //    if (!isCommited) return false;

        //    return true;
        //}

        //bool CommitHedAndDtl(IQuote quote, string dataSetName)
        //{
        //    var quoteCommitResult = _store.CommitQuote(quote.GetQuoteDataSet());
        //    var quoteNum = quote.GetQuoteNum();

        //    // read the result message
        //    var isCommitted = ReadCommitResultMessage(dataSetName, quoteCommitResult, quoteNum);

        //    if (!isCommitted)
        //    {
        //        quote.Job.RegisterException(new Exception(quoteCommitResult));
        //        return false;
        //    }

        //    return true;
        //}

        //bool CommitAsm(IQuote quote, string dataSetName)
        //{
        //    return true;//same rollup way
        //    var quoteAsmCommitResult = _store.CommitQuoteAsm(quote.GetQuoteAsmDataSet());
        //    var quoteNum = quote.GetQuoteNum();

        //    // read the result message
        //    var isAsmCommitted = ReadCommitResultMessage(dataSetName, quoteAsmCommitResult, quoteNum);

        //    if (!isAsmCommitted)
        //    {
        //        quote.Job.RegisterException(new Exception(quoteAsmCommitResult));
        //        return false;
        //    }

        //    return true;
        //}

        //bool UpdateRelatedOperation(IQuote quote, string dataSetName)
        //{
        //    _store.UpdateQuoteOperation(quote);//Rollup
        //    var quoteNum = quote.GetQuoteNum();

        //    var quoteAsmCommitResult = _store.UpdateRelatedOperation(quote);

        //    // read the result message
        //    var isAsmCommitted = ReadCommitResultMessage(dataSetName, quoteAsmCommitResult, quoteNum);

        //    if (!isAsmCommitted)
        //    {
        //        quote.Job.RegisterException(new Exception(quoteAsmCommitResult));
        //        return false;
        //    }

        //    return true;
        //}

        //bool ReadCommitResultMessage(string dataSetName, string resultMessage, int quoteNum)
        //{
        //    if (string.IsNullOrWhiteSpace(resultMessage))
        //    {
        //        _logger.Info("Commited " + dataSetName + ". Status: Successful! QuoteNum: " + quoteNum);
        //        return true;
        //    }
        //    else
        //    {
        //        _logger.Info("Commited " + dataSetName + ". Status: " + resultMessage);
        //        if (quoteNum != 0)
        //        {
        //            try
        //            {
        //                _store.DeleteQuote(quoteNum);
        //            }
        //            catch (Ice.Common.RecordNotFoundException)
        //            {
        //                // ignore
        //            }
        //        }
        //        return false;
        //    }
        //}

    }
}
