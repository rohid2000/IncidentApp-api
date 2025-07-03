using Microsoft.EntityFrameworkCore;
using Moq;

namespace IncidentsAppApiTest.IncidentController
{
    using IncidentsAppApi.Body_s;
    using IncidentsAppApi.Controllers;
    using IncidentsAppApi.Database;
    using IncidentsAppApi.Models;
    using IncidentsAppApiTest.Helpers;
    using IncidentsAppApiTest.Providers;
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

            var users = new List<User>
            {
                new User { Id = 1, Username = "Test", IsAdmin = false }
            }.AsQueryable();

            _userSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            _userSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            _userSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            _userSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

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
        public async Task GetAllIncidents_ReturnsOk()
        {
            //Arrange
            var adminUser = new User { Id = 1, Username = "AdminUser", IsAdmin = true };
            var incident = new Incident
            {
                Description = "Test Incident",
                UserId = 1,
                ReportDate = DateTime.Now,
                Location = "Test locatie"
            };

            var updatedUserSet = new Mock<DbSet<User>>();
            var updatedIncidentSet = new Mock<DbSet<Incident>>();

            var users = new List<User> { adminUser }.AsQueryable();
            updatedUserSet.As<IAsyncEnumerable<User>>()
                .Setup(m => m.GetAsyncEnumerator(default))
                .Returns(new TestAsyncEnumeratorHelper<User>(users.GetEnumerator()));
            updatedUserSet.As<IQueryable<User>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<User>(users.Provider));
            updatedUserSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            updatedUserSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            updatedUserSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var incidents = new List<Incident> { incident }.AsQueryable();
            updatedIncidentSet.As<IAsyncEnumerable<Incident>>()
                .Setup(m => m.GetAsyncEnumerator(default))
                .Returns(new TestAsyncEnumeratorHelper<Incident>(incidents.GetEnumerator()));
            updatedIncidentSet.As<IQueryable<Incident>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Incident>(incidents.Provider));
            updatedIncidentSet.As<IQueryable<Incident>>().Setup(m => m.Expression).Returns(incidents.Expression);
            updatedIncidentSet.As<IQueryable<Incident>>().Setup(m => m.ElementType).Returns(incidents.ElementType);
            updatedIncidentSet.As<IQueryable<Incident>>().Setup(m => m.GetEnumerator()).Returns(incidents.GetEnumerator());

            _context.Setup(x => x.Users).Returns(updatedUserSet.Object);
            _context.Setup(x => x.Incidents).Returns(updatedIncidentSet.Object);

            var userData = new AllIIncident { username = "AdminUser" };

            //Act
            var result = await _controller.GetAllIncidents(userData);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<IncidentReturn>>(okResult.Value);

            Assert.Single(returnValue);
            Assert.Equal("Test Incident", returnValue[0].Description);
        }

        [Fact]
        public async Task GetIncidentsByUserId_No_User_Found_ReturnsNotFound()
        {
            //Arrange
            var userId = 0;

            _context.Setup(x => x.Users.FindAsync(userId))
                    .ReturnsAsync((User)null);

            //Act
            var result = await _controller.GetIncidentsByUserid(userId);
            
            //Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetIncidentsByUserId_With_ReturnsOk()
        {
            // Arrange
            int userId = 1;
            var user = new User { Id = userId, Username = "TestUser" };

            var incidents = new List<Incident>
            {
                new Incident { Id = 1, UserId = userId, Description = "Test Incident 1" },
                new Incident { Id = 2, UserId = userId, Description = "Test Incident 2" }
            }.AsQueryable();

            _context.Setup(x => x.Users.FindAsync(userId))
                    .ReturnsAsync(user);

            var mockIncidents = new Mock<DbSet<Incident>>();
            mockIncidents.As<IQueryable<Incident>>().Setup(m => m.Provider).Returns(incidents.Provider);
            mockIncidents.As<IQueryable<Incident>>().Setup(m => m.Expression).Returns(incidents.Expression);
            mockIncidents.As<IQueryable<Incident>>().Setup(m => m.ElementType).Returns(incidents.ElementType);
            mockIncidents.As<IQueryable<Incident>>().Setup(m => m.GetEnumerator()).Returns(incidents.GetEnumerator());

            _context.Setup(x => x.Incidents).Returns(mockIncidents.Object);

            // Act
            var result = await _controller.GetIncidentsByUserid(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedIncidents = Assert.IsType<List<Incident>>(okResult.Value);
            Assert.Equal(2, returnedIncidents.Count);
        }

        [Fact]
        public async Task AddIncident_Incident_Null_ReturnsBadRequest()
        {
            //Arrange
            AddIncidentBody incident = null;

            //Act
            var result = await _controller.AddIncident(incident);

            //Assert
            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public async Task AddIncident_With_ReturnsOk()
        {
            // Arrange
            var incident = new AddIncidentBody
            {
                Description = "Test incident",
                UserId = 1,
                Location = "Test location"
            };

            var mockIncidents = new Mock<DbSet<Incident>>();
            _context.Setup(x => x.Incidents).Returns(mockIncidents.Object);
            _context.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _controller.AddIncident(incident);

            // Assert
            Assert.IsType<OkResult>(result.Result);

            mockIncidents.Verify(x => x.Add(It.Is<Incident>(i =>
                i.Description == incident.Description &&
                i.UserId == incident.UserId
            )), Times.Once);

            _context.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpateIncidentStatusAndPriority_Incident_Null_ReturnBadRequest()
        {
            // Arrange
            _context.Setup(x => x.Incidents.FindAsync(It.IsAny<int>()))
                    .ReturnsAsync((Incident)null);

            var controller = new IncidentController(_context.Object);
            var patchData = new PatchIncidentBody { Status = "Gemeld" };

            // Act
            var result = await controller.UpateIncidentStatusAndPriority(1, patchData);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task UpateIncidentStatusAndPriority_With_ReturnsOk()
        {
            // Arrange
            var incident = new Incident { Id = 1, Status = "Gemeld" };

            _context.Setup(x => x.Incidents.FindAsync(1))
                    .ReturnsAsync(incident);

            _context.Setup(x => x.SaveChangesAsync(default))
                    .ReturnsAsync(1);

            var controller = new IncidentController(_context.Object);
            var patchData = new PatchIncidentBody { Status = "In Behandeling", Priority = "High" };

            // Act
            var result = await controller.UpateIncidentStatusAndPriority(1, patchData);

            // Assert
            Assert.IsType<OkResult>(result);
            Assert.Equal("In Behandeling", incident.Status);
            Assert.Equal("High", incident.Priority); 
            _context.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteIncidentByid_No_Incident_Found_ReturnsNotFound()
        {
            // Arrange
            _context.Setup(x => x.Incidents.FindAsync(It.IsAny<int>()))
                       .ReturnsAsync((Incident)null);

            var controller = new IncidentController(_context.Object);

            // Act
            var result = await controller.DeleteIncidentByid(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _context.Verify(x => x.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task DeleteIncidentByid_ReturnsOk()
        {
            // Arrange
            var incident = new Incident { Id = 1 };

            _context.Setup(x => x.Incidents.FindAsync(1))
                      .ReturnsAsync(incident);

            _context.Setup(x => x.SaveChangesAsync(default))
                      .ReturnsAsync(1);

            // Setup Remove to track deletion
            _context.Setup(x => x.Incidents.Remove(incident))
                      .Verifiable();

            var controller = new IncidentController(_context.Object);

            // Act
            var result = await controller.DeleteIncidentByid(1);

            // Assert
            Assert.IsType<OkResult>(result);
            _context.Verify(x => x.Incidents.Remove(incident), Times.Once); 
            _context.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }
    }
}
