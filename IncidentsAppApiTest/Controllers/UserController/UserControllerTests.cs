using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace IncidentsAppApiTest.Controllers.UserController
{
    using IncidentsAppApi;
    using IncidentsAppApi.Controllers;
    using IncidentsAppApi.Database;
    using IncidentsAppApi.Models;
    using IncidentsAppApiTest.Helpers;

    public class UserControllerTests
    {
        private readonly Mock<MyDbContext> _context;
        private readonly UserController _controller;
        private readonly Mock<DbSet<User>> _userSet;
        public UserControllerTests()
        {
            var options = new DbContextOptions<MyDbContext>();
            _context = new Mock<MyDbContext>(options);
            _userSet = new Mock<DbSet<User>>();

            var users = new List<User>
            {
                new User { Id = 1, Username = "TestUser", IsAdmin = false }
            }.AsQueryable();

            _userSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            _userSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            _userSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            _userSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            _context.SetupGet(x => x.Users).Returns(_userSet.Object);

            _controller = new UserController(_context.Object);
        }

        [Fact]
        public async Task GetAllIncidents_ReturnsOk()
        {
            //Arrange
            var testUsers = new List<User>
            {
                new User { Id = 1, Username = "TestUser1", IsAdmin = false },
                new User { Id = 2, Username = "TestUser2", IsAdmin = true }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<User>>();

            mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(testUsers.Provider);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(testUsers.Expression);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(testUsers.ElementType);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(testUsers.GetEnumerator());

            mockDbSet.As<IAsyncEnumerable<User>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumeratorHelper<User>(testUsers.GetEnumerator()));

            _context.Setup(c => c.Users).Returns(mockDbSet.Object);

            //Act
            var result = await _controller.GetAllUsers();

            //Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            var returnedUsers = Assert.IsType<List<User>>(okResult.Value);
            Assert.Equal(2, returnedUsers.Count);
        }

        [Fact]
        public async Task GetUserById_No_Users_Found_ReturnsNotFound()
        {
            //Arrange
            var userId = 0;

            _context.Setup(x => x.Users.FindAsync(userId))
                    .ReturnsAsync((User)null);

            //Act
            var result = await _controller.GetUserById(userId);

            //Assesrt
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetUserById_With_ReturnsOk()
        {
            //Arrange
            var testUser = new User 
            { 
                Id = 1, 
                Username = "TestUser", 
                IsAdmin = false 
            };

            _context.Setup(x => x.Users.FindAsync(1))
                   .ReturnsAsync(testUser);

            //Act
            var result = await _controller.GetUserById(1);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedUser = Assert.IsType<User>(okResult.Value);
            Assert.Equal(testUser.Id, returnedUser.Id);
            Assert.Equal(testUser.Username, returnedUser.Username);
        }

        [Fact]
        public async Task AddUser_ReturnsCreatedAtAction()
        {
            //Arrange
            Encryptor.Init();

            var testPassword = "a";
            var encryptedPassword = Encryptor.Encrypt(testPassword);

            var newUser = new LoginModel
            {
                Username = "TestUser",
                Password = testPassword
            };
            
            _context.Setup(x => x.Users).Returns(_userSet.Object);
            _context.Setup(x => x.SaveChangesAsync(default))
                   .ReturnsAsync(1)
                   .Verifiable();

            //Act
            var result = await _controller.AddUser(newUser);

            //Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetUserById), createdAtActionResult.ActionName);

            var createdUser = Assert.IsType<User>(createdAtActionResult.Value);
            Assert.Equal(newUser.Username, createdUser.Username);

            Assert.NotNull(createdUser.Password);
            Assert.NotEqual(testPassword, createdUser.Password.ToString());
            Assert.False(createdUser.IsAdmin);

            _userSet.Verify(x => x.Add(It.Is<User>(u =>
                u.Username == newUser.Username &&
                !u.IsAdmin
            )), Times.Once);

            _context.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteUerById_No_User_Found_ReturnsNotFound()
        {
            //Arrange
            _context.Setup(x => x.Users.FindAsync(It.IsAny<int>()))
                    .ReturnsAsync((User)null);

            //Act
            var result = await _controller.DeleteUserById(1);

            //Assert
            Assert.IsType<NotFoundResult>(result);
            _context.Verify(x => x.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task DeleteUerById_Returns_NoContent()
        {
            //Arrange
            var user = new User { Id = 1};

            _context.Setup(x => x.Users.FindAsync(1))
                      .ReturnsAsync(user);

            _context.Setup(x => x.SaveChangesAsync(default))
                      .ReturnsAsync(1);

            _context.Setup(x => x.Users.Remove(user))
                      .Verifiable();

            //Act
            var result = await _controller.DeleteUserById(1);

            //Assert
            Assert.IsType<NoContentResult>(result);
            _context.Verify(x => x.Users.Remove(user), Times.Once);
            _context.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }
    }
}
