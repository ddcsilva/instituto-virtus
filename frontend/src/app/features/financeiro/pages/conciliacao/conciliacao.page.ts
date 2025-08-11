import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatStepper, MatStepperModule } from '@angular/material/stepper';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSliderModule } from '@angular/material/slider';
import { MatRadioModule } from '@angular/material/radio';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { Router } from '@angular/router';

import { PageHeaderComponent } from '../../../../shared/ui/components/page-header/page-header.component';
import { FinanceiroStore } from '../../data-access/financeiro.store';
import { PessoasStore } from '../../../pessoas/data-access/pessoas.store';
import {
  MensalidadeParaAlocar,
  CreatePagamentoRequest,
} from '../../models/financeiro.model';

@Component({
  selector: 'app-conciliacao',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatStepperModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatCheckboxModule,
    MatSliderModule,
    MatRadioModule,
    MatChipsModule,
    MatDividerModule,
    MatProgressBarModule,
    MatDatepickerModule,
    MatNativeDateModule,
    CurrencyPipe,
    PageHeaderComponent,
  ],
  providers: [FinanceiroStore, PessoasStore],
  template: `
    <app-page-header
      title="Conciliação de Pagamentos"
      subtitle="Registre e aloque pagamentos para mensalidades"
    />

    <mat-stepper #stepper linear>
      <!-- Step 1: Dados do Pagamento -->
      <mat-step [stepControl]="pagamentoForm">
        <ng-template matStepLabel>Dados do Pagamento</ng-template>

        <mat-card>
          <mat-card-content>
            <form [formGroup]="pagamentoForm">
              <div class="form-grid">
                <mat-form-field>
                  <mat-label>Responsável</mat-label>
                  <mat-select
                    formControlName="responsavelId"
                    (selectionChange)="onResponsavelChange($event.value)"
                  >
                    @for (responsavel of responsaveis(); track responsavel.id) {
                    <mat-option [value]="responsavel.id">
                      {{ responsavel.nome }} - {{ responsavel.cpf }}
                    </mat-option>
                    }
                  </mat-select>
                  @if (pagamentoForm.get('responsavelId')?.hasError('required'))
                  {
                  <mat-error>Selecione o responsável</mat-error>
                  }
                </mat-form-field>

                <mat-form-field>
                  <mat-label>Valor do Pagamento</mat-label>
                  <input
                    matInput
                    type="number"
                    formControlName="valor"
                    placeholder="0,00"
                  />
                  <span matPrefix>R$ </span>
                  @if (pagamentoForm.get('valor')?.hasError('required')) {
                  <mat-error>Informe o valor</mat-error>
                  } @if (pagamentoForm.get('valor')?.hasError('min')) {
                  <mat-error>Valor deve ser maior que zero</mat-error>
                  }
                </mat-form-field>

                <mat-form-field>
                  <mat-label>Data do Pagamento</mat-label>
                  <input
                    matInput
                    [matDatepicker]="picker"
                    formControlName="dataPagamento"
                  />
                  <mat-datepicker-toggle matIconSuffix [for]="picker" />
                  <mat-datepicker #picker />
                  @if (pagamentoForm.get('dataPagamento')?.hasError('required'))
                  {
                  <mat-error>Informe a data</mat-error>
                  }
                </mat-form-field>

                <mat-form-field>
                  <mat-label>Meio de Pagamento</mat-label>
                  <mat-select formControlName="meioPagamento">
                    <mat-option value="Pix">PIX</mat-option>
                    <mat-option value="Dinheiro">Dinheiro</mat-option>
                    <mat-option value="Cartao">Cartão</mat-option>
                    <mat-option value="Boleto">Boleto</mat-option>
                    <mat-option value="Transferencia">Transferência</mat-option>
                    <mat-option value="Outro">Outro</mat-option>
                  </mat-select>
                  @if (pagamentoForm.get('meioPagamento')?.hasError('required'))
                  {
                  <mat-error>Selecione o meio de pagamento</mat-error>
                  }
                </mat-form-field>

                <mat-form-field class="full-width">
                  <mat-label>Observações</mat-label>
                  <textarea
                    matInput
                    formControlName="observacao"
                    rows="3"
                  ></textarea>
                </mat-form-field>
              </div>

              @if (saldo() && saldo()! > 0) {
              <div class="saldo-info">
                <mat-icon>account_balance_wallet</mat-icon>
                <span>Saldo disponível: {{ saldo() | currency : 'BRL' }}</span>
              </div>
              }
            </form>
          </mat-card-content>
          <mat-card-actions align="end">
            <button
              mat-raised-button
              color="primary"
              [disabled]="pagamentoForm.invalid"
              (click)="stepper.next()"
            >
              Próximo
            </button>
          </mat-card-actions>
        </mat-card>
      </mat-step>

      <!-- Step 2: Seleção de Mensalidades -->
      <mat-step>
        <ng-template matStepLabel>Alocação de Valores</ng-template>

        <mat-card>
          <mat-card-header>
            <mat-card-title>Mensalidades em Aberto</mat-card-title>
            <mat-card-subtitle>
              Selecione e aloque os valores para cada mensalidade
            </mat-card-subtitle>
          </mat-card-header>

          <mat-card-content>
            <div class="alocacao-controls">
              <mat-radio-group
                [value]="estrategiaAlocacao()"
                (valueChange)="estrategiaAlocacao.set($event)"
              >
                <mat-radio-button value="automatica"
                  >Alocação Automática</mat-radio-button
                >
                <mat-radio-button value="manual"
                  >Alocação Manual</mat-radio-button
                >
              </mat-radio-group>

              @if (estrategiaAlocacao() === 'automatica') {
              <button
                mat-raised-button
                color="accent"
                (click)="alocarAutomaticamente()"
              >
                <mat-icon>auto_awesome</mat-icon>
                Alocar (Mais Antigos Primeiro)
              </button>
              }

              <button mat-button (click)="limparAlocacao()">
                <mat-icon>clear</mat-icon>
                Limpar Alocação
              </button>
            </div>

            <table
              mat-table
              [dataSource]="mensalidadesParaAlocar()"
              class="full-width"
            >
              <ng-container matColumnDef="select">
                <th mat-header-cell *matHeaderCellDef></th>
                <td mat-cell *matCellDef="let mensalidade">
                  <mat-checkbox
                    [checked]="mensalidade.selecionada"
                    (change)="toggleMensalidade(mensalidade.mensalidade.id)"
                    [disabled]="estrategiaAlocacao() === 'automatica'"
                  >
                  </mat-checkbox>
                </td>
              </ng-container>

              <ng-container matColumnDef="competencia">
                <th mat-header-cell *matHeaderCellDef>Competência</th>
                <td mat-cell *matCellDef="let mensalidade">
                  {{ formatCompetencia(mensalidade.mensalidade.competencia) }}
                </td>
              </ng-container>

              <ng-container matColumnDef="vencimento">
                <th mat-header-cell *matHeaderCellDef>Vencimento</th>
                <td mat-cell *matCellDef="let mensalidade">
                  {{ mensalidade.mensalidade.vencimento | date : 'dd/MM/yyyy' }}
                  @if (isVencido(mensalidade.mensalidade.vencimento)) {
                  <mat-chip color="warn">Vencido</mat-chip>
                  }
                </td>
              </ng-container>

              <ng-container matColumnDef="aluno">
                <th mat-header-cell *matHeaderCellDef>Aluno</th>
                <td mat-cell *matCellDef="let mensalidade">
                  {{ mensalidade.mensalidade.matricula?.aluno?.nome || '-' }}
                </td>
              </ng-container>

              <ng-container matColumnDef="valor">
                <th mat-header-cell *matHeaderCellDef>Valor</th>
                <td mat-cell *matCellDef="let mensalidade">
                  {{ mensalidade.mensalidade.valor | currency : 'BRL' }}
                </td>
              </ng-container>

              <ng-container matColumnDef="pendente">
                <th mat-header-cell *matHeaderCellDef>Pendente</th>
                <td mat-cell *matCellDef="let mensalidade">
                  <strong>{{
                    mensalidade.valorPendente | currency : 'BRL'
                  }}</strong>
                </td>
              </ng-container>

              <ng-container matColumnDef="alocar">
                <th mat-header-cell *matHeaderCellDef>Valor a Alocar</th>
                <td mat-cell *matCellDef="let mensalidade">
                  @if (estrategiaAlocacao() === 'manual' &&
                  mensalidade.selecionada) {
                  <mat-form-field class="valor-input">
                    <input
                      matInput
                      type="number"
                      [value]="mensalidade.valorAlocado"
                      [max]="mensalidade.valorPendente"
                      (input)="
                        atualizarValorAlocado(
                          mensalidade.mensalidade.id,
                          $event
                        )
                      "
                    />
                    <span matPrefix>R$ </span>
                  </mat-form-field>
                  } @else {
                  <span [class.valor-alocado]="mensalidade.valorAlocado > 0">
                    {{ mensalidade.valorAlocado | currency : 'BRL' }}
                  </span>
                  }
                </td>
              </ng-container>

              <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
              <tr mat-row *matRowDef="let row; columns: displayedColumns"></tr>
            </table>

            <div class="resumo-alocacao">
              <div class="resumo-item">
                <span>Valor do Pagamento:</span>
                <strong>{{ valorPagamento() | currency : 'BRL' }}</strong>
              </div>
              @if (saldo() && saldo()! > 0) {
              <div class="resumo-item">
                <span>Saldo Anterior:</span>
                <strong>{{ saldo() | currency : 'BRL' }}</strong>
              </div>
              }
              <div class="resumo-item">
                <span>Total Alocado:</span>
                <strong [class.success]="totalAlocado() > 0">
                  {{ totalAlocado() | currency : 'BRL' }}
                </strong>
              </div>
              <mat-divider></mat-divider>
              <div class="resumo-item">
                <span>Saldo Restante:</span>
                <strong [class.warn]="saldoRestante() > 0">
                  {{ saldoRestante() | currency : 'BRL' }}
                </strong>
              </div>
            </div>
          </mat-card-content>

          <mat-card-actions align="end">
            <button mat-button (click)="stepper.previous()">Anterior</button>
            <button
              mat-raised-button
              color="primary"
              [disabled]="totalAlocado() === 0"
              (click)="stepper.next()"
            >
              Próximo
            </button>
          </mat-card-actions>
        </mat-card>
      </mat-step>

      <!-- Step 3: Confirmação -->
      <mat-step>
        <ng-template matStepLabel>Confirmação</ng-template>

        <mat-card>
          <mat-card-header>
            <mat-card-title>Confirme os Dados do Pagamento</mat-card-title>
          </mat-card-header>

          <mat-card-content>
            <div class="confirmacao-grid">
              <div class="confirmacao-section">
                <h3>Dados do Pagamento</h3>
                <div class="info-item">
                  <span>Responsável:</span>
                  <strong>{{ getResponsavelNome() }}</strong>
                </div>
                <div class="info-item">
                  <span>Valor:</span>
                  <strong>{{ valorPagamento() | currency : 'BRL' }}</strong>
                </div>
                <div class="info-item">
                  <span>Data:</span>
                  <strong>{{
                    pagamentoForm.get('dataPagamento')?.value
                      | date : 'dd/MM/yyyy'
                  }}</strong>
                </div>
                <div class="info-item">
                  <span>Meio:</span>
                  <strong>{{
                    pagamentoForm.get('meioPagamento')?.value
                  }}</strong>
                </div>
              </div>

              <div class="confirmacao-section">
                <h3>Alocações</h3>
                @for (alocacao of getAlocacoesResumo(); track
                alocacao.mensalidadeId) {
                <div class="alocacao-item">
                  <span>{{ alocacao.competencia }} - {{ alocacao.aluno }}</span>
                  <strong>{{ alocacao.valor | currency : 'BRL' }}</strong>
                </div>
                } @if (saldoRestante() > 0) {
                <mat-divider></mat-divider>
                <div class="info-item warn">
                  <span>Saldo a ser creditado:</span>
                  <strong>{{ saldoRestante() | currency : 'BRL' }}</strong>
                </div>
                }
              </div>
            </div>
          </mat-card-content>

          <mat-card-actions align="end">
            <button mat-button (click)="stepper.previous()">Anterior</button>
            <button
              mat-raised-button
              color="primary"
              (click)="confirmarPagamento()"
            >
              <mat-icon>check</mat-icon>
              Confirmar Pagamento
            </button>
          </mat-card-actions>
        </mat-card>
      </mat-step>
    </mat-stepper>
  `,
  styles: [
    `
      .form-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
        gap: 16px;
        margin-bottom: 16px;
      }

      .full-width {
        grid-column: 1 / -1;
        width: 100%;
      }

      .saldo-info {
        display: flex;
        align-items: center;
        gap: 8px;
        padding: 12px;
        background: #e3f2fd;
        border-radius: 4px;
        color: #1976d2;
        margin-top: 16px;
      }

      .alocacao-controls {
        display: flex;
        align-items: center;
        gap: 16px;
        margin-bottom: 24px;
        padding: 16px;
        background: #f5f5f5;
        border-radius: 4px;
      }

      .valor-input {
        width: 120px;
      }

      .valor-alocado {
        color: #4caf50;
        font-weight: 500;
      }

      .resumo-alocacao {
        margin-top: 24px;
        padding: 16px;
        background: #f5f5f5;
        border-radius: 4px;
      }

      .resumo-item {
        display: flex;
        justify-content: space-between;
        padding: 8px 0;
      }

      .resumo-item.success strong {
        color: #4caf50;
      }

      .resumo-item.warn strong {
        color: #ff9800;
      }

      .confirmacao-grid {
        display: grid;
        grid-template-columns: 1fr 1fr;
        gap: 32px;
      }

      .confirmacao-section h3 {
        margin-bottom: 16px;
        color: #666;
      }

      .info-item,
      .alocacao-item {
        display: flex;
        justify-content: space-between;
        padding: 8px 0;
      }

      .info-item.warn {
        color: #ff9800;
      }

      mat-chip {
        font-size: 11px;
        height: 20px;
        margin-left: 8px;
      }

      @media (max-width: 768px) {
        .confirmacao-grid {
          grid-template-columns: 1fr;
        }
      }
    `,
  ],
})
export class ConciliacaoPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly financeiroStore = inject(FinanceiroStore);
  private readonly pessoasStore = inject(PessoasStore);

  readonly responsaveis = signal<any[]>([]);
  readonly mensalidadesParaAlocar = signal<MensalidadeParaAlocar[]>([]);
  readonly saldo = signal<number | null>(null);
  readonly valorPagamento = signal(0);
  readonly totalAlocado = signal(0);

  readonly estrategiaAlocacao = signal('automatica');

  readonly displayedColumns = [
    'select',
    'competencia',
    'vencimento',
    'aluno',
    'valor',
    'pendente',
    'alocar',
  ];

  readonly pagamentoForm = this.fb.nonNullable.group({
    responsavelId: ['', Validators.required],
    valor: [0, [Validators.required, Validators.min(0.01)]],
    dataPagamento: [new Date(), Validators.required],
    meioPagamento: ['Pix', Validators.required],
    observacao: [''],
  });

  readonly saldoRestante = computed(() => {
    const valor = this.valorPagamento();
    const saldo = this.saldo() || 0;
    const alocado = this.totalAlocado();
    return valor + saldo - alocado;
  });

  ngOnInit(): void {
    this.loadResponsaveis();

    // Subscribe to store
    this.financeiroStore.mensalidadesParaAlocar$.subscribe((mensalidades) =>
      this.mensalidadesParaAlocar.set(mensalidades)
    );

    this.financeiroStore.totalAlocado$.subscribe((total) =>
      this.totalAlocado.set(total)
    );

    this.financeiroStore.saldoResponsavel$.subscribe((saldo) =>
      this.saldo.set(saldo?.saldo || null)
    );

    this.pagamentoForm
      .get('valor')
      ?.valueChanges.subscribe((valor) => this.valorPagamento.set(valor));
  }

  loadResponsaveis(): void {
    this.pessoasStore.loadPessoas({ tipo: 'Responsavel', ativo: true });
    this.pessoasStore.responsaveis$.subscribe((responsaveis) =>
      this.responsaveis.set(responsaveis)
    );
  }

  onResponsavelChange(responsavelId: string): void {
    if (responsavelId) {
      this.financeiroStore.loadMensalidadesResponsavel(responsavelId);
    }
  }

  toggleMensalidade(mensalidadeId: string): void {
    this.financeiroStore.toggleMensalidadeSelecao(mensalidadeId);
  }

  atualizarValorAlocado(mensalidadeId: string, event: Event): void {
    const valor = Number((event.target as HTMLInputElement).value);
    this.financeiroStore.atualizarValorAlocado({ mensalidadeId, valor });
  }

  alocarAutomaticamente(): void {
    this.financeiroStore.alocarAutomaticamente(this.valorPagamento());
  }

  limparAlocacao(): void {
    this.financeiroStore.limparAlocacao();
  }

  formatCompetencia(competencia: string): string {
    const [ano, mes] = competencia.split('-');
    return `${mes}/${ano}`;
  }

  isVencido(vencimento: string): boolean {
    return new Date(vencimento) < new Date();
  }

  getResponsavelNome(): string {
    const id = this.pagamentoForm.get('responsavelId')?.value;
    return this.responsaveis().find((r) => r.id === id)?.nome || '';
  }

  getAlocacoesResumo(): any[] {
    return this.mensalidadesParaAlocar()
      .filter((m) => m.valorAlocado > 0)
      .map((m) => ({
        mensalidadeId: m.mensalidade.id,
        competencia: this.formatCompetencia(m.mensalidade.competencia),
        aluno: m.mensalidade.matricula?.aluno?.nome || '-',
        valor: m.valorAlocado,
      }));
  }

  confirmarPagamento(): void {
    if (this.pagamentoForm.invalid) return;

    const formValue = this.pagamentoForm.getRawValue();
    const alocacoes = this.financeiroStore.obterAlocacoes();

    const request: CreatePagamentoRequest = {
      ...formValue,
      alocacoes,
    };

    this.financeiroStore.criarPagamento(request);

    // Navigate on success
    this.financeiroStore.loading$.subscribe((loading) => {
      if (!loading) {
        this.router.navigate(['/financeiro/pagamentos']);
      }
    });
  }
}
