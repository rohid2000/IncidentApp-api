using IncidentsAppApi.Database;
using IncidentsAppApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IncidentsAppApiTest.UserController
{
    using IncidentsAppApi.Controllers;
    using IncidentsAppApi.Database;
    using IncidentsAppApi.Models;
    public class UserControllerTests
    {
        private readonly Mock<MyDbContext> _context;
        private readonly UserController _controller;
        private readonly Mock<DbSet<Incident>> _incidentSet;
        private readonly Mock<DbSet<User>> _userSet;
        public UserControllerTests()
        {
            var options = new DbContextOptions<MyDbContext>();
            _context = new Mock<MyDbContext>(options);
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

            _controller = new UserController(_context.Object);
        }

        [Fact]
        public async Task GetAllIncidents_ReturnsOk()
        {

        }
    }
}
