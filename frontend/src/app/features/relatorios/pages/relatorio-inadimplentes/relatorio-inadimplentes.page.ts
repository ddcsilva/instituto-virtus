import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { PageHeaderComponent } from '../../../../shared/ui/components/page-header/page-header.component';
import { FinanceiroService } from '../../../financeiro/data-access/financeiro.service';

interface Inadimplente {
  alunoId: string;
  alunoNome: string;
  cpfAluno: string;
  responsavelNome: string;
  telefoneResponsavel: string;
  emailResponsavel: string;
  turmas: string[];
  totalDebito: number;
  mensalidadesAtraso: number;
  competenciaMaisAntiga: string;
  diasAtraso: number;
}

@Component({
  selector: 'app-relatorio-inadimplentes',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatTableModule,
    MatFormFieldModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    CurrencyPipe,
    PageHeaderComponent,
  ],
  template: `
    <app-page-header
      title="Relatório de Inadimplentes"
      subtitle="Alunos com mensalidades em atraso"
    />

    <mat-card class="filtros-card">
      <mat-card-content>
        <form [formGroup]="filtroForm" class="filtros-form">
          <mat-form-field>
            <mat-label>Competência</mat-label>
            <mat-select formControlName="competencia">
              @for (mes of mesesDisponiveis(); track mes.value) {
              <mat-option [value]="mes.value">{{ mes.label }}</mat-option>
              }
            </mat-select>
          </mat-form-field>

          <mat-form-field>
            <mat-label>Dias de Atraso</mat-label>
            <mat-select formControlName="diasAtraso">
              <mat-option value="">Todos</mat-option>
              <mat-option value="30">Mais de 30 dias</mat-option>
              <mat-option value="60">Mais de 60 dias</mat-option>
              <mat-option value="90">Mais de 90 dias</mat-option>
            </mat-select>
          </mat-form-field>

          <button mat-raised-button color="primary" (click)="gerarRelatorio()">
            <mat-icon>search</mat-icon>
            Gerar Relatório
          </button>

          <button
            mat-button
            (click)="exportarExcel()"
            [disabled]="inadimplentes().length === 0"
          >
            <mat-icon>download</mat-icon>
            Exportar Excel
          </button>
        </form>
      </mat-card-content>
    </mat-card>

    @if (loading()) {
    <div class="loading">
      <mat-spinner />
    </div>
    } @else {
    <!-- Resumo -->
    <div class="resumo-cards">
      <mat-card>
        <mat-card-content>
          <div class="stat">
            <mat-icon color="warn">people</mat-icon>
            <div>
              <h3>Total de Inadimplentes</h3>
              <strong>{{ inadimplentes().length }}</strong>
            </div>
          </div>
        </mat-card-content>
      </mat-card>

      <mat-card>
        <mat-card-content>
          <div class="stat">
            <mat-icon color="warn">attach_money</mat-icon>
            <div>
              <h3>Valor Total em Atraso</h3>
              <strong class="valor-warn">{{
                valorTotalAtraso() | currency : 'BRL'
              }}</strong>
            </div>
          </div>
        </mat-card-content>
      </mat-card>

      <mat-card>
        <mat-card-content>
          <div class="stat">
            <mat-icon color="warn">event_busy</mat-icon>
            <div>
              <h3>Média de Dias em Atraso</h3>
              <strong>{{ mediaDiasAtraso() }} dias</strong>
            </div>
          </div>
        </mat-card-content>
      </mat-card>
    </div>

    <!-- Tabela de Inadimplentes -->
    <mat-card>
      <mat-card-header>
        <mat-card-title>Lista de Inadimplentes</mat-card-title>
      </mat-card-header>
      <mat-card-content>
        <table mat-table [dataSource]="inadimplentes()" class="full-width">
          <ng-container matColumnDef="aluno">
            <th mat-header-cell *matHeaderCellDef>Aluno</th>
            <td mat-cell *matCellDef="let item">
              <div class="aluno-info">
                <strong>{{ item.alunoNome }}</strong>
                <small>CPF: {{ item.cpfAluno }}</small>
              </div>
            </td>
          </ng-container>

          <ng-container matColumnDef="responsavel">
            <th mat-header-cell *matHeaderCellDef>Responsável</th>
            <td mat-cell *matCellDef="let item">
              <div class="responsavel-info">
                <div>{{ item.responsavelNome }}</div>
                <small>{{ item.telefoneResponsavel }}</small>
              </div>
            </td>
          </ng-container>

          <ng-container matColumnDef="turmas">
            <th mat-header-cell *matHeaderCellDef>Turmas</th>
            <td mat-cell *matCellDef="let item">
              @for (turma of item.turmas; track turma) {
              <mat-chip>{{ turma }}</mat-chip>
              }
            </td>
          </ng-container>

          <ng-container matColumnDef="debito">
            <th mat-header-cell *matHeaderCellDef>Débito Total</th>
            <td mat-cell *matCellDef="let item">
              <strong class="valor-warn">{{
                item.totalDebito | currency : 'BRL'
              }}</strong>
            </td>
          </ng-container>

          <ng-container matColumnDef="atraso">
            <th mat-header-cell *matHeaderCellDef>Situação</th>
            <td mat-cell *matCellDef="let item">
              <div class="atraso-info">
                <mat-chip color="warn"
                  >{{ item.mensalidadesAtraso }} mensalidade(s)</mat-chip
                >
                <small>{{ item.diasAtraso }} dias de atraso</small>
                <small
                  >Desde
                  {{ formatCompetencia(item.competenciaMaisAntiga) }}</small
                >
              </div>
            </td>
          </ng-container>

          <ng-container matColumnDef="acoes">
            <th mat-header-cell *matHeaderCellDef>Ações</th>
            <td mat-cell *matCellDef="let item">
              <button
                mat-icon-button
                (click)="enviarLembrete(item)"
                matTooltip="Enviar Lembrete"
              >
                <mat-icon>email</mat-icon>
              </button>
              <button
                mat-icon-button
                (click)="enviarWhatsApp(item)"
                matTooltip="WhatsApp"
              >
                <mat-icon>chat</mat-icon>
              </button>
            </td>
          </ng-container>

          <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
          <tr mat-row *matRowDef="let row; columns: displayedColumns"></tr>

          <tr class="mat-row" *matNoDataRow>
            <td class="mat-cell" colspan="6">
              <div class="empty-state">
                <mat-icon>check_circle</mat-icon>
                <p>Nenhum inadimplente encontrado</p>
              </div>
            </td>
          </tr>
        </table>
      </mat-card-content>
    </mat-card>
    }
  `,
  styles: [
    `
      .filtros-card {
        margin-bottom: 24px;
      }

      .filtros-form {
        display: flex;
        gap: 16px;
        align-items: center;
        flex-wrap: wrap;
      }

      .loading {
        display: flex;
        justify-content: center;
        padding: 48px;
      }

      .resumo-cards {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
        gap: 24px;
        margin-bottom: 24px;
      }

      .stat {
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
        }
      }

      .full-width {
        width: 100%;
      }

      .aluno-info,
      .responsavel-info {
        display: flex;
        flex-direction: column;
        gap: 4px;

        small {
          color: #666;
        }
      }

      .atraso-info {
        display: flex;
        flex-direction: column;
        gap: 4px;

        small {
          color: #666;
          font-size: 11px;
        }
      }

      mat-chip {
        font-size: 11px;
        margin: 2px;
      }

      .empty-state {
        text-align: center;
        padding: 48px;
        color: #999;

        mat-icon {
          font-size: 48px;
          width: 48px;
          height: 48px;
          color: #4caf50;
        }
      }
    `,
  ],
})
export class RelatorioInadimplentesPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly financeiroService = inject(FinanceiroService);

  readonly loading = signal(false);
  readonly inadimplentes = signal<Inadimplente[]>([]);
  readonly valorTotalAtraso = signal(0);
  readonly mediaDiasAtraso = signal(0);

  readonly displayedColumns = [
    'aluno',
    'responsavel',
    'turmas',
    'debito',
    'atraso',
    'acoes',
  ];

  readonly filtroForm = this.fb.group({
    competencia: [this.getCompetenciaAtual()],
    diasAtraso: [''],
  });

  readonly mesesDisponiveis = signal(this.getMesesDisponiveis());

  ngOnInit(): void {
    this.gerarRelatorio();
  }

  gerarRelatorio(): void {
    this.loading.set(true);
    const competencia = this.filtroForm.get('competencia')?.value || '';

    this.financeiroService.getInadimplentes(competencia).subscribe({
      next: (data) => {
        this.processarInadimplentes(data);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
      },
    });
  }

  processarInadimplentes(data: any[]): void {
    // Simulated processing - adapt based on actual API response
    const inadimplentes: Inadimplente[] = data.map((item) => ({
      alunoId: item.alunoId,
      alunoNome: item.alunoNome,
      cpfAluno: item.cpfAluno,
      responsavelNome: item.responsavelNome,
      telefoneResponsavel: item.telefoneResponsavel,
      emailResponsavel: item.emailResponsavel,
      turmas: item.turmas,
      totalDebito: item.totalDebito,
      mensalidadesAtraso: item.mensalidadesAtraso,
      competenciaMaisAntiga: item.competenciaMaisAntiga,
      diasAtraso: item.diasAtraso,
    }));

    // Aplicar filtro de dias se selecionado
    const diasFiltro = this.filtroForm.get('diasAtraso')?.value;
    if (diasFiltro) {
      const filtrados = inadimplentes.filter(
        (i) => i.diasAtraso >= parseInt(diasFiltro)
      );
      this.inadimplentes.set(filtrados);
    } else {
      this.inadimplentes.set(inadimplentes);
    }

    // Calcular totais
    const total = this.inadimplentes().reduce(
      (acc, i) => acc + i.totalDebito,
      0
    );
    this.valorTotalAtraso.set(total);

    const media =
      this.inadimplentes().length > 0
        ? Math.round(
            this.inadimplentes().reduce((acc, i) => acc + i.diasAtraso, 0) /
              this.inadimplentes().length
          )
        : 0;
    this.mediaDiasAtraso.set(media);
  }

  getCompetenciaAtual(): string {
    const hoje = new Date();
    return `${hoje.getFullYear()}-${String(hoje.getMonth() + 1).padStart(
      2,
      '0'
    )}`;
  }

  getMesesDisponiveis(): any[] {
    const meses = [];
    const hoje = new Date();

    for (let i = 0; i < 12; i++) {
      const data = new Date(hoje.getFullYear(), hoje.getMonth() - i, 1);
      const value = `${data.getFullYear()}-${String(
        data.getMonth() + 1
      ).padStart(2, '0')}`;
      const label = `${this.getNomeMes(data.getMonth())}/${data.getFullYear()}`;
      meses.push({ value, label });
    }

    return meses;
  }

  getNomeMes(mes: number): string {
    const nomes = [
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
    return nomes[mes];
  }

  formatCompetencia(competencia: string): string {
    const [ano, mes] = competencia.split('-');
    return `${this.getNomeMes(parseInt(mes) - 1)}/${ano}`;
  }

  exportarExcel(): void {
    // Implementar exportação para Excel
    console.log('Exportar para Excel');
  }

  enviarLembrete(inadimplente: Inadimplente): void {
    // Implementar envio de e-mail
    console.log('Enviar lembrete para:', inadimplente.emailResponsavel);
  }

  enviarWhatsApp(inadimplente: Inadimplente): void {
    const mensagem = `Olá ${
      inadimplente.responsavelNome
    }, identificamos pendências financeiras do aluno(a) ${
      inadimplente.alunoNome
    }. Valor total: ${this.formatarMoeda(
      inadimplente.totalDebito
    )}. Por favor, entre em contato conosco.`;
    const telefone = inadimplente.telefoneResponsavel.replace(/\D/g, '');
    const url = `https://wa.me/55${telefone}?text=${encodeURIComponent(
      mensagem
    )}`;
    window.open(url, '_blank');
  }

  private formatarMoeda(valor: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(valor);
  }
}
