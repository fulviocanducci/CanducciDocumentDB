using Canducci.DocumentDB;
using Newtonsoft.Json;

namespace Canducci.ConsoleApp.Models
{
    public class Credit
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }
    public abstract class RepositoryCreditAbstract :
        Repository<Credit>,
        IRepository<Credit>
    {
        public RepositoryCreditAbstract(ConnectionDocumentDB db)
            : base(db, "cars")
        {
        }
    }

    public class RepositoryCredit :
        RepositoryCreditAbstract
    {
        public RepositoryCredit(ConnectionDocumentDB db)
            : base(db)
        {
        }
    }
}
