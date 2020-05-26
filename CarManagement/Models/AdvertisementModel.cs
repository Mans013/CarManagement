using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AdvertisementManagement.Models
{
    public class AdvertisementModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("CarID")]
        public string CarID { get; set; }

        [BsonElement("Description")]
        public string Description { get; set; }

        [BsonElement("Price")]
        public int Price { get; set; }

        [BsonElement("Image")]
        public string Image { get; set; }
    }
}
