using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RabbitMQManagement;
using CarManagement.Services;
using CarManagement.Models;
using Newtonsoft.Json;

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

        [HttpPost]
        public ActionResult<CarModel> Post([FromBody] CarModel car)
        {
            var newCar = _carService.Create(car);
            return Ok(JsonConvert.SerializeObject(newCar));
        }

        [Route("sell")]
        [HttpGet]
        public void SellRandomCar()
        {
            _carService.SellRandomCar();
        }

    }
}
