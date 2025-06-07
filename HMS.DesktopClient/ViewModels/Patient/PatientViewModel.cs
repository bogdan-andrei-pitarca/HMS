using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMS.Shared.DTOs.Patient;
using HMS.Shared.Services;

namespace HMS.DesktopClient.ViewModels.Patient
{
    /// <summary>
    /// View model that provides patient data and functionality for listing and managing patients.
    /// </summary>
    /// <remarks>
    /// This view model handles loading patients based on the current user's role.
    /// For doctors, it loads only patients associated with their appointments.
    /// For administrators, it loads all patients in the system.
    /// </remarks>
    public class PatientViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Event raised when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;
        
        /// <summary>
        /// The service used to retrieve and manage patient data.
        /// </summary>
        /// <remarks>
        /// This service communicates with the patient-related API endpoints.
        /// </remarks>
        private PatientService PatientService;
        
        /// <summary>
        /// The service used to retrieve appointment data.
        /// </summary>
        /// <remarks>
        /// This service is used to find appointments associated with a doctor
        /// to determine which patients they should see.
        /// </remarks>
        private AppointmentService AppointmentService;
        
        /// <summary>
        /// The collection of patients to display in the UI.
        /// </summary>
        private ObservableCollection<PatientDto> _patients;

        /// <summary>
        /// Initializes a new instance of the <see cref="PatientViewModel"/> class.
        /// </summary>
        /// <param name="patientService">The service used to retrieve and manage patient data.</param>
        /// <param name="appointmentService">The service used to retrieve appointment data.</param>
        /// <remarks>
        /// This constructor initializes the services required for patient data management.
        /// </remarks>
        public PatientViewModel(PatientService patientService, AppointmentService appointmentService)
        {
            PatientService = patientService;
            AppointmentService = appointmentService;
        }

        /// <summary>
        /// Initializes the view model by loading the appropriate patient data.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method should be called when the view is first loaded to populate
        /// the patients collection.
        /// </remarks>
        public async Task InitializeAsync()
        {
            await LoadPatientsAsync();
        }

        /// <summary>
        /// Gets or sets the collection of patients to display in the UI.
        /// </summary>
        /// <remarks>
        /// This observable collection is bound to UI elements and automatically
        /// notifies them when patients are added, removed, or modified.
        /// </remarks>
        public ObservableCollection<PatientDto> Patients
        {
            get => _patients;
            private set
            {
                _patients = value;
                OnPropertyChanged(nameof(Patients));
            }
        }

        /// <summary>
        /// Loads the appropriate patients based on the current user's role.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// For doctors, this method loads only patients associated with their appointments.
        /// For administrators, it loads all patients in the system.
        /// </remarks>
        private async Task LoadPatientsAsync()
        {
            try
            {
                Patients = new ObservableCollection<PatientDto>();
                if (App.CurrentDoctor == null)
                {
                    var patients = await PatientService.GetAllPatientsAsync();
                    if (patients != null)
                    {
                        foreach (var patient in patients)
                        {
                            Patients.Add(patient);
                        }
                    }
                    Debug.WriteLine("No current doctor set, loaded all patients.");
                    return;
                }
                var appointments = await AppointmentService.GetAppointmentsForDoctor(App.CurrentDoctor.Id);
                var patientIds = appointments.Select(a => a.PatientId).Distinct().ToList();
                if (patientIds.Count == 0)
                {
                    Patients = new ObservableCollection<PatientDto>();
                    return;
                }
                Debug.WriteLine($"Found {patientIds.Count} unique patient IDs from appointments for doctor with ID {App.CurrentDoctor.Id}.");
                foreach (var patient in patientIds)
                {
                    var patientDto = await PatientService.GetPatientByIdAsync(patient);
                    if (patientDto != null)
                    {
                        Patients.Add(patientDto);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading patients: {ex.Message}");
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
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
