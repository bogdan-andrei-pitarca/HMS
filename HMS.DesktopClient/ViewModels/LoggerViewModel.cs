using HMS.DesktopClient.Utils;
using HMS.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HMS.DesktopClient.ViewModels
{
    /// <summary>
    /// View model for the system logging functionality, providing access to system logs and filtering capabilities.
    /// </summary>
    /// <remarks>
    /// This view model enables administrators to view and filter system logs based on various criteria
    /// such as user ID, action type, and timestamp. It provides a comprehensive auditing interface
    /// for monitoring system activities.
    /// </remarks>
    public class LoggerViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The service responsible for retrieving log data from the system.
        /// </summary>
        private readonly LoggerService _loggerService;

        /// <summary>
        /// Gets the collection of log entries to display in the UI.
        /// </summary>
        /// <remarks>
        /// This observable collection is automatically updated when log filtering is applied
        /// and notifies the UI of any changes.
        /// </remarks>
        public ObservableCollection<LogDto> logs { get; } = new ObservableCollection<LogDto>();

        /// <summary>
        /// Gets or sets the collection of available action types for filtering logs.
        /// </summary>
        /// <remarks>
        /// This collection is populated from the database with all distinct action types
        /// found in the logs.
        /// </remarks>
        private ObservableCollection<string> _actionTypes = new ObservableCollection<string>();
        
        /// <summary>
        /// Gets or sets the collection of action types that can be used for filtering logs.
        /// </summary>
        /// <remarks>
        /// When this property changes, UI elements bound to it are automatically updated.
        /// </remarks>
        public ObservableCollection<string> actionTypes
        {
            get => _actionTypes;
            private set
            {
                _actionTypes = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the user ID input for filtering logs by user.
        /// </summary>
        private string _userIdInput = string.Empty;
        
        /// <summary>
        /// Gets or sets the user ID string input for filtering logs by specific user.
        /// </summary>
        /// <remarks>
        /// This is the text entered by the user in the UI to filter logs by user ID.
        /// </remarks>
        public string user_id_input
        {
            get => _userIdInput;
            set
            {
                if (_userIdInput != value)
                {
                    _userIdInput = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected action type for filtering logs.
        /// </summary>
        private string _selectedActionType;
        
        /// <summary>
        /// Gets or sets the action type selected for filtering logs.
        /// </summary>
        /// <remarks>
        /// When a new action type is selected, the selection is logged for debugging
        /// and UI elements are updated accordingly.
        /// </remarks>
        public string selected_action_type
        {
            get => _selectedActionType;
            set
            {
                if (_selectedActionType != value)
                {
                    _selectedActionType = value;
                    System.Diagnostics.Debug.WriteLine($"Selected action type set to: {value}");
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected timestamp for filtering logs by date and time.
        /// </summary>
        private DateTime? _selectedTimestamp;
        
        /// <summary>
        /// Gets or sets the timestamp selected for filtering logs by date.
        /// </summary>
        /// <remarks>
        /// This allows filtering logs to show only entries from a specific date.
        /// </remarks>
        public DateTime? selected_timestamp
        {
            get => _selectedTimestamp;
            set
            {
                if (_selectedTimestamp != value)
                {
                    _selectedTimestamp = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the command for loading all logs without any filtering.
        /// </summary>
        /// <remarks>
        /// This command resets any active filters and shows all available logs.
        /// </remarks>
        public ICommand loadAllLogsCommand { get; }
        
        /// <summary>
        /// Gets the command for filtering logs by user ID.
        /// </summary>
        /// <remarks>
        /// This command uses the value in user_id_input to filter the logs.
        /// </remarks>
        public ICommand filterLogsByUserIdCommand { get; }
        
        /// <summary>
        /// Gets the command for filtering logs by action type.
        /// </summary>
        /// <remarks>
        /// This command uses the selected_action_type to filter the logs.
        /// </remarks>
        public ICommand filterLogsByActionTypeCommand { get; }
        
        /// <summary>
        /// Gets the command for filtering logs by timestamp.
        /// </summary>
        /// <remarks>
        /// This command uses the selected_timestamp to filter the logs.
        /// </remarks>
        public ICommand filterLogsByTimestampCommand { get; }
        
        /// <summary>
        /// Gets the command for applying all active filters simultaneously.
        /// </summary>
        /// <remarks>
        /// This command combines all specified filters (user ID, action type, and timestamp)
        /// to narrow down the log entries displayed.
        /// </remarks>
        public ICommand applyAllFiltersCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerViewModel"/> class.
        /// </summary>
        /// <param name="loggerService">The service used to retrieve log data.</param>
        /// <remarks>
        /// This constructor initializes the commands and sets up the logger service connection.
        /// </remarks>
        public LoggerViewModel(LoggerService loggerService)
        {
            _loggerService = loggerService;

            actionTypes = new ObservableCollection<string>();

            loadAllLogsCommand = new RelayCommand(async _ =>
            {
                var allLogs = await _loggerService.GetAllLogsAsync();
                UpdateLogsCollection(allLogs);
                await UpdateActionTypesAsync();
            });

            filterLogsByUserIdCommand = new RelayCommand(async _ =>
            {
                if (int.TryParse(user_id_input, out int userId))
                {
                    var filteredLogs = await _loggerService.GetLogsByUserIdAsync(userId);
                    UpdateLogsCollection(filteredLogs);
                }
            }, _ => true);

            filterLogsByActionTypeCommand = new RelayCommand(async _ =>
            {
                if (!string.IsNullOrEmpty(selected_action_type))
                {
                    System.Diagnostics.Debug.WriteLine($"Filtering by action type: {selected_action_type}");
                    var filteredLogs = await _loggerService.GetLogsByActionAsync(selected_action_type);
                    UpdateLogsCollection(filteredLogs);
                }
            }, _ => true);

            filterLogsByTimestampCommand = new RelayCommand(async _ =>
            {
                if (selected_timestamp.HasValue)
                {
                    var filteredLogs = await _loggerService.GetLogsByTimestampAsync(selected_timestamp.Value);
                    UpdateLogsCollection(filteredLogs);
                }
            }, _ => true);

            applyAllFiltersCommand = new RelayCommand(async _ =>
            {
                int? userId = null;
                if (int.TryParse(user_id_input, out int parsedUserId))
                {
                    userId = parsedUserId;
                }

                var filteredLogs = await _loggerService.GetLogsByMultipleFiltersAsync(
                    userId,
                    selected_action_type,
                    selected_timestamp);

                UpdateLogsCollection(filteredLogs);
            });
        }

        /// <summary>
        /// Initializes the action types collection asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method should be called when the view is first loaded to populate
        /// the action types dropdown.
        /// </remarks>
        public async Task InitializeActionTypesAsync()
        {
            await UpdateActionTypesAsync();
        }

        /// <summary>
        /// Updates the logs collection with a new set of log entries.
        /// </summary>
        /// <param name="newLogs">The new collection of logs to display.</param>
        /// <remarks>
        /// This method clears the existing logs and adds the new ones, ensuring
        /// that the UI is updated accordingly.
        /// </remarks>
        private void UpdateLogsCollection(IEnumerable<LogDto> newLogs)
        {
            logs.Clear();
            foreach (var log in newLogs)
            {
                logs.Add(log);
            }
        }

        /// <summary>
        /// Updates the available action types collection asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method retrieves all distinct action types from the database
        /// and updates the actionTypes collection.
        /// </remarks>
        private async Task UpdateActionTypesAsync()
        {
            var types = await _loggerService.GetActionTypesAsync();

            actionTypes.Clear();
            foreach (var type in types)
            {
                actionTypes.Add(type);
            }
        }

        #region INotifyPropertyChanged

        /// <summary>
        /// Event raised when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        /// <remarks>
        /// This method uses the CallerMemberName attribute to automatically determine
        /// the property name when not explicitly provided.
        /// </remarks>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    /// <summary>
    /// A simple implementation of the ICommand interface for use in view models.
    /// </summary>
    /// <remarks>
    /// This class enables the creation of commands that can be bound to UI elements
    /// and executed in response to user actions.
    /// </remarks>
    public class RelayCommand : ICommand
    {
        /// <summary>
        /// Determines whether the command can be executed.
        /// </summary>
        private readonly Predicate<object> _canExecute;
        
        /// <summary>
        /// The action to execute when the command is invoked.
        /// </summary>
        private readonly Action<object> _execute;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The action to execute when the command is invoked.</param>
        /// <param name="canExecute">The predicate that determines whether the command can be executed.</param>
        /// <remarks>
        /// If canExecute is null, the command can always be executed.
        /// </remarks>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Event raised when the ability to execute the command changes.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        /// <remarks>
        /// This method should be called when conditions that affect whether the command can execute change.
        /// </remarks>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Determines whether the command can be executed with the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter to use when determining whether the command can be executed.</param>
        /// <returns>True if the command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        /// <summary>
        /// Executes the command with the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter to pass to the command's execution method.</param>
        public void Execute(object parameter) => _execute(parameter);
    }
}