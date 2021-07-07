using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.OrderLibrary
{
    class ConvertJobsToOrdersService
    {
        Epicor10 _epicor;
        Epicor10Cache _cache;

        public ConvertJobsToOrdersService(Epicor10 epicor, Epicor10Cache cache)
        {
            _epicor = epicor;
            _cache = cache;
        }

        public List<IOrder> Convert(CamductJobCollection jobs)
        {
            var udService = new UdService(_epicor);
            var orderFactory = new OrderFactory(_cache, udService);
            var orders = new List<IOrder>();
            foreach (var job in jobs)
            {
                try
                {
                    var order = orderFactory.CreateOrder(job);
                    orders.Add(order);
                }
                catch (Exception ex)
                {
                    var orderException = new OrderMapException(job, ex);
                    job.RegisterException(orderException);
                }
            }

            return orders;
        }
    }
}
