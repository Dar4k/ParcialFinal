using Domain.Interfaces;

namespace Infrastructure.Logging;

public class LoggerAdapter : ILogger
{
    public void Log(string message)
    {
        Logger.Log(message); // delega al logger real
    }
}