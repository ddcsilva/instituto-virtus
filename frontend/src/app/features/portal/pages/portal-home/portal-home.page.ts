import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatListModule } from '@angular/material/list';
import { MatTabsModule } from '@angular/material/tabs';
import { MatBadgeModule } from '@angular/material/badge';

import { AuthService } from '../../../../core/auth/services/auth.service';
import { PageHeaderComponent } from '../../../../shared/ui/components/page-header/page-header.component';

@Component({
  selector: 'app-portal-home',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatChipsModule,
    MatListModule,
    MatTabsModule,
    MatBadgeModule,
    PageHeaderComponent,
  ],
  template: `
    <app-page-header
      title="Portal do {{ userType() }}"
      subtitle="Acompanhe suas informações acadêmicas e financeiras"
    />

    <!-- Cards de Acesso Rápido -->
    <div class="portal-grid">
      <mat-card class="portal-card" routerLink="/portal/turmas">
        <mat-card-content>
          <mat-icon class="card-icon" color="primary">school</mat-icon>
          <h3>Minhas Turmas</h3>
          <p>{{ turmasAtivas() }} turma(s) ativa(s)</p>
        </mat-card-content>
      </mat-card>

      <mat-card class="portal-card" routerLink="/portal/financeiro">
        <mat-card-content>
          <mat-icon
            class="card-icon"
            [color]="temPendencias() ? 'warn' : 'primary'"
          >
            attach_money
          </mat-icon>
          <h3>Financeiro</h3>
          <p>
            @if (temPendencias()) {
            <mat-chip color="warn"
              >{{ mensalidadesPendentes() }} pendência(s)</mat-chip
            >
            } @else {
            <mat-chip color="primary">Em dia</mat-chip>
            }
          </p>
        </mat-card-content>
      </mat-card>

      <mat-card class="portal-card" routerLink="/portal/boletim">
        <mat-card-content>
          <mat-icon class="card-icon" color="primary">grade</mat-icon>
          <h3>Boletim</h3>
          <p>Notas e médias</p>
        </mat-card-content>
      </mat-card>

      <mat-card class="portal-card" routerLink="/portal/frequencia">
        <mat-card-content>
          <mat-icon class="card-icon" color="primary">fact_check</mat-icon>
          <h3>Frequência</h3>
          <p>{{ frequenciaMedia() }}% de presença</p>
        </mat-card-content>
      </mat-card>
    </div>

    <!-- Informações Detalhadas -->
    <div class="info-section">
      @if (isResponsavel()) {
      <!-- Tabs para Responsável com múltiplos alunos -->
      <mat-tab-group>
        @for (aluno of alunosVinculados(); track aluno.id) {
        <mat-tab [label]="aluno.nome">
          <div class="tab-content">
            <div class="info-cards">
              <!-- Próximas Aulas -->
              <mat-card>
                <mat-card-header>
                  <mat-card-title>Próximas Aulas</mat-card-title>
                </mat-card-header>
                <mat-card-content>
                  <mat-list>
                    @for (aula of getProximasAulas(aluno.id); track aula.id) {
                    <mat-list-item>
                      <mat-icon matListItemIcon>event</mat-icon>
                      <div matListItemTitle>{{ aula.turma }}</div>
                      <div matListItemLine>
                        {{ aula.data | date : 'dd/MM' }} - {{ aula.horario }}
                      </div>
                    </mat-list-item>
                    } @empty {
                    <p class="empty-state">Nenhuma aula agendada</p>
                    }
                  </mat-list>
                </mat-card-content>
              </mat-card>

              <!-- Últimas Notas -->
              <mat-card>
                <mat-card-header>
                  <mat-card-title>Últimas Notas</mat-card-title>
                </mat-card-header>
                <mat-card-content>
                  <mat-list>
                    @for (nota of getUltimasNotas(aluno.id); track nota.id) {
                    <mat-list-item>
                      <mat-icon matListItemIcon>assignment</mat-icon>
                      <div matListItemTitle>
                        {{ nota.avaliacao }} - {{ nota.turma }}
                      </div>
                      <div matListItemLine>
                        <mat-chip
                          [color]="nota.valor >= 6 ? 'primary' : 'warn'"
                        >
                          {{ nota.valor }}/10
                        </mat-chip>
                      </div>
                    </mat-list-item>
                    } @empty {
                    <p class="empty-state">Nenhuma nota lançada</p>
                    }
                  </mat-list>
                </mat-card-content>
              </mat-card>
            </div>

            <!-- Resumo Financeiro do Aluno -->
            <mat-card class="financeiro-resumo">
              <mat-card-header>
                <mat-card-title
                  >Resumo Financeiro - {{ aluno.nome }}</mat-card-title
                >
              </mat-card-header>
              <mat-card-content>
                <div class="financeiro-grid">
                  <div class="financeiro-item">
                    <span>Mensalidades em Aberto:</span>
                    <strong>{{ getMensalidadesAbertas(aluno.id) }}</strong>
                  </div>
                  <div class="financeiro-item">
                    <span>Valor Total Pendente:</span>
                    <strong class="valor-pendente">{{
                      getValorPendente(aluno.id) | currency : 'BRL'
                    }}</strong>
                  </div>
                  <div class="financeiro-item">
                    <span>Próximo Vencimento:</span>
                    <strong>{{
                      getProximoVencimento(aluno.id) | date : 'dd/MM/yyyy'
                    }}</strong>
                  </div>
                </div>
              </mat-card-content>
              <mat-card-actions>
                <button
                  mat-button
                  color="primary"
                  routerLink="/portal/financeiro"
                >
                  Ver Detalhes
                </button>
              </mat-card-actions>
            </mat-card>
          </div>
        </mat-tab>
        }
      </mat-tab-group>
      } @else {
      <!-- View para Aluno -->
      <div class="info-cards">
        <!-- Próximas Aulas -->
        <mat-card>
          <mat-card-header>
            <mat-card-title>Próximas Aulas</mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <mat-list>
              @for (aula of proximasAulas(); track aula.id) {
              <mat-list-item>
                <mat-icon matListItemIcon>event</mat-icon>
                <div matListItemTitle>{{ aula.turma }}</div>
                <div matListItemLine>
                  {{ aula.data | date : 'dd/MM' }} - {{ aula.horario }}
                </div>
              </mat-list-item>
              } @empty {
              <p class="empty-state">Nenhuma aula agendada</p>
              }
            </mat-list>
          </mat-card-content>
        </mat-card>

        <!-- Últimas Notas -->
        <mat-card>
          <mat-card-header>
            <mat-card-title>Últimas Notas</mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <mat-list>
              @for (nota of ultimasNotas(); track nota.id) {
              <mat-list-item>
                <mat-icon matListItemIcon>assignment</mat-icon>
                <div matListItemTitle>
                  {{ nota.avaliacao }} - {{ nota.turma }}
                </div>
                <div matListItemLine>
                  <mat-chip [color]="nota.valor >= 6 ? 'primary' : 'warn'">
                    {{ nota.valor }}/10
                  </mat-chip>
                </div>
              </mat-list-item>
              } @empty {
              <p class="empty-state">Nenhuma nota lançada</p>
              }
            </mat-list>
          </mat-card-content>
        </mat-card>
      </div>
      }
    </div>

    <!-- Avisos e Comunicados -->
    <mat-card class="avisos-card">
      <mat-card-header>
        <mat-card-title>
          <mat-icon>notifications</mat-icon>
          Avisos e Comunicados
        </mat-card-title>
      </mat-card-header>
      <mat-card-content>
        <mat-list>
          @for (aviso of avisos(); track aviso.id) {
          <mat-list-item>
            <mat-icon matListItemIcon [color]="aviso.importante ? 'warn' : ''">
              {{ aviso.importante ? 'priority_high' : 'info' }}
            </mat-icon>
            <div matListItemTitle>{{ aviso.titulo }}</div>
            <div matListItemLine>{{ aviso.data | date : 'dd/MM/yyyy' }}</div>
            <div matListItemLine>{{ aviso.descricao }}</div>
          </mat-list-item>
          } @empty {
          <p class="empty-state">Nenhum aviso no momento</p>
          }
        </mat-list>
      </mat-card-content>
    </mat-card>
  `,
  styles: [
    `
      .portal-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
        gap: 24px;
        margin-bottom: 32px;
      }

      .portal-card {
        cursor: pointer;
        transition: transform 0.2s, box-shadow 0.2s;

        &:hover {
          transform: translateY(-4px);
          box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
        }

        mat-card-content {
          text-align: center;
          padding: 24px;
        }

        .card-icon {
          font-size: 48px;
          width: 48px;
          height: 48px;
          margin: 0 auto 16px;
        }

        h3 {
          margin: 16px 0 8px;
          font-size: 18px;
        }

        p {
          margin: 0;
          color: #666;
        }
      }

      .info-section {
        margin-bottom: 32px;
      }

      .tab-content {
        padding: 24px 0;
      }

      .info-cards {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
        gap: 24px;
        margin-bottom: 24px;
      }

      .financeiro-resumo {
        margin-top: 24px;
      }

      .financeiro-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
        gap: 16px;
        padding: 16px;
        background: #f5f5f5;
        border-radius: 4px;
      }

      .financeiro-item {
        display: flex;
        flex-direction: column;
        gap: 8px;

        span {
          color: #666;
          font-size: 14px;
        }

        strong {
          font-size: 18px;

          &.valor-pendente {
            color: #f44336;
          }
        }
      }

      .avisos-card {
        mat-card-title {
          display: flex;
          align-items: center;
          gap: 8px;
        }
      }

      .empty-state {
        text-align: center;
        color: #999;
        padding: 24px;
      }

      mat-chip {
        font-size: 12px;
      }
    `,
  ],
})
export class PortalHomePage implements OnInit {
  private readonly authService = inject(AuthService);

  readonly userType = signal('');
  readonly turmasAtivas = signal(0);
  readonly mensalidadesPendentes = signal(0);
  readonly frequenciaMedia = signal(95);
  readonly temPendencias = signal(false);

  readonly alunosVinculados = signal<any[]>([]);
  readonly proximasAulas = signal<any[]>([]);
  readonly ultimasNotas = signal<any[]>([]);
  readonly avisos = signal<any[]>([]);

  ngOnInit(): void {
    const user = this.authService.currentUser();
    if (user) {
      this.userType.set(user.tipo === 'Responsavel' ? 'Responsável' : 'Aluno');
      this.loadPortalData(user.tipo, user.pessoaId);
    }
  }

  isResponsavel(): boolean {
    return this.authService.currentUser()?.tipo === 'Responsavel';
  }

  loadPortalData(tipo: string, pessoaId?: string): void {
    // Simulated data - replace with actual API calls
    if (tipo === 'Responsavel') {
      this.alunosVinculados.set([
        { id: '1', nome: 'João Silva' },
        { id: '2', nome: 'Maria Silva' },
      ]);
      this.mensalidadesPendentes.set(2);
      this.temPendencias.set(true);
    } else {
      this.proximasAulas.set([
        { id: '1', turma: 'Violão Básico', data: new Date(), horario: '14:00' },
        { id: '2', turma: 'Teologia I', data: new Date(), horario: '19:00' },
      ]);

      this.ultimasNotas.set([
        { id: '1', avaliacao: 'Prova 1', turma: 'Violão Básico', valor: 8.5 },
        { id: '2', avaliacao: 'Trabalho', turma: 'Teologia I', valor: 9.0 },
      ]);
    }

    this.turmasAtivas.set(2);

    this.avisos.set([
      {
        id: '1',
        titulo: 'Recesso de Fim de Ano',
        descricao: 'As aulas estarão suspensas de 23/12 a 07/01',
        data: new Date(),
        importante: true,
      },
      {
        id: '2',
        titulo: 'Avaliações Finais',
        descricao:
          'As avaliações finais serão realizadas na última semana de novembro',
        data: new Date(),
        importante: false,
      },
    ]);
  }

  getProximasAulas(alunoId: string): any[] {
    // Mock - implementar chamada real
    return [
      { id: '1', turma: 'Violão Básico', data: new Date(), horario: '14:00' },
    ];
  }

  getUltimasNotas(alunoId: string): any[] {
    // Mock - implementar chamada real
    return [
      { id: '1', avaliacao: 'Prova 1', turma: 'Violão Básico', valor: 8.5 },
    ];
  }

  getMensalidadesAbertas(alunoId: string): number {
    return 2;
  }

  getValorPendente(alunoId: string): number {
    return 500;
  }

  getProximoVencimento(alunoId: string): Date {
    return new Date();
  }
}
