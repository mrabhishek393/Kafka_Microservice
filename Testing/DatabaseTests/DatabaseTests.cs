using Database;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Testing
{
    public class DatabaseTests
    {
        private Mock<DbSet<ModelProp>>? mocked_dbset= new Mock<DbSet<ModelProp>>();
        private Mock<TestdbContext>? mocked_context = new Mock<TestdbContext>();


        [Fact]
        public async Task Test_GetByIdAsync()
        {

            //Arrange
            var expected = new ModelProp { Id = 2, Name = "Apple", Version = 14 };
            mocked_dbset.Setup(m => m.FindAsync(2).Result).Returns(expected);

            mocked_context.Setup(c => c.GroupInfos).Returns(mocked_dbset.Object);

            var service = new DatabaseServices(mocked_context.Object);

            //Act
            var actual = await service.GetbyIdAsync(2);

            //Assert
            Assert.IsType<ModelProp>(actual);
            Assert.Equal("Apple", actual.Name);
            Assert.Equal(14, actual.Version);
        }

        [Fact]
        public async Task Test_InsertAsync()
        {
         
            //Arrange
            mocked_context.Setup(c => c.GroupInfos).Returns(mocked_dbset.Object);

            mocked_dbset.Setup(c => c.AddAsync(It.IsAny<ModelProp>(), It.IsAny<CancellationToken>()))
             .Returns((ModelProp model, CancellationToken token) => new ValueTask<EntityEntry<ModelProp>>()).Verifiable();
           
            mocked_context.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(() => Task.Run(() => { return 1; })).Verifiable();
            var service = new DatabaseServices(mocked_context.Object);

            //Act
            await service.InsertAysnc(It.IsAny<ModelProp>());


            //Assert
            mocked_dbset.Verify(c => c.AddAsync(It.IsAny<ModelProp>(), It.IsAny<CancellationToken>()),Times.Once);
            mocked_context.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
           

        }

        //[Fact]
        //public async Task PostValidModel_NewData_Database()
        //{

        //}
    }
}