using CarManagement.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using RabbitMQManagement;
using System;

namespace CarManagement.Services
{
    public class CarManagementService
    {
        private IMongoCollection<CarModel> cars;
        RabbitMQMessagePublisher _messagePublisher;
        public CarManagementService(IConfiguration config, RabbitMQMessagePublisher messagePublisher)
        {
            var client = new MongoClient(config.GetConnectionString("CarChampDb"));
            var database = client.GetDatabase("CarChampDB");
            cars = database.GetCollection<CarModel>("Cars");
            this._messagePublisher = messagePublisher;
        }

        public List<CarModel> Get()
        {
            return cars.Find(car => true).ToList();
        }

        public CarModel Create(CarModel car)
        {
            cars.InsertOne(car);
            return car;
        }
    }
}
