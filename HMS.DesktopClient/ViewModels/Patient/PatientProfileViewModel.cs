using HMS.Shared.DTOs;
using HMS.Shared.DTOs.Patient;
using HMS.Shared.Proxies.Implementations;
using HMS.Shared.Services;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace HMS.DesktopClient.ViewModels
{
    /// <summary>
    /// View model for the patient profile screen that provides patient information and editing capabilities.
    /// </summary>
    /// <remarks>
    /// This view model manages data for viewing and editing a patient's profile, including personal information,
    /// medical details, and contact information. It supports updating patient data to the server.
    /// </remarks>
    public class PatientProfileViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The authenticated user with token information.
        /// </summary>
        /// <remarks>
        /// Contains the authentication token needed for API requests.
        /// </remarks>
        private readonly UserWithTokenDto _user;    // holds token and user info
        
        /// <summary>
        /// The patient data being displayed and edited.
        /// </summary>
        /// <remarks>
        /// Contains all patient-specific information and user information, except the token.
        /// </remarks>
        private PatientDto _patient;                 // holds patient-specific + user info (except token)
        
        /// <summary>
        /// The service used to communicate with patient-related API endpoints.
        /// </summary>
        private readonly PatientService _patientService;

        /// <summary>
        /// Event raised when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="PatientProfileViewModel"/> class.
        /// </summary>
        /// <param name="user">The authenticated user with token information.</param>
        /// <param name="patient">The patient data to display and edit.</param>
        /// <remarks>
        /// This constructor initializes the patient service with the user's authentication token.
        /// </remarks>
        public PatientProfileViewModel(UserWithTokenDto user, PatientDto patient)
        {
            _user = user;
            _patient = patient;

            var proxy = new PatientProxy(_user.Token);
            _patientService = new PatientService(proxy);
        }

        /// <summary>
        /// Gets or sets the name of the patient.
        /// </summary>
        /// <remarks>
        /// This property is bound to the UI and updates the underlying patient data when changed.
        /// </remarks>
        public string Name
        {
            get => _patient.Name ?? "";
            set
            {
                if (_patient.Name != value)
                {
                    _patient.Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        /// <summary>
        /// Gets or sets the phone number of the patient.
        /// </summary>
        /// <remarks>
        /// This property is bound to the UI and updates the underlying patient data when changed.
        /// </remarks>
        public string PhoneNumber
        {
            get => _patient.PhoneNumber ?? "";
            set
            {
                if (_patient.PhoneNumber != value)
                {
                    _patient.PhoneNumber = value;
                    OnPropertyChanged(nameof(PhoneNumber));
                }
            }
        }

        /// <summary>
        /// Gets or sets the CNP (personal identification number) of the patient.
        /// </summary>
        /// <remarks>
        /// This property is bound to the UI and updates the underlying patient data when changed.
        /// The CNP is a unique identifier for individuals in the Romanian system.
        /// </remarks>
        public string CNP
        {
            get => _patient.CNP ?? "";
            set
            {
                if (_patient.CNP != value)
                {
                    _patient.CNP = value;
                    OnPropertyChanged(nameof(CNP));
                }
            }
        }

        /// <summary>
        /// Gets or sets the blood type of the patient.
        /// </summary>
        /// <remarks>
        /// This property is bound to the UI and updates the underlying patient data when changed.
        /// The blood type is important for medical procedures and emergencies.
        /// </remarks>
        public string BloodType
        {
            get => _patient.BloodType ?? "";
            set
            {
                if (_patient.BloodType != value)
                {
                    _patient.BloodType = value;
                    OnPropertyChanged(nameof(BloodType));
                }
            }
        }

        /// <summary>
        /// Gets or sets the emergency contact information for the patient.
        /// </summary>
        /// <remarks>
        /// This property is bound to the UI and updates the underlying patient data when changed.
        /// The emergency contact is used to notify someone in case of emergency.
        /// </remarks>
        public string EmergencyContact
        {
            get => _patient.EmergencyContact ?? "";
            set
            {
                if (_patient.EmergencyContact != value)
                {
                    _patient.EmergencyContact = value;
                    OnPropertyChanged(nameof(EmergencyContact));
                }
            }
        }

        /// <summary>
        /// Gets or sets the allergies information for the patient.
        /// </summary>
        /// <remarks>
        /// This property is bound to the UI and updates the underlying patient data when changed.
        /// Allergies information is critical for safe medical treatment.
        /// </remarks>
        public string Allergies
        {
            get => _patient.Allergies ?? "";
            set
            {
                if (_patient.Allergies != value)
                {
                    _patient.Allergies = value;
                    OnPropertyChanged(nameof(Allergies));
                }
            }
        }

        /// <summary>
        /// Gets or sets the weight of the patient in kilograms.
        /// </summary>
        /// <remarks>
        /// This property is bound to the UI and updates the underlying patient data when changed.
        /// The weight is used for dosage calculations and health assessment.
        /// </remarks>
        public float Weight
        {
            get => _patient.Weight;
            set
            {
                if (_patient.Weight != value)
                {
                    _patient.Weight = value;
                    OnPropertyChanged(nameof(Weight));
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the patient in centimeters.
        /// </summary>
        /// <remarks>
        /// This property is bound to the UI and updates the underlying patient data when changed.
        /// The height is used for BMI calculations and health assessment.
        /// </remarks>
        public float Height
        {
            get => _patient.Height;
            set
            {
                if (_patient.Height != value)
                {
                    _patient.Height = value;
                    OnPropertyChanged(nameof(Height));
                }
            }
        }

        /// <summary>
        /// Gets or sets the birth date of the patient.
        /// </summary>
        /// <remarks>
        /// This property is bound to the UI and updates the underlying patient data when changed.
        /// The birth date is used to calculate age and for demographic information.
        /// </remarks>
        public DateTime BirthDate
        {
            get => _patient.BirthDate;
            set
            {
                if (_patient.BirthDate != value)
                {
                    _patient.BirthDate = value;
                    OnPropertyChanged(nameof(BirthDate));
                }
            }
        }

        /// <summary>
        /// Gets or sets the address of the patient.
        /// </summary>
        /// <remarks>
        /// This property is bound to the UI and updates the underlying patient data when changed.
        /// The address is used for contact information and administrative purposes.
        /// </remarks>
        public string Address
        {
            get => _patient.Address ?? "";
            set
            {
                if (_patient.Address != value)
                {
                    _patient.Address = value;
                    OnPropertyChanged(nameof(Address));
                }
            }
        }

        /// <summary>
        /// Gets the unique identifier of the patient.
        /// </summary>
        /// <remarks>
        /// This ID is used for database operations and referencing the patient in the system.
        /// </remarks>
        public int Id => _patient.Id;
        
        /// <summary>
        /// Gets the email address of the patient.
        /// </summary>
        /// <remarks>
        /// The email is used for communication and account recovery.
        /// </remarks>
        public string Email => _patient.Email ?? "";
        
        /// <summary>
        /// Gets the role of the patient in the system.
        /// </summary>
        /// <remarks>
        /// This will typically be "Patient" but is included for consistency with the user model.
        /// </remarks>
        public string Role => _patient.Role.ToString();
        
        /// <summary>
        /// Gets the formatted date and time when the patient account was created.
        /// </summary>
        /// <remarks>
        /// This provides account age information for administrative purposes.
        /// </remarks>
        public string CreatedAt => _patient.CreatedAt.ToString("yyyy-MM-dd HH:mm");

        /// <summary>
        /// Gets the authentication token for the current user.
        /// </summary>
        /// <remarks>
        /// This token is used for authenticating API requests to the server.
        /// </remarks>
        public string Token => _user.Token;

        /// <summary>
        /// Gets or sets the error message to display to the user.
        /// </summary>
        /// <remarks>
        /// This property is updated when operations fail and is used to display error information in the UI.
        /// </remarks>
        private string _errorMessage;
        
        /// <summary>
        /// Gets or sets the error message displayed to the user when operations fail.
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
        /// Updates the patient information in the database.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a boolean value indicating whether the update was successful.
        /// </returns>
        /// <remarks>
        /// This method sends the current patient data to the server for persistence.
        /// If an error occurs, the exception message is captured and displayed via the ErrorMessage property.
        /// </remarks>
        public async Task<bool> UpdatePatientAsync()
        {
            try
            {
                return await _patientService.UpdatePatientAsync(_patient);
            }
            catch (Exception exception)
            {
                ErrorMessage = exception.Message;
                return false;
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        /// <remarks>
        /// This method is called by property setters to notify the UI when values change,
        /// enabling proper UI updates via data binding.
        /// </remarks>
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
