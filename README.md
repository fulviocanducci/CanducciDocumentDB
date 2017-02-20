## Canducci DocumentDB Repository

[![NuGet](https://img.shields.io/nuget/v/CanducciDocumentDB.svg?style=plastic&label=version)](https://www.nuget.org/packages/CanducciDocumentDB/)

[![Canducci Excel](http://i682.photobucket.com/albums/vv181/parapua/1487622555_database-px-png_zpsqh2d6d60.png)](https://www.nuget.org/packages/CanducciDocumentDB/)

### Configurações:

Configure a classe `ConnectionDocumentDB` que tem um construtor com três paramentros:

> - url
> - key
> - database

##### Código:

```csharp
string url = "https://localhost:8081/";
string key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
string database = "database";
ConnectionDocumentDB _db = new ConnectionDocumentDB(url, key, database);
```

Apos configurar a classe, precisamos criar uma classe que represente o modelo da coleção e sua respectiva classe `respository`.

##### Código modelo para coleção:

```csharp
public class Credit
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "description")]
    [Required(ErrorMessage = "Digite a descrição ...")]
    public string Description { get; set; }
}
```

__Observação:__ a classe `Credit` foi decorada com `JsonProperty` atributo que vem do pacote [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/), 
item já instalado quando é instalado esse pacote [Canducci DocumentDB](https://www.nuget.org/packages/CanducciDocumentDB/)

##### Código da `Repository`:

A classe `abstract` `RepositoryCreditAbstract` que herda de `Repository` e `IRepository` como contrato, tem um construtor com a classe `ConnectionDocumentDB db` e é chamado a sua base para a passagem da variável `db` e o nome da coleção que no caso é `credit`.

```csharp
public abstract class RepositoryCreditAbstract: Repository<Credit>, IRepository<Credit>
{
    public RepositoryCreditAbstract(ConnectionDocumentDB db)
        : base(db, "credit")
    {
    }
}
```
A classe `concret` `RepositoryCredit` herda de `RepositoryCreditAbstract` e também possui um construtor com a classe `ConnectionDocumentDB db` e chama a sua base para a passagem da variável `db`, não precisando passar o segundo paramentro ato feito na classe `abstract` `RepositoryCreditAbstract`.
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






