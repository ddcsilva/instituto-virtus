using Virtus.Application.DTOs.Dashboard;

namespace Virtus.Application.Interfaces;

public interface IDashboardService
{
    public Task<DashboardDTO> ObterDadosDashboardAsync();
}
