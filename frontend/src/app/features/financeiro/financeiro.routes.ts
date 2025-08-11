import { Routes } from '@angular/router';

export const FINANCEIRO_ROUTES: Routes = [
  {
    path: '',
    redirectTo: 'mensalidades',
    pathMatch: 'full',
  },
  {
    path: 'mensalidades',
    loadComponent: () =>
      import('./pages/mensalidades-list/mensalidades-list.page').then(
        (m) => m.MensalidadesListPage
      ),
  },
  {
    path: 'pagamentos',
    loadComponent: () =>
      import('./pages/pagamentos-list/pagamentos-list.page').then(
        (m) => m.PagamentosListPage
      ),
  },
  {
    path: 'pagamentos/novo',
    loadComponent: () =>
      import('./pages/pagamento-form/pagamento-form.page').then(
        (m) => m.PagamentoFormPage
      ),
  },
  {
    path: 'conciliacao',
    loadComponent: () =>
      import('./pages/conciliacao/conciliacao.page').then(
        (m) => m.ConciliacaoPage
      ),
  },
];
