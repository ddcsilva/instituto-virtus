import { Routes } from '@angular/router';

export const AVALIACOES_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/avaliacoes-list/avaliacoes-list.page').then(
        (m) => m.AvaliacoesListPage
      ),
  },
  {
    path: 'configurar/:turmaId',
    loadComponent: () =>
      import('./pages/configurar-avaliacao/configurar-avaliacao.page').then(
        (m) => m.ConfigurarAvaliacaoPage
      ),
  },
  {
    path: 'lancar/:avaliacaoId',
    loadComponent: () =>
      import('./pages/lancar-notas/lancar-notas.page').then(
        (m) => m.LancarNotasPage
      ),
  },
  {
    path: 'boletim/:turmaId',
    loadComponent: () =>
      import('./pages/boletim-turma/boletim-turma.page').then(
        (m) => m.BoletimTurmaPage
      ),
  },
];
