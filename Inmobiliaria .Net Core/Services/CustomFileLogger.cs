public class CustomFileLogger : ILogger
{
	private readonly string _filePath;
	private readonly string _categoryName;

	public CustomFileLogger(string categoryName)
	{
		_categoryName = categoryName;
		var date = DateTime.Now.ToString("yyyy-MM-dd");
		_filePath = Path.Combine("Logs", $"{date}.log");
		Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);  // Crea la carpeta si no existe
	}

	public IDisposable? BeginScope<TState>(TState state) where TState : notnull
	{
		return null;  // No soporta scopes por simplicidad
	}

	public bool IsEnabled(LogLevel logLevel)
	{
		return logLevel == LogLevel.Error;
	}

	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
	{
		if (!IsEnabled(logLevel))
		{
			return;
		}

		string message = formatter(state, exception);
		string logMessage = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} [{logLevel}] [{_categoryName}] {message}";
		if (exception != null)
		{
			logMessage += $"\n{exception}";
		}

		File.AppendAllText(_filePath, logMessage + Environment.NewLine);
	}
}