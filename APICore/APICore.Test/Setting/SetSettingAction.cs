using APICore.API.Controllers;
using APICore.Common.DTO.Request;
using Microsoft.AspNetCore.Mvc;
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
    public class SetSettingAction
    {
        private DbContextOptions<CoreDbContext> ContextOptions { get; }

        public SetSettingAction()
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
        }

        [Fact(DisplayName = "Set A Setting Should Return Ok Status Code (200)")]
        public void SetASettingShouldReturnOk()
        {
            // ARRANGE
            var fakeSetting = new SettingRequest
            {
                Key = "FakeKeyTwo",
                Value = "FakeValue"
            };

            var settingController = Injector.Create<SettingController>();

            // ACT
            var taskResult = (ObjectResult)settingController.SetSetting(fakeSetting).Result;

            // ASSERT
            Assert.Equal(200, taskResult.StatusCode);
        }

    }
}
