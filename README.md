## Canducci DocumentDB Repository

[![NuGet](https://img.shields.io/nuget/v/CanducciDocumentDB.svg?style=plastic&label=version)](https://www.nuget.org/packages/CanducciDocumentDB/)

[![Canducci Excel](http://i682.photobucket.com/albums/vv181/parapua/1487622555_database-px-png_zpsqh2d6d60.png)](https://www.nuget.org/packages/CanducciDocumentDB/)

### Configura��es:

Configure a classe `ConnectionDocumentDB` que tem um construtor com tr�s paramentros:

> - url
> - key
> - database

##### C�digo:

```csharp
string url = "https://localhost:8081/";
string key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
string database = "database";
ConnectionDocumentDB _db = new ConnectionDocumentDB(url, key, database);
```

Apos configurar a classe, precisamos criar uma classe que represente o modelo da cole��o e sua respectiva classe `respository`.

##### C�digo modelo para cole��o:

```csharp
public class Credit
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "description")]
    [Required(ErrorMessage = "Digite a descri��o ...")]
    public string Description { get; set; }
}
```

__Observa��o:__ a classe `Credit` foi decorada com `JsonProperty` atributo que vem do pacote [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/), 
item j� instalado quando � instalado esse pacote [Canducci DocumentDB](https://www.nuget.org/packages/CanducciDocumentDB/)

##### C�digo da `Repository`:

A classe `abstract` `RepositoryCreditAbstract` que herda de `Repository` e `IRepository` como contrato, tem um construtor com a classe `ConnectionDocumentDB db` e � chamado a sua base para a passagem da vari�vel `db` e o nome da cole��o que no caso � `credit`.

```csharp
public abstract class RepositoryCreditAbstract: Repository<Credit>, IRepository<Credit>
{
    public RepositoryCreditAbstract(ConnectionDocumentDB db)
        : base(db, "credit")
    {
    }
}
```

A classe `concret` `RepositoryCredit` herda de `RepositoryCreditAbstract` e tamb�m possui um construtor com a classe `ConnectionDocumentDB db` e chama a sua base para a passagem da vari�vel `db`, n�o precisando passar o segundo parametro ato feito na classe `abstract` `RepositoryCreditAbstract`.

```csharp
public class RepositoryCredit: RepositoryCreditAbstract
{
    public RepositoryCredit(ConnectionDocumentDB db)
        : base(db)
    {
    }
} 
```

Com a codifica��o dessas classes, permite a grava��o dessa cole��o configurada, onde a estrutura possui os seguintes m�todos:


- Inserir cole��o

```csharp
ConnectionDocumentDB _db = new ConnectionDocumentDB(url, key, database);
RepositoryCreditAbstract db = new RepositoryCredit(db);
Credit credit = new Credit { Description = "Matem�tica" };
credit = await db.InsertAsync(credit);
```

- Atualizar cole��o

```csharp
ConnectionDocumentDB _db = new ConnectionDocumentDB(url, key, database);
RepositoryCreditAbstract db = new RepositoryCredit(db);

Credit credit = db.FindAsync("d70e21fd-b9e3-430b-a934-778ce3a871b3");
credit.Description = "Hist�ria";
await db.UpdateAsync(credit);
```

- Excluir uma cole��o

```csharp
ConnectionDocumentDB _db = new ConnectionDocumentDB(url, key, database);
RepositoryCreditAbstract db = new RepositoryCredit(db);

await db.DeleteAsync("d70e21fd-b9e3-430b-a934-778ce3a871b3");
```

- Trazer uma cole��o pelo `Id`

```csharp
ConnectionDocumentDB _db = new ConnectionDocumentDB(url, key, database);
RepositoryCreditAbstract db = new RepositoryCredit(db);

Credit credit = await db.FindAsync("d70e21fd-b9e3-430b-a934-778ce3a871b3");
```

- Trazer todos as cole��es

```csharp
ConnectionDocumentDB _db = new ConnectionDocumentDB(url, key, database);
RepositoryCreditAbstract db = new RepositoryCredit(db);

IEnumerable<Credit> creditList = await db.AllAsync();
```

___

### Web ASP.Net

Para configurar um aplica��o `Web`, � utilizado o [Ninject.MVC5](https://www.nuget.org/packages/Ninject.MVC5/), que ao final da instala��o do seu pacote ter� um classe a ser configurada na pasta `App_Start` no arquivo `NinjectWebCommon.cs`.

No m�todo `void RegisterServices(IKernel kernel)` fa�a:

```csharp
private static void RegisterServices(IKernel kernel)
{
    kernel.Bind<ConnectionDocumentDB>()                
        .ToSelf()
        .WithConstructorArgument("url", ConfigurationManager.AppSettings["url"])
        .WithConstructorArgument("key", ConfigurationManager.AppSettings["key"])
        .WithConstructorArgument("database", ConfigurationManager.AppSettings["database"]);

    kernel.Bind<RepositoryCarAbstract>().To<RepositoryCar>();
    kernel.Bind<RepositoryCreditAbstract>().To<RepositoryCredit>();
}
```
Observa��o que foi configurado dentro do `Web.config` na chave `appSettings` as configura��es de conex�o:

```xml
<appSettings>
    ...
    <add key="url" value="https://localhost:8081/" />
    <add key="key" value="C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==" />
    <add key="database" value="Todo" />
</appSettings>
```

feito isso a sua aplica��o `Web` est� preparada para receber inje��o de dependencia, como exemplo de um `controller`:

```csharp
public class CreditsController : Controller
{
    protected RepositoryCreditAbstract Repository { get; private set; }

    public CreditsController(RepositoryCreditAbstract repository)
    {
        Repository = repository;
    }
    protected override void Dispose(bool disposing)
    {
        if (Repository != null) Repository.Dispose();
        base.Dispose(disposing);
    }
        
    public async Task<ActionResult> Index(int? page)
    {
        return View(await Repository.AllAsync());
    }
                
    public async Task<ActionResult> Details(string id)
    {            
        return View(await Repository.FindAsync(id));
    }

    public ActionResult Create()
    {
        return View();
    }
                
    [HttpPost]
    public async Task<ActionResult> Create(Credit credit)
    {
        try
        {
            credit = await Repository.InsertAsync(credit);
            if (string.IsNullOrEmpty(credit.Id))
                return RedirectToAction("Index");
            return RedirectToAction("Edit", new { id = credit.Id });
        }
        catch
        {
            return View();
        }
    }

    public async Task<ActionResult> Edit(string id)
    {
        return View(await Repository.FindAsync(id));
    }

    [HttpPost]
    public async Task<ActionResult> Edit(string id, Credit credit)
    {
        try
        {
            await Repository.UpdateAsync(credit, id);
            return RedirectToAction("Edit", new { id = credit.Id });
        }
        catch
        {
            return View();
        }
    }
        
    public async Task<ActionResult> Delete(string id)
    {
        return View(await Repository.FindAsync(id));
    }

    [HttpPost]
    public async Task<ActionResult> Delete(string id, Credit credit)
    {
        try
        {
            await Repository.DeleteAsync(id);
            return RedirectToAction("Index");
        }
        catch
        {
            return View();
        }
    }
}
```

Gere todas as `Views` respectivas de cada m�todo, onde o mesmo possibilitar� as opera��es para essa cole��o.