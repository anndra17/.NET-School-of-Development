using ReadingList.Application.Abstractions.Logs;
using ReadingList.CLI.UI.Abstractions;

namespace ReadingList.CLI.Adapters
{
    public sealed class InfrastructureLoggerAdapter : ISystemLogger
    {
        private readonly IConsoleNotifier _ui;
        public InfrastructureLoggerAdapter(IConsoleNotifier ui) => _ui = ui;

        public void Info(string message) => _ui.Info(message);

        public void Warn(string message) => _ui.Warn(message);

        public void Error(string message) => _ui.Error(message);
    }
}
