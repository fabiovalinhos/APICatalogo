
namespace ApiCatalogo.Logging;

public class CustomLogger: ILogger
{
    readonly string loggerName;

    readonly CustomLoggingProviderConfiguration loggerConfig;


    public CustomLogger(string name, CustomLoggingProviderConfiguration config)
    {
        loggerName = name;
        loggerConfig = config;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel == loggerConfig.LogLevel;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        string message = 
            $"{logLevel.ToString()}: {eventId.Id} - {formatter(state, exception)}";

        EscreverTextoNoArquivo(message);
    }

    private void EscreverTextoNoArquivo(string mensagem)
    {
        string basePath = AppContext.BaseDirectory;

        string logFilePath = Path.Combine(basePath, "log", "aplicacao_log.txt");

        Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

        using (StreamWriter streamWriter = new StreamWriter(logFilePath, true))
        {
            try
            {
                streamWriter.WriteLine(mensagem);
                streamWriter.Close();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}