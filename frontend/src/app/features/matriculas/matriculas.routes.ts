import { Routes } from '@angular/router';

export const MATRICULAS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/matriculas-list/matriculas-list.page').then(
        (m) => m.MatriculasListPage
      ),
  },
  {
    path: 'nova',
    loadComponent: () =>
      import('./pages/matricula-form/matricula-form.page').then(
        (m) => m.MatriculaFormPage
      ),
  },
  {
    path: ':id/detalhes',
    loadComponent: () =>
      import('./pages/matricula-detalhes/matricula-detalhes.page').then(
        (m) => m.MatriculaDetalhesPage
      ),
  },
];
