using Virtus.Application.DTOs.Dashboard;

namespace Virtus.Application.Interfaces;

public interface IServicoDashboard
{
    public Task<DashboardDTO> ObterDadosDashboardAsync();
}
