using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

public class ConsoleTests
{
    private static (Fleet Fleet, string Output) Run(string input)
    {
        ServiceCollection services = new ServiceCollection();
        DependencyInjection.Configure(services);
        using StringReader reader = new StringReader(input);
        using StringWriter writer = new StringWriter();
        services.RemoveAll<TextReader>();
        services.RemoveAll<TextWriter>();
        services.AddSingleton<TextReader>(reader);
        services.AddSingleton<TextWriter>(writer);
        using var provider = services.BuildServiceProvider();
        var fleet = provider.GetRequiredService<Fleet>();
        provider.GetRequiredService<ConsoleApplication>().Run();
        return (fleet, writer.ToString());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void CanEnterEveryTransportTypeAndReadReports(int type)
    {
        string energy = type >= 3 ? "2,5\n" : "";
        var result = Run($"1\n{type}\nТранспорт\nT-1\n0\n5\n6\n{energy}3\n4\n5\n6\n0\n");
        Assert.Single(result.Fleet.AllTransport);
        Assert.Equal(type >= 3 ? 2.5m : 0m, result.Fleet.SumEnergy());
        Assert.Contains("Транспорт прошёл техосмотр", result.Output);
        Assert.Contains("T-1 — Транспорт", result.Output);
        Assert.Contains("Всего подходящих устройств: 1", result.Output);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void CanEnterEveryEquipmentType(int type)
    {
        string energy = type >= 2 ? "3.5\n" : "";
        var result = Run($"2\n{type}\nВещь\nE-1\n{energy}4\n6\n0\n");
        Assert.Single(result.Fleet.AllEquipment);
        Assert.Equal(type >= 2 ? 3.5m : 0m, result.Fleet.SumEnergy());
        Assert.Contains("E-1 — Вещь", result.Output);
    }

    [Fact]
    public void InvalidInputIsRetriedWithoutRestartingWholeEntry()
    {
        var result = Run("abc\n7\n1\n3\n   \nСамокат\n\nS\n-1\n0\n0\n11\n5\n0\n6\noops\n-2\n2.5\n0\n");
        Assert.Single(result.Fleet.AllTransport);
        Assert.Equal(2.5m, result.Fleet.SumEnergy());
        Assert.Contains("Введите непустую строку", result.Output);
        Assert.Contains("Введите целое число", result.Output);
    }

    [Fact]
    public void EmptyReportsAndSubmenuCancellationWork()
    {
        var result = Run("3\n4\n5\n6\n1\n0\n2\n0\n0\n");
        Assert.Empty(result.Fleet.AllInventory());
        Assert.Contains("На балансе пока нет объектов", result.Output);
        Assert.Contains("Нет транспорта, подходящего новичкам", result.Output);
        Assert.Contains("Суточное энергопотребление: 0", result.Output);
    }

    [Fact]
    public void OldTransportIsRejectedAndDuplicateNumberDoesNotStopMenu()
    {
        var result = Run("1\n1\nСтарый\nOLD\n11\n5\n6\n2\n1\nШлем\nH\n2\n1\nДругой\nh\n3\n0\n");
        Assert.Empty(result.Fleet.AllTransport);
        Assert.Single(result.Fleet.AllEquipment);
        Assert.Contains("не прошёл техосмотр", result.Output);
        Assert.Contains("уже занят", result.Output);
        Assert.Contains("Вещи: 1", result.Output);
    }

    [Theory]
    [InlineData("")]
    [InlineData("1\n1\n")]
    [InlineData("1\n3\nИмя\nN\n0\n5\n6\n")]
    public void EndOfInputTerminatesWithoutAddingIncompleteObject(string input)
    {
        var result = Run(input);
        Assert.Empty(result.Fleet.AllInventory());
        Assert.Contains("Ввод завершён", result.Output);
    }

    [Fact]
    public void EnergyOverflowIsReportedAndMenuContinues()
    {
        string max = decimal.MaxValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
        var result = Run($"2\n3\nПервый\nC1\n{max}\n2\n3\nВторой\nC2\n1\n6\n0\n");
        Assert.Contains("Сумма слишком велика", result.Output);
        Assert.Contains("Работа завершена", result.Output);
    }
}
