﻿namespace Zad10.Dtos;

public class PrescriptionOutDto
{
    public int IdPrescription { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public DoctorOutDto Doctor { get; set; }
    public PatientOutDto Patient { get; set; }
    public IEnumerable<MedicamentDto> Medicaments { get; set; }
}