public interface IEnergyConsumption
{
    // суточный расход в кВт·ч
    public decimal EnergyConsumption { get; }
}

public interface IInventoryItem
{
    public string InventoryNumber { get; }
    public string Name { get; }
}

public abstract class Equipment : IInventoryItem
{
    public string Name { get; }
    public string InventoryNumber { get; }
    protected Equipment(string name, string inventoryNumber)
    {
        if (string.IsNullOrWhiteSpace(inventoryNumber))
        {
            throw new ArgumentException("Значение не должно быть пустым или состоять из пробелов.", nameof(inventoryNumber));
        }
        InventoryNumber = inventoryNumber.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Значение не должно быть пустым или состоять из пробелов.", nameof(name));
        }
        Name = name.Trim();
    }

}


public class Helmet : Equipment
{
    public Helmet(string name, string inventoryNumber) : base(name, inventoryNumber){}
}


public class DockingStation : Equipment, IEnergyConsumption{
    public decimal EnergyConsumption { get; }
    public DockingStation(string name, string inventoryNumber, decimal energyConsumption) : base(name, inventoryNumber)
    {
        if (energyConsumption < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(energyConsumption), "Значение не может быть отрицательным.");
        }
        EnergyConsumption = energyConsumption;
    }
}
public class ChargingCabinet : Equipment, IEnergyConsumption{
    public decimal EnergyConsumption { get; }
    public ChargingCabinet(string name, string inventoryNumber, decimal energyConsumption) : base(name, inventoryNumber)
    {
        if (energyConsumption < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(energyConsumption), "Значение не может быть отрицательным.");
        }
        EnergyConsumption = energyConsumption;
    }
}

public abstract class Transport : IInventoryItem
{

    public string Name { get; }
    // простота (для новичка или только для опытного - число от 1 до 10)
    public int Ease { get; }
    public string InventoryNumber { get; }
    // качество тормозов - число от 1 до 10, чем ниже, тем тормоза хуже
    public int BrakeQuality { get; }
    // фактическое количество полных лет эксплуатации
    public int ServiceLife { get; }
    protected Transport(int serviceLife, int brakeQuality, int ease, string name, string inventoryNumber)
    {
        if (serviceLife < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(serviceLife), "Значение не может быть отрицательным.");
        }
        ServiceLife = serviceLife;
        if (brakeQuality <= 0 || brakeQuality > 10)
        {
            throw new ArgumentOutOfRangeException(nameof(brakeQuality), "Допустимы значения от 1 до 10.");
        }
        BrakeQuality = brakeQuality;
        if (ease < 1 || ease > 10)
        {
            throw new ArgumentOutOfRangeException(nameof(ease), "Допустимы значения от 1 до 10.");
        }
        Ease = ease;
        if (string.IsNullOrWhiteSpace(inventoryNumber))
        {
            throw new ArgumentException("Значение не должно быть пустым или состоять из пробелов.", nameof(inventoryNumber));
        }
        InventoryNumber = inventoryNumber.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Значение не должно быть пустым или состоять из пробелов.", nameof(name));
        }
        Name = name.Trim();

    }
    public bool IsSuitableForBeginner()
    {
        if (Ease >= 6)
        {
            return true;
        }
        return false;
    }

}
public class Scooter : Transport
{
    public Scooter(int serviceLife, int brakeQuality, int ease, string name, string inventoryNumber) : base(serviceLife, brakeQuality, ease, name, inventoryNumber){}
}

public class Bike : Transport
{
    public Bike(int serviceLife, int brakeQuality, int ease, string name, string inventoryNumber) : base(serviceLife,brakeQuality, ease, name, inventoryNumber){}
}

public class ElectroScooter : Transport, IEnergyConsumption{
    public decimal EnergyConsumption { get; }
    public ElectroScooter (int serviceLife, int brakeQuality, int ease, string name, string inventoryNumber, decimal energyConsumption) : base(serviceLife, brakeQuality, ease, name, inventoryNumber)
    {
        if (energyConsumption < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(energyConsumption), "Значение не может быть отрицательным.");
        }
        EnergyConsumption = energyConsumption;
    }
}

public class ElectroBike : Transport, IEnergyConsumption{
    public decimal EnergyConsumption { get; }
    public ElectroBike (int serviceLife, int brakeQuality, int ease, string name, string inventoryNumber, decimal energyConsumption) : base(serviceLife, brakeQuality, ease, name, inventoryNumber)
    {
        if (energyConsumption < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(energyConsumption), "Значение не может быть отрицательным.");
        }
        EnergyConsumption = energyConsumption;
    }
}
