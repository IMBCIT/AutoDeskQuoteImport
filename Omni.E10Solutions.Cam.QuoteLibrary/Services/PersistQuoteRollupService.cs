using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Bag = System.Collections.Generic.Dictionary<string, object>;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class PersistQuoteRollupService : BasePersistQuote, IPersistQuoteService
    {
        //Epicor10 _store;
        //ILog _logger;

        public PersistQuoteRollupService(Epicor10 store, ILog logger):base(store, logger)
        {
            //_store = store;
            //_logger = logger;
        }
        protected override void BeforePersistQuote(IQuote quote, Bag bag)
        {
            bag.Add("shorChar01", quote.GetQuoteDataSet().QuoteHed[0]["ShortChar01"]);
        }
        protected override bool BeforeCommitAsm(IQuote quote, Bag bag)
        {
            var shorChar01 = bag["shorChar01"].ToString();
            var newShortChar = quote.GetQuoteDataSet().QuoteHed[0]["ShortChar01"].ToString();
            if (!newShortChar.Equals(shorChar01))
            {
                quote.GetQuoteDataSet().QuoteHed[0]["ShortChar01"] = shorChar01;
                return CommitHedAndDtl(quote, "Quote Data 2");
            }
            return true;
        }
        //public override bool PersistQuote(IQuote quote)
        //{
        //    // return PersistQuote_Original(quote);
        //    // return PersistQuote_SlowWay(quote);

        //    // assume if this fails, no UD.
        //    if (PersistQuote_RelatedOperation(quote))
        //    {
        //        // assume if UD fails... just log and move on.
        //        return PersistReportingFields(quote);
        //    }
        //    return false;
        //}

        //public bool PersistQuote_RelatedOperation(IQuote quote)
        //{
        //    bool isCommited;
        //    var shorChar01 = quote.GetQuoteDataSet().QuoteHed[0]["ShortChar01"].ToString();

        //    isCommited = CommitHedAndDtl(quote, "Quote Data");
        //    if (!isCommited) return false;

        //    var newShortChar = quote.GetQuoteDataSet().QuoteHed[0]["ShortChar01"].ToString();
        //    if (isCommited && !newShortChar.Equals(shorChar01))
        //    {
        //        quote.GetQuoteDataSet().QuoteHed[0]["ShortChar01"] = shorChar01;
        //        isCommited = CommitHedAndDtl(quote, "Quote Data 2");

        //        if (!isCommited) return false;
        //    }

        //    //foreach (var dtl in quote.GetDtls())
        //    //{
        //    //    //quote.SetAsmLinks(dtl.POLine, dtl.QuoteNum, dtl.QuoteLine);
        //    //}

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

        #region Unused

        public bool PersistQuote_SlowWay(IQuote quote)
        {
            bool isCommited;

            isCommited = CommitHedAndDtl(quote, "Quote Data");
            if (!isCommited) return false;

            foreach (var dtl in quote.GetDtls())
            {
                quote.SetAsmLinks(dtl.POLine, dtl.QuoteNum, dtl.QuoteLine);
            }


            quote.MarkMtlsAsRelatedToFirstOperation();
            _store.UpdateQuoteAsm_LongForm(quote.GetQuoteAsmDataSet(), quote.GetDtls());

            quote.MarkQuoteAsImported();
            quote.MarkLinesEngineered();

            isCommited = CommitHedAndDtl(quote, "Final Quote Data");
            if (!isCommited) return false;

            return true;
        }

        public bool PersistQuote_Original(IQuote quote)
        {
            bool isCommited;
            var shorChar01 = quote.GetQuoteDataSet().QuoteHed[0]["ShortChar01"].ToString();

            isCommited = CommitHedAndDtl(quote, "Quote Data");

            var newShortChar = quote.GetQuoteDataSet().QuoteHed[0]["ShortChar01"].ToString();
            if (isCommited && !newShortChar.Equals(shorChar01))
            {
                quote.GetQuoteDataSet().QuoteHed[0]["ShortChar01"] = shorChar01;
                isCommited = CommitHedAndDtl(quote, "Quote Data 2");
            }

            if (!isCommited) return false;

            foreach (var dtl in quote.GetDtls())
            {
                quote.SetAsmLinks(dtl.POLine, dtl.QuoteNum, dtl.QuoteLine);
            }

            isCommited = CommitAsm(quote, "Asm Data");
            if (!isCommited) return false;

            RefreshQuoteDs(quote);

            quote.MarkQuoteAsImported();
            quote.MarkLinesEngineered();

            isCommited = CommitHedAndDtl(quote, "Final Quote Data");
            if (!isCommited) return false;

            return true;
        }

        public bool PersistQuoteToUDStaging(IQuote quote)
        {
            // placeholder for testing saving data to UD.
            return false;
        } 
        #endregion

        //void RefreshQuoteDs(IQuote quote)
        //{
        //    var newDs = _store.GetQuoteDataSet(quote.GetQuoteNum());
        //    quote.RefreshQuoteDS(newDs);
        //    _store.MakeDataSetUDCompatible(quote.GetQuoteDataSet());            
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
        //    return true;
        //    // Next section is not needed
        //    var quoteNum = quote.GetQuoteNum();
        //    var quoteAsmCommitResult = _store.CommitQuoteAsm(quote.GetQuoteAsmDataSet(), quoteNum);


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
        //    var resultMsg = _store.UpdateQuoteOperation(quote);

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

        //bool PersistReportingFields(IQuote quote)
        //{
        //    var commitResult = _store.CommitReportingData(quote.GetReportingDataSet(), quote.Job.Plant, quote.Job.Name);

        //    var isCommitted = ReadCommitResultMessage("Report Fields (UD03)", commitResult.Message, quote.GetQuoteNum());

        //    if (!isCommitted)
        //    {
        //        quote.Job.RegisterException(commitResult);
        //        return false;
        //    }

        //    return true;
        //}
    }
}
