using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HMS.Shared.DTOs;
using HMS.Shared.DTOs.Patient;
using HMS.Shared.Proxies.Implementations;

namespace HMS.DesktopClient.ViewModels
{
    /// <summary>
    /// View model that provides data and functionality for creating new medical records.
    /// </summary>
    /// <remarks>
    /// This view model manages the form data, selection lists, and validation for creating
    /// a new medical record in the system. It handles loading reference data and orchestrating
    /// the relationships between departments, doctors, and procedures.
    /// </remarks>
    public class MedicalRecordCreationFormViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The proxy used to communicate with the doctor service.
        /// </summary>
        /// <remarks>
        /// Provides access to doctor data for selection in the form.
        /// </remarks>
        private readonly DoctorProxy _doctorProxy;

        /// <summary>
        /// The proxy used to communicate with the procedure service.
        /// </summary>
        /// <remarks>
        /// Provides access to medical procedure data for selection in the form.
        /// </remarks>
        private readonly ProcedureProxy _procedureProxy;

        /// <summary>
        /// The proxy used to communicate with the department service.
        /// </summary>
        /// <remarks>
        /// Provides access to department data for filtering doctors and procedures.
        /// </remarks>
        private readonly DepartmentProxy _departmentProxy;

        /// <summary>
        /// The proxy used to communicate with the patient service.
        /// </summary>
        /// <remarks>
        /// Provides access to patient data for selection in the form.
        /// </remarks>
        private readonly PatientProxy _patientProxy;

        /// <summary>
        /// Gets the collection of available doctors for selection.
        /// </summary>
        /// <remarks>
        /// This collection is filtered based on the selected department.
        /// </remarks>
        public ObservableCollection<DoctorDto> DoctorsList { get; } = new();

        /// <summary>
        /// Gets the collection of all departments in the hospital.
        /// </summary>
        /// <remarks>
        /// Used to filter doctors and procedures by their respective departments.
        /// </remarks>
        public ObservableCollection<DepartmentDto> DepartmentsList { get; } = new();

        /// <summary>
        /// Gets the collection of all patients in the system.
        /// </summary>
        /// <remarks>
        /// Used to select the patient for whom the medical record is being created.
        /// </remarks>
        public ObservableCollection<PatientDto> PatientsList { get; } = new();

        /// <summary>
        /// Gets the collection of available medical procedures for selection.
        /// </summary>
        /// <remarks>
        /// This collection is filtered based on the selected department.
        /// </remarks>
        public ObservableCollection<ProcedureDto> ProceduresList { get; } = new();

        /// <summary>
        /// Gets or sets the currently selected doctor for the medical record.
        /// </summary>
        /// <remarks>
        /// The doctor responsible for the diagnosis and treatment described in this record.
        /// </remarks>
        private DoctorDto? _selectedDoctor;
        
        /// <summary>
        /// Gets or sets the doctor selected for this medical record.
        /// </summary>
        /// <remarks>
        /// When changed, notifies the UI to update bindings.
        /// </remarks>
        public DoctorDto? SelectedDoctor
        {
            get => _selectedDoctor;
            set { _selectedDoctor = value; OnPropertyChanged(nameof(SelectedDoctor)); }
        }

        /// <summary>
        /// Gets or sets the currently selected patient for the medical record.
        /// </summary>
        /// <remarks>
        /// The patient who received the medical care described in this record.
        /// </remarks>
        private PatientDto? _selectedPatient;
        
        /// <summary>
        /// Gets or sets the patient selected for this medical record.
        /// </summary>
        /// <remarks>
        /// When changed, notifies the UI to update bindings.
        /// </remarks>
        public PatientDto? SelectedPatient
        {
            get => _selectedPatient;
            set { _selectedPatient = value; OnPropertyChanged(nameof(SelectedPatient)); }
        }

        /// <summary>
        /// Gets or sets the currently selected medical procedure for the record.
        /// </summary>
        /// <remarks>
        /// The medical procedure or treatment that was performed.
        /// </remarks>
        private ProcedureDto? _selectedProcedure;
        
        /// <summary>
        /// Gets or sets the procedure selected for this medical record.
        /// </summary>
        /// <remarks>
        /// When changed, notifies the UI to update bindings.
        /// </remarks>
        public ProcedureDto? SelectedProcedure
        {
            get => _selectedProcedure;
            set { _selectedProcedure = value; OnPropertyChanged(nameof(SelectedProcedure)); }
        }

        /// <summary>
        /// Gets or sets the currently selected department for filtering.
        /// </summary>
        /// <remarks>
        /// When a department is selected, the available doctors and procedures are filtered accordingly.
        /// </remarks>
        private DepartmentDto? _selectedDepartment;
        
        /// <summary>
        /// Gets or sets the department selected for filtering doctors and procedures.
        /// </summary>
        /// <remarks>
        /// When changed, automatically triggers reloading of doctors and procedures lists.
        /// </remarks>
        public DepartmentDto? SelectedDepartment
        {
            get => _selectedDepartment;
            set
            {
                _selectedDepartment = value;
                OnPropertyChanged(nameof(SelectedDepartment));
                _ = LoadDoctorsAndProceduresAsync(0);
                _ = LoadDoctorsAndProceduresAsync(DepartmentsList.IndexOf(_selectedDepartment) + 1);
            }
        }

        /// <summary>
        /// Gets or sets the date of the appointment.
        /// </summary>
        /// <remarks>
        /// Used for recording when the medical examination or procedure took place.
        /// </remarks>
        private DateTimeOffset _appointmentDateOffset = DateTimeOffset.Now;
        
        /// <summary>
        /// Gets or sets the date component of the appointment date and time.
        /// </summary>
        /// <remarks>
        /// Uses DateTimeOffset to handle timezone considerations.
        /// </remarks>
        public DateTimeOffset AppointmentDateOffset
        {
            get => _appointmentDateOffset;
            set { _appointmentDateOffset = value; OnPropertyChanged(nameof(AppointmentDateOffset)); }
        }

        /// <summary>
        /// Gets or sets the time of the appointment as a string.
        /// </summary>
        /// <remarks>
        /// Stored as a string to facilitate user input in various time formats.
        /// </remarks>
        private string _appointmentTime;
        
        /// <summary>
        /// Gets or sets the time component of the appointment as a string.
        /// </summary>
        /// <remarks>
        /// When changed, notifies the UI to update bindings.
        /// </remarks>
        public string AppointmentTime
        {
            get => _appointmentTime;
            set { _appointmentTime = value; OnPropertyChanged(nameof(AppointmentTime)); }
        }

        /// <summary>
        /// Gets or sets the medical conclusion or diagnosis.
        /// </summary>
        /// <remarks>
        /// Contains the doctor's findings, diagnosis, and treatment recommendations.
        /// </remarks>
        private string _conclusion;
        
        /// <summary>
        /// Gets or sets the medical conclusion or diagnosis for this record.
        /// </summary>
        /// <remarks>
        /// This is the primary medical information captured in the record.
        /// </remarks>
        public string Conclusion
        {
            get => _conclusion;
            set { _conclusion = value; OnPropertyChanged(nameof(Conclusion)); }
        }

        /// <summary>
        /// Gets the collection of document file paths attached to this medical record.
        /// </summary>
        /// <remarks>
        /// These could be lab results, images, or other supporting documents.
        /// </remarks>
        public ObservableCollection<string> DocumentPaths { get; } = new();

        /// <summary>
        /// Adds a document file path to the collection of attached documents.
        /// </summary>
        /// <param name="path">The file system path to the document.</param>
        /// <remarks>
        /// Validates that the path is not empty before adding it to the collection.
        /// </remarks>
        public void AddDocument(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
                DocumentPaths.Add(path);
        }

        /// <summary>
        /// Gets the combined date and time of the appointment.
        /// </summary>
        /// <remarks>
        /// Combines the date from AppointmentDateOffset with the time from AppointmentTime.
        /// If the time cannot be parsed, only the date is used.
        /// </remarks>
        public DateTime AppointmentDate
        {
            get
            {
                if (TimeSpan.TryParse(AppointmentTime, out var time))
                    return AppointmentDateOffset.Date + time;
                return AppointmentDateOffset.Date;
            }
        }

        /// <summary>
        /// Initializes the view model by loading all required reference data.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// Loads departments and patients to populate selection lists.
        /// </remarks>
        public async Task InitializeAsync()
        {
            var departments = await _departmentProxy.GetAllAsync();
            DepartmentsList.Clear();
            foreach (var dept in departments)
                DepartmentsList.Add(dept);

            var patients = await _patientProxy.GetAllAsync();
            PatientsList.Clear();
            foreach (var patient in patients)
                PatientsList.Add(patient);
        }

        /// <summary>
        /// Loads doctors and procedures filtered by the specified department ID.
        /// </summary>
        /// <param name="departmentId">The ID of the department to filter by.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// When departmentId is 0, the lists are cleared without loading new data.
        /// This method updates the DoctorsList and ProceduresList collections.
        /// </remarks>
        private async Task LoadDoctorsAndProceduresAsync(int departmentId)
        {
            DoctorsList.Clear();
            ProceduresList.Clear();

            if (departmentId == 0) return;

            var doctors = await _doctorProxy.GetAllAsync();
            foreach (var doctor in doctors.Where(d => d.DepartmentId == departmentId))
                DoctorsList.Add(doctor);

            var procedures = await _procedureProxy.GetAllAsync();
            foreach (var proc in procedures.Where(p => p.DepartmentId == departmentId))
                ProceduresList.Add(proc);

            SelectedDoctor = null;
            SelectedProcedure = null;

            Debug.WriteLine($"Loaded {DoctorsList.Count} doctors and {ProceduresList.Count} procedures.");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MedicalRecordCreationFormViewModel"/> class.
        /// </summary>
        /// <param name="doctorProxy">The proxy for accessing doctor data.</param>
        /// <param name="procedureProxy">The proxy for accessing procedure data.</param>
        /// <param name="departmentProxy">The proxy for accessing department data.</param>
        /// <param name="patientProxy">The proxy for accessing patient data.</param>
        /// <remarks>
        /// This constructor injects all required dependencies for the view model to function.
        /// </remarks>
        public MedicalRecordCreationFormViewModel(
            DoctorProxy doctorProxy,
            ProcedureProxy procedureProxy,
            DepartmentProxy departmentProxy,
            PatientProxy patientProxy)
        {
            _doctorProxy = doctorProxy;
            _procedureProxy = procedureProxy;
            _departmentProxy = departmentProxy;
            _patientProxy = patientProxy;
        }

        /// <summary>
        /// Event raised when a property value changes to notify UI components.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
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