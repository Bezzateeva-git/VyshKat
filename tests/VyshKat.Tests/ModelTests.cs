using Xunit;

public class ModelTests
{
    [Theory]
    [InlineData(10, 5, true)]
    [InlineData(11, 5, false)]
    [InlineData(10, 4, false)]
    [InlineData(11, 4, false)]
    [InlineData(0, 10, true)]
    public void InspectionChecksAgeAndBrakes(int years, int brakes, bool expected)
    {
        Bike transport = new Bike(years, brakes, 5, "Велосипед", "B");
        Assert.Equal(years, transport.ServiceLife);
        Assert.Equal(expected, new FleetServiceCenter().Checking(transport));
    }

    [Theory]
    [InlineData(5, false)]
    [InlineData(6, true)]
    [InlineData(1, false)]
    [InlineData(10, true)]
    public void BeginnerRuleIncludesSix(int ease, bool expected)
    {
        Assert.Equal(expected, new Scooter(0, 5, ease, "Самокат", "S").IsSuitableForBeginner());
    }

    [Theory]
    [InlineData(-1, 5, 5)]
    [InlineData(0, 0, 5)]
    [InlineData(0, 11, 5)]
    [InlineData(0, 5, 0)]
    [InlineData(0, 5, 11)]
    public void InvalidNumericCharacteristicsAreRejected(int years, int brakes, int ease)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Scooter(years, brakes, ease, "Имя", "N"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void NamesAndNumbersMustBeNonblank(string? invalid)
    {
        Assert.Throws<ArgumentException>(() => new Scooter(0, 5, 5, invalid!, "N"));
        Assert.Throws<ArgumentException>(() => new Scooter(0, 5, 5, "Имя", invalid!));
        Assert.Throws<ArgumentException>(() => new Helmet(invalid!, "N"));
        Assert.Throws<ArgumentException>(() => new Helmet("Имя", invalid!));
    }

    [Fact]
    public void NamesAndNumbersAreTrimmed()
    {
        Scooter scooter = new Scooter(0, 5, 5, " Имя ", " N ");
        Helmet helmet = new Helmet(" Шлем ", " H ");
        Assert.Equal("Имя", scooter.Name);
        Assert.Equal("N", scooter.InventoryNumber);
        Assert.Equal("Шлем", helmet.Name);
        Assert.Equal("H", helmet.InventoryNumber);
    }

    [Fact]
    public void EveryEnergyTypeRejectsNegativeConsumptionAndAcceptsZero()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ElectroScooter(0, 5, 5, "ЭС", "ES", -1m));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ElectroBike(0, 5, 5, "ЭВ", "EB", -1m));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ChargingCabinet("Шкаф", "C", -1m));
        Assert.Throws<ArgumentOutOfRangeException>(() => new DockingStation("Станция", "D", -1m));

        Assert.Equal(0m, new ElectroScooter(0, 5, 5, "ЭС", "ES", 0m).EnergyConsumption);
        Assert.Equal(0m, new ElectroBike(0, 5, 5, "ЭВ", "EB", 0m).EnergyConsumption);
        Assert.Equal(0m, new ChargingCabinet("Шкаф", "C", 0m).EnergyConsumption);
        Assert.Equal(0m, new DockingStation("Станция", "D", 0m).EnergyConsumption);
    }
}
