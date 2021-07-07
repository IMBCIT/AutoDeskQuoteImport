using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class PersistQuoteLineService : IPersistQuoteService
    {
        Epicor10 _store;
        ILog _logger;

        public PersistQuoteLineService(Epicor10 store, ILog logger)
        {
            _store = store;
            _logger = logger;
        }

        public bool PersistQuote(IQuote quote)
        {
            // return PersistQuote_Original(quote);
            // return PersistQuote_SlowWay(quote);

            return PersistQuote_RelatedOperation(quote);
        }

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

        public bool PersistQuote_RelatedOperation(IQuote quote)
        {
            bool isCommited;

            isCommited = CommitHedAndDtl(quote, "Quote Data");
            if (!isCommited) return false;

            foreach (var dtl in quote.GetDtls())
            {
                quote.SetAsmLinks(dtl.POLine, dtl.QuoteNum, dtl.QuoteLine);
            }

            isCommited = CommitAsm(quote, "Asm Data");
            if (!isCommited) return false;

            isCommited = UpdateRelatedOperation(quote, "Related Operation Data");
            if (!isCommited) return false;

            quote.MarkQuoteAsImported();
            quote.MarkLinesEngineered();

            isCommited = CommitHedAndDtl(quote, "Final Quote Data");
            if (!isCommited) return false;



            return true;
        }

        public bool PersistQuote_Original(IQuote quote)
        {
            bool isCommited;

            isCommited = CommitHedAndDtl(quote, "Quote Data");
            if (!isCommited) return false;

            foreach (var dtl in quote.GetDtls())
            {
                quote.SetAsmLinks(dtl.POLine, dtl.QuoteNum, dtl.QuoteLine);
            }

            isCommited = CommitAsm(quote, "Asm Data");
            if (!isCommited) return false;

            quote.MarkQuoteAsImported();
            quote.MarkLinesEngineered();

            isCommited = CommitHedAndDtl(quote, "Final Quote Data");
            if (!isCommited) return false;

            return true;
        }

        bool CommitHedAndDtl(IQuote quote, string dataSetName)
        {
            var quoteCommitResult = _store.CommitQuote(quote.GetQuoteDataSet());
            var quoteNum = quote.GetQuoteNum();

            // read the result message
            var isCommitted = ReadCommitResultMessage(dataSetName, quoteCommitResult, quoteNum);

            if (!isCommitted)
            {
                quote.Job.RegisterException(new Exception(quoteCommitResult));
                return false;
            }

            return true;
        }

        bool CommitAsm(IQuote quote, string dataSetName)
        {
            var quoteAsmCommitResult = _store.CommitQuoteAsm(quote.GetQuoteAsmDataSet());
            var quoteNum = quote.GetQuoteNum();

            // read the result message
            var isAsmCommitted = ReadCommitResultMessage(dataSetName, quoteAsmCommitResult, quoteNum);

            if (!isAsmCommitted)
            {
                quote.Job.RegisterException(new Exception(quoteAsmCommitResult));
                return false;
            }

            return true;
        }

        bool UpdateRelatedOperation(IQuote quote, string dataSetName)
        {
            var quoteNum = quote.GetQuoteNum();
            var quoteLines = quote.GetDtls().Select(d => d.QuoteLine);
            var quoteAsmCommitResult = _store.UpdateRelatedOperation(quoteNum, quoteLines);

            // read the result message
            var isAsmCommitted = ReadCommitResultMessage(dataSetName, quoteAsmCommitResult, quoteNum);

            if (!isAsmCommitted)
            {
                quote.Job.RegisterException(new Exception(quoteAsmCommitResult));
                return false;
            }

            return true;
        }

        bool ReadCommitResultMessage(string dataSetName, string resultMessage, int quoteNum)
        {
            if (string.IsNullOrWhiteSpace(resultMessage))
            {
                _logger.Info("Commited " + dataSetName + ". Status: Successful! QuoteNum: " + quoteNum);
                return true;
            }
            else
            {
                _logger.Info("Commited " + dataSetName + ". Status: " + resultMessage);
                if (quoteNum != 0)
                {
                    try
                    {
                        _store.DeleteQuote(quoteNum);
                    }
                    catch (Ice.Common.RecordNotFoundException)
                    {
                        // ignore
                    }
                }
                return false;
            }
        }

    }
}
