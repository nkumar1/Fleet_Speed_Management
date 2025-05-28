using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Interface
{
    public interface IKafkaProducerService
    {
        Task SendMessageAsync<T>(string topic, T message);
    }
}
