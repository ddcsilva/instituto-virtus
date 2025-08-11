import { Routes } from '@angular/router';

export const CURSOS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/cursos-list/cursos-list.page').then(
        (m) => m.CursosListPage
      ),
  },
  {
    path: 'novo',
    loadComponent: () =>
      import('./pages/curso-form/curso-form.page').then((m) => m.CursoFormPage),
  },
  {
    path: ':id/editar',
    loadComponent: () =>
      import('./pages/curso-form/curso-form.page').then((m) => m.CursoFormPage),
  },
];
