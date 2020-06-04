using CarManagement.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using RabbitMQManagement;
using System;
using MaintenanceManagement.Models;
using MongoDB.Bson;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using Polly;

namespace CarManagement.Services
{
    public class CarManagementService
    {
        private IMongoCollection<CarModel> cars;
        RabbitMQMessagePublisher _messagePublisher;
        public CarManagementService(RabbitMQMessagePublisher messagePublisher)
        {
            var client = new MongoClient("mongodb://localhost:27017/");
            var database = client.GetDatabase("CarChampDb");
            cars = database.GetCollection<CarModel>("Cars");
            this._messagePublisher = messagePublisher;
        }

        public void changeToTestService()
        {
            var client = new MongoClient("mongodb://localhost:27017/");
            var database = client.GetDatabase("CarChampDb");
            cars = database.GetCollection<CarModel>("CarTest");
        }

        public List<CarModel> Get()
        {
            return cars.Find(car => true).ToList();
        }

        public CarModel Get(string carId)
        {
            var filter = Builders<CarModel>.Filter.Eq("_id", ObjectId.Parse(carId));
            return cars.Find(filter).FirstOrDefault();
        }

        public CarModel GetByLicencePlate(string carLicencePlate)
        {
            var filter = Builders<CarModel>.Filter.Eq("LicencePlate", carLicencePlate);
            return cars.Find(filter).FirstOrDefault();
        }

        public CarModel Create(CarModel car)
        {
            cars.InsertOne(car);
            return car;
        }

        public bool Sell(string carId)
        {
            //send message
            var filter = Builders<CarModel>.Filter.Eq("_id", ObjectId.Parse(carId));
            var result = cars.Find(filter).FirstOrDefault();
            if (result != null)
            {
                _messagePublisher.PublishMessageAsync("SellCar", result, "car.log");
                return true;
            }
            return false;
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
            var filter = Builders<CarModel>.Filter.Eq("_id", ObjectId.Parse(maintenance.Car));
            var update = Builders<CarModel>.Update.Push<MaintenanceModel>("MaintenanceHistory", maintenance);
            cars.UpdateOne(filter, update);
                    
            return cars.Find(filter).FirstOrDefault();
        }

        public Task TestLimits()
        {
            return Task.Run(() =>
                Policy
                    .Handle<Exception>()
                    .WaitAndRetry(9, r => TimeSpan.FromSeconds(5), (ex, ts) => { })
                    .Execute(() =>
                    {
                        int limit = 1000;
                        bool done = false;

                        for (int count = 0; count <= limit; count++)
                        {
                            var publishTask = _messagePublisher.PublishMessageAsync("LimitTest", "string test message", "maintenance.log");
                            publishTask.Wait();
                            publishTask.Dispose();
                        }
                        done = true;
                        Console.WriteLine("Switch, 1000 done");
                    }));
        }
    }
}
