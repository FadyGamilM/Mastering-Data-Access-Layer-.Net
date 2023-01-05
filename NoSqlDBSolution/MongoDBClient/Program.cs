using Microsoft.Extensions.Configuration;

public class Program
{
   public static void Main()
   {

   }

   public static string GetConnectionString(string connString = "Default")
   {
      var output = "";

      var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json");
      
      var config = builder.Build();

      output = config.GetConnectionString(connString);

      return output;
   }
}