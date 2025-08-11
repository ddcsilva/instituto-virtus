import { Component, inject, signal, computed, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, Router } from '@angular/router';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatBadgeModule } from '@angular/material/badge';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatRippleModule } from '@angular/material/core';
import { MatDividerModule } from '@angular/material/divider';
import { AuthService } from '../../auth/services/auth.service';
import { LoadingService } from '../../http/services/loading.service';
import { fromEvent, Subject } from 'rxjs';
import { takeUntil, debounceTime } from 'rxjs/operators';

interface NavItem {
  label: string;
  icon: string;
  route: string;
  roles: string[];
  badge?: number;
  badgeColor?: 'primary' | 'accent' | 'warn';
}

interface QuickAction {
  icon: string;
  tooltip: string;
  action: () => void;
  badge?: number;
}

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    RouterLink,
    MatSidenavModule,
    MatToolbarModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    MatMenuModule,
    MatProgressBarModule,
    MatBadgeModule,
    MatTooltipModule,
    MatRippleModule,
    MatDividerModule,
  ],
  templateUrl: './shell.component.html',
  styleUrls: ['./shell.component.scss'],
})
export class ShellComponent implements OnInit, OnDestroy {
  private readonly authService = inject(AuthService);
  private readonly loadingService = inject(LoadingService);
  readonly router = inject(Router);
  private readonly destroy$ = new Subject<void>();

  readonly usuarioAtual = this.authService.usuarioAtual;
  readonly carregando = this.loadingService.isLoading;
  readonly isHandset = signal(window.innerWidth < 768);
  readonly isCollapsed = signal(false);
  readonly currentTime = signal(new Date());

  // Notificações simuladas - em produção viriam de um serviço
  readonly notificationCount = signal(3);
  readonly hasNewMessages = signal(true);

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

  readonly quickActions: QuickAction[] = [
    {
      icon: 'notifications',
      tooltip: 'Notificações',
      action: () => this.openNotifications(),
      badge: 3,
    },
    {
      icon: 'message',
      tooltip: 'Mensagens',
      action: () => this.openMessages(),
      badge: 2,
    },
    {
      icon: 'help_outline',
      tooltip: 'Ajuda',
      action: () => this.openHelp(),
    },
  ];

  readonly itensVisiveis = computed(() => {
    const tipoUsuario = this.usuarioAtual()?.tipo;
    if (!tipoUsuario) return [];
    return this.navItems.filter(item => item.roles.includes(tipoUsuario));
  });

  readonly saudacao = computed(() => {
    const hora = this.currentTime().getHours();
    if (hora < 12) return 'Bom dia';
    if (hora < 18) return 'Boa tarde';
    return 'Boa noite';
  });

  readonly userRole = computed(() => {
    const tipo = this.usuarioAtual()?.tipo;
    switch (tipo) {
      case 'Admin':
        return 'Administrador';
      case 'Coordenador':
        return 'Coordenação';
      case 'Professor':
        return 'Professor';
      case 'Aluno':
        return 'Aluno';
      case 'Responsavel':
        return 'Responsável';
      default:
        return tipo || 'Usuário';
    }
  });

  ngOnInit(): void {
    // Atualizar responsividade
    fromEvent(window, 'resize')
      .pipe(debounceTime(200), takeUntil(this.destroy$))
      .subscribe(() => {
        this.isHandset.set(window.innerWidth < 768);
      });

    // Atualizar relógio
    setInterval(() => {
      this.currentTime.set(new Date());
    }, 60000); // Atualiza a cada minuto
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  toggleSidenav(): void {
    this.isCollapsed.update(v => !v);
  }

  logout(): void {
    this.authService.logout();
  }

  navigateToProfile(): void {
    this.router.navigate(['/perfil']);
  }

  navigateToSettings(): void {
    this.router.navigate(['/configuracoes']);
  }

  openNotifications(): void {
    // Implementar abertura de painel de notificações
    console.log('Abrindo notificações...');
    this.notificationCount.set(0);
  }

  openMessages(): void {
    // Implementar abertura de mensagens
    console.log('Abrindo mensagens...');
    this.hasNewMessages.set(false);
  }

  openHelp(): void {
    // Implementar abertura de ajuda
    window.open('/ajuda', '_blank');
  }

  obterIniciais(): string {
    const nome = this.usuarioAtual()?.nome || '';
    return nome
      .split(' ')
      .map(n => n.charAt(0))
      .slice(0, 2)
      .join('')
      .toUpperCase();
  }

  formatTime(): string {
    const now = this.currentTime();
    return now.toLocaleTimeString('pt-BR', {
      hour: '2-digit',
      minute: '2-digit',
    });
  }

  formatDate(): string {
    const now = this.currentTime();
    const options: Intl.DateTimeFormatOptions = {
      weekday: 'long',
      day: 'numeric',
      month: 'long',
    };
    return now.toLocaleDateString('pt-BR', options);
  }
}
