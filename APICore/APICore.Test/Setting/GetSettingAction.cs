using APICore.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Wangkanai.Detection.Services;
using Xunit;
using Microsoft.EntityFrameworkCore;
using APICore.Data;
using APICore.Data.Entities.Enums;
using APICore.Data.Entities;
using System.Threading.Tasks;
using Moq.Inject;
using System.Linq;

namespace APICore.Tests.Setting
{
    public class GetSettingAction
    {
        private DbContextOptions<CoreDbContext> ContextOptions { get; }

        public GetSettingAction()
        {
            ContextOptions = new DbContextOptionsBuilder<CoreDbContext>()
                                       .UseInMemoryDatabase("TestStatusDatabase")
                                       .Options;
            SeedAsync().Wait();
        }
        private async Task SeedAsync()
        {
            await using var context = new CoreDbContext(ContextOptions);

            if (!context.Users.Any())
            {
                await context.Users.AddAsync(new User
                {
                    Email = "carlos@itguy.com",
                    FullName = "Carlos Delgado",
                    Gender = 0,
                    Phone = "+53 12345678",
                    Password = @"gM3vIavHvte3fimrk2uVIIoAB//f2TmRuTy4IWwNWp0=",
                    Status = StatusEnum.ACTIVE
                });

                await context.SaveChangesAsync();
            }
            if (!context.Setting.Any())
            {
                await context.Setting.AddAsync(new Data.Entities.Setting
                {
                    Key = "FakeKeyOne",
                    Value = "FakeValue"
                });

                await context.SaveChangesAsync();
            }
        }

        [Fact(DisplayName = "Get A Setting That Exist Should Return Ok Status Code (200)")]
        public void GetASettingThatExistShouldReturnOk()
        {
            //ARRANGE
            var fakeSetting = "FakeKeyOne";

            var settingController = Injector.Create<SettingController>();

            // ACT
            var taskResult = (ObjectResult)settingController.GetSetting(fakeSetting).Result;

            // ASSERT
            Assert.Equal(200, taskResult.StatusCode);
        }
    }
}
