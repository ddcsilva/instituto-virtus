import { Routes } from '@angular/router';
import { authGuard } from './core/auth/guards/auth.guard';
import { roleGuard } from './core/auth/guards/role.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./features/auth/pages/login/login.page').then(m => m.LoginPage),
  },
  {
    path: '',
    loadComponent: () => import('./core/layout/shell/shell.component').then(m => m.ShellComponent),
    canMatch: [authGuard],
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full',
      },
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/dashboard.page').then(m => m.DashboardPage),
      },
      {
        path: 'pessoas',
        loadChildren: () => import('./features/pessoas/pessoas.routes').then(m => m.PESSOAS_ROUTES),
        canMatch: [roleGuard(['Admin', 'Coordenador'])],
      },
      {
        path: 'usuarios',
        loadChildren: () =>
          import('./features/usuarios/usuarios.routes').then(m => m.USUARIOS_ROUTES),
        canMatch: [roleGuard(['Admin', 'Coordenador'])],
      },
      {
        path: 'cursos',
        loadChildren: () => import('./features/cursos/cursos.routes').then(m => m.CURSOS_ROUTES),
        canMatch: [roleGuard(['Admin', 'Coordenador'])],
      },
      {
        path: 'turmas',
        loadChildren: () => import('./features/turmas/turmas.routes').then(m => m.TURMAS_ROUTES),
        canMatch: [roleGuard(['Admin', 'Coordenador', 'Professor'])],
      },
      {
        path: 'matriculas',
        loadChildren: () =>
          import('./features/matriculas/matriculas.routes').then(m => m.MATRICULAS_ROUTES),
        canMatch: [roleGuard(['Admin', 'Coordenador'])],
      },
      {
        path: 'financeiro',
        loadChildren: () =>
          import('./features/financeiro/financeiro.routes').then(m => m.FINANCEIRO_ROUTES),
        canMatch: [roleGuard(['Admin', 'Coordenador'])],
      },
      {
        path: 'presencas',
        loadChildren: () =>
          import('./features/presencas/presencas.routes').then(m => m.PRESENCAS_ROUTES),
        canMatch: [roleGuard(['Admin', 'Coordenador', 'Professor'])],
      },
      {
        path: 'avaliacoes',
        loadChildren: () =>
          import('./features/avaliacoes/avaliacoes.routes').then(m => m.AVALIACOES_ROUTES),
        canMatch: [roleGuard(['Admin', 'Coordenador', 'Professor'])],
      },
      {
        path: 'portal',
        loadChildren: () => import('./features/portal/portal.routes').then(m => m.PORTAL_ROUTES),
        canMatch: [roleGuard(['Aluno', 'Responsavel'])],
      },
      {
        path: 'relatorios',
        loadChildren: () =>
          import('./features/relatorios/relatorios.routes').then(m => m.RELATORIOS_ROUTES),
        canMatch: [roleGuard(['Admin', 'Coordenador'])],
      },
    ],
  },
  {
    path: '**',
    redirectTo: 'login',
  },
];
