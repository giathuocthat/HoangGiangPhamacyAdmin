using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Service.Interfaces;
using ThuocGiaThatAdmin.Service.Services;

namespace Test.GiaThuocThat
{
    [TestFixture]
    public class BrandServiceUnitTest
    {
        private  IBrandService _brandService;
        private  Mock<IBrandRepository> _mockBrandRepository;

        [OneTimeSetUp]
        public void SetUp()
        {
            _mockBrandRepository = new Mock<IBrandRepository>(MockBehavior.Strict);
            _brandService = new BrandService(_mockBrandRepository.Object);
        }

        [Test]
        public void GetBrand_ShouldReturnBrand_WhenBrandExists()
        {
            // Arrange
            var brandId = 1;
            var expectedBrand = new Brand
            {
                Id = brandId,
                Name = "Test Brand",
                Slug = "test-brand"
            };

            _mockBrandRepository
                .Setup(repo => repo.GetByIdAsync(brandId))
                .ReturnsAsync(expectedBrand);
            // Act
            var result = _brandService.GetBrandByIdAsync(brandId).Result;
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedBrand.Id, result.Id);
            Assert.AreEqual(expectedBrand.Name, result.Name);
            Assert.AreEqual(expectedBrand.Slug, result.Slug);
            _mockBrandRepository.Verify(repo => repo.GetByIdAsync(brandId), Times.Once);
        }

        [Test]
        public void GetBrand_ShouldReturnNull_WhenBrandDoesNotExist()
        {
            // Arrange
            var brandId = 999; // Non-existing brand ID
            _mockBrandRepository
                .Setup(repo => repo.GetByIdAsync(brandId))
                .ReturnsAsync((Brand?)null);
            // Act
            var result = _brandService.GetBrandByIdAsync(brandId).Result;
            // Assert
            Assert.IsNull(result);
            _mockBrandRepository.Verify(repo => repo.GetByIdAsync(brandId), Times.Once);
        }

        [Test]
        public void GetBrand_ShouldThrowArgumentException_WhenBrandIdIsInvalid()
        {
            // Arrange
            var invalidBrandId = -1;
            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _brandService.GetBrandByIdAsync(invalidBrandId));

            Assert.That(ex.Message, Does.Contain("Brand ID must be greater than 0"));

            _mockBrandRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }
    }
}
