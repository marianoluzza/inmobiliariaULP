public static class FileLoggerExtensions
{
	public static ILoggingBuilder AddFileLogger(this ILoggingBuilder builder)
	{
		builder.Services.AddSingleton<ILoggerProvider>(new CustomFileLoggerProvider());
		return builder;
	}
}