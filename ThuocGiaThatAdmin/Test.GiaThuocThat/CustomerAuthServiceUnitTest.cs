using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Service.Interfaces;
using ThuocGiaThatAdmin.Service.Services;

namespace Test.GiaThuocThat
{

    [TestFixture]
    public class CustomerAuthServiceUnitTest
    {
        private ICustomerAuthService _customerAuthService;
        private IConfiguration _configuration;
        [SetUp]
        public void SetUp()
        {
            var mockContext = new Mock<TrueMecContext>(MockBehavior.Default, new DbContextOptions<TrueMecContext>());

            var mockDbSet = new Mock<DbSet<Customer>>();
            mockContext.Setup(c => c.Customers).Returns(mockDbSet.Object);

            var mockConfiguration = new Mock<IConfiguration>();

            var inMemorySettings = new Dictionary<string, string> {
                    {"Jwt", "InnerValue"},
                    {"Jwt:Key", "mock key;"}
                };

            _configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(inMemorySettings)
                    .Build();

            _customerAuthService = new CustomerAuthService(mockContext.Object, _configuration);
        }

        [Test]
        public void GenerateJwtToken_ShouldThrowException_WhenJwtKeyIsMissing()
        {
            // Arrange
            var customer = new Customer
            {
                Id = 1,
                FullName = "Test User",
            };

            Assert.Throws<InvalidOperationException>(() => _customerAuthService.GenerateJwtToken(customer));
        }
    }

}
