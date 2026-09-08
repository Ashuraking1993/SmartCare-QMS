namespace QSmart.Application.DTOs.Appointment;

public class CreateAppointmentRequest
{
    public Guid DoctorId { get; set; }

    public DateTime AppointmentDate { get; set; }
}