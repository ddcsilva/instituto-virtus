export const RELATORIOS_ROUTES_UPDATED: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/relatorios-dashboard/relatorios-dashboard.page').then(
        (m) => m.RelatoriosDashboardPage
      ),
  },
  {
    path: 'inadimplentes',
    loadComponent: () =>
      import(
        './pages/relatorio-inadimplentes/relatorio-inadimplentes.page'
      ).then((m) => m.RelatorioInadimplentesPage),
  },
  {
    path: 'frequencia',
    loadComponent: () =>
      import('./pages/relatorio-frequencia/relatorio-frequencia.page').then(
        (m) => m.RelatorioFrequenciaPage
      ),
  },
  {
    path: 'aprovacao',
    loadComponent: () =>
      import('./pages/relatorio-aprovacao/relatorio-aprovacao.page').then(
        (m) => m.RelatorioAprovacaoPage
      ),
  },
  {
    path: 'financeiro',
    loadComponent: () =>
      import('./pages/relatorio-financeiro/relatorio-financeiro.page').then(
        (m) => m.RelatorioFinanceiroPage
      ),
  },
];
