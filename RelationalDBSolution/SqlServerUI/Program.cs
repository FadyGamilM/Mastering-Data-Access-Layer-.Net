// a method that takes a connection string name as a paramter 
// and fetch the connection string value from json file 
// so later we can just replace the connectionStringName paramter 
// to use another data source rather than the local Sql Server DB
using Microsoft.Extensions.Configuration;

string GetConnString (string connStringKey = "DefaultConnection")
{
    // define the builder 
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json");

    // define the config variable
    var config = builder.Build();

    // get the connection string from the json file and return the result
    var connStringValue = config.GetConnectionString(connStringKey);
    return connStringValue;
}

// test the method
var connString = GetConnString();
Console.WriteLine(connString);