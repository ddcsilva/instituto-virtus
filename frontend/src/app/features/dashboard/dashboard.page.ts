import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { AuthService } from '../../core/auth/services/auth.service';
import { PageHeaderComponent } from '../../shared/ui/components/page-header/page-header.component';

interface DashboardCard {
  title: string;
  value: string | number;
  icon: string;
  color: string;
  route: string;
  trend?: number;
}

interface QuickAction {
  label: string;
  icon: string;
  route: string;
  color: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    PageHeaderComponent,
  ],
  template: `
    <app-page-header
      [title]="'Bem-vindo, ' + userName()"
      [subtitle]="userRole() + ' - Instituto Virtus'"
    />

    <!-- Cards de Resumo -->
    <div class="dashboard-grid">
      @for (card of visibleCards(); track card.title) {
      <mat-card class="stat-card" [routerLink]="card.route">
        <mat-card-content>
          <div class="stat-header">
            <mat-icon [style.color]="card.color">{{ card.icon }}</mat-icon>
            @if (card.trend) {
            <mat-chip [color]="card.trend > 0 ? 'primary' : 'warn'">
              {{ card.trend > 0 ? '+' : '' }}{{ card.trend }}%
            </mat-chip>
            }
          </div>
          <div class="stat-value">{{ card.value }}</div>
          <div class="stat-label">{{ card.title }}</div>
        </mat-card-content>
      </mat-card>
      }
    </div>

    <!-- Ações Rápidas -->
    <h2 class="section-title">Ações Rápidas</h2>
    <div class="actions-grid">
      @for (action of visibleActions(); track action.label) {
      <mat-card class="action-card" [routerLink]="action.route">
        <mat-card-content>
          <mat-icon [style.color]="action.color">{{ action.icon }}</mat-icon>
          <span>{{ action.label }}</span>
        </mat-card-content>
      </mat-card>
      }
    </div>

    <!-- Atividades Recentes -->
    @if (isAdmin() || isCoordinator()) {
    <h2 class="section-title">Atividades Recentes</h2>
    <mat-card>
      <mat-card-content>
        <div class="activity-list">
          @for (activity of recentActivities(); track activity.id) {
          <div class="activity-item">
            <mat-icon [style.color]="activity.color">{{
              activity.icon
            }}</mat-icon>
            <div class="activity-content">
              <p>{{ activity.description }}</p>
              <small>{{
                activity.timestamp | date : 'dd/MM/yyyy HH:mm'
              }}</small>
            </div>
          </div>
          } @empty {
          <p class="empty-state">Nenhuma atividade recente</p>
          }
        </div>
      </mat-card-content>
    </mat-card>
    }

    <!-- Portal do Aluno/Responsável -->
    @if (isStudent() || isParent()) {
    <div class="portal-grid">
      <mat-card>
        <mat-card-header>
          <mat-card-title>Minhas Turmas</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <div class="portal-list">
            @for (turma of minhasTurmas(); track turma.id) {
            <div class="portal-item">
              <strong>{{ turma.nome }}</strong>
              <span>{{ turma.horario }}</span>
            </div>
            } @empty {
            <p class="empty-state">Nenhuma turma matriculada</p>
            }
          </div>
        </mat-card-content>
        <mat-card-actions>
          <button mat-button color="primary" routerLink="/portal/turmas">
            Ver Todas
          </button>
        </mat-card-actions>
      </mat-card>

      <mat-card>
        <mat-card-header>
          <mat-card-title>Próximos Pagamentos</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <div class="portal-list">
            @for (pagamento of proximosPagamentos(); track pagamento.id) {
            <div class="portal-item">
              <strong>{{ pagamento.competencia }}</strong>
              <span [class.warn]="pagamento.vencido">
                R$ {{ pagamento.valor }}
              </span>
            </div>
            } @empty {
            <p class="empty-state">Nenhum pagamento pendente</p>
            }
          </div>
        </mat-card-content>
        <mat-card-actions>
          <button mat-button color="primary" routerLink="/portal/financeiro">
            Ver Todos
          </button>
        </mat-card-actions>
      </mat-card>
    </div>
    }
  `,
  styles: [
    `
      .dashboard-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
        gap: 24px;
        margin-bottom: 32px;
      }

      .stat-card {
        cursor: pointer;
        transition: transform 0.2s, box-shadow 0.2s;

        &:hover {
          transform: translateY(-4px);
          box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
        }
      }

      .stat-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 16px;
      }

      .stat-header mat-icon {
        font-size: 32px;
        width: 32px;
        height: 32px;
      }

      .stat-value {
        font-size: 32px;
        font-weight: 500;
        margin-bottom: 8px;
      }

      .stat-label {
        color: #666;
        font-size: 14px;
      }

      .section-title {
        font-size: 20px;
        font-weight: 500;
        margin: 32px 0 16px;
      }

      .actions-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
        gap: 16px;
        margin-bottom: 32px;
      }

      .action-card {
        cursor: pointer;
        transition: transform 0.2s;

        &:hover {
          transform: scale(1.05);
        }

        mat-card-content {
          display: flex;
          flex-direction: column;
          align-items: center;
          padding: 24px;

          mat-icon {
            font-size: 40px;
            width: 40px;
            height: 40px;
            margin-bottom: 8px;
          }

          span {
            text-align: center;
            font-size: 14px;
          }
        }
      }

      .activity-list {
        display: flex;
        flex-direction: column;
        gap: 16px;
      }

      .activity-item {
        display: flex;
        gap: 16px;
        padding: 12px 0;
        border-bottom: 1px solid #f0f0f0;

        &:last-child {
          border-bottom: none;
        }

        mat-icon {
          flex-shrink: 0;
        }

        .activity-content {
          flex: 1;

          p {
            margin: 0 0 4px;
          }

          small {
            color: #999;
          }
        }
      }

      .portal-grid {
        display: grid;
        grid-template-columns: 1fr 1fr;
        gap: 24px;
      }

      .portal-list {
        display: flex;
        flex-direction: column;
        gap: 12px;
      }

      .portal-item {
        display: flex;
        justify-content: space-between;
        padding: 8px 0;
        border-bottom: 1px solid #f0f0f0;

        &:last-child {
          border-bottom: none;
        }

        .warn {
          color: #f44336;
        }
      }

      .empty-state {
        text-align: center;
        color: #999;
        padding: 24px;
        margin: 0;
      }

      @media (max-width: 768px) {
        .portal-grid {
          grid-template-columns: 1fr;
        }
      }
    `,
  ],
})
export class DashboardPage implements OnInit {
  private readonly authService = inject(AuthService);

  readonly userName = signal('');
  readonly userRole = signal('');
  readonly loading = signal(true);

  readonly cards = signal<DashboardCard[]>([]);
  readonly actions = signal<QuickAction[]>([]);
  readonly recentActivities = signal<any[]>([]);
  readonly minhasTurmas = signal<any[]>([]);
  readonly proximosPagamentos = signal<any[]>([]);

  ngOnInit(): void {
    const user = this.authService.currentUser();
    if (user) {
      this.userName.set(user.nome);
      this.userRole.set(this.getRoleName(user.tipo));
      this.loadDashboardData(user.tipo);
    }
  }

  getRoleName(tipo: string): string {
    const roles: Record<string, string> = {
      Admin: 'Administrador',
      Coordenador: 'Coordenador',
      Professor: 'Professor',
      Responsavel: 'Responsável',
      Aluno: 'Aluno',
    };
    return roles[tipo] || tipo;
  }

  isAdmin(): boolean {
    return this.authService.currentUser()?.tipo === 'Admin';
  }

  isCoordinator(): boolean {
    return this.authService.currentUser()?.tipo === 'Coordenador';
  }

  isTeacher(): boolean {
    return this.authService.currentUser()?.tipo === 'Professor';
  }

  isStudent(): boolean {
    return this.authService.currentUser()?.tipo === 'Aluno';
  }

  isParent(): boolean {
    return this.authService.currentUser()?.tipo === 'Responsavel';
  }

  visibleCards(): DashboardCard[] {
    const userType = this.authService.currentUser()?.tipo;

    if (userType === 'Admin' || userType === 'Coordenador') {
      return [
        {
          title: 'Total de Alunos',
          value: 245,
          icon: 'school',
          color: '#4caf50',
          route: '/pessoas',
          trend: 5,
        },
        {
          title: 'Turmas Ativas',
          value: 18,
          icon: 'groups',
          color: '#2196f3',
          route: '/turmas',
        },
        {
          title: 'Mensalidades em Aberto',
          value: 'R$ 15.480',
          icon: 'attach_money',
          color: '#ff9800',
          route: '/financeiro',
        },
        {
          title: 'Taxa de Presença',
          value: '92%',
          icon: 'fact_check',
          color: '#9c27b0',
          route: '/presencas',
          trend: -2,
        },
      ];
    }

    if (userType === 'Professor') {
      return [
        {
          title: 'Minhas Turmas',
          value: 4,
          icon: 'groups',
          color: '#2196f3',
          route: '/turmas',
        },
        {
          title: 'Total de Alunos',
          value: 68,
          icon: 'school',
          color: '#4caf50',
          route: '/turmas',
        },
        {
          title: 'Aulas Hoje',
          value: 3,
          icon: 'event',
          color: '#ff9800',
          route: '/presencas',
        },
        {
          title: 'Avaliações Pendentes',
          value: 12,
          icon: 'grade',
          color: '#9c27b0',
          route: '/avaliacoes',
        },
      ];
    }

    return [];
  }

  visibleActions(): QuickAction[] {
    const userType = this.authService.currentUser()?.tipo;

    if (userType === 'Admin' || userType === 'Coordenador') {
      return [
        {
          label: 'Nova Matrícula',
          icon: 'add_circle',
          route: '/matriculas/nova',
          color: '#4caf50',
        },
        {
          label: 'Registrar Pagamento',
          icon: 'payment',
          route: '/financeiro/conciliacao',
          color: '#2196f3',
        },
        {
          label: 'Lançar Presença',
          icon: 'fact_check',
          route: '/presencas',
          color: '#ff9800',
        },
        {
          label: 'Gerar Relatório',
          icon: 'assessment',
          route: '/relatorios',
          color: '#9c27b0',
        },
      ];
    }

    if (userType === 'Professor') {
      return [
        {
          label: 'Lançar Presença',
          icon: 'fact_check',
          route: '/presencas',
          color: '#ff9800',
        },
        {
          label: 'Lançar Notas',
          icon: 'grade',
          route: '/avaliacoes',
          color: '#9c27b0',
        },
        {
          label: 'Minhas Turmas',
          icon: 'groups',
          route: '/turmas',
          color: '#2196f3',
        },
      ];
    }

    if (userType === 'Aluno' || userType === 'Responsavel') {
      return [
        {
          label: 'Minhas Turmas',
          icon: 'groups',
          route: '/portal/turmas',
          color: '#2196f3',
        },
        {
          label: 'Financeiro',
          icon: 'attach_money',
          route: '/portal/financeiro',
          color: '#4caf50',
        },
        {
          label: 'Boletim',
          icon: 'grade',
          route: '/portal/boletim',
          color: '#9c27b0',
        },
        {
          label: 'Frequência',
          icon: 'fact_check',
          route: '/portal/frequencia',
          color: '#ff9800',
        },
      ];
    }

    return [];
  }

  loadDashboardData(userType: string): void {
    // Simulated data - replace with actual API calls
    if (userType === 'Admin' || userType === 'Coordenador') {
      this.recentActivities.set([
        {
          id: 1,
          description: 'Nova matrícula: João Silva - Violão Básico',
          timestamp: new Date(),
          icon: 'person_add',
          color: '#4caf50',
        },
        {
          id: 2,
          description: 'Pagamento recebido: Maria Santos - R$ 250,00',
          timestamp: new Date(),
          icon: 'payment',
          color: '#2196f3',
        },
        {
          id: 3,
          description: 'Turma criada: Teologia I - Manhã',
          timestamp: new Date(),
          icon: 'group_add',
          color: '#ff9800',
        },
      ]);
    }

    if (userType === 'Aluno' || userType === 'Responsavel') {
      this.minhasTurmas.set([
        { id: 1, nome: 'Violão Básico', horario: 'Seg/Qua 14:00' },
        { id: 2, nome: 'Teologia I', horario: 'Ter/Qui 19:00' },
      ]);

      this.proximosPagamentos.set([
        { id: 1, competencia: 'Janeiro/2024', valor: 250, vencido: false },
        { id: 2, competencia: 'Fevereiro/2024', valor: 250, vencido: false },
      ]);
    }

    this.loading.set(false);
  }
}
