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
using System.IO;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Html;

namespace CarManagement.Controllers
{
    [Route("api/[controller]")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
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


        [Route("stats")]
        [HttpGet]
        public ActionResult stats()
        {
            string Html = System.IO.File.ReadAllText("./Resources/results/sample.html");
            return Content(Html, "text/html");
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
