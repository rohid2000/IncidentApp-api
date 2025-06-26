using IncidentsAppApi.Controllers;
using IncidentsAppApi.Database;
using IncidentsAppApi.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IncidentsAppApiTest.IncidentController
{
    public class IncidentControllerTests
    {
        private readonly Mock<MyDbContext> _context;
        private readonly IncidentsAppApi.Controllers.IncidentController _controller;

        [Fact]
        public async Task GetAllIncidents_No_Users_Found_Test()
        {
            //Arrange
            

            //Act


            //Assert
        }
    }
}
