using APICore.API.Controllers;
using APICore.Common.DTO.Request;
using APICore.Services;
using APICore.Services.Impls;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Moq;
using Wangkanai.Detection.Services;
using Xunit;
using Microsoft.EntityFrameworkCore;
using APICore.Data;
using APICore.Data.Entities.Enums;
using APICore.Data.Entities;
using System.Threading.Tasks;
using APICore.Data.UoW;
using Wangkanai.Detection.Models;

namespace APICore.Tests.Setting
{
    public class GetSettingAction
    {
        private DbContextOptions<CoreDbContext> ContextOptions { get; }
        private readonly Mock<IConfiguration> Config;
        private readonly Mock<IDetectionService> DetectionService;

        public GetSettingAction()
        {
            ContextOptions = new DbContextOptionsBuilder<CoreDbContext>()
                                       .UseInMemoryDatabase("TestStatusDatabase")
                                       .Options;
            Config = new Mock<IConfiguration>();
            Config.Setup(setup => setup.GetSection("BearerTokens")["Issuer"]).Returns(@"http://apicore.com");
            Config.Setup(setup => setup.GetSection("BearerTokens")["Key"]).Returns(@"GUID-A54a-SS15-SwEr-opo4-56YH");
            Config.Setup(setup => setup.GetSection("BearerTokens")["Audience"]).Returns(@"Any");
            Config.Setup(setup => setup.GetSection("BearerTokens")["AccessTokenExpirationHours"]).Returns("7");
            Config.Setup(setup => setup.GetSection("BearerTokens")["RefreshTokenExpirationHours"]).Returns("60");

            DetectionService = new Mock<IDetectionService>();
            DetectionService.Setup(setup => setup.UserAgent).Returns(new UserAgent(@"Mozilla / 5.0(Windows NT 10.0; Win64; x64; rv: 86.0) Gecko / 20100101 Firefox / 86.0"));

            SeedAsync().Wait();
        }
        private async Task SeedAsync()
        {
            await using var context = new CoreDbContext(ContextOptions);

            if (await context.Users.AnyAsync() == false)
            {
                await context.Users.AddAsync(new User
                {
                    Id = 1,
                    Email = "carlos@itguy.com",
                    FullName = "Carlos Delgado",
                    Gender = 0,
                    Phone = "+53 12345678",
                    Password = @"gM3vIavHvte3fimrk2uVIIoAB//f2TmRuTy4IWwNWp0=",
                    Status = StatusEnum.ACTIVE
                });

                await context.SaveChangesAsync();
            }
            if (await context.Setting.AnyAsync() == false)
            {
                await context.Setting.AddAsync(new Data.Entities.Setting
                {
                    Id = 1,
                    Key = "FakeKey",
                    Value = "FakeValue"
                });

                await context.SaveChangesAsync();
            }
        }

        [Fact(DisplayName = "Get A Setting That Exist Should Return Ok Status Code (200)")]
        public void GetASettingThatExistShouldReturnOk()
        {
            //ARRANGE
            var fakeSetting = "FakeKey";

            var httpContext = new DefaultHttpContext();
            using var context = new CoreDbContext(ContextOptions);

            var settingService = new SettingService(new UnitOfWork(context), new Mock<IStringLocalizer<IAccountService>>().Object);
            var settingController = new SettingController(settingService, new Mock<AutoMapper.IMapper>().Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext
                }
            };

            // ACT
            var taskResult = (ObjectResult)settingController.GetSetting(fakeSetting).Result;

            // ASSERT
            Assert.Equal(200, taskResult.StatusCode);
        }
    }
}
