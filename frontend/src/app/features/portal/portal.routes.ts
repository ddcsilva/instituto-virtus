import { Routes } from '@angular/router';

export const PORTAL_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/portal-home/portal-home.page').then(
        (m) => m.PortalHomePage
      ),
  },
  {
    path: 'turmas',
    loadComponent: () =>
      import('./pages/minhas-turmas/minhas-turmas.page').then(
        (m) => m.MinhasTurmasPage
      ),
  },
  {
    path: 'financeiro',
    loadComponent: () =>
      import('./pages/meu-financeiro/meu-financeiro.page').then(
        (m) => m.MeuFinanceiroPage
      ),
  },
  {
    path: 'boletim',
    loadComponent: () =>
      import('./pages/meu-boletim/meu-boletim.page').then(
        (m) => m.MeuBoletimPage
      ),
  },
  {
    path: 'frequencia',
    loadComponent: () =>
      import('./pages/minha-frequencia/minha-frequencia.page').then(
        (m) => m.MinhaFrequenciaPage
      ),
  },
];
