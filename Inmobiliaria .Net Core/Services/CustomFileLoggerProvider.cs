public class CustomFileLoggerProvider : ILoggerProvider
{

	public CustomFileLoggerProvider()
	{
	}

	public ILogger CreateLogger(string categoryName)
	{
		return new CustomFileLogger(categoryName);
	}

	public void Dispose()
	{
		// Limpieza si es necesario
	}
}