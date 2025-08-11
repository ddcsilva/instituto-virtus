import { Routes } from '@angular/router';

export const TURMAS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/turmas-list/turmas-list.page').then(
        (m) => m.TurmasListPage
      ),
  },
  {
    path: 'nova',
    loadComponent: () =>
      import('./pages/turma-form/turma-form.page').then((m) => m.TurmaFormPage),
  },
  {
    path: ':id/editar',
    loadComponent: () =>
      import('./pages/turma-form/turma-form.page').then((m) => m.TurmaFormPage),
  },
  {
    path: ':id/alunos',
    loadComponent: () =>
      import('./pages/turma-alunos/turma-alunos.page').then(
        (m) => m.TurmaAlunosPage
      ),
  },
  {
    path: ':id/grade',
    loadComponent: () =>
      import('./pages/turma-grade/turma-grade.page').then(
        (m) => m.TurmaGradePage
      ),
  },
];
