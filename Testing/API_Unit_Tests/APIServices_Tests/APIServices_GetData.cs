using AutoMapper;
using Database;
using Database.Models;
using Microsoft.Extensions.Configuration;
using Moq;
using Producer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using API;

namespace Testing.API_Unit_Tests.APIServices_Tests
{
    public class APIServices_GetData
    {
        private Mock<IConfiguration>? _config=new();
        private Mock<IDatabaseServices>? _databaseservices= new ();
        private Mock<IProducerService>? _producerservices= new ();
        private Mock<IMapper>? _mapper= new();

        [Fact]
        public async Task GetAllRecords_AnyError_ThrowsException()
        {
            //Arrange
            _databaseservices.Setup(m => m.GetAllAsync()).ThrowsAsync(new Exception());


            
            APIServices apiservices = 
                new APIServices(
                _config.Object, 
                _databaseservices.Object, 
                _producerservices.Object,
                _mapper.Object
                );


            //Act and Assert
            await Assert.ThrowsAsync<Exception>(()=>apiservices.GetAllRecords());

        }

        [Fact]
        public async Task GetAllRecords_NoException_ReturnAllRecords()
        {
            //Arrange

            List<GroupModel> expected=new List<GroupModel>
            {
                new GroupModel { Id = It.IsAny<Guid>(), Name =It.IsAny<String>(),Entitlements=It.IsAny<List<Guid>>(),CreatedAt=It.IsAny<DateTime>()},
                new GroupModel { Id = It.IsAny<Guid>(), Name =It.IsAny<String>(),Entitlements=It.IsAny<List<Guid>>(),CreatedAt=It.IsAny<DateTime>()}
            };

            _databaseservices.Setup(m => m.GetAllAsync()).Returns(Task.FromResult(expected));

            APIServices apiservices =
                new APIServices(
                _config.Object,
                _databaseservices.Object,
                _producerservices.Object,
                _mapper.Object
                );

            //Act
            var actual= await apiservices.GetAllRecords();

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetSingleRowById_AnyError_ThrowsException()
        {
            //Arrange
            _databaseservices.Setup(m => m.GetbyIdAsync(It.IsAny<Guid>())).ThrowsAsync(new Exception());

            APIServices apiservices =
                new APIServices(
                _config.Object,
                _databaseservices.Object,
                _producerservices.Object,
                _mapper.Object
                );


            //Act and Assert
            await Assert.ThrowsAsync<Exception>(() => apiservices.GetSingleRowById(It.IsAny<Guid>()));

        }

        [Fact]
        public async Task GetSingleRowById_NoException_ReturnsExpectedRecord()
        {
            //Arrange
            Guid mId = new Guid("3c8eabd5-f602-4f73-9b82-91032ad733e1");

            List<GroupModel> expected = new List<GroupModel>
            {
                new GroupModel { Id = mId, Name ="SG",
                    Entitlements=new List<Guid> { new Guid("baccd2c9-14fd-44b3-8859-fbac82b00386") },
                    CreatedAt=new DateTime(1,1,1,1,1,1)},
                new GroupModel { Id = It.IsAny<Guid>(), Name =It.IsAny<String>(),Entitlements=It.IsAny<List<Guid>>(),CreatedAt=It.IsAny<DateTime>()}
            };

            _databaseservices.Setup(m => m.GetbyIdAsync(mId)).Returns(Task.FromResult(expected.Find(x => x.Id==mId)));

            APIServices apiservices =
                new APIServices(
                _config.Object,
                _databaseservices.Object,
                _producerservices.Object,
                _mapper.Object
                );

            //Act
            var actual = await apiservices.GetSingleRowById(mId);

            //Assert
            Assert.Equal(expected.First(), actual);
        }

        [Fact]
        public async Task GetSingleRowById_NoException_ReturnsNullIfRecordDoesntExist()
        {
            Guid mId = new Guid("3c8eabd5-f602-4f73-9b82-91032ad733e1");

            List<GroupModel> expected = new List<GroupModel>
            {
                new GroupModel { Id = It.IsAny<Guid>(), Name =It.IsAny<String>(),Entitlements=It.IsAny<List<Guid>>(),CreatedAt=It.IsAny<DateTime>()},
                new GroupModel { Id = It.IsAny<Guid>(), Name =It.IsAny<String>(),Entitlements=It.IsAny<List<Guid>>(),CreatedAt=It.IsAny<DateTime>()}
            };

            _databaseservices.Setup(m => m.GetbyIdAsync(mId)).Returns(Task.FromResult(expected.Find(x => x.Id == It.IsAny<Guid>())));

            APIServices apiservices =
              new APIServices(
              _config.Object,
              _databaseservices.Object,
              _producerservices.Object,
              _mapper.Object
              );

            //Act
            var actual = await apiservices.GetSingleRowById(mId);

            //Assert
            Assert.Null(actual.Entitlements);
        }

    }
}
