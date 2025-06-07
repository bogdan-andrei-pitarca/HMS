using HMS.Shared.DTOs;

namespace HMS.DesktopClient.ViewModels
{
    /// <summary>
    /// View model that provides detailed information about a specific medical record.
    /// </summary>
    /// <remarks>
    /// This view model is used for displaying comprehensive details of a medical record,
    /// including patient information, doctor details, diagnoses, and treatments.
    /// </remarks>
    public class MedicalRecordsDetailViewModel
    {
        /// <summary>
        /// Gets the medical record data to be displayed.
        /// </summary>
        /// <remarks>
        /// Contains all information related to the medical record including patient data,
        /// doctor information, diagnosis, procedures, and timestamps.
        /// </remarks>
        public MedicalRecordDto MedicalRecord { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MedicalRecordsDetailViewModel"/> class.
        /// </summary>
        /// <param name="medicalRecord">The medical record data to display.</param>
        /// <remarks>
        /// This constructor takes a medical record DTO and prepares it for display in the UI.
        /// </remarks>
        public MedicalRecordsDetailViewModel(MedicalRecordDto medicalRecord)
        {
            MedicalRecord = medicalRecord;
        }
    }
}