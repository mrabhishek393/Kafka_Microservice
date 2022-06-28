using API;
using API.DTOs;
using AutoMapper;
using Database;
using Database.Models;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using Producer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Testing.API_Unit_Tests.APIServices_Tests
{
    public class APIServices_AddEntitlements
    {
        private Mock<IConfiguration>? _config = new();
        private Mock<IDatabaseServices>? _databaseservices = new();
        private Mock<IProducerService>? _producerservices = new();
        private Mock<IMapper>? _mapper = new();


        [Theory]
        [MemberData(nameof(TestData_Same))]
        public async Task AddEntitlements_NoException_SameData_ReturnsNull(
            Guid mId,
            List<GroupModel> from_db,
            PatchDTO request)
        {
            var oldvalue = from_db.Find(x => x.Id == mId);
            _databaseservices.Setup(m => m.GetbyIdAsync(mId)).Returns(Task.FromResult(oldvalue));

            APIServices apiservices =
              new APIServices(
              _config.Object,
              _databaseservices.Object,
              _producerservices.Object,
              _mapper.Object
              );

            var actual = await apiservices.AddEntitlements(request);

            Assert.Null(actual);
        }


        [Theory]
        [MemberData(nameof(TestData_Different))]
        public async Task AddEntitlements_NoException_DifferentData_CallsUpdateAndProduce(
            Guid mId,
            List<GroupModel> from_db,
            PatchDTO request,
            GroupModel newvalue)
        {
            var oldvalue = from_db.Find(x => x.Id == mId);
            _databaseservices.Setup(m => m.GetbyIdAsync(mId)).Returns(Task.FromResult(oldvalue));

            _mapper.Setup(m => m.Map<PatchDTO, GroupModel>(request)).Returns(newvalue);

            APIServices apiservices =
              new APIServices(
              _config.Object,
              _databaseservices.Object,
              _producerservices.Object,
              _mapper.Object
              );

            await apiservices.AddEntitlements(request);

            _databaseservices.Verify(c => c.GetbyIdAsync(mId), Times.Once);
            _mapper.Verify(c => c.Map<PatchDTO, GroupModel>(request), Times.Once);

            _databaseservices.Verify(c => c.UpdateAsync(oldvalue,newvalue),Times.Once);
            _producerservices.Verify( c=> c.WriteMessage(It.IsAny<string>(),JsonConvert.SerializeObject(newvalue)),Times.Once);

        }

        [Theory]
        [MemberData(nameof(TestData_Different))]
        public async Task UpdateAsync_AnyError_ThrowsException(
           Guid mId,
           List<GroupModel> from_db,
           PatchDTO request,
           GroupModel newvalue)
        {

            var oldvalue = from_db.Find(x => x.Id == mId);

            _databaseservices.Setup(m => m.GetbyIdAsync(mId)).Returns(Task.FromResult(oldvalue));
            _mapper.Setup(m => m.Map<PatchDTO, GroupModel>(request)).Returns(newvalue);

            _databaseservices.Setup(m => m.UpdateAsync(oldvalue, newvalue))
                .ThrowsAsync(new Exception());

            APIServices apiservices =
                new APIServices(
                _config.Object,
                _databaseservices.Object,
                _producerservices.Object,
                _mapper.Object
                );

            await Assert.ThrowsAsync<Exception>(() => apiservices.AddEntitlements(request));
        }
        //Testing Data
        public static IEnumerable<object[]> TestData_Same =>

           new List<object[]>
           {

                //1st set of parameters
                new object[] {

                    new Guid("3c8eabd5-f602-4f73-9b82-91032ad733e1"),

                    //Data from database
                    new List<GroupModel>
                    {
                        new GroupModel { Id = new Guid("3c8eabd5-f602-4f73-9b82-91032ad733e1"), Name ="SG",
                        Entitlements=new List<Guid> { new Guid("baccd2c9-14fd-44b3-8859-fbac82b00386") },
                        CreatedAt=new DateTime(1,1,1,1,1,1)},
                        new GroupModel { Id = It.IsAny<Guid>(), Name =It.IsAny<String>(),Entitlements=It.IsAny<List<Guid>>(),CreatedAt=It.IsAny<DateTime>()}
                    },

                    //request update data from API endpoint
                    new PatchDTO
                    {
                        Id = new Guid("3c8eabd5-f602-4f73-9b82-91032ad733e1"),
                        Entitlements = new List<Guid> { new Guid("baccd2c9-14fd-44b3-8859-fbac82b00386") },
                    }

                    
                }
           };

        public static IEnumerable<object[]> TestData_Different =>

           new List<object[]>
           {

                //1st set of parameters
                new object[] {

                    new Guid("3c8eabd5-f602-4f73-9b82-91032ad733e1"),

                    //Data from database
                    new List<GroupModel>
                    {
                        new GroupModel { Id = new Guid("3c8eabd5-f602-4f73-9b82-91032ad733e1"), Name ="SG",
                        Entitlements=new List<Guid> { new Guid("baccd2c9-14fd-44b3-8859-fbac82b00386") },
                        CreatedAt=new DateTime(1,1,1,1,1,1)},
                        new GroupModel { Id = It.IsAny<Guid>(), Name =It.IsAny<String>(),Entitlements=It.IsAny<List<Guid>>(),CreatedAt=It.IsAny<DateTime>()}
                    },

                    //request update data from API endpoint
                    new PatchDTO
                    {
                        Id = new Guid("3c8eabd5-f602-4f73-9b82-91032ad733e1"),
                        Entitlements = new List<Guid> { Guid.NewGuid() },
                    },

                    new GroupModel
                    {
                        Id=new Guid("3c8eabd5-f602-4f73-9b82-91032ad733e1"),
                        Name="SG",
                        Entitlements = new List<Guid> {  new Guid("d8b15e65-84c3-4aba-8cd5-b3880b646e15"), new Guid("baccd2c9-14fd-44b3-8859-fbac82b00386") }
                    }
                }
           };
    }
}
