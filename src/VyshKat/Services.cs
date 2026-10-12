public interface IServiceCenter
{
    bool Checking(Transport tr);
}

public class FleetServiceCenter : IServiceCenter
{
    public bool Checking(Transport tr)
    {
        if (tr is null)
        {
            throw new ArgumentNullException(nameof(tr));
        }
        if (tr.ServiceLife > 10 || tr.BrakeQuality < 5)
        {
            return false;
        }
        return true;
    }
}
