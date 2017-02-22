using System;
using System.Threading.Tasks;
using Canducci.DocumentDB;
using Canducci.ConsoleApp.Models;
using System.Configuration;

namespace Canducci.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(GetId().Result);
            Console.ReadKey();
        }

        static async Task<string> GetId()
        {
            string url = ConfigurationManager.AppSettings["url"];
            string key = ConfigurationManager.AppSettings["key"];
            string database = ConfigurationManager.AppSettings["database"];

            Credit c = new Credit();
            using (ConnectionDocumentDB db = new ConnectionDocumentDB(url, key, database))
            using (RepositoryCarAbstract rep = new RepositoryCar(db))
            using (RepositoryCreditAbstract repc = new RepositoryCredit(db))
            {                
                c.Description = "Uol.com.br";
                c = await repc.InsertAsync(c);

                Car cr = new Car();
                cr.Port = 10;
                cr.Description = "Title1";
                cr.Created = DateTime.Now.AddDays(-1);
                cr.Status = true;

                var abc = await rep.InsertAsync(cr);
            }
            return c.Id;
        }
    }
}
