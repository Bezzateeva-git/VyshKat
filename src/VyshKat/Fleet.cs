public class Fleet
{
    private readonly List<Transport> _transport = new List<Transport>();
    private readonly List<Equipment> _equipment = new List<Equipment>();
    private readonly IServiceCenter _service;

    public IReadOnlyList<Transport> AllTransport { get; }
    public IReadOnlyList<Equipment> AllEquipment { get; }
    public int Count
    {
        get { return _transport.Count; }
    }

    public Fleet(IServiceCenter service)
    {
        if (service is null)
        {
            throw new ArgumentNullException(nameof(service));
        }
        _service = service;
        AllTransport = _transport.AsReadOnly();
        AllEquipment = _equipment.AsReadOnly();
    }

    public bool TryAddTransport(Transport tr)
    {
        if (tr is null)
        {
            throw new ArgumentNullException(nameof(tr));
        }
        CheckInventoryNumber(tr);
        if (!_service.Checking(tr))
        {
            return false;
        }

        _transport.Add(tr);
        return true;
    }

    public void AddEquipment(Equipment eq)
    {
        if (eq is null)
        {
            throw new ArgumentNullException(nameof(eq));
        }
        CheckInventoryNumber(eq);
        _equipment.Add(eq);
    }

    private void CheckInventoryNumber(IInventoryItem candidate)
    {
        foreach (IInventoryItem item in AllInventory())
        {
            if (string.Equals(item.InventoryNumber, candidate.InventoryNumber,
                StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    $"Инвентарный номер «{candidate.InventoryNumber}» уже занят.",
                    nameof(candidate));
            }
        }
    }

    // Копия списка для отчёта.
    public List<IInventoryItem> AllInventory()
    {
        List<IInventoryItem> result = new List<IInventoryItem>();
        result.AddRange(_transport);
        result.AddRange(_equipment);
        return result;
    }

    public List<Transport> TransportForBeginners()
    {
        List<Transport> result = new List<Transport>();
        foreach (Transport tr in _transport)
        {
            if (tr.IsSuitableForBeginner())
            {
                result.Add(tr);
            }
        }
        return result;
    }

    public decimal SumEnergy()
    {
        decimal result = 0m;
        foreach (IInventoryItem item in AllInventory())
        {
            if (item is IEnergyConsumption consumer)
            {
                result += consumer.EnergyConsumption;
            }
        }
        return result;
    }
}
