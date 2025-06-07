using HMS.Shared.DTOs;
using HMS.Shared.Proxies.Implementations;
using HMS.Shared.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.DesktopClient.ViewModels.Doctor
{
    /// <summary>
    /// View model for the doctor profile screen that provides doctor information and editing capabilities.
    /// </summary>
    public class DoctorProfileViewModel
    {
        private readonly UserWithTokenDto _user;
        private DoctorDto _doctor;
        private readonly DoctorService _doctorService;

        /// <summary>
        /// Event that is fired when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoctorProfileViewModel"/> class.
        /// </summary>
        /// <param name="user">The authenticated user with token.</param>
        /// <param name="doctor">The doctor data to display and edit.</param>
        public DoctorProfileViewModel(UserWithTokenDto user, DoctorDto doctor)
        {
            _user = user;
            _doctor = doctor;

            var proxy = new DoctorProxy(_user.Token);
            _doctorService = new DoctorService(proxy);
        }

        /// <summary>
        /// Gets or sets the name of the doctor.
        /// </summary>
        public string Name
        {
            get => _doctor.Name;
            set
            {
                if (_doctor.Name != value)
                {
                    _doctor.Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        /// <summary>
        /// Gets or sets the phone number of the doctor.
        /// </summary>
        public string PhoneNumber
        {
            get => _doctor.PhoneNumber;
            set
            {
                if (_doctor.PhoneNumber != value)
                {
                    _doctor.PhoneNumber = value;
                    OnPropertyChanged(nameof(PhoneNumber));
                }
            }
        }

        /// <summary>
        /// Gets or sets the CNP (Personal Numeric Code) of the doctor.
        /// </summary>
        public string CNP
        {
            get => _doctor.CNP;
            set
            {
                if (_doctor.CNP != value)
                {
                    _doctor.CNP = value;
                    OnPropertyChanged(nameof(CNP));
                }
            }
        }

        /// <summary>
        /// Gets or sets the department ID where the doctor works.
        /// </summary>
        public int DepartmentId
        {
            get => _doctor.DepartmentId;
            set
            {
                if (_doctor.DepartmentId != value)
                {
                    _doctor.DepartmentId = value;
                    OnPropertyChanged(nameof(DepartmentId));
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the department where the doctor works.
        /// </summary>
        public string DepartmentName
        {
            get => _doctor.DepartmentName;
            set
            {
                if (_doctor.DepartmentName != value)
                {
                    _doctor.DepartmentName = value;
                    OnPropertyChanged(nameof(DepartmentName));
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of years of experience the doctor has.
        /// </summary>
        public int YearsOfExperience
        {
            get => _doctor.YearsOfExperience;
            set
            {
                if (_doctor.YearsOfExperience != value)
                {
                    _doctor.YearsOfExperience = value;
                    OnPropertyChanged(nameof(YearsOfExperience));
                }
            }
        }

        /// <summary>
        /// Gets or sets the license number of the doctor.
        /// </summary>
        public string LicenseNumber
        {
            get => _doctor.LicenseNumber;
            set
            {
                if (_doctor.LicenseNumber != value)
                {
                    _doctor.LicenseNumber = value;
                    OnPropertyChanged(nameof(LicenseNumber));
                }
            }
        }

        // Read-only
        /// <summary>
        /// Gets the unique identifier of the doctor.
        /// </summary>
        public int Id => _doctor.Id;

        /// <summary>
        /// Gets the email address of the doctor.
        /// </summary>
        public string Email => _doctor.Email;

        /// <summary>
        /// Gets the role of the user (doctor) in the system.
        /// </summary>
        public string Role => _doctor.Role.ToString();

        /// <summary>
        /// Gets the account creation date and time of the doctor.
        /// </summary>
        public string CreatedAt => _doctor.CreatedAt.ToString("yyyy-MM-dd HH:mm");

        /// <summary>
        /// Gets the authentication token of the user.
        /// </summary>
        public string Token => _user.Token;

        /// <summary>
        /// Updates the doctor information asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous update operation. The task result contains a boolean value indicating whether the update was successful.</returns>
        public async Task<bool> UpdateDoctorAsync()
        {
            return await _doctorService.UpdateDoctorAsync(_doctor);
        }

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
