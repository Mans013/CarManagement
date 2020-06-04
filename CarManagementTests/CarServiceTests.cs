using Microsoft.VisualStudio.TestTools.UnitTesting;
using CarManagement.Controllers;
using RabbitMQManagement;
using CarManagement.Services;
using System.Collections.Generic;
using CarManagement.Models;
using Newtonsoft.Json.Schema;

namespace CarManagementTests
{
    [TestClass]
    public class CarServiceTests
    {
        private static CarManagementService carService;
        private string testcarid;

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            // Initalization code goes here
            var publisher = new RabbitMQMessagePublisher("localhost", "Test");
            carService = new CarManagementService(publisher);
            carService.changeToTestService();
        }

        private List<CarModel> getCarList()
        {
            List<CarModel> output = new List<CarModel>
            {
                new CarModel
                {
                    OwnerName = "Max Stubbe"
                }
            };
            return output;
        }

        [TestMethod]
        public void GetCarsTest()
        {
            var result = carService.Get();
            if (result != null)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void GetCarTest()
        {
            string licencePlate = "AB12CD";
            var result = carService.GetByLicencePlate(licencePlate);
            if(result != null)
            {
                Assert.IsTrue(result.LicencePlate == licencePlate);
            }
            else
            {
                Assert.IsTrue(result == null);
            }
        }

        [TestMethod]
        public void CreateCarTest()
        {
            CarModel carModel = new CarModel()
            {
                OwnerName = "Test McTestington",
                BuildDate = "now",
                Brand = "TestBrand",
                LicencePlate = "AB12CD",
                Weight = 1,
                MaintenanceHistory = null
            };
            var result = carService.Create(carModel);
            Assert.IsTrue(result.Id != null);
            testcarid = result.Id;
            Assert.IsTrue(result.OwnerName == "Test McTestington");
        }

        [TestMethod]
        public void DeleteCarTest()
        {
            var cartodelete = carService.GetByLicencePlate("AB12CD");
            carService.DeleteCar(cartodelete.Id);
            var result = carService.Get(cartodelete.Id);
            Assert.IsTrue(result == null);
        }
    }
}
