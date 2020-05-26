using CarManagement.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using RabbitMQManagement;
using System;
using MaintenanceManagement.Models;
using MongoDB.Bson;
using System.Linq;

namespace CarManagement.Services
{
    public class CarManagementService
    {
        private IMongoCollection<CarModel> cars;
        RabbitMQMessagePublisher _messagePublisher;
        public CarManagementService(IConfiguration config, RabbitMQMessagePublisher messagePublisher)
        {
            var client = new MongoClient(config.GetConnectionString("CarChampDb"));
            var database = client.GetDatabase("CarChampDb");
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

        public void SellRandomCar()
        {
            //send message
            var result = cars.Find(car => true).FirstOrDefault();
            _messagePublisher.PublishMessageAsync("SellCar", result, "car.log");
        }

        public void DeleteCar(string id)
        {
            var filter = Builders<CarModel>.Filter.Eq("_id", ObjectId.Parse(id));
            cars.DeleteOne(filter);
        }

        public CarModel addMaintenance(MaintenanceModel maintenance)
        {
            Console.WriteLine(maintenance);
            var filter = Builders<CarModel>.Filter.Eq("_id", ObjectId.Parse(maintenance.Id));
            var update = Builders<CarModel>.Update.Push<MaintenanceModel>("MaintenanceHistory", maintenance);
            cars.UpdateOne(filter, update);
                    
            return cars.Find(filter).FirstOrDefault();
        }
    }
}
