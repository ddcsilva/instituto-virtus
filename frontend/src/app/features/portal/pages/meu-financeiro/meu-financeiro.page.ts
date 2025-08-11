import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';
import { MatExpansionModule } from '@angular/material/expansion';

import { PageHeaderComponent } from '../../../../shared/ui/components/page-header/page-header.component';
import { AuthService } from '../../../../core/auth/services/auth.service';
import { FinanceiroService } from '../../../financeiro/data-access/financeiro.service';
import { Mensalidade, Pagamento } from '../../../financeiro/models/financeiro.model';

@Component({
  selector: 'app-meu-financeiro',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatTableModule,
    MatChipsModule,
    MatButtonModule,
    MatIconModule,
    MatTabsModule,
    MatExpansionModule,
    CurrencyPipe,
    PageHeaderComponent,
  ],
  template: `
    <app-page-header title="Meu Financeiro" subtitle="Acompanhe suas mensalidades e pagamentos" />

    <!-- Resumo Financeiro -->
    <div class="resumo-cards">
      <mat-card>
        <mat-card-content>
          <div class="resumo-item">
            <mat-icon color="warn">warning</mat-icon>
            <div>
              <h3>Em Aberto</h3>
              <strong class="valor-warn">{{ totalAberto() | currency : 'BRL' }}</strong>
            </div>
          </div>
        </mat-card-content>
      </mat-card>

      <mat-card>
        <mat-card-content>
          <div class="resumo-item">
            <mat-icon color="primary">check_circle</mat-icon>
            <div>
              <h3>Pago este Mês</h3>
              <strong class="valor-success">{{ totalPagoMes() | currency : 'BRL' }}</strong>
            </div>
          </div>
        </mat-card-content>
      </mat-card>

      <mat-card>
        <mat-card-content>
          <div class="resumo-item">
            <mat-icon color="accent">account_balance_wallet</mat-icon>
            <div>
              <h3>Saldo</h3>
              <strong class="valor-info">{{ saldo() | currency : 'BRL' }}</strong>
            </div>
          </div>
        </mat-card-content>
      </mat-card>
    </div>

    <mat-tab-group>
      <!-- Tab Mensalidades -->
      <mat-tab label="Mensalidades">
        <div class="tab-content">
          @if (isResponsavel()) {
          <!-- Agrupado por Aluno para Responsável -->
          <mat-accordion>
            @for (aluno of alunosComMensalidades(); track aluno.id) {
            <mat-expansion-panel [expanded]="aluno.temPendencias">
              <mat-expansion-panel-header>
                <mat-panel-title>
                  {{ aluno.nome }}
                  @if (aluno.temPendencias) {
                  <mat-chip color="warn">{{ aluno.pendencias }} pendência(s)</mat-chip>
                  }
                </mat-panel-title>
                <mat-panel-description>
                  {{ aluno.turmas }}
                </mat-panel-description>
              </mat-expansion-panel-header>

              <table mat-table [dataSource]="aluno.mensalidades" class="full-width">
                <ng-container matColumnDef="competencia">
                  <th mat-header-cell *matHeaderCellDef>Competência</th>
                  <td mat-cell *matCellDef="let mensalidade">
                    {{ formatCompetencia(mensalidade.competencia) }}
                  </td>
                </ng-container>

                <ng-container matColumnDef="vencimento">
                  <th mat-header-cell *matHeaderCellDef>Vencimento</th>
                  <td mat-cell *matCellDef="let mensalidade">
                    {{ mensalidade.vencimento | date : 'dd/MM/yyyy' }}
                  </td>
                </ng-container>

                <ng-container matColumnDef="valor">
                  <th mat-header-cell *matHeaderCellDef>Valor</th>
                  <td mat-cell *matCellDef="let mensalidade">
                    {{ mensalidade.valor | currency : 'BRL' }}
                  </td>
                </ng-container>

                <ng-container matColumnDef="status">
                  <th mat-header-cell *matHeaderCellDef>Status</th>
                  <td mat-cell *matCellDef="let mensalidade">
                    <mat-chip [color]="getStatusColor(mensalidade.status)">
                      {{ getStatusLabel(mensalidade.status) }}
                    </mat-chip>
                  </td>
                </ng-container>

                <ng-container matColumnDef="acoes">
                  <th mat-header-cell *matHeaderCellDef>Ações</th>
                  <td mat-cell *matCellDef="let mensalidade">
                    @if (mensalidade.status === 'Pago') {
                    <button mat-icon-button (click)="baixarRecibo(mensalidade)">
                      <mat-icon>receipt</mat-icon>
                    </button>
                    }
                  </td>
                </ng-container>

                <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
                <tr mat-row *matRowDef="let row; columns: displayedColumns"></tr>
              </table>
            </mat-expansion-panel>
            }
          </mat-accordion>
          } @else {
          <!-- Tabela simples para Aluno -->
          <mat-card>
            <mat-card-content>
              <table mat-table [dataSource]="mensalidades()" class="full-width">
                <!-- Mesmas colunas da tabela acima -->
                <ng-container matColumnDef="competencia">
                  <th mat-header-cell *matHeaderCellDef>Competência</th>
                  <td mat-cell *matCellDef="let mensalidade">
                    {{ formatCompetencia(mensalidade.competencia) }}
                  </td>
                </ng-container>

                <ng-container matColumnDef="vencimento">
                  <th mat-header-cell *matHeaderCellDef>Vencimento</th>
                  <td mat-cell *matCellDef="let mensalidade">
                    {{ mensalidade.vencimento | date : 'dd/MM/yyyy' }}
                  </td>
                </ng-container>

                <ng-container matColumnDef="valor">
                  <th mat-header-cell *matHeaderCellDef>Valor</th>
                  <td mat-cell *matCellDef="let mensalidade">
                    {{ mensalidade.valor | currency : 'BRL' }}
                  </td>
                </ng-container>

                <ng-container matColumnDef="status">
                  <th mat-header-cell *matHeaderCellDef>Status</th>
                  <td mat-cell *matCellDef="let mensalidade">
                    <mat-chip [color]="getStatusColor(mensalidade.status)">
                      {{ getStatusLabel(mensalidade.status) }}
                    </mat-chip>
                  </td>
                </ng-container>

                <ng-container matColumnDef="acoes">
                  <th mat-header-cell *matHeaderCellDef>Ações</th>
                  <td mat-cell *matCellDef="let mensalidade">
                    @if (mensalidade.status === 'Pago') {
                    <button mat-icon-button (click)="baixarRecibo(mensalidade)">
                      <mat-icon>receipt</mat-icon>
                    </button>
                    }
                  </td>
                </ng-container>

                <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
                <tr mat-row *matRowDef="let row; columns: displayedColumns"></tr>
              </table>
            </mat-card-content>
          </mat-card>
          }
        </div>
      </mat-tab>

      <!-- Tab Histórico de Pagamentos -->
      <mat-tab label="Histórico de Pagamentos">
        <div class="tab-content">
          <mat-card>
            <mat-card-content>
              <table mat-table [dataSource]="pagamentos()" class="full-width">
                <ng-container matColumnDef="data">
                  <th mat-header-cell *matHeaderCellDef>Data</th>
                  <td mat-cell *matCellDef="let pagamento">
                    {{ pagamento.dataPagamento | date : 'dd/MM/yyyy' }}
                  </td>
                </ng-container>

                <ng-container matColumnDef="valorPago">
                  <th mat-header-cell *matHeaderCellDef>Valor Pago</th>
                  <td mat-cell *matCellDef="let pagamento">
                    {{ pagamento.valor | currency : 'BRL' }}
                  </td>
                </ng-container>

                <ng-container matColumnDef="meio">
                  <th mat-header-cell *matHeaderCellDef>Meio</th>
                  <td mat-cell *matCellDef="let pagamento">
                    {{ pagamento.meioPagamento }}
                  </td>
                </ng-container>

                <ng-container matColumnDef="mensalidades">
                  <th mat-header-cell *matHeaderCellDef>Mensalidades Pagas</th>
                  <td mat-cell *matCellDef="let pagamento">
                    {{ pagamento.parcelas.length }} mensalidade(s)
                  </td>
                </ng-container>

                <tr mat-header-row *matHeaderRowDef="pagamentosColumns"></tr>
                <tr mat-row *matRowDef="let row; columns: pagamentosColumns"></tr>
              </table>
            </mat-card-content>
          </mat-card>
        </div>
      </mat-tab>
    </mat-tab-group>
  `,
  styles: [
    `
      .resumo-cards {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
        gap: 24px;
        margin-bottom: 32px;
      }

      .resumo-item {
        display: flex;
        align-items: center;
        gap: 16px;

        mat-icon {
          font-size: 40px;
          width: 40px;
          height: 40px;
        }

        h3 {
          margin: 0 0 8px;
          font-size: 14px;
          color: #666;
        }

        strong {
          font-size: 24px;

          &.valor-warn {
            color: #f44336;
          }
          &.valor-success {
            color: #4caf50;
          }
          &.valor-info {
            color: #2196f3;
          }
        }
      }

      .tab-content {
        padding: 24px 0;
      }

      .full-width {
        width: 100%;
      }

      mat-chip {
        font-size: 12px;
        margin-left: 8px;
      }

      mat-expansion-panel-header {
        mat-panel-title {
          display: flex;
          align-items: center;
          gap: 8px;
        }
      }
    `,
  ],
})
export class MeuFinanceiroPage implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly financeiroService = inject(FinanceiroService);

  readonly mensalidades = signal<Mensalidade[]>([]);
  readonly pagamentos = signal<Pagamento[]>([]);
  readonly alunosComMensalidades = signal<any[]>([]);

  readonly totalAberto = signal(0);
  readonly totalPagoMes = signal(0);
  readonly saldo = signal(0);

  readonly displayedColumns = ['competencia', 'vencimento', 'valor', 'status', 'acoes'];
  readonly pagamentosColumns = ['data', 'valorPago', 'meio', 'mensalidades'];

  ngOnInit(): void {
    this.loadFinanceiroData();
  }

  isResponsavel(): boolean {
    return this.authService.usuarioAtual()?.tipo === 'Responsavel';
  }

  loadFinanceiroData(): void {
    const usuario = this.authService.usuarioAtual();
    if (!usuario || !usuario.pessoaId) return;

    if (this.isResponsavel()) {
      // Carregar mensalidades de todos os alunos vinculados
      this.financeiroService
        .getMensalidadesResponsavel(usuario.pessoaId)
        .subscribe(mensalidades => {
          this.processarMensalidadesResponsavel(mensalidades);
        });

      // Carregar saldo
      this.financeiroService.getSaldoResponsavel(usuario.pessoaId).subscribe(saldo => {
        this.saldo.set(saldo.saldo);
      });
    } else {
      // Carregar mensalidades do próprio aluno
      this.financeiroService.getMensalidades({ alunoId: usuario.pessoaId }).subscribe(result => {
        this.mensalidades.set(result.items);
        this.calcularTotais(result.items);
      });
    }

    // Carregar histórico de pagamentos
    this.financeiroService.getPagamentos({ responsavelId: usuario.pessoaId }).subscribe(result => {
      this.pagamentos.set(result.items);
    });
  }

  processarMensalidadesResponsavel(mensalidades: Mensalidade[]): void {
    // Agrupar por aluno
    const alunosMap = new Map<string, any>();

    mensalidades.forEach(m => {
      const alunoId = m.matricula?.aluno?.id || '';
      const alunoNome = m.matricula?.aluno?.nome || 'Aluno';

      if (!alunosMap.has(alunoId)) {
        alunosMap.set(alunoId, {
          id: alunoId,
          nome: alunoNome,
          turmas: new Set<string>(),
          mensalidades: [],
          pendencias: 0,
          temPendencias: false,
        });
      }

      const aluno = alunosMap.get(alunoId);
      aluno.mensalidades.push(m);
      aluno.turmas.add(m.matricula?.turma?.nome || '');

      if (m.status === 'EmAberto' || m.status === 'Atrasado') {
        aluno.pendencias++;
        aluno.temPendencias = true;
      }
    });

    const alunosArray = Array.from(alunosMap.values()).map(a => ({
      ...a,
      turmas: Array.from(a.turmas).join(', '),
    }));

    this.alunosComMensalidades.set(alunosArray);
    this.calcularTotais(mensalidades);
  }

  calcularTotais(mensalidades: Mensalidade[]): void {
    const aberto = mensalidades
      .filter(m => m.status === 'EmAberto' || m.status === 'Atrasado')
      .reduce((acc, m) => acc + (m.valor - m.valorPago), 0);

    const mesAtual = new Date().getMonth();
    const anoAtual = new Date().getFullYear();

    const pagoMes = mensalidades
      .filter(m => {
        if (!m.dataPagamento) return false;
        const dataPag = new Date(m.dataPagamento);
        return dataPag.getMonth() === mesAtual && dataPag.getFullYear() === anoAtual;
      })
      .reduce((acc, m) => acc + m.valorPago, 0);

    this.totalAberto.set(aberto);
    this.totalPagoMes.set(pagoMes);
  }

  formatCompetencia(competencia: string): string {
    const [ano, mes] = competencia.split('-');
    const meses = [
      'Jan',
      'Fev',
      'Mar',
      'Abr',
      'Mai',
      'Jun',
      'Jul',
      'Ago',
      'Set',
      'Out',
      'Nov',
      'Dez',
    ];
    return `${meses[parseInt(mes) - 1]}/${ano}`;
  }

  getStatusColor(status: string): 'primary' | 'warn' | 'accent' {
    switch (status) {
      case 'Pago':
        return 'primary';
      case 'EmAberto':
        return 'accent';
      case 'Atrasado':
        return 'warn';
      default:
        return 'accent';
    }
  }

  getStatusLabel(status: string): string {
    const labels: Record<string, string> = {
      EmAberto: 'Em Aberto',
      Pago: 'Pago',
      PagoParcial: 'Pago Parcial',
      Atrasado: 'Atrasado',
      Cancelado: 'Cancelado',
    };
    return labels[status] || status;
  }

  baixarRecibo(mensalidade: Mensalidade): void {
    // Implementar download do recibo
    console.log('Baixar recibo:', mensalidade.id);
  }
}
