namespace PokemonGo.Logger
{
	public static class Log
	{
		private static ILogger _logger;

		public static void SetLogger(ILogger logger)
		{
			_logger = logger;
		}

		public static void Write(string message, LogLevel level = LogLevel.Info)
		{
		    _logger?.Write(message, level);
		}
	}


}