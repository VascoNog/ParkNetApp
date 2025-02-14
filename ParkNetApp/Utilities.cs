﻿namespace ParkNetApp;

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

    public static List<Slot> GetSlots(string layout, List<Floor> floorsOfParkingLot)
    {
        int numberOfFloors = floorsOfParkingLot.Count;
        int floorIndex = 0;

        var slotsOfParkingLot = new List<Slot>();
        var originalMatrix = GetRowsMatrix(layout);
        int numbRows = originalMatrix.Length;

        while (floorIndex < numberOfFloors)
        {
            for (int i = 0; i < numbRows; i++)
            {
                if (IsSlotRow(originalMatrix, i))
                {
                    string codeLetter = GetCodeLetter(i);

                    for (int j = 0; j < originalMatrix[i].Length; j++)
                    {
                        if (!char.IsWhiteSpace(originalMatrix[i][j]))
                        {
                            slotsOfParkingLot.Add(new Slot
                            {
                                Code = $"{codeLetter}{j}",
                                SlotType = originalMatrix[i][j],
                                FloorId = floorsOfParkingLot[floorIndex].Id
                            });
                        }
                    }
                }
                else
                {
                    floorIndex++;
                    if (floorIndex >= numberOfFloors)
                        return slotsOfParkingLot;
                }
            }
        }

        return slotsOfParkingLot;
    }

    public static List<Floor> GetFloors(string layout, int parkingLotId)
    {
        var floorsOfParkingLot = new List<Floor>();
        var originalMatrix = GetRowsMatrix(layout);

        var numbFloor = 1;
        // First floor
        floorsOfParkingLot.Add(new Floor
        {
            Name = $"Floor{numbFloor}",
            ParkingLotId = parkingLotId
        });

        int numbRows = originalMatrix.Length;
        for (int i = 0; i < numbRows; i++)
        {
            if (!IsSlotRow(originalMatrix, i))
            {
                numbFloor++;
                floorsOfParkingLot.Add(new Floor
                {
                    Name = $"Floor{numbFloor}",
                    ParkingLotId = parkingLotId
                });
            }
        }
        return floorsOfParkingLot;
    }

    private static bool IsSlotRow(char[][] matrix, int row)
    {
        for (int i = 0; i < matrix[row].Length; i++)
        {
            if (!char.IsWhiteSpace(matrix[row][i])) // Correção aqui
                return true;
        }
        return false;
    }

    public static string GetCodeLetter(int row)
    {
        const int maxAlphabetLetters = 26;
        if (row < maxAlphabetLetters)
            return ((char)('A' + row)).ToString();
        else
            return ((char)('A' + ((row / maxAlphabetLetters) - 1))).ToString() + ((char)('A' + row % 26)).ToString();
    }

    public static char[][] GetRowsMatrix(string layout)
        => layout.Split("\n").Select(x => x.ToCharArray()).ToArray();
}