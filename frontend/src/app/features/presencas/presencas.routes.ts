import { Routes } from '@angular/router';

export const PRESENCAS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/presencas-list/presencas-list.page').then(
        (m) => m.PresencasListPage
      ),
  },
  {
    path: 'lancar',
    loadComponent: () =>
      import('./pages/lancar-presenca/lancar-presenca.page').then(
        (m) => m.LancarPresencaPage
      ),
  },
  {
    path: 'frequencia/:turmaId',
    loadComponent: () =>
      import('./pages/frequencia-turma/frequencia-turma.page').then(
        (m) => m.FrequenciaTurmaPage
      ),
  },
];
