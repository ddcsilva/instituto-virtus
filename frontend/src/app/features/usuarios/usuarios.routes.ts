import { Routes } from '@angular/router';

export const USUARIOS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/usuarios-list/usuarios-list.page').then(m => m.UsuariosListPage),
  },
  {
    path: 'novo',
    loadComponent: () =>
      import('../pessoas/pages/usuario-form/usuario-form.page').then(m => m.UsuarioFormPage),
  },
];
