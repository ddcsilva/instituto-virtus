import { Routes } from '@angular/router';

export const PESSOAS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/pessoas-list/pessoas-list.page').then(m => m.PessoasListPage),
  },
  {
    path: 'usuarios/novo',
    loadComponent: () =>
      import('./pages/usuario-form/usuario-form.page').then(m => m.UsuarioFormPage),
  },
  {
    path: 'novo',
    loadComponent: () => import('./pages/pessoa-form/pessoa-form.page').then(m => m.PessoaFormPage),
  },
  {
    path: ':id/editar',
    loadComponent: () => import('./pages/pessoa-form/pessoa-form.page').then(m => m.PessoaFormPage),
  },
  {
    path: ':id/vinculos',
    loadComponent: () =>
      import('./pages/pessoa-vinculos/pessoa-vinculos.page').then(m => m.PessoaVinculosPage),
  },
];
