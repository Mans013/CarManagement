using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RabbitMQManagement;
using CarManagement.Services;
using CarManagement.Models;
using Newtonsoft.Json;
using Polly;

namespace CarManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        RabbitMQMessagePublisher _messagePublisher;
        private readonly CarManagementService _carService;


        public CarController(RabbitMQMessagePublisher messagePublisher, CarManagementService carService)
        {
            _messagePublisher = messagePublisher;
            _carService = carService;
        }

        [HttpGet]
        public ActionResult<List<CarModel>> Get()
        {
            return _carService.Get();
        }

        [Route("test")]
        [HttpGet]
        public void Test()
        {
            Task.Run(() =>
                Policy
                    .Handle<Exception>()
                    .WaitAndRetry(9, r => TimeSpan.FromSeconds(5), (ex, ts) => { })
                    .Execute(() =>
                    {
                        while (true)
                        {
                            var TestTask = _carService.TestLimits();
                            TestTask.Wait();
                            TestTask.Dispose();
                        }
                    }));
            //return Ok("Test Complete");
        }

        [Route("{carId}")]
        [HttpGet]
        public ActionResult<CarModel> Get(string carId)
        {
            return _carService.Get(carId);
        }

        [HttpPost]
        public ActionResult<CarModel> Post([FromBody] CarModel car)
        {
            var newCar = _carService.Create(car);
            return Ok(JsonConvert.SerializeObject(newCar));
        }

        [Route("{carId}/sell")]
        [HttpPost]
        public ActionResult<string> SellCar(string carId)
        {
            if (_carService.Sell(carId))
            {
                return "Car sold";
            }
            else
            {
                return "Car can not be sold";
            }
            
        }

        [Route("sell")]
        [HttpGet]
        public void SellRandomCar()
        {
            _carService.SellRandomCar();
        }

        

    }
}
