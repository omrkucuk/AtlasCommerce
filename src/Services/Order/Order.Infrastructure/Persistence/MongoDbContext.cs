using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Order.Domain.Entities;
using Order.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Infrastructure.Persistence
{
    public sealed class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(MongoDbSettings settings)
        {
            // MongoDb driver class map'i kayıt eder- bu olmadan private setter'lı property'ler serialize/deserialize edilemez.
            RegisterClassMaps();

            var client = new MongoClient(settings.ConnectionStrings);
            _database = client.GetDatabase(settings.DatabaseName);
        }

        public IMongoCollection<Order.Domain.Entities.Order> Orders =>
            _database.GetCollection<Order.Domain.Entities.Order>("orders");

        private static bool _classMapRegistered = false;
        private static readonly object _lock = new();

        private void RegisterClassMaps()
        {
            lock (_lock)
            {
                if(_classMapRegistered) return;
            }

            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            // Convention pack - private field'ları otomatik bul
            var pack = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true)
            };
            ConventionRegistry.Register("AtlasConventions", pack, _ => true);

            // MongoDB'ye private constructor ile entity'leri nasıl oluşturacağını söylüyoruz
            BsonClassMap.RegisterClassMap<Order.Domain.Entities.Order>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);

                // _items field'ını "items" element adıyla map et
                // MongoDB bu field'a reflection ile yazabilir
                var itemsField = typeof(Order.Domain.Entities.Order)
                    .GetField("_items", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (itemsField != null)
                    cm.MapField("_items").SetElementName("items");

                var notesField = typeof(Order.Domain.Entities.Order)
                    .GetField("_notes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (notesField != null)
                    cm.MapField("_notes").SetElementName("notes");

                cm.UnmapField("_domainEvents");
            });

            BsonClassMap.RegisterClassMap<OrderItem>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<OrderNote>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<Money>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<Address>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<PaymentInfo>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });

            _classMapRegistered = true;
        }
    }
}
