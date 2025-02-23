
namespace ParkNetApp;

public class Utilities
{

    public static bool IsLayoutValid(string layout)
        => !string.IsNullOrEmpty(layout)
        && Regex.IsMatch(layout, "^[CM ]+$");

    public static List<Slot> GetSlots(string layout, List<Floor> floorsOfParkingLot)
    {
        int numberOfFloors = floorsOfParkingLot.Count;
        int floorIndex = 0;
        bool isLastRowBlank = false;
        int lastRowWithSlots = 0;

        var adjustLastRowWithSlots = 0;

        var slotsOfParkingLot = new List<Slot>();
        var originalMatrix = GetRowsMatrix(layout);
        int numbRows = originalMatrix.Length;

        for (int i = 0; i < numbRows; i++)
        {
            if (IsSlotRow(originalMatrix, i))
            {
                lastRowWithSlots = i - adjustLastRowWithSlots;

                isLastRowBlank = false;
                for (int j = 0; j < originalMatrix[i].Length; j++)
                {
                    if (!char.IsWhiteSpace(originalMatrix[i][j]))
                    {
                        var codeLetter = GetCodeLetter(lastRowWithSlots);
                        slotsOfParkingLot.Add(new Slot
                        {
                            Code = $"{codeLetter}{j}",
                            SlotType = originalMatrix[i][j],
                            FloorId = floorsOfParkingLot[floorIndex].Id
                        });
                    }
                    if (floorIndex >= numberOfFloors)
                        return slotsOfParkingLot;
                }
            }
            else
            {
                if (isLastRowBlank)
                {
                    adjustLastRowWithSlots++;
                    continue;
                }
                adjustLastRowWithSlots++;
                // Se última não foi blank e a corrente é, então é um novo floor
                floorIndex++;
                isLastRowBlank = true;
            }
        }
        return slotsOfParkingLot;
    }

    public static List<Floor> GetFloors(string layout, int parkingLotId)
    {
        var floorsOfParkingLot = new List<Floor>();
        var originalMatrix = GetRowsMatrix(layout);

        var numbFloor = 1;

        bool IsLastRowBlank = false;

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
                if (!IsLastRowBlank)
                {
                    numbFloor++;
                    floorsOfParkingLot.Add(new Floor
                    {
                        Name = $"Floor{numbFloor}",
                        ParkingLotId = parkingLotId
                    });
                }
                IsLastRowBlank = true;
            }
            else
                IsLastRowBlank = false;
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
    {
        layout = Regex.Replace(layout, @"^\s*\n\s*", ""); // Remove espaços e quebras de linha iniciais
        return layout.Split("\n").Select(x => x.ToCharArray()).ToArray();
    }

}