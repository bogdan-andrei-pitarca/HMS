using System;

namespace Hospital.ViewModels.MedicalRecord
{
    /// <summary>
    /// View model representing a single medical record item in a list display.
    /// </summary>
    /// <remarks>
    /// This class provides a flat representation of medical record data optimized for display
    /// in list views, grids, or other collection-based UI components.
    /// </remarks>
    public class MedicalRecordListItemViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the medical record.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the patient associated with this medical record.
        /// </summary>
        /// <remarks>
        /// Used for filtering or navigating to patient details.
        /// </remarks>
        public int PatientId { get; set; }

        /// <summary>
        /// Gets or sets the full name of the patient.
        /// </summary>
        /// <remarks>
        /// This is the display name shown in the UI for easier identification.
        /// </remarks>
        public string PatientName { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the doctor who created this medical record.
        /// </summary>
        /// <remarks>
        /// Used for filtering or navigating to doctor details.
        /// </remarks>
        public int DoctorId { get; set; }

        /// <summary>
        /// Gets or sets the full name of the doctor.
        /// </summary>
        /// <remarks>
        /// This is the display name shown in the UI for easier identification.
        /// </remarks>
        public string DoctorName { get; set; }

        /// <summary>
        /// Gets or sets the department name the doctor belongs to.
        /// </summary>
        /// <remarks>
        /// Provides context about the medical specialty involved in this record.
        /// </remarks>
        public string DoctorDepartment { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the medical procedure performed.
        /// </summary>
        /// <remarks>
        /// Used for filtering or navigating to procedure details.
        /// </remarks>
        public int ProcedureId { get; set; }

        /// <summary>
        /// Gets or sets the name of the medical procedure performed.
        /// </summary>
        /// <remarks>
        /// Describes the specific medical intervention or examination that was conducted.
        /// </remarks>
        public string ProcedureName { get; set; }

        /// <summary>
        /// Gets or sets the department responsible for the procedure.
        /// </summary>
        /// <remarks>
        /// May differ from the doctor's department in some cases of interdepartmental care.
        /// </remarks>
        public string ProcedureDepartment { get; set; }

        /// <summary>
        /// Gets or sets the medical diagnosis provided for the patient.
        /// </summary>
        /// <remarks>
        /// Contains the doctor's assessment of the patient's condition.
        /// </remarks>
        public string Diagnosis { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this medical record was created.
        /// </summary>
        /// <remarks>
        /// Used for sorting, filtering, and displaying the chronology of patient care.
        /// </remarks>
        public DateTime CreatedAt { get; set; }
    }
}
