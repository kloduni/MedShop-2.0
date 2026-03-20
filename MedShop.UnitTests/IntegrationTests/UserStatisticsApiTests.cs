using MedShop.Tests.Mocks;
using MedShop.WebApi.Controllers;
using NuGet.Protocol;

namespace MedShop.Tests.IntegrationTests
{
    [TestFixture]
    public class UserStatisticsApiTests
    {
        private UsersStatisticsApiController statisticsController;

        [SetUp]
        public void SetUp()
        {
            statisticsController = new UsersStatisticsApiController(UserStatisticsServiceMock.Instance);
        }

        [Test]
        public async Task UsersInfo_ShouldReturnCorrectValues()
        {
            var result = await statisticsController.GetUsersStatistics();

            var value = result.ToJson();

            Assert.IsNotNull(result);

            Assert.That(value.Contains("TotalProducts\":5"), Is.True);
            Assert.That(value.Contains("TotalUsers\":5"), Is.True);     
            Assert.That(value.Contains("ActiveProducts\":3"), Is.True);
            Assert.That(value.Contains("ActiveUsers\":2"), Is.True);
        }
    }
}
