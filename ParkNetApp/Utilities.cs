
using System.Text.Json;

namespace ParkNetApp;

public class Utilities
{


    //   // public string fileName = "Schema_01LisbonPortugal.txt"; // Exemplo dos nomes usando o Parking Lot Internal Code

    //    public static string GetParkingLotSchemaPath(string fileName)
    //    {
    //        var file = fileName;
    //        var dir = String.Empty;
    //        const string configFile = "config.txt";
    //        if (!File.Exists(configFile))
    //        {
    //            Console.WriteLine("Choose a directory: ");
    //            dir = GetValidDirectory();
    //            File.WriteAllText(configFile, dir);
    //        }
    //        dir = File.ReadAllText(configFile);
    //        var filePath = Path.Combine(dir, file);
    //        return filePath;
    //    }

    //    public static string GetValidDirectory()
    //    {
    //        string dir;
    //        while (true)
    //        {
    //            dir = Console.ReadLine();
    //            if (!String.IsNullOrEmpty(dir) && Directory.Exists(dir))
    //            {
    //                break;
    //            }

    //            Console.Write("Invalid directory! Please try again: ");
    //        }

    //        return dir;
    //    }

    //    public static void SaveParkingLotSchema(string schemaPath, string stringSchema)
    //    {
    //        try
    //        {
    //            string jsonSchema = JsonSerializer.Serialize(stringSchema);
    //            File.WriteAllText(schemaPath, jsonSchema);
    //        }
    //        catch
    //        {
    //            throw new Exception("Error saving Parking Lot Schema.");
    //        }
    //    }




}
