using ClientLib.Authentication;
using System.Configuration;
using System.Data;
using System.Windows;
using WPFUI.View;
using WPFUI.ViewModel;

namespace WPFUI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private static ISessionManager _sessionManager = new BearerSessionManager();
    private static AccountOperationManager _accountOperations = new AccountOperationManager(_sessionManager);

    private static MainWindow? _mainWindow;
    private static MainViewModel? _mainViewModel;


    public App()
    {
        Startup += AppStartup;
    }

    private void AppStartup(object sender, StartupEventArgs a)
    {
        _mainWindow = new MainWindow();
        _mainViewModel = new MainViewModel(_sessionManager, _accountOperations);
        _mainWindow.DataContext = _mainViewModel;
        _mainWindow.Show();
    }
}

