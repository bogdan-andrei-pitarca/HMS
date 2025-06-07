using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMS.Shared.DTOs;
using HMS.Shared.DTOs.Patient;
using HMS.Shared.Proxies.Implementations;
using HMS.Shared.Services;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using HMS.Shared.Entities;

namespace HMS.DesktopClient.ViewModels
{
    /// <summary>
    /// View model for the admin profile screen that provides admin information and editing capabilities.
    /// </summary>
    public class AdminProfileViewModel : INotifyPropertyChanged
    {
        private readonly UserWithTokenDto _user;  // holds token and user info
        private AdminDto _admin;                  // holds admin-specific + user info (except token)
        private readonly AdminService _adminService;

        /// <summary>
        /// Event that is fired when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminProfileViewModel"/> class.
        /// </summary>
        /// <param name="user">The authenticated user with token.</param>
        /// <param name="admin">The admin data to display and edit.</param>
        public AdminProfileViewModel(UserWithTokenDto user, AdminDto admin)
        {
            _user = user;
            _admin = admin;
            var proxy = new AdminProxy(_user.Token);
            _adminService = new AdminService(proxy);
        }

        /// <summary>
        /// Gets or sets the name of the admin.
        /// </summary>
        public string Name
        {
            get => _admin.Name ?? "";
            set
            {
                if (_admin.Name != value)
                {
                    _admin.Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        /// <summary>
        /// Gets or sets the phone number of the admin.
        /// </summary>
        public string PhoneNumber
        {
            get => _admin.PhoneNumber ?? "";
            set
            {
                if (_admin.PhoneNumber != value)
                {
                    _admin.PhoneNumber = value;
                    OnPropertyChanged(nameof(PhoneNumber));
                }
            }
        }

        /// <summary>
        /// Gets or sets the email address of the admin.
        /// </summary>
        public string Email
        {
            get => _admin.Email ?? "";
            set
            {
                if (_admin.Email != value)
                {
                    _admin.Email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

        /// <summary>
        /// Gets or sets the CNP (Personal Numeric Code) of the admin.
        /// </summary>
        public string CNP
        {
            get => _admin.CNP ?? "";
            set
            {
                if (_admin.CNP != value)
                {
                    _admin.CNP = value;
                    OnPropertyChanged(nameof(CNP));
                }
            }
        }

        /// <summary>
        /// Gets the ID of the admin.
        /// </summary>
        public int Id => _admin.Id;

        /// <summary>
        /// Gets the role of the admin.
        /// </summary>
        public string Role => _admin.Role.ToString();

        /// <summary>
        /// Gets the creation date and time of the admin account.
        /// </summary>
        public string CreatedAt => _admin.CreatedAt.ToString("yyyy-MM-dd HH:mm");


        /// <summary>
        /// Gets the token used for authenticating the admin.
        /// </summary>
        public string Token => _user.Token;

        private string _errorMessage;
        /// <summary>
        /// Gets or sets the error message to display to the user.
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged(nameof(ErrorMessage));
                }
            }
        }

        /// <summary>
        /// Loads admin data asynchronously from the service by admin ID.
        /// </summary>
        /// <param name="adminId">The ID of the admin to load.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="Exception">Thrown when the admin is not found.</exception>
        public async Task LoadAdminAsync(int adminId)
        {
            var admin = await _adminService.GetAdminByIdAsync(adminId);
            if (admin == null)
                throw new Exception("Admin not found");
            _admin = admin;
            // Raise property changed for all properties that depend on _admin
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(PhoneNumber));
            OnPropertyChanged(nameof(Email));
            OnPropertyChanged(nameof(CNP));
            OnPropertyChanged(nameof(Id));
            OnPropertyChanged(nameof(Role));
            OnPropertyChanged(nameof(CreatedAt));
        }

        /// <summary>
        /// Raises the PropertyChanged event for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        private void OnPropertyChanged(string propertyName) =>
             PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
