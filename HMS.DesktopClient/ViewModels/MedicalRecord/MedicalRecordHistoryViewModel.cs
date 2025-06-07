using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using HMS.Shared.DTOs;
using HMS.Shared.Proxies.Implementations;
using System.Linq;

namespace HMS.DesktopClient.ViewModels
{
    /// <summary>
    /// View model that provides a collection of medical records for a specific patient.
    /// </summary>
    /// <remarks>
    /// This view model is responsible for loading and maintaining a patient's medical history,
    /// facilitating the display of their complete medical record timeline.
    /// </remarks>
    public class MedicalRecordHistoryViewModel
    {
        /// <summary>
        /// The proxy used to communicate with the medical record service.
        /// </summary>
        /// <remarks>
        /// Handles all API calls related to retrieving medical record data.
        /// </remarks>
        private readonly MedicalRecordProxy _medicalRecordProxy;

        /// <summary>
        /// Gets the collection of medical records for the specified patient.
        /// </summary>
        /// <remarks>
        /// This observable collection is bound to UI elements to display the patient's medical history.
        /// It automatically notifies the UI when records are added, removed, or modified.
        /// </remarks>
        public ObservableCollection<MedicalRecordDto> MedicalRecords { get; } = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="MedicalRecordHistoryViewModel"/> class.
        /// </summary>
        /// <param name="patientId">The unique identifier of the patient whose records to retrieve.</param>
        /// <param name="medicalRecordProxy">The proxy used to fetch medical record data.</param>
        /// <remarks>
        /// Upon initialization, this constructor triggers the asynchronous loading of medical records
        /// for the specified patient.
        /// </remarks>
        public MedicalRecordHistoryViewModel(int patientId, MedicalRecordProxy medicalRecordProxy)
        {
            _medicalRecordProxy = medicalRecordProxy;
            InitializeAsync(patientId);
        }

        /// <summary>
        /// Loads all medical records for the specified patient.
        /// </summary>
        /// <param name="patientId">The unique identifier of the patient whose records to retrieve.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method fetches all medical records from the server, filters them by patient ID,
        /// and populates the <see cref="MedicalRecords"/> collection with the results.
        /// Error handling is implemented to prevent application crashes.
        /// </remarks>
        private async Task InitializeAsync(int patientId)
        {
            try
            {
                var records = await _medicalRecordProxy.GetAllAsync();
                foreach (var record in records.Where(r => r.PatientId == patientId))
                {
                    MedicalRecords.Add(record);
                }
            }
            catch (Exception ex)
            {
                // Handle error appropriately (e.g., logging, UI notification)
                Console.WriteLine($"Error loading medical records: {ex.Message}");
            }
        }
    }
}