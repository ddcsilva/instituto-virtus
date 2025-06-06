import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/dashboard',
    pathMatch: 'full',
  },
  {
    path: 'dashboard',
    loadComponent: () =>
      import('./features/dashboard/dashboard/dashboard.component').then(
        (c) => c.DashboardComponent
      ),
  },
  {
    path: 'alunos',
    loadComponent: () =>
      import('./features/alunos/alunos/alunos.component').then(
        (c) => c.AlunosComponent
      ),
  },
  {
    path: 'turmas',
    loadComponent: () =>
      import('./features/turmas/turmas/turmas.component').then(
        (c) => c.TurmasComponent
      ),
  },
  {
    path: 'pagamentos',
    loadComponent: () =>
      import('./features/pagamentos/pagamentos/pagamentos.component').then(
        (c) => c.PagamentosComponent
      ),
  },
];
