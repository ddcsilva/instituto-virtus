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
  template: `
    <mat-sidenav-container class="sidenav-container">
      <mat-sidenav
        #drawer
        class="sidenav"
        [attr.role]="isHandset() ? 'dialog' : 'navigation'"
        [mode]="isHandset() ? 'over' : 'side'"
        [opened]="!isHandset()"
      >
        <mat-toolbar class="sidenav-header">
          <span class="logo">Instituto Virtus</span>
        </mat-toolbar>

        <mat-nav-list>
          @for (item of visibleNavItems(); track item.route) {
          <a
            mat-list-item
            [routerLink]="item.route"
            routerLinkActive="active"
            (click)="isHandset() && drawer.close()"
          >
            <mat-icon matListItemIcon>{{ item.icon }}</mat-icon>
            <span matListItemTitle>{{ item.label }}</span>
          </a>
          }
        </mat-nav-list>
      </mat-sidenav>

      <mat-sidenav-content>
        <mat-toolbar color="primary" class="toolbar">
          @if (isHandset()) {
          <button type="button" mat-icon-button (click)="drawer.toggle()">
            <mat-icon>menu</mat-icon>
          </button>
          }

          <span class="spacer"></span>

          <button mat-icon-button [matMenuTriggerFor]="userMenu">
            <mat-icon>account_circle</mat-icon>
          </button>

          <mat-menu #userMenu="matMenu">
            <div class="user-info">
              <strong>{{ currentUser()?.nome }}</strong>
              <small>{{ currentUser()?.tipo }}</small>
            </div>
            <mat-divider></mat-divider>
            <button mat-menu-item (click)="logout()">
              <mat-icon>logout</mat-icon>
              <span>Sair</span>
            </button>
          </mat-menu>
        </mat-toolbar>

        @if (isLoading()) {
        <mat-progress-bar mode="indeterminate" />
        }

        <main class="content">
          <router-outlet />
        </main>
      </mat-sidenav-content>
    </mat-sidenav-container>
  `,
  styles: [
    `
      .sidenav-container {
        height: 100%;
      }

      .sidenav {
        width: 250px;
      }

      .sidenav-header {
        background: var(--mat-primary);
        color: white;
      }

      .logo {
        font-size: 1.2rem;
        font-weight: 500;
      }

      .toolbar {
        position: sticky;
        top: 0;
        z-index: 100;
      }

      .spacer {
        flex: 1 1 auto;
      }

      .content {
        padding: 24px;
        min-height: calc(100vh - 64px);
        background: #f5f5f5;
      }

      .user-info {
        padding: 12px 16px;
        display: flex;
        flex-direction: column;
        gap: 4px;
      }

      .active {
        background: rgba(0, 0, 0, 0.04);
      }

      @media (max-width: 768px) {
        .sidenav {
          width: 200px;
        }

        .content {
          padding: 16px;
        }
      }
    `,
  ],
})
export class ShellComponent {
  private readonly authService = inject(AuthService);
  private readonly loadingService = inject(LoadingService);

  readonly currentUser = this.authService.currentUser;
  readonly isLoading = this.loadingService.isLoading;
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

  readonly visibleNavItems = computed(() => {
    const userRole = this.currentUser()?.tipo;
    if (!userRole) return [];
    return this.navItems.filter((item) => item.roles.includes(userRole));
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
