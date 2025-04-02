using ClientLib;
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
    private readonly ISessionManager _sessionManager;
    private readonly IAccountOperationManager _accountOperations;
    private readonly IPollManager _pollManager;

    private MainWindow? _mainWindow;
    private MainViewModel? _mainViewModel;


    public App()
    {
        _sessionManager = new BearerSessionManager();
        _accountOperations = new ApiAccountOperationManager(_sessionManager);
        _pollManager = new ApiPollManager(_sessionManager);
        Startup += AppStartup;
    }

    private void AppStartup(object sender, StartupEventArgs a)
    {
        _mainWindow = new MainWindow();
        _mainViewModel = new MainViewModel(_accountOperations, _pollManager);
        _mainWindow.DataContext = _mainViewModel;
        _mainWindow.Show();
    }
}

