using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bag = System.Collections.Generic.Dictionary<string, object>;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public interface IPersistQuoteService
    {
        bool PersistQuote(IQuote quote);
    }
    public abstract class BasePersistQuote : IPersistQuoteService
    {
        protected Epicor10 _store;
        protected ILog _logger;

        public BasePersistQuote(Epicor10 store, ILog logger)
        {
            _store = store;
            _logger = logger;
        }

        public virtual bool PersistQuote(IQuote quote)
        {
            // return PersistQuote_Original(quote);
            // return PersistQuote_SlowWay(quote);

            // assume if this fails, no UD.
            if (PersistQuote_RelatedOperation(quote))
            {
                // assume if UD fails... just log and move on.
                return PersistReportingFields(quote);
            }
            return false;
        }
        protected virtual void BeforePersistQuote(IQuote quote, Bag bag) { }
        protected virtual bool BeforeCommitAsm(IQuote quote, Bag bag) => true;
        protected virtual bool PersistQuote_RelatedOperation(IQuote quote)
        {
            bool isCommited;
            var bag = new Bag();
            
            BeforePersistQuote(quote, bag);
            
            isCommited = CommitHedAndDtl(quote, "Quote Data");
            if (!isCommited) return false;

            if (!BeforeCommitAsm(quote, bag)) return false;

            isCommited = CommitAsm(quote, "Asm Data");
            if (!isCommited) return false;

            isCommited = UpdateRelatedOperation(quote, "Related Operation Data");
            if (!isCommited) return false;

            RefreshQuoteDs(quote);

            quote.MarkQuoteAsImported();
            quote.MarkLinesEngineered();

            isCommited = CommitHedAndDtl(quote, "Final Quote Data");
            return isCommited;
        }

        protected virtual void RefreshQuoteDs(IQuote quote)
        {
            var newDs = _store.GetQuoteDataSet(quote.GetQuoteNum());
            quote.RefreshQuoteDS(newDs);
            _store.MakeDataSetUDCompatible(quote.GetQuoteDataSet());
        }
            
        protected virtual bool CommitHedAndDtl(IQuote quote, string dataSetName)
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

        protected virtual bool CommitAsm(IQuote quote, string dataSetName)
        {
            //var quoteAsmCommitResult = _store.CommitQuoteAsm(quote.GetQuoteAsmDataSet());
            //var quoteNum = quote.GetQuoteNum();

            //// read the result message
            //var isAsmCommitted = ReadCommitResultMessage(dataSetName, quoteAsmCommitResult, quoteNum);

            //if (!isAsmCommitted)
            //{
            //    quote.Job.RegisterException(new Exception(quoteAsmCommitResult));
            //    return false;
            //}

            return true;
        }

        protected virtual bool UpdateRelatedOperation(IQuote quote, string dataSetName)

        {
            _ = _store.UpdateQuoteOperation(quote);

            var quoteNum = quote.GetQuoteNum();

            var quoteAsmCommitResult = _store.UpdateRelatedOperation(quote);

            // read the result message
            var isAsmCommitted = ReadCommitResultMessage(dataSetName, quoteAsmCommitResult, quoteNum);

            if (!isAsmCommitted)
            {
                quote.Job.RegisterException(new Exception(quoteAsmCommitResult));
                return false;
            }

            return true;
        }

        protected virtual bool ReadCommitResultMessage(string dataSetName, string resultMessage, int quoteNum)
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

        protected virtual bool PersistReportingFields(IQuote quote)
        {
            var commitResult = _store.CommitReportingData(quote.GetReportingDataSet(), quote.Job.Plant, quote.Job.Name);

            var isCommitted = ReadCommitResultMessage("Report Fields (UD03)", commitResult.Message, quote.GetQuoteNum());

            if (!isCommitted)
            {
                quote.Job.RegisterException(commitResult);
                return false;
            }

            return true;
        }
    }
}
