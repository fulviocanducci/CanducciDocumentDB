using Canducci.DocumentDB;
using Newtonsoft.Json;
using System;
namespace Canducci.Web.Models
{
    public class Car
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "port")]
        public int Port { get; set; }

        [JsonProperty(PropertyName = "created")]
        public DateTime Created { get; set; }

        [JsonProperty(PropertyName = "value")]
        public decimal Value { get; set; }

        [JsonProperty(PropertyName = "status")]
        public bool Status { get; set; }
    }

    public abstract class RepositoryCarAbstract:
        Repository<Car>,
        IRepository<Car>
    {
        public RepositoryCarAbstract(ConnectionDocumentDB db)
            :base(db, "cars")
        {
        } 
    }

    public class RepositoryCar :
        RepositoryCarAbstract
    {
        public RepositoryCar(ConnectionDocumentDB db) 
            : base(db)
        {
        }
    }
}
