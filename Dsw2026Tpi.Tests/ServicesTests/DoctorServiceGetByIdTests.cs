using Dsw2026Tpi.Application.Services;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
using NSubstitute;
using Xunit;

namespace Dsw2026Tpi.Tests.ServicesTests;

public class DoctorServiceGetByIdTests
{
    private readonly IPersistence _persistence;
    private readonly DoctorService _service;

    public DoctorServiceGetByIdTests()
    {
        _persistence = Substitute.For<IPersistence>();
        _service = new DoctorService(_persistence);
    }

    [Fact]
    public async Task GetByIdAsync_CuandoElMedicoExiste_DevuelveSusDatos()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var doctor = new Doctor("Dr. Test", "MP-123", null, doctorId);

        _persistence.GetById<Doctor>(doctorId)
            .Returns(doctor);

        // Act
        var resultado = await _service.GetByIdAsync(doctorId);

        // Assert
        Assert.Equal(doctorId, resultado.Id);
        Assert.Equal("Dr. Test", resultado.Name);
        Assert.Equal("MP-123", resultado.LicenseNumber);
        Assert.True(resultado.IsActive);
    }

    [Fact]
public async Task GetByIdAsync_CuandoElMedicoTieneEspecialidad_DevuelveSusDatos()
{
    // Arrange
    var doctorId = Guid.NewGuid();
    var specialityId = Guid.NewGuid();

    var speciality = new Speciality(
        "Cardiología",
        "Especialidad del corazón",
        specialityId
    );

    var doctor = new Doctor(
        "Dr. Test",
        "MP-123",
        specialityId,
        doctorId
    );

    _persistence.GetById<Doctor>(doctorId)
        .Returns(doctor);

    _persistence.GetById<Speciality>(specialityId)
        .Returns(speciality);

    // Act
    var resultado = await _service.GetByIdAsync(doctorId);

    // Assert
    Assert.Equal(doctorId, resultado.Id);
    Assert.Equal("Dr. Test", resultado.Name);
    Assert.Equal("MP-123", resultado.LicenseNumber);

   Assert.NotNull(resultado.Specialty);
   Assert.Equal(specialityId, resultado.Specialty.Id);
   Assert.Equal("Cardiología", resultado.Specialty.Name);
}

    [Fact]
    public async Task GetByIdAsync_CuandoElMedicoNoExiste_LanzaExcepcion()
    {
        // Arrange
        var doctorId = Guid.NewGuid();

        _persistence.GetById<Doctor>(doctorId)
            .Returns((Doctor?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _service.GetByIdAsync(doctorId));
    }

    [Fact]
    public async Task GetByIdAsync_CuandoElMedicoEstaInactivo_LanzaExcepcion()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var doctor = new Doctor("Dr. Test", "MP-123", null, doctorId);

        doctor.Deactivate();

        _persistence.GetById<Doctor>(doctorId)
            .Returns(doctor);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _service.GetByIdAsync(doctorId));
    }

}