import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { AuthService } from '../../auth/services/auth.service';
import { LoadingService } from '../../http/services/loading.service';

interface NavItem {
  label: string;
  icon: string;
  route: string;
  roles: string[];
}

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatSidenavModule,
    MatToolbarModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    MatMenuModule,
    MatProgressBarModule,
  ],
  templateUrl: './shell.component.html',
  styleUrls: ['./shell.component.scss'],
})
export class ShellComponent {
  private readonly authService = inject(AuthService);
  private readonly loadingService = inject(LoadingService);

  readonly usuarioAtual = this.authService.usuarioAtual;
  readonly carregando = this.loadingService.isLoading;
  readonly isHandset = signal(window.innerWidth < 768);

  private readonly navItems: NavItem[] = [
    {
      label: 'Dashboard',
      icon: 'dashboard',
      route: '/dashboard',
      roles: ['Admin', 'Coordenador', 'Professor', 'Responsavel', 'Aluno'],
    },
    {
      label: 'Pessoas',
      icon: 'people',
      route: '/pessoas',
      roles: ['Admin', 'Coordenador'],
    },
    {
      label: 'Cursos',
      icon: 'school',
      route: '/cursos',
      roles: ['Admin', 'Coordenador'],
    },
    {
      label: 'Turmas',
      icon: 'groups',
      route: '/turmas',
      roles: ['Admin', 'Coordenador', 'Professor'],
    },
    {
      label: 'Matrículas',
      icon: 'assignment',
      route: '/matriculas',
      roles: ['Admin', 'Coordenador'],
    },
    {
      label: 'Financeiro',
      icon: 'attach_money',
      route: '/financeiro',
      roles: ['Admin', 'Coordenador'],
    },
    {
      label: 'Presenças',
      icon: 'fact_check',
      route: '/presencas',
      roles: ['Admin', 'Coordenador', 'Professor'],
    },
    {
      label: 'Avaliações',
      icon: 'grade',
      route: '/avaliacoes',
      roles: ['Admin', 'Coordenador', 'Professor'],
    },
    {
      label: 'Portal',
      icon: 'account_box',
      route: '/portal',
      roles: ['Aluno', 'Responsavel'],
    },
    {
      label: 'Relatórios',
      icon: 'assessment',
      route: '/relatorios',
      roles: ['Admin', 'Coordenador'],
    },
  ];

  readonly itensVisiveis = computed(() => {
    const tipoUsuario = this.usuarioAtual()?.tipo;
    if (!tipoUsuario) return [];
    return this.navItems.filter(item => item.roles.includes(tipoUsuario));
  });

  constructor() {
    window.addEventListener('resize', () => {
      this.isHandset.set(window.innerWidth < 768);
    });
  }

  logout(): void {
    this.authService.logout();
  }
}
