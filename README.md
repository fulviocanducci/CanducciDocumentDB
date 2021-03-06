## Canducci DocumentDB Repository

[![NuGet Badge](https://buildstats.info/nuget/CanducciDocumentDB)](https://www.nuget.org/packages/CanducciDocumentDB/)

[![Canducci Excel](http://i682.photobucket.com/albums/vv181/parapua/1487622555_database-px-png_zpsqh2d6d60.png)](https://www.nuget.org/packages/CanducciDocumentDB/)

### Instala��o:

A instala��o pode ser feito pelo [Package Manager Console](https://docs.microsoft.com/en-us/nuget/tools/package-manager-console)

```csharp
PM> Install-Package CanducciDocumentDB
```

ou ent�o [Nuget](https://www.nuget.org/packages/CanducciDocumentDB/)

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
ConnectionDocumentDB db = new ConnectionDocumentDB(url, key, database);
```

Apos configurar a classe, precisamos criar uma classe que represente o __modelo da cole��o__ e sua respectiva classe `respository`.

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

A classe `concret` `RepositoryCredit` herda de `RepositoryCreditAbstract` e tamb�m possui um `construtor` com a classe `ConnectionDocumentDB db` e chama a sua base para a passagem da vari�vel `db`, n�o precisando passar o segundo parametro ato feito na classe `abstract` `RepositoryCreditAbstract`.

```csharp
public class RepositoryCredit: RepositoryCreditAbstract
{
    public RepositoryCredit(ConnectionDocumentDB db)
        : base(db)
    {
    }
} 
```

Com a codifica��o dessas classes, permite a grava��o dessa cole��o configurada (vale lembrar que a cria��o do `DatabaseId` e `CollectionId` e manual, ent�o, entre no Azure e configure essas duas informa��es), onde a estrutura possui os seguintes m�todos:


- __Inserir cole��o__

```csharp
ConnectionDocumentDB db = new ConnectionDocumentDB(url, key, database);
RepositoryCreditAbstract rep = new RepositoryCredit(db);
Credit credit = new Credit { Description = "Matem�tica" };
credit = await rep.InsertAsync(credit);
```

- __Atualizar cole��o__

```csharp
ConnectionDocumentDB db = new ConnectionDocumentDB(url, key, database);
RepositoryCreditAbstract rep = new RepositoryCredit(db);

Credit credit = await rep.FindAsync("d70e21fd-b9e3-430b-a934-778ce3a871b3");
credit.Description = "Hist�ria";
await rep.UpdateAsync(credit);
```

- __Excluir uma cole��o__

```csharp
ConnectionDocumentDB db = new ConnectionDocumentDB(url, key, database);
RepositoryCreditAbstract rep = new RepositoryCredit(db);

await rep.DeleteAsync("d70e21fd-b9e3-430b-a934-778ce3a871b3");
```

- __Trazer uma cole��o pelo `Id`__

```csharp
ConnectionDocumentDB db = new ConnectionDocumentDB(url, key, database);
RepositoryCreditAbstract rep = new RepositoryCredit(db);

Credit credit = await rep.FindAsync("d70e21fd-b9e3-430b-a934-778ce3a871b3");
```

- __Trazer todos as cole��es__

```csharp
ConnectionDocumentDB db = new ConnectionDocumentDB(url, key, database);
RepositoryCreditAbstract rep = new RepositoryCredit(db);

IEnumerable<Credit> creditList = await rep.AllAsync();
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
__Observa��o:__ foi configurado dentro do `Web.config` na chave `appSettings` as configura��es de conex�o, segue abaixo:

```xml
<appSettings>
    ...
 <add key="url" value="https://localhost:8081/"/>
 <add key="key" value="C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="/>
 <add key="database" value="Todo"/>
</appSettings>
```

Feito isso a sua aplica��o `Web` est� preparada para receber __Inje��o de Dependencia__, como por exemplo esse `controller`:

```csharp
public class CreditsController : Controller
{
    protected RepositoryCreditAbstract Repository { get; private set; }

    public CreditsController(RepositoryCreditAbstract repository)
    {
        Repository = repository;
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
A partir desse ponto gere todas as `Views` respectivas de cada m�todo, onde o mesmo possibilitar� as opera��es de `CRUD` para essa cole��o.

Um exemplo de um registro gravado dessa cole��o:

```xml
{
    "id": "d70e21fd-b9e3-430b-a934-778ce3a871b3",
    "title": "LINGUA PORTUGUESA",
    "value": 200,
    "created": "2015-02-28T00:00:00",
    "active": true,
    "_rid": "aFh5AITdaQABAAAAAAAAAA==",
    "_self": "dbs/aFh5AA==/colls/aFh5AITdaQA=/docs/aFh5AITdaQABAAAAAAAAAA==/",
    "_etag": "\"00001400-0000-0000-0000-58ab120e0000\"",
    "_attachments": "attachments/",
    "_ts": 1487606286
}
```