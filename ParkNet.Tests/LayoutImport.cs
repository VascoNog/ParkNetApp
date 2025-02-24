using ParkNetApp;


namespace ParkNet.Tests;

public class LayoutImport
{
    public const int parkingLotId = 1;
    public string layout =
        @"
        CC CC MM 
        C      C
               C

        MM MMCMC

            CC
             CC




        CCC MMMM";

    [Fact]
    public void ImportLayout_ReturnsCorrectNumberOfFloors()
    {

        //Act
        var floors = Utilities.GetFloors(layout, parkingLotId);
        var result = floors.Count();

        //Assert
        Assert.Equal(4, result);


    }

    [Fact]
    public void ImportLayout_ReturnsCorrectThirdFloorDesignation()
    {
        //Act
        var floors = Utilities.GetFloors(layout, parkingLotId);
        var result = floors[2].Name;

        //Assert
        Assert.Equal("Floor3", result);
    }

    [Fact]
    public void ImportLayout_ReturnsCorrectNumberOfSlots()
    {
        //Act
        var slots = Utilities.GetSlots(layout, Utilities.GetFloors(layout, parkingLotId));
        var result = slots.Count();


        //Assert
        Assert.Equal(27, result);
    }

    [Fact]
    public void ImportLayout_ReturnsCorrectNumberOfMotorcycleSlots()
    {
        //Act
        var slots = Utilities.GetSlots(layout, Utilities.GetFloors(layout, parkingLotId));
        var result = slots.Count(s => s.SlotType =='M');


        //Assert
        Assert.Equal(11, result);
    }

    [Fact]
    public void ImportLayout_ReturnsCorrectNumberOfCarSlots()
    {
        //Act
        var slots = Utilities.GetSlots(layout, Utilities.GetFloors(layout, parkingLotId));
        var result = slots.Count(s => s.SlotType == 'C');


        //Assert
        Assert.Equal(16, result);
    }

    [Fact]
    public void ImportLayoutWithoutCars_ReturnsCarSlotCountAsZero()
    {
        //Arrange
        string thisLayout =
         @"
         MMMMMM
         M    M
         M    M

              M  
              M
         M    M
         MMMMMM";

        //Act
        var slots = Utilities.GetSlots(thisLayout, Utilities.GetFloors(thisLayout, parkingLotId));
        var result = slots.Count(s => s.SlotType == 'C');

        //Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void ImportLayout_DetectsInvalidSlotsCorrectly()
    {
        //Arrange
        string thisLayout = "C C\n CC\nCC CC\n M M M";

        List<string> wrongSlotCodes = ["A1","A3","B0","C2","D0","D2","D4"];
        List<string> correctSlotCodes = new();
        //Act
        var slots = Utilities.GetSlots(thisLayout, Utilities.GetFloors(thisLayout, parkingLotId));
        foreach(var slot in slots)
        {
            Console.WriteLine(slot.Code);
            correctSlotCodes.Add(slot.Code);
        }
        var result = wrongSlotCodes.Any(wsc => correctSlotCodes.Contains(wsc));

        //Assert
        Assert.False(result);
    }

    [Fact]
    public void ImportLayout_DetectsAllSlotsCorrectly()
    {
        //Arrange
        string x = "C C\n CC\nCC CC\n M M M";

        List<string> slotCodesToChecked = ["A0", "A2", "B1", "B2", "C0", "C1", "C3", "C4", "D1", "D3", "D5"];
        List<string> correctSlotCodes = new();
        //Act
        var slots = Utilities.GetSlots(x, Utilities.GetFloors(x, parkingLotId));
        foreach (var slot in slots)
        {
            Console.WriteLine(slot.Code);
            correctSlotCodes.Add(slot.Code);
        }
        var result = slotCodesToChecked.Any(sctc => !correctSlotCodes.Contains(sctc));

        //Assert
        Assert.False(result);
    }



}
