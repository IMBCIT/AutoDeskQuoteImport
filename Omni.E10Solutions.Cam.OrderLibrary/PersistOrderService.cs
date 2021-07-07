using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Omni.E10Solutions.Cam.OrderLibrary
{
    class PersistOrderService
    {
        ILog _logger;
        Epicor10 _epicor;

        public PersistOrderService(Epicor10 epicor, ILog logger)
        {
            _epicor = epicor;
            _logger = logger;
        }

        public bool PersistOrder(IOrder order)
        {
            // commit the hed and dtl.
            var orderCommitResult = _epicor.CommitOrder(order.GetOrderDataSet());
            var orderNum = order.GetOrderNum();

            // read the result message
            var isCommitted = ReadCommitResultMessage("Order Data", orderCommitResult, orderNum);

            if (!isCommitted)
            {
                order.Job.RegisterException(new Exception(orderCommitResult));
                return false;
            }

            return true;
        }

        bool ReadCommitResultMessage(string dataSetName, string resultMessage, int orderNum)
        {
            if (string.IsNullOrWhiteSpace(resultMessage))
            {
                _logger.Info("Commited " + dataSetName + ". Status: Successful! OrderNum: " + orderNum);
                return true;
            }
            else
            {
                _logger.Info("Commited " + dataSetName + ". Status: " + resultMessage);
                if (orderNum != 0)
                {
                    _epicor.DeleteOrder(orderNum);
                }
                return false;
            }
        }
    }
}
