using Microsoft.EntityFrameworkCore;
using Moq;

namespace IncidentsAppApiTest.IncidentController
{
    using IncidentsAppApi.Controllers;
    using IncidentsAppApi.Database;
    using IncidentsAppApi.Models;
    using Microsoft.AspNetCore.Mvc;

    public class IncidentControllerTests
    {
        private readonly Mock<MyDbContext> _context;
        private readonly IncidentController _controller;
        private readonly Mock<DbSet<Incident>> _incidentSet;
        private readonly Mock<DbSet<User>> _userSet;

        public IncidentControllerTests()
        {
            var options = new DbContextOptions<MyDbContext>();
            _context = new Mock<MyDbContext>(options);
            _incidentSet = new Mock<DbSet<Incident>>();
            _userSet = new Mock<DbSet<User>>();

            // Setup the Users DbSet
            var users = new List<User>
            {
                new User { Id = 1, Username = "Test", IsAdmin = false }
            }.AsQueryable();

            _userSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            _userSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            _userSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            _userSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            // Mock the expression-bodied property
            _context.SetupGet(x => x.Users).Returns(_userSet.Object);
            _context.SetupGet(x => x.Incidents).Returns(_incidentSet.Object);

            _controller = new IncidentController(_context.Object);
        }

        [Fact]
        public async Task GetAllIncidents_No_Users_Found_ReturnsBadRequest()
        {
            //Arrange
            var userData = new AllIIncident { username = "" };

            //Act
            var result = await _controller.GetAllIncidents(userData);

            //Assert
            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public async Task GetAllIncidents_No_Users_Found_As_Admin_ReturnsBadRequest()
        {
            // Arrange
            var userData = new AllIIncident { username = "Test" };

            // Act
            var result = await _controller.GetAllIncidents(userData);

            // Assert
            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public async Task GetAllIncidents_With_ResultOk()
        {
            //Arrange
            var userData = new AllIIncident { username = "Test" };

            //Act
            var result = await _controller.GetAllIncidents(userData);

            //Assert
            Assert.IsType<OkResult>(result.Result);
        }
    }
}
