using Newtonsoft.Json;
using Canducci.DocumentDBCore;
namespace Canducci.WebCore.Models
{
    public class Jornal
    {
        public Jornal()
        {

        }

        public Jornal(string title)
        {
            Title = Title;
        }

        public Jornal(string id, string title)
            :this(title)
        {
            Id = id;
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "title")]
        [JsonRequired()]
        public string Title { get; set; }


        public Jornal Create(string title)
        {
            return new Jornal(title);
        }

        public Jornal Create(string id, string title)
        {
            return new Jornal(id, title);
        }

    }


    public abstract class RepositoryJornalAbstract : 
        Repository<Jornal>,
        IRepository<Jornal>
    {
        public RepositoryJornalAbstract(ConnectionDocumentDB db)
            :base(db, "jornal")
        {
        }
    }

    public class RepositoryJornal : RepositoryJornalAbstract
    {
        public RepositoryJornal(ConnectionDocumentDB db) 
            : base(db)
        {
        }
    }

}
