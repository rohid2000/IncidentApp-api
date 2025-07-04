using Microsoft.EntityFrameworkCore;
using Moq;

namespace IncidentsAppApiTest.Controllers.AuthenticationController
{
    using IncidentsAppApi;
    using IncidentsAppApi.Controllers;
    using IncidentsAppApi.Database;
    using IncidentsAppApi.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    public class AuthenticationControllerTests
    {
        private readonly Mock<IConfiguration> _config;
        private readonly Mock<MyDbContext> _context;
        private readonly AuthenticationController _controller;
        private readonly Mock<DbSet<User>> _userSet;
        public AuthenticationControllerTests()
        {
            _config = new Mock<IConfiguration>();
            var options = new DbContextOptions<MyDbContext>();
            _context = new Mock<MyDbContext>(options);
            _userSet = new Mock<DbSet<User>>();
            _controller = new AuthenticationController(_config.Object, _context.Object);

            Encryptor.Init();
        }

        [Fact]
        public void Login_UserNotFound_ReturnsNotFound()
        {
            //Arrange
            var users = new List<User>().AsQueryable();

            _userSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            _userSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            _userSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            _context.Setup(x => x.Users).Returns(_userSet.Object);

            var loginModel = new LoginModel { Username = "TestUser", Password = "a" };

            //Act
            var result = _controller.Login(loginModel);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Login_WrongPassword_ReturnsBadRequest()
        {
            //Arrange
            var user = new User
            {
                Username = "TestUser",
                Password = Encryptor.Encrypt("a")
            };

            var users = new List<User> { user }.AsQueryable();

            _userSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            _userSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            _userSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            _context.Setup(x => x.Users).Returns(_userSet.Object);

            var loginModel = new LoginModel { Username = "TestUser", Password = "b" };

            //Act
            var result = _controller.Login(loginModel);

            //Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void Login_ValidCredentials_ReturnsOk()
        {
            //Arrange
            var testPassword = "a";
            var encryptedPassword = Encryptor.Encrypt(testPassword);
            var testUser = new User
            {
                Id = 1,
                Username = "TestUser",
                Password = encryptedPassword,
                IsAdmin = false
            };

            var users = new List<User> { testUser }.AsQueryable();

            _userSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            _userSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            _userSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            _userSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            _context.Setup(x => x.Users).Returns(_userSet.Object);

            var loginModel = new LoginModel { Username = "TestUser", Password = testPassword };

            //Act
            var result = _controller.Login(loginModel);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            var response = okResult.Value;
            Assert.NotNull(response);

            var type = response.GetType();
            var id = type.GetProperty("id")?.GetValue(response, null);
            var username = type.GetProperty("username")?.GetValue(response, null);
            var isAdmin = type.GetProperty("isAdmin")?.GetValue(response, null);

            Assert.Equal(1, id);
            Assert.Equal("TestUser", username);
            Assert.False((bool)isAdmin);
        }
    }
}
