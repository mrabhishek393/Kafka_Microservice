using Confluent.Kafka;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.ConsumerApp_Unit_Tests.ConsumerServices_Tests
{
    public class StartConsumerLoop
    {
        private Mock<IConsumer<Ignore, string>>? _consumer = new();

    }
}
