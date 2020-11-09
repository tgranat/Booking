﻿using Booking.Controllers;
using Booking.Models.Entities;
using Booking.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Test.Controllers
{
    [TestClass]
    class GymClassesTests
    {
        private Mock<IGymClassRepository> repository;
        private GymClassesController controller;

        [TestInitialize]
        public void SetUp()
        {
            // Mock GymClassRepository
            repository = new Mock<IGymClassRepository>();

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(u => u.GymClassRepository).Returns(repository.Object);
            // See Dimitris code regarding test when automapper is used

            var mockUserStore = new Mock<IUserStore<ApplicationUser>>();
            var mockUserManager =
                new Mock<UserManager<ApplicationUser>>(mockUserStore, null, null, null, null, null, null, null, null);

            controller = new GymClassesController(mockUnitOfWork.Object, mockUserManager.Object);
        }

        private List<GymClass> GetGymClassList()
        {
            return new List<GymClass>
            {
                new GymClass
                {
                    Id = 1,
                    Name = "Spinning",
                    Description = "Spinning 1",
                    StartDate = DateTime.Now.AddDays(3),
                    Duration = new TimeSpan(0,60,0)
                },
                new GymClass
                {
                    Id = 2,
                    Name = "Box",
                    Description = "SpinBoxning 1",
                    StartDate = DateTime.Now.AddDays(3),
                    Duration = new TimeSpan(0,60,0)
                }
            };
        }
    }
}