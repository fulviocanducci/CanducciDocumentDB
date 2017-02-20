using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Canducci.DocumentDB;
using Canducci.ConsoleApp.Models;

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
            Credit c = new Credit();
            using (ConnectionDocumentDB db = new ConnectionDocumentDB())
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
