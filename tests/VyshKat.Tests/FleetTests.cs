using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

public class StubServiceCenter : IServiceCenter
{
    private readonly bool _result;
    public int Calls { get; private set; }
    public StubServiceCenter(bool result) { _result = result; }
    public bool Checking(Transport transport)
    {
        Calls++;
        return _result;
    }
}

public class FleetTests
{
    [Theory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void AdmissionUsesDependencyRegisteredInContainer(bool accepted, int count)
    {
        ServiceCollection services = new ServiceCollection();
        DependencyInjection.Configure(services);
        services.RemoveAll<IServiceCenter>();
        StubServiceCenter inspection = new StubServiceCenter(accepted);
        services.AddSingleton<IServiceCenter>(inspection);
        using var provider = services.BuildServiceProvider();
        Fleet fleet = provider.GetRequiredService<Fleet>();
        ElectroScooter scooter = new ElectroScooter(0, 10, 6, "Самокат", "T-1", 2m);

        Assert.Equal(accepted, fleet.TryAddTransport(scooter));
        Assert.Equal(count, fleet.Count);
        Assert.Equal(count, fleet.TransportForBeginners().Count);
        Assert.Equal(count, fleet.AllInventory().Count);
        Assert.Equal(accepted ? 2m : 0m, fleet.SumEnergy());
        Assert.Equal(1, inspection.Calls);
        Assert.Same(fleet, provider.GetRequiredService<Fleet>());
    }

    [Fact]
    public void InventoryContainsTransportAndEquipmentAndEnergyUsesInterface()
    {
        Fleet fleet = new Fleet(new FleetServiceCenter());
        fleet.TryAddTransport(new Scooter(0, 5, 5, "Самокат", "S"));
        fleet.TryAddTransport(new Bike(0, 5, 6, "Велосипед", "B"));
        fleet.TryAddTransport(new ElectroScooter(0, 5, 7, "ЭС", "ES", 2m));
        fleet.TryAddTransport(new ElectroBike(0, 5, 8, "ЭВ", "EB", 3m));
        fleet.AddEquipment(new Helmet("Шлем", "H"));
        fleet.AddEquipment(new ChargingCabinet("Шкаф", "C", 4m));
        fleet.AddEquipment(new DockingStation("Станция", "D", 5m));
        fleet.AddEquipment(new NewEnergyItem("Новый тип", "NEW", 6m));
        Assert.Equal(4, fleet.Count);
        Assert.Equal(8, fleet.AllInventory().Count);
        Assert.Equal(20m, fleet.SumEnergy());
        List<Transport> beginners = fleet.TransportForBeginners();
        Assert.Equal(3, beginners.Count);
        Assert.Equal("B", beginners[0].InventoryNumber);
        Assert.Equal("ES", beginners[1].InventoryNumber);
        Assert.Equal("EB", beginners[2].InventoryNumber);
        foreach (IInventoryItem item in fleet.AllInventory())
        {
            Assert.False(string.IsNullOrWhiteSpace(item.Name));
        }
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void DuplicateNumbersAreRejectedAcrossBothCollections(bool firstEquipment, bool secondEquipment)
    {
        StubServiceCenter inspection = new StubServiceCenter(true);
        Fleet fleet = new Fleet(inspection);
        if (firstEquipment) fleet.AddEquipment(new Helmet("Первый", " X-1 "));
        else fleet.TryAddTransport(new Bike(0, 5, 5, "Первый", " X-1 "));
        int callsBefore = inspection.Calls;
        if (secondEquipment)
            Assert.Throws<ArgumentException>(() => fleet.AddEquipment(new Helmet("Второй", "x-1")));
        else
            Assert.Throws<ArgumentException>(() => fleet.TryAddTransport(new Scooter(0, 5, 5, "Второй", "x-1")));
        Assert.Single(fleet.AllInventory());
        Assert.Equal(callsBefore, inspection.Calls);
    }

    [Fact]
    public void RejectedTransportDoesNotReserveItsNumber()
    {
        Fleet fleet = new Fleet(new FleetServiceCenter());
        Assert.False(fleet.TryAddTransport(new Scooter(11, 5, 6, "Старый", "X")));
        Assert.True(fleet.TryAddTransport(new Scooter(10, 5, 6, "Новый", "X")));
        Assert.Single(fleet.AllTransport);
    }

    [Fact]
    public void ReadOnlyViewsCannotBypassAdmissionAndSnapshotsDoNotModifyFleet()
    {
        Fleet fleet = new Fleet(new FleetServiceCenter());
        Scooter scooter = new Scooter(0, 5, 6, "Самокат", "X");
        Assert.Throws<NotSupportedException>(() => ((IList<Transport>)fleet.AllTransport).Add(scooter));
        Assert.Throws<NotSupportedException>(() => ((IList<Equipment>)fleet.AllEquipment).Add(new Helmet("Шлем", "H")));
        fleet.TryAddTransport(scooter);
        fleet.TransportForBeginners().Clear();
        fleet.AllInventory().Clear();
        Assert.Equal(1, fleet.Count);
    }

    [Fact]
    public void EmptyFleetAndNullArguments()
    {
        Assert.Throws<ArgumentNullException>(() => new Fleet(null!));
        Fleet fleet = new Fleet(new FleetServiceCenter());
        Assert.Throws<ArgumentNullException>(() => fleet.TryAddTransport(null!));
        Assert.Throws<ArgumentNullException>(() => fleet.AddEquipment(null!));
        Assert.Throws<ArgumentNullException>(() => new FleetServiceCenter().Checking(null!));
        Assert.Empty(fleet.AllInventory());
        Assert.Empty(fleet.TransportForBeginners());
        Assert.Equal(0m, fleet.SumEnergy());
    }

    private class NewEnergyItem : Equipment, IEnergyConsumption
    {
        public decimal EnergyConsumption { get; }
        public NewEnergyItem(string name, string number, decimal energy) : base(name, number)
        { EnergyConsumption = energy; }
    }
}
