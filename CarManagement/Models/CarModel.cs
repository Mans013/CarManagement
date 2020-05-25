using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarManagement.Models
{
    public class CarModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("LicencePlate")]
        public string LicencePlate { get; set; }

        [BsonElement("Brand")]
        public string Brand { get; set; }

        [BsonElement("Miles")]
        public int Miles { get; set; }

        [BsonElement("Weight")]
        public int Weight { get; set; }

        [BsonElement("OwnerName")]
        public string OwnerName { get; set; }

        [BsonElement("BuildDate")]
        public string BuildDate { get; set; }

        [BsonElement("MaintenanceHistory")]
        public List<MaintenanceModel> MaintenanceHistory { get; set; }
    }
}
