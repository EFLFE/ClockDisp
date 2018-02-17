using System.Windows;
using ClockDisp.Register;

namespace ClockDisp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
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
