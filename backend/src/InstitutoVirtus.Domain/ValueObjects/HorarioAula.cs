namespace InstitutoVirtus.Domain.ValueObjects;

public class HorarioAula : ValueObject
{
    public TimeSpan HoraInicio { get; private set; }
    public TimeSpan HoraFim { get; private set; }

    protected HorarioAula() { }

    public HorarioAula(TimeSpan horaInicio, TimeSpan horaFim)
    {
        if (horaFim <= horaInicio)
            throw new ArgumentException("Hora fim deve ser maior que hora início");

        var duracao = horaFim - horaInicio;
        if (duracao != TimeSpan.FromMinutes(50))
            throw new ArgumentException("Aula deve ter duração de 50 minutos");

        HoraInicio = horaInicio;
        HoraFim = horaFim;
    }

    public string FormatoString()
    {
        return $"{HoraInicio:hh\\:mm}-{HoraFim:hh\\:mm}";
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return HoraInicio;
        yield return HoraFim;
    }
}