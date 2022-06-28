using API;
using API.DTOs;
using AutoMapper;
using Confluent.Kafka;
using Database;
using Database.Models;
using Microsoft.Extensions.Configuration;
using Moq;
using Producer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Testing.API_Unit_Tests.APIServices_Tests
{
    public class APIServices_InsertData
    {
        private Mock<IConfiguration>? _config = new();
        private Mock<IDatabaseServices>? _databaseservices = new();
        private Mock<IProducerService>? _producerservices = new();
        private Mock<IMapper>? _mapper = new();

        [Fact]
        public async Task InsertAsync_NoException_InsertData()
        {
            APIServices apiservices =
               new APIServices(
               _config.Object,
               _databaseservices.Object,
               _producerservices.Object,
               _mapper.Object
               );

            await apiservices.InsertAsync(It.IsAny<AddDataDTO>());

            _mapper.Verify(c => c.Map<GroupModel>(It.IsAny<AddDataDTO>()), Times.Once);
            _databaseservices.Verify(c => c.InsertAysnc(It.IsAny<GroupModel>()),Times.Once);
            _producerservices.Verify(c => c.WriteMessage(It.IsAny<string>(), It.IsAny<string>()),Times.Once);
        }

        //[Fact]
        //public async Task InsertAsync_ForKafkaProduceError_ThrowsProduceException()
        //{

        //    _producerservices.Setup(m => m.WriteMessage(It.IsAny<string>(), It.IsAny<string>()))
        //        .ThrowsAsync(new ProduceException<Null, string>(It.IsAny<Error>(),It.IsAny< DeliveryResult < Null,string>>()));

        //    APIServices apiservices =
        //        new APIServices(
        //        _config.Object,
        //        _databaseservices.Object,
        //        _producerservices.Object,
        //        _mapper.Object
        //        );

        //    await Assert.ThrowsAsync<ProduceException<Null,string>>(() => apiservices.InsertAsync(It.IsAny<AddDataDTO>()));
        //}

        [Fact]
        public async Task InsertAsync_ForAnyError_ThrowsException()
        {

            _databaseservices.Setup(m=>m.InsertAysnc(It.IsAny<GroupModel>())).ThrowsAsync(new Exception());
            _producerservices.Setup(m => m.WriteMessage(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            APIServices apiservices =
                new APIServices(
                _config.Object,
                _databaseservices.Object,
                _producerservices.Object,
                _mapper.Object
                );

            await Assert.ThrowsAsync<Exception>(() => apiservices.InsertAsync(It.IsAny<AddDataDTO>()));
        }


    }
}
