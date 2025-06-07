using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMS.Shared.DTOs;
using HMS.Shared.Proxies.Implementations;
using System.Collections.ObjectModel;
using System.Net.Http;
using HMS.Shared.Enums;
using System.ComponentModel;
using System.Windows.Input;
using System.Globalization;

namespace HMS.DesktopClient.ViewModels
{
    /// <summary>
    /// View model for managing doctor shifts in the hospital management system.
    /// </summary>
    /// <remarks>
    /// This view model provides functionality for administrators to create, view, update, and delete
    /// shifts for doctors. It handles all the data binding and command logic required by the UI.
    /// </remarks>
    public class ShiftViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The proxy service used to communicate with the shift-related API endpoints.
        /// </summary>
        private readonly ShiftProxy _shiftProxy;

        /// <summary>
        /// Gets or sets the role of the current user accessing the shifts functionality.
        /// </summary>
        /// <remarks>
        /// 0: Admin - full access to manage shifts
        /// 1: Doctor - read-only access to shifts
        /// 2: Patient - no access to shifts
        /// </remarks>
        public int UserRole { get; set; } // 0: Admin, 1: Doctor, 2: Patient

        /// <summary>
        /// Gets or sets the collection of shifts to display in the UI.
        /// </summary>
        /// <remarks>
        /// This observable collection automatically notifies the UI when shifts are added, 
        /// removed, or modified.
        /// </remarks>
        public ObservableCollection<ShiftDto> Shifts { get; set; } = new ObservableCollection<ShiftDto>();

        /// <summary>
        /// Event that is raised when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Gets or sets the date selected for a new or edited shift.
        /// </summary>
        private DateTimeOffset? _inputShiftDate;
        
        /// <summary>
        /// Gets or sets the date for the shift being created or edited.
        /// </summary>
        /// <remarks>
        /// When this value changes, the associated commands' CanExecute status is updated.
        /// </remarks>
        public DateTimeOffset? InputShiftDate
        {
            get => _inputShiftDate;
            set
            {
                if (_inputShiftDate != value)
                {
                    _inputShiftDate = value;
                    OnPropertyChanged(nameof(InputShiftDate));
                    AddShiftCommand.RaiseCanExecuteChanged();
                    UpdateShiftCommand.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the start time string for a new or edited shift.
        /// </summary>
        private string _inputShiftStartTimeString = string.Empty;
        
        /// <summary>
        /// Gets or sets the start time for the shift being created or edited as a string in format "HH:mm".
        /// </summary>
        /// <remarks>
        /// When this value changes, the associated commands' CanExecute status is updated.
        /// </remarks>
        public string InputShiftStartTimeString
        {
            get => _inputShiftStartTimeString;
            set
            {
                if (_inputShiftStartTimeString != value)
                {
                    _inputShiftStartTimeString = value;
                    OnPropertyChanged(nameof(InputShiftStartTimeString));
                    AddShiftCommand.RaiseCanExecuteChanged();
                    UpdateShiftCommand.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the end time string for a new or edited shift.
        /// </summary>
        private string _inputShiftEndTimeString = string.Empty;
        
        /// <summary>
        /// Gets or sets the end time for the shift being created or edited as a string in format "HH:mm".
        /// </summary>
        /// <remarks>
        /// When this value changes, the associated commands' CanExecute status is updated.
        /// </remarks>
        public string InputShiftEndTimeString
        {
            get => _inputShiftEndTimeString;
            set
            {
                if (_inputShiftEndTimeString != value)
                {
                    _inputShiftEndTimeString = value;
                    OnPropertyChanged(nameof(InputShiftEndTimeString));
                    AddShiftCommand.RaiseCanExecuteChanged();
                    UpdateShiftCommand.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the comma-separated list of doctor IDs assigned to a new or edited shift.
        /// </summary>
        private string _inputShiftDoctorIdsString = string.Empty;
        
        /// <summary>
        /// Gets or sets the comma-separated list of doctor IDs for the shift being created or edited.
        /// </summary>
        /// <remarks>
        /// When this value changes, the associated commands' CanExecute status is updated.
        /// </remarks>
        public string InputShiftDoctorIdsString
        {
            get => _inputShiftDoctorIdsString;
            set
            {
                if (_inputShiftDoctorIdsString != value)
                {
                    _inputShiftDoctorIdsString = value;
                    OnPropertyChanged(nameof(InputShiftDoctorIdsString));
                    AddShiftCommand.RaiseCanExecuteChanged();
                    UpdateShiftCommand.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the currently selected shift for viewing or editing.
        /// </summary>
        private ShiftDto? _selectedShift;
        
        /// <summary>
        /// Gets or sets the shift that is currently selected in the UI.
        /// </summary>
        /// <remarks>
        /// When a shift is selected, its details are loaded into the input fields for editing.
        /// When deselected (set to null), the input fields are cleared.
        /// </remarks>
        public ShiftDto? SelectedShift
        {
            get => _selectedShift;
            set
            {
                if (_selectedShift != value)
                {
                    _selectedShift = value;
                    OnPropertyChanged(nameof(SelectedShift));
                    if (_selectedShift != null)
                    {
                        InputShiftDate = new DateTimeOffset(_selectedShift.Date.ToDateTime(TimeOnly.MinValue));
                        InputShiftStartTimeString = _selectedShift.StartTime.ToString("HH:mm");
                        InputShiftEndTimeString = _selectedShift.EndTime.ToString("HH:mm");
                    }
                    else
                    {
                        ClearInputFields();
                    }
                    UpdateShiftCommand.RaiseCanExecuteChanged();
                    DeleteShiftCommand.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Gets a formatted string representation of the doctor IDs assigned to the selected shift.
        /// </summary>
        /// <remarks>
        /// Returns "None" if no shift is selected or if the selected shift has no assigned doctors.
        /// </remarks>
        public string FormattedDoctorIds => SelectedShift?.FormattedDoctorIds ?? "None";

        /// <summary>
        /// Gets the command for adding a new shift.
        /// </summary>
        public RelayCommand AddShiftCommand { get; }
        
        /// <summary>
        /// Gets the command for updating an existing shift.
        /// </summary>
        public RelayCommand UpdateShiftCommand { get; }
        
        /// <summary>
        /// Gets the command for deleting an existing shift.
        /// </summary>
        public RelayCommand DeleteShiftCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShiftViewModel"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client to use for API requests.</param>
        /// <param name="token">The authentication token for API authorization.</param>
        /// <remarks>
        /// This constructor initializes the proxy service and sets up the commands used by the UI.
        /// </remarks>
        public ShiftViewModel(HttpClient httpClient, string token)
        {
            _shiftProxy = new ShiftProxy(httpClient, token);

            AddShiftCommand = new RelayCommand(async _ => await AddShiftAsync(), _ => CanAddShift());
            UpdateShiftCommand = new RelayCommand(async _ => await UpdateShiftAsync(), _ => CanUpdateDeleteShift());
            DeleteShiftCommand = new RelayCommand(async _ => await DeleteShiftAsync(), _ => CanUpdateDeleteShift());
        }

        /// <summary>
        /// Determines whether the current user can add a new shift.
        /// </summary>
        /// <returns>True if the user is an admin and all required fields have valid values; otherwise, false.</returns>
        private bool CanAddShift()
        {
            return UserRole == 0 && 
                   InputShiftDate.HasValue &&
                   TimeOnly.TryParseExact(InputShiftStartTimeString, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out _ ) &&
                   TimeOnly.TryParseExact(InputShiftEndTimeString, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out _ );
        }

        /// <summary>
        /// Determines whether the current user can update or delete the selected shift.
        /// </summary>
        /// <returns>True if the user is an admin and a shift is currently selected; otherwise, false.</returns>
        private bool CanUpdateDeleteShift()
        {
            return UserRole == 0 && SelectedShift != null;
        }

        /// <summary>
        /// Clears all input fields used for creating or editing shifts.
        /// </summary>
        /// <remarks>
        /// This method is called after an operation is completed or when deselecting a shift.
        /// </remarks>
        private void ClearInputFields()
        {
            InputShiftDate = null;
            InputShiftStartTimeString = string.Empty;
            InputShiftEndTimeString = string.Empty;
        }

        /// <summary>
        /// Loads all shifts from the database if the current user is an administrator.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method handles exceptions that might occur during the data loading process.
        /// </remarks>
        public async Task LoadShiftsAsync()
        {
            if (UserRole == 0)
            {
                try
                {
                    var allShifts = await _shiftProxy.GetAllAsync();
                    Shifts.Clear();
                    foreach (var shift in allShifts)
                    {
                        Shifts.Add(shift);
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    Console.WriteLine($"HTTP request error loading shifts: {httpEx.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred loading shifts: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Adds a new shift to the database using the values from the input fields.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method validates the input, creates a new shift, and adds it to the database.
        /// It handles exceptions that might occur during the process.
        /// </remarks>
        public async Task AddShiftAsync()
        {
            if (!CanAddShift()) return;
            try
            {
                DateOnly date = DateOnly.FromDateTime(InputShiftDate!.Value.Date);
                TimeOnly startTime = TimeOnly.ParseExact(InputShiftStartTimeString, "HH:mm", CultureInfo.InvariantCulture);
                TimeOnly endTime = TimeOnly.ParseExact(InputShiftEndTimeString, "HH:mm", CultureInfo.InvariantCulture);

                var newShift = new ShiftDto
                {
                    Date = date,
                    StartTime = startTime,
                    EndTime = endTime,
                    DoctorIds = new List<int>() // Initialize with empty list
                };

                var addedShift = await _shiftProxy.AddAsync(newShift);
                Shifts.Add(addedShift);
                ClearInputFields();
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP request error adding shift: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred adding shift: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the currently selected shift in the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method sends the updated shift data to the server and handles exceptions
        /// that might occur during the process.
        /// </remarks>
        public async Task UpdateShiftAsync()
        {
            if (!CanUpdateDeleteShift() || SelectedShift == null) return;
            try
            {
                // Update the selected shift with new values
                SelectedShift.Date = DateOnly.FromDateTime(InputShiftDate!.Value.Date);
                SelectedShift.StartTime = TimeOnly.ParseExact(InputShiftStartTimeString, "HH:mm", CultureInfo.InvariantCulture);
                SelectedShift.EndTime = TimeOnly.ParseExact(InputShiftEndTimeString, "HH:mm", CultureInfo.InvariantCulture);

                bool success = await _shiftProxy.UpdateAsync(SelectedShift);

                if (success)
                {
                    ClearInputFields();
                }
                else
                {
                    Console.WriteLine($"Update failed for shift with ID {SelectedShift.Id} on backend.");
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP request error updating shift: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred updating shift: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes the currently selected shift from the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method sends a delete request to the server and, if successful, removes the shift
        /// from the local collection. It handles exceptions that might occur during the process.
        /// </remarks>
        public async Task DeleteShiftAsync()
        {
            if (!CanUpdateDeleteShift() || SelectedShift == null) return;
            try
            {
                bool success = await _shiftProxy.DeleteAsync(SelectedShift.Id);

                if (success)
                {
                    Shifts.Remove(SelectedShift);
                    ClearInputFields();
                }
                else
                {
                    Console.WriteLine($"Delete failed for shift with ID {SelectedShift.Id} on backend.");
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP request error deleting shift: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred deleting shift: {ex.Message}");
            }
        }
    }
}
