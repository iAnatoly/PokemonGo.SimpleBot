namespace PokemonGo.Logger
{
	public interface ILogger
	{
		void Write(string message, LogLevel level = LogLevel.Info);
	}
}