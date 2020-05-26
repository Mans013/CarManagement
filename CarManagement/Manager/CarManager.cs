using Microsoft.Extensions.Hosting;
using CarManagement.Models;
using RabbitMQManagement;
using System;
using System.Threading;
using System.Threading.Tasks;
using CarManagement.Services;
using MaintenanceManagement.Models;
using AdvertisementManagement.Models;

namespace CarManagement.Manager
{
    public class CarManager : IHostedService, IMessageHandlerCallback
    {
        private RabbitMQMessageHandler _messageHandler;
        private CarManagementService _carService;

        public CarManager(RabbitMQMessageHandler messageHandler, CarManagementService carService)
        {
            _messageHandler = messageHandler;
            _carService = carService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _messageHandler.Start(this);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _messageHandler.Stop();
            return Task.CompletedTask;
        }

        public async Task<bool> HandleMessageAsync(string messageType, string message)
        {
            try
            {
                switch (messageType)
                {
                    case "CarSold":
                        _carService.DeleteCar(MessageSerializer.Deserialize<AdvertisementModel>(message).CarID);
                        break;
                    case "MaintenanceDone":
                        _carService.addMaintenance(MessageSerializer.Deserialize<MaintenanceModel>(message));
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return true;
        }

    }
}
