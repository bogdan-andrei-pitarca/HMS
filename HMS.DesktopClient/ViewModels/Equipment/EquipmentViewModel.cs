using HMS.Shared.DTOs;
using HMS.Shared.Proxies.Implementations;
using HMS.Shared.Services;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace HMS.DesktopClient.ViewModels
{
    /// <summary>
    /// View model for the equipment screen that provides equipment information and editing capabilities.
    /// </summary>
    public class EquipmentViewModel : INotifyPropertyChanged
    {
        private readonly UserWithTokenDto _user;
        private EquipmentDto _equipment;
        private readonly EquipmentService _equipmentService;

        /// <summary>
        /// Event that is fired when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentViewModel"/> class.
        /// </summary>
        /// <param name="user">The authenticated user with token.</param>
        /// <param name="equipment">The equipment data to display and edit.</param>
        public EquipmentViewModel(UserWithTokenDto user, EquipmentDto equipment)
        {
            _user = user;
            _equipment = equipment;

            var proxy = new EquipmentProxy(_user.Token);
            _equipmentService = new EquipmentService(proxy);
        }

        /// <summary>
        /// Gets or sets the unique identifier of the equipment.
        /// </summary>
        public int Id
        {
            get => _equipment.Id;
            set
            {
                if (_equipment.Id != value)
                {
                    _equipment.Id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the equipment.
        /// </summary>
        public string Name
        {
            get => _equipment.Name ?? "";
            set
            {
                if (_equipment.Name != value)
                {
                    _equipment.Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        /// <summary>
        /// Gets or sets the specification details of the equipment.
        /// </summary>
        public string Specification
        {
            get => _equipment.Specification ?? "";
            set
            {
                if (_equipment.Specification != value)
                {
                    _equipment.Specification = value;
                    OnPropertyChanged(nameof(Specification));
                }
            }
        }

        /// <summary>
        /// Gets or sets the type/category of the equipment.
        /// </summary>
        public string Type
        {
            get => _equipment.Type ?? "";
            set
            {
                if (_equipment.Type != value)
                {
                    _equipment.Type = value;
                    OnPropertyChanged(nameof(Type));
                }
            }
        }

        /// <summary>
        /// Gets or sets the stock availability of the equipment.
        /// </summary>
        public int Stock
        {
            get => _equipment.Stock;
            set
            {
                if (_equipment.Stock != value)
                {
                    _equipment.Stock = value;
                    OnPropertyChanged(nameof(Stock));
                }
            }
        }

        /// <summary>
        /// Gets the authentication token associated with the current user.
        /// </summary>
        /// <remarks>
        /// This token is used for authenticating API requests to the server.
        /// </remarks>
        public string Token => _user.Token;

        /// <summary>
        /// Updates the equipment information in the database.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a boolean value indicating whether the update was successful.
        /// </returns>
        /// <remarks>
        /// This method sends the current equipment data to the server for persistence.
        /// </remarks>
        public async Task<bool> UpdateEquipmentAsync()
        {
            await _equipmentService.UpdateAsync(_equipment);
            return true;
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        /// <remarks>
        /// This method is called by property setters to notify the UI that a property has changed
        /// and any bindings should be updated.
        /// </remarks>
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}