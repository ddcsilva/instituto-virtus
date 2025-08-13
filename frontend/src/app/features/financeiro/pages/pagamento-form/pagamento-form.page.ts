import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { PageHeaderComponent } from '../../../../shared/ui/components/page-header/page-header.component';
import { PessoasService } from '../../../pessoas/data-access/pessoas.service';
import { FinanceiroStore } from '../../data-access/financeiro.store';
import { FinanceiroService } from '../../data-access/financeiro.service';

@Component({
  selector: 'app-pagamento-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatSnackBarModule,
    PageHeaderComponent,
  ],
  providers: [FinanceiroStore],
  template: `
    <app-page-header
      title="Novo Pagamento"
      subtitle="Registre um novo pagamento e aloque nas mensalidades"
    />

    <mat-card>
      <mat-card-content>
        <form [formGroup]="form" class="form-grid" (ngSubmit)="onSubmit()">
          <mat-form-field class="full">
            <mat-label>Responsável</mat-label>
            <input
              matInput
              placeholder="Nome ou CPF"
              [value]="responsavelNome"
              (input)="onBuscarResponsavel($any($event.target).value)"
            />
          </mat-form-field>
          <div class="responsaveis-list" *ngIf="responsaveis.length">
            <button
              mat-stroked-button
              *ngFor="let r of responsaveis"
              (click)="selecionarResponsavel(r)"
            >
              {{ r.nome }} — {{ r.cpf || '-' }}
            </button>
          </div>

          <mat-form-field>
            <mat-label>Valor total</mat-label>
            <input matInput type="number" min="0" step="0.01" formControlName="valorTotal" />
          </mat-form-field>

          <mat-form-field>
            <mat-label>Meio de pagamento</mat-label>
            <mat-select formControlName="meioPagamento">
              <mat-option value="Pix">Pix</mat-option>
              <mat-option value="Dinheiro">Dinheiro</mat-option>
              <mat-option value="Cartao">Cartão</mat-option>
              <mat-option value="Transferencia">Transferência</mat-option>
              <mat-option value="Outro">Outro</mat-option>
            </mat-select>
          </mat-form-field>

          <div class="actions">
            <button
              mat-raised-button
              color="primary"
              type="button"
              (click)="carregarMensalidades()"
              [disabled]="form.controls.responsavelId.invalid"
            >
              <mat-icon>search</mat-icon>
              Carregar mensalidades do responsável
            </button>
          </div>

          <div class="mensalidades" *ngIf="(mensalidades$ | async)?.length">
            <h3>Mensalidades em aberto</h3>
            <div class="mensalidade" *ngFor="let m of mensalidades$ | async">
              <div class="info">
                <div class="linha">
                  <strong>{{ m.mensalidade.matricula?.aluno?.nome || 'Aluno' }}</strong> —
                  {{ m.mensalidade.competencia }}
                </div>
                <div class="linha">
                  Venc.: {{ m.mensalidade.vencimento | date : 'dd/MM/yyyy' }} • Valor pendente:
                  {{ m.valorPendente | currency : 'BRL' }}
                </div>
              </div>
              <mat-form-field>
                <mat-label>Alocar</mat-label>
                <input
                  matInput
                  type="number"
                  min="0"
                  [value]="m.valorAlocado"
                  (input)="atualizarAlocacao(m.mensalidade.id, $any($event.target).value)"
                />
              </mat-form-field>
              <button
                mat-stroked-button
                color="primary"
                type="button"
                (click)="alternarSelecao(m.mensalidade.id)"
              >
                {{ m.selecionada ? 'Remover' : 'Selecionar' }}
              </button>
            </div>

            <div class="alocacao-actions">
              <button mat-button type="button" (click)="alocarAutomatico()">
                Alocação automática
              </button>
              <button mat-button type="button" (click)="limparAlocacao()">Limpar</button>
              <span class="total"
                >Total alocado:
                <strong>{{ totalAlocado$ | async | currency : 'BRL' }}</strong></span
              >
            </div>
          </div>

          <div class="submit-actions">
            <button mat-button type="button" (click)="cancel()">Cancelar</button>
            <button
              mat-raised-button
              color="primary"
              type="submit"
              [disabled]="form.invalid || ((mensalidades$ | async)?.length ?? 0) === 0"
            >
              Confirmar Pagamento
            </button>
          </div>
        </form>
      </mat-card-content>
    </mat-card>
  `,
  styles: [
    `
      .form-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
        gap: 16px;
      }
      .form-grid .full {
        grid-column: 1 / -1;
      }
      .actions {
        grid-column: 1 / -1;
        display: flex;
        gap: 8px;
      }
      .mensalidades {
        grid-column: 1 / -1;
        margin-top: 8px;
      }
      .mensalidade {
        display: grid;
        grid-template-columns: 1fr 200px auto;
        gap: 12px;
        align-items: center;
        padding: 8px 0;
        border-bottom: 1px solid #eee;
      }
      .mensalidade .linha {
        font-size: 12px;
        color: #555;
      }
      .alocacao-actions {
        display: flex;
        gap: 12px;
        align-items: center;
        justify-content: flex-end;
        margin-top: 8px;
      }
      .total strong {
        color: #1976d2;
      }
      .submit-actions {
        grid-column: 1 / -1;
        display: flex;
        justify-content: flex-end;
        gap: 8px;
        margin-top: 16px;
      }
    `,
  ],
})
export class PagamentoFormPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly snackBar = inject(MatSnackBar);
  private readonly pessoasService = inject(PessoasService);
  private readonly financeiroService = inject(FinanceiroService);
  private readonly store = inject(FinanceiroStore);

  readonly form = this.fb.nonNullable.group({
    responsavelId: ['', Validators.required],
    valorTotal: [0, [Validators.required, Validators.min(0)]],
    meioPagamento: ['Pix', Validators.required],
  });

  responsavelNome = '';
  responsaveis: any[] = [];

  readonly mensalidades$ = this.store.mensalidadesParaAlocar$;
  readonly totalAlocado$ = this.store.totalAlocado$;
  private currentTotalAlocado = 0;

  ngOnInit(): void {
    this.totalAlocado$.subscribe(v => (this.currentTotalAlocado = Number(v || 0)));
  }

  carregarMensalidades(): void {
    const id = this.form.get('responsavelId')?.value as string;
    if (!id) return;
    this.store.loadMensalidadesResponsavel(id);
  }

  onBuscarResponsavel(term: string): void {
    this.responsavelNome = term;
    const filtro = term?.trim();
    if (!filtro || filtro.length < 3) {
      this.responsaveis = [];
      return;
    }
    const isCpf = /\d{3}\.\d{3}\.\d{3}-\d{2}|\d{11}/.test(filtro);
    this.pessoasService
      .getAll({
        nome: isCpf ? undefined : filtro,
        cpf: isCpf ? filtro.replace(/\D/g, '') : undefined,
        tipo: 'Responsavel',
        page: 0,
        pageSize: 5,
      })
      .subscribe(res => (this.responsaveis = res.items || []));
  }

  selecionarResponsavel(r: any): void {
    this.form.patchValue({ responsavelId: r.id });
    this.responsavelNome = `${r.nome}${r.cpf ? ' — ' + r.cpf : ''}`;
    this.responsaveis = [];
  }

  alocarAutomatico(): void {
    const valor = Number(this.form.get('valorTotal')?.value || 0);
    this.store.alocarAutomaticamente(valor);
  }

  limparAlocacao(): void {
    this.store.limparAlocacao();
  }

  atualizarAlocacao(id: string, valorStr: string): void {
    const valor = Number(valorStr || 0);
    this.store.atualizarValorAlocado({ mensalidadeId: id, valor });
  }

  alternarSelecao(id: string): void {
    this.store.toggleMensalidadeSelecao(id);
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    const valorTotal = Number(this.form.get('valorTotal')?.value || 0);
    const totalAlocadoAtual = this.currentTotalAlocado;
    if (totalAlocadoAtual > valorTotal) {
      this.snackBar.open('Total alocado é maior que o valor do pagamento', 'Fechar', {
        duration: 4000,
      });
      return;
    }
    const meio = this.form.get('meioPagamento')?.value as string;
    const responsavelId = this.form.get('responsavelId')?.value as string;

    const alocacoes = this.store.obterAlocacoes();
    if (alocacoes.length === 0) {
      this.snackBar.open('Nenhuma mensalidade selecionada para alocação', 'Fechar', {
        duration: 3000,
      });
      return;
    }

    this.financeiroService
      .criarPagamento({
        responsavelId,
        valor: valorTotal,
        dataPagamento: new Date().toISOString(),
        meioPagamento: meio as any,
        alocacoes: alocacoes.map(a => ({ mensalidadeId: a.mensalidadeId, valor: a.valor })),
      } as any)
      .subscribe({
        next: () =>
          this.snackBar.open('Pagamento registrado com sucesso', 'Fechar', { duration: 3000 }),
        error: () =>
          this.snackBar.open('Erro ao registrar pagamento', 'Fechar', { duration: 3000 }),
      });
  }

  cancel(): void {
    history.back();
  }
}
