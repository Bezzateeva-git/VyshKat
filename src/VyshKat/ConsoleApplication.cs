using System.Globalization;

public class ConsoleApplication
{
    private readonly Fleet _fleet;
    private readonly TextReader _input;
    private readonly TextWriter _output;

    public ConsoleApplication(Fleet fleet, TextReader input, TextWriter output)
    {
        if (fleet is null)
        {
            throw new ArgumentNullException(nameof(fleet));
        }
        _fleet = fleet;
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }
        _input = input;
        if (output is null)
        {
            throw new ArgumentNullException(nameof(output));
        }
        _output = output;
    }

    public void Run()
    {
        try
        {
            bool isRunning = true;
            while (isRunning)
            {
                _output.WriteLine("\n1 — Добавить транспорт");
                _output.WriteLine("2 — Добавить вещь");
                _output.WriteLine("3 — Показать количество объектов");
                _output.WriteLine("4 — Показать весь инвентарь");
                _output.WriteLine("5 — Показать транспорт для новичков");
                _output.WriteLine("6 — Показать суточное энергопотребление");
                _output.WriteLine("0 — Завершить работу");

                int command = ReadIntInRange("Выберите действие: ", 0, 6);
                try
                {
                    switch (command)
                    {
                        case 1:
                            AddTransport(_fleet);
                            break;
                        case 2:
                            AddEquipment(_fleet);
                            break;
                        case 3:
                            _output.WriteLine($"Транспорт: {_fleet.AllTransport.Count}");
                            _output.WriteLine($"Вещи: {_fleet.AllEquipment.Count}");
                            _output.WriteLine($"Всего: {_fleet.AllTransport.Count + _fleet.AllEquipment.Count}");
                            break;
                        case 4:
                            PrintInventory(_fleet);
                            break;
                        case 5:
                            PrintBeginnerTransport(_fleet);
                            break;
                        case 6:
                            _output.WriteLine($"Суточное энергопотребление: {_fleet.SumEnergy()} кВт·ч");
                            break;
                        case 0:
                            isRunning = false;
                            break;
                    }
                }
                catch (ArgumentException exception)
                {
                    // После ошибки возвращаемся в меню.
                    _output.WriteLine($"Операция не выполнена: {exception.Message}");
                }
                catch (OverflowException)
                {
                    _output.WriteLine("Сумма слишком велика для типа decimal.");
                }
            }
            _output.WriteLine("Работа завершена.");
        }
        catch (EndOfStreamException)
        {
            // Поток ввода закончился.
            _output.WriteLine("\nВвод завершён. Работа программы завершена.");
        }
    }

    private string ReadLineOrExit()
    {
        string? input = _input.ReadLine();
        if (input is null)
        {
            throw new EndOfStreamException();
        }
        return input;
    }

    private int ReadIntInRange(string prompt, int min, int max)
    {
        while (true)
        {
            _output.Write(prompt);
            bool flag = int.TryParse(ReadLineOrExit(), out int result);
            if (flag && result >= min && result <= max)
            {
                return result;
            }

            _output.WriteLine($"Введите целое число от {min} до {max}.");
        }
    }

    private decimal ReadDecimalInRange(string prompt, decimal min, decimal max)
    {
        while (true)
        {
            _output.Write(prompt);
            // Разрешаем и 2,5, и 2.5.
            string input = ReadLineOrExit().Trim().Replace(',', '.');
            bool flag = decimal.TryParse(
                input,
                NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out decimal result);

            if (flag && result >= min && result <= max)
            {
                return result;
            }

            _output.WriteLine($"Введите число от {min} до {max}. Дробная часть — через точку или запятую.");
        }
    }

    private string ReadString(string prompt)
    {
        while (true)
        {
            _output.Write(prompt);
            string result = ReadLineOrExit();
            if (!string.IsNullOrWhiteSpace(result))
            {
                return result.Trim();
            }

            _output.WriteLine("Введите непустую строку, содержащую не только пробелы.");
        }
    }

    private void AddTransport(Fleet park)
    {
        _output.WriteLine("\n1 — Самокат\n2 — Велосипед\n3 — Электросамокат\n4 — Электровелосипед\n0 — Назад");
        int type = ReadIntInRange("Вид транспорта: ", 0, 4);
        if (type == 0)
        {
            return;
        }

        string name = ReadString("Название: ");
        string inventoryNumber = ReadString("Инвентарный номер: ");
        int serviceLife = ReadIntInRange("Полных лет эксплуатации (от 0): ", 0, int.MaxValue);
        int brakeQuality = ReadIntInRange("Качество тормозов (1–10): ", 1, 10);
        int ease = ReadIntInRange("Простота использования (1–10): ", 1, 10);

        Transport transport;
        switch (type)
        {
            case 1:
                transport = new Scooter(serviceLife, brakeQuality, ease, name, inventoryNumber);
                break;
            case 2:
                transport = new Bike(serviceLife, brakeQuality, ease, name, inventoryNumber);
                break;
            case 3:
            {
                decimal energy = ReadDecimalInRange("Суточный расход энергии, кВт·ч (от 0): ", 0m, decimal.MaxValue);
                transport = new ElectroScooter(serviceLife, brakeQuality, ease, name, inventoryNumber, energy);
                break;
            }
            case 4:
            {
                decimal energy = ReadDecimalInRange("Суточный расход энергии, кВт·ч (от 0): ", 0m, decimal.MaxValue);
                transport = new ElectroBike(serviceLife, brakeQuality, ease, name, inventoryNumber, energy);
                break;
            }
            default:
                return;
        }

        if (park.TryAddTransport(transport))
        {
            _output.WriteLine("Транспорт прошёл техосмотр и добавлен в парк.");
        }
        else
            _output.WriteLine("Транспорт не прошёл техосмотр и не был добавлен.");
    }

    private void AddEquipment(Fleet park)
    {
        _output.WriteLine("\n1 — Шлем\n2 — Док-станция с электропитанием\n3 — Зарядный шкаф\n0 — Назад");
        int type = ReadIntInRange("Вид вещи: ", 0, 3);
        if (type == 0)
        {
            return;
        }

        string name = ReadString("Название: ");
        string inventoryNumber = ReadString("Инвентарный номер: ");
        Equipment equipment;
        switch (type)
        {
            case 1:
                equipment = new Helmet(name, inventoryNumber);
                break;
            case 2:
            {
                decimal energy = ReadDecimalInRange("Собственный суточный расход, кВт·ч (от 0): ", 0m, decimal.MaxValue);
                equipment = new DockingStation(name, inventoryNumber, energy);
                break;
            }
            case 3:
            {
                decimal energy = ReadDecimalInRange("Собственный суточный расход, кВт·ч (от 0): ", 0m, decimal.MaxValue);
                equipment = new ChargingCabinet(name, inventoryNumber, energy);
                break;
            }
            default:
                return;
        }

        park.AddEquipment(equipment);
        _output.WriteLine("Вещь добавлена на баланс.");
    }

    private void PrintInventory(Fleet park)
    {
        if (park.AllTransport.Count == 0 && park.AllEquipment.Count == 0)
        {
            _output.WriteLine("На балансе пока нет объектов.");
            return;
        }

        _output.WriteLine("\nИнвентарный номер — название");
        foreach (IInventoryItem item in park.AllInventory())
            _output.WriteLine($"{item.InventoryNumber} — {item.Name}");
    }

    private void PrintBeginnerTransport(Fleet park)
    {
        int count = 0;
        List<Transport> tfb = park.TransportForBeginners();
        foreach (Transport transport in tfb)
        {
            _output.WriteLine($"{transport.InventoryNumber} — {transport.Name}, простота: {transport.Ease}");
            count++;
        }

        if (count == 0)
        {
            _output.WriteLine("Нет транспорта, подходящего новичкам.");
        }
        else
            _output.WriteLine($"Всего подходящих устройств: {count}");
    }


}
