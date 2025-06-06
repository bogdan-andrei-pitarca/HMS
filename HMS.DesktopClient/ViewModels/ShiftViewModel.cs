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
    public class ShiftViewModel : INotifyPropertyChanged
    {
        private readonly ShiftProxy _shiftProxy;

        public int UserRole { get; set; } // 0: Admin, 1: Doctor, 2: Patient

        public ObservableCollection<ShiftDto> Shifts { get; set; } = new ObservableCollection<ShiftDto>();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private DateTimeOffset? _inputShiftDate;
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

        private string _inputShiftStartTimeString = string.Empty;
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

        private string _inputShiftEndTimeString = string.Empty;
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

        private ShiftDto? _selectedShift;
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

        public RelayCommand AddShiftCommand { get; }
        public RelayCommand UpdateShiftCommand { get; }
        public RelayCommand DeleteShiftCommand { get; }

        public ShiftViewModel(HttpClient httpClient, string token)
        {
            _shiftProxy = new ShiftProxy(httpClient, token);

            AddShiftCommand = new RelayCommand(async _ => await AddShiftAsync(), _ => CanAddShift());
            UpdateShiftCommand = new RelayCommand(async _ => await UpdateShiftAsync(), _ => CanUpdateDeleteShift());
            DeleteShiftCommand = new RelayCommand(async _ => await DeleteShiftAsync(), _ => CanUpdateDeleteShift());
        }

        private bool CanAddShift()
        {
            return UserRole == 0 && 
                   InputShiftDate.HasValue &&
                   TimeOnly.TryParseExact(InputShiftStartTimeString, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out _ ) &&
                   TimeOnly.TryParseExact(InputShiftEndTimeString, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out _ );
        }

        private bool CanUpdateDeleteShift()
        {
            return UserRole == 0 && SelectedShift != null;
        }

        private void ClearInputFields()
        {
            InputShiftDate = null;
            InputShiftStartTimeString = string.Empty;
            InputShiftEndTimeString = string.Empty;
        }

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
