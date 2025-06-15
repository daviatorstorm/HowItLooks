namespace HowItLooks.Services;

public class FileLoggerService
{
    private object _lock = new();
    private readonly string _path;

    public FileLoggerService(string path)
    {
        _path = path;
    }

    public void Write(string message)
    {
        lock (_lock)
        {
            File.AppendAllText(
                $"{DateTime.Now.ToString("dd.MM.yyyy mm:hh:ss:ff")}: {message}", _path);
        }
    }
}
