using System.Windows;
using ClockDisp.P543Data;

namespace ClockDisp
{
    public partial class App : Application
    {
        public const string VERSION = "1.1.1";

        public App()
        {
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Compot.ThreadActive = false;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Compot.ThreadActive = false;
            base.OnExit(e);
        }

    }
}
