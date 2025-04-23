using ClientLib;
using ClientLib.Authentication;
using ClientLib.DataManagers;
using ClientLib.Persistance;
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
    private readonly IParticipantManager _participantManager;
    private readonly IVoteAdministrationManager _adminManager;
    private readonly IKeyManager _keyManager;
    private readonly ILocalVoteDataAccess _localDataAccess;
    private readonly IVoteManager _voteManager;
    private readonly ShufflerApiWakeupManager _wakeupManager;

    private MainWindow? _mainWindow;
    private MainViewModel? _mainViewModel;


    public App()
    {
        _sessionManager = new BearerSessionManager();
        _accountOperations = new ApiAccountOperationManager(_sessionManager);
        _pollManager = new ApiPollManager(_sessionManager);
        _participantManager = new ApiParticipantManager(_sessionManager);
        _adminManager = new ApiVoteAdministrationManager(_sessionManager);
        _keyManager = new KeyManager(_sessionManager);
        _localDataAccess = new XMLVoteDataAccess();
        _voteManager = new ApiVoteManager(_adminManager, _keyManager, _localDataAccess);
        _wakeupManager = new ShufflerApiWakeupManager();
        Startup += AppStartup;
    }

    private void AppStartup(object sender, StartupEventArgs a)
    {
        _mainWindow = new MainWindow();
        _mainViewModel = new MainViewModel(_accountOperations, _pollManager, _participantManager, _adminManager, _keyManager, _voteManager, _wakeupManager);
        _mainWindow.DataContext = _mainViewModel;
        _mainWindow.Show();
    }
}

