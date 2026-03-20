using MedShop.Core.Contracts;
using MedShop.Core.Models.User;
using Moq;

namespace MedShop.Tests.Mocks
{
    public class UserStatisticsServiceMock
    {
        public static IUserStatisticsService Instance
        {
            get
            {
                var userStatisticsServiceMock = new Mock<IUserStatisticsService>();

                userStatisticsServiceMock.Setup(s => s.UsersInfo())
                    .ReturnsAsync(new StatisticsViewModel()
                    {
                        TotalProducts = 5,
                        ActiveProducts = 3,
                        TotalUsers = 5,
                        ActiveUsers = 2
                    });
                return userStatisticsServiceMock.Object;
            }
        }
    }
}
