namespace InstitutoVirtus.Infrastructure.Services;

using InstitutoVirtus.Application.Common.Interfaces;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.UtcNow;
    public DateTime Today => DateTime.Today;
}