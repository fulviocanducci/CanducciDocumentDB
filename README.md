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
A classe `concret` `RepositoryCredit` herda de `RepositoryCreditAbstract` e tamb�m possui um construtor com a classe `ConnectionDocumentDB db` e chama a sua base para a passagem da vari�vel `db`, n�o precisando passar o segundo paramentro ato feito na classe `abstract` `RepositoryCreditAbstract`.
_
```csharp
public class RepositoryCredit: RepositoryCreditAbstract
{
    public RepositoryCredit(ConnectionDocumentDB db)
        : base(db)
    {
    }
} 
```






