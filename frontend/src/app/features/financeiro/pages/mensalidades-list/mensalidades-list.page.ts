import { Component, OnInit, inject, signal, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ReactiveFormsModule, FormControl } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import {
  MatDialog,
  MatDialogModule,
  MAT_DIALOG_DATA,
  MatDialogRef,
} from '@angular/material/dialog';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { PageHeaderComponent } from '../../../../shared/ui/components/page-header/page-header.component';
import { FinanceiroService } from '../../data-access/financeiro.service';

@Component({
  selector: 'app-mensalidades-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    ReactiveFormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatDialogModule,
    MatDatepickerModule,
    MatNativeDateModule,
    PageHeaderComponent,
  ],
  template: `
    <app-page-header title="Mensalidades" subtitle="Gerencie as mensalidades dos alunos" />

    <mat-card>
      <mat-card-content>
        <div class="filters">
          <mat-form-field>
            <mat-label>Competência</mat-label>
            <input matInput [formControl]="competenciaControl" placeholder="AAAA-MM" />
          </mat-form-field>
          <mat-form-field>
            <mat-label>Status</mat-label>
            <mat-select [formControl]="statusControl">
              <mat-option value="">Todos</mat-option>
              <mat-option value="EmAberto">Em aberto</mat-option>
              <mat-option value="Pago">Pago</mat-option>
              <mat-option value="Atrasado">Atrasado</mat-option>
              <mat-option value="Cancelado">Cancelado</mat-option>
            </mat-select>
          </mat-form-field>
          <button mat-raised-button color="primary" (click)="load()">Filtrar</button>
        </div>

        <div class="table-container">
          <table mat-table [dataSource]="items()" class="full-width">
            <ng-container matColumnDef="aluno">
              <th mat-header-cell *matHeaderCellDef>Aluno</th>
              <td mat-cell *matCellDef="let m">{{ m.alunoNome }}</td>
            </ng-container>
            <ng-container matColumnDef="curso">
              <th mat-header-cell *matHeaderCellDef>Curso</th>
              <td mat-cell *matCellDef="let m">{{ m.cursoNome }}</td>
            </ng-container>
            <ng-container matColumnDef="competencia">
              <th mat-header-cell *matHeaderCellDef>Competência</th>
              <td mat-cell *matCellDef="let m">{{ m.competencia }}</td>
            </ng-container>
            <ng-container matColumnDef="valor">
              <th mat-header-cell *matHeaderCellDef>Valor</th>
              <td mat-cell *matCellDef="let m">{{ m.valor | currency : 'BRL' }}</td>
            </ng-container>
            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let m">
                <mat-chip [ngStyle]="getStatusStyle(m.status)">{{ mapStatus(m.status) }}</mat-chip>
              </td>
            </ng-container>
            <ng-container matColumnDef="datas">
              <th mat-header-cell *matHeaderCellDef>Venc./Pagto.</th>
              <td mat-cell *matCellDef="let m">
                <div>
                  <span>{{ m.dataVencimento | date : 'dd/MM/yyyy' }}</span>
                  <span *ngIf="m.dataPagamento">
                    • {{ m.dataPagamento | date : 'dd/MM/yyyy' }}</span
                  >
                </div>
              </td>
            </ng-container>
            <ng-container matColumnDef="acoes">
              <th mat-header-cell *matHeaderCellDef>Ações</th>
              <td mat-cell *matCellDef="let m">
                <button
                  mat-button
                  color="primary"
                  (click)="registrarPagamento(m)"
                  [disabled]="(m.status || '').toLowerCase() === 'pago'"
                >
                  Registrar pagamento
                </button>
              </td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="displayed"></tr>
            <tr mat-row *matRowDef="let row; columns: displayed"></tr>

            <tr class="mat-row" *matNoDataRow>
              <td class="mat-cell" [attr.colspan]="displayed.length">
                <div class="empty-state">
                  <mat-icon>receipt_long</mat-icon>
                  <p>Nenhuma mensalidade encontrada</p>
                </div>
              </td>
            </tr>
          </table>
        </div>

        <mat-paginator
          [length]="total()"
          [pageSize]="pageSize"
          [pageSizeOptions]="[10, 25, 50]"
          (page)="onPage($event)"
        />
      </mat-card-content>
    </mat-card>
  `,
  styles: [
    `
      .filters {
        display: flex;
        gap: 16px;
        margin-bottom: 24px;
        flex-wrap: wrap;
      }
      .filters mat-form-field {
        flex: 1;
        min-width: 200px;
      }
      .table-container {
        overflow-x: auto;
      }
      table {
        width: 100%;
      }
      .empty-state {
        padding: 48px;
        text-align: center;
        color: #999;
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        min-height: 160px;
      }
      .empty-state mat-icon {
        font-size: 48px;
        width: 48px;
        height: 48px;
      }
    `,
  ],
})
export class MensalidadesListPage implements OnInit {
  private readonly service = inject(FinanceiroService);
  private readonly dialog = inject(MatDialog);

  readonly items = signal<any[]>([]);
  readonly total = signal(0);
  readonly displayed = ['aluno', 'curso', 'competencia', 'valor', 'status', 'datas', 'acoes'];
  readonly pageSize = 10;

  readonly competenciaControl = new FormControl('');
  readonly statusControl = new FormControl('');

  ngOnInit(): void {
    this.load();
  }

  load(page = 0): void {
    // Backend atual expõe /api/mensalidades?ano=YYYY&mes=MM&status=...
    const comp = (this.competenciaControl.value || '').toString();
    const [ano, mes] = comp.includes('-') ? comp.split('-') : ['', ''];
    const status = (this.statusControl.value || '').toString();

    this.service
      // Reutilizando getMensalidades (ajuste de rota pode ser feito depois)
      .getMensalidades({ page, pageSize: this.pageSize } as any)
      .subscribe(resp => {
        const list = Array.isArray(resp) ? resp : (resp as any).items || (resp as any).Items || [];
        const mapped = list.map((m: any) => ({
          id: m.Id ?? m.id,
          matriculaId: m.MatriculaId ?? m.matriculaId,
          alunoNome: m.AlunoNome ?? m.alunoNome ?? '-',
          cursoNome: m.CursoNome ?? m.cursoNome ?? '-',
          competencia: m.Competencia ?? m.competencia ?? '-',
          valor: m.Valor ?? m.valor ?? 0,
          status: m.Status ?? m.status ?? '-',
          dataVencimento: m.DataVencimento ?? m.dataVencimento,
          dataPagamento: m.DataPagamento ?? m.dataPagamento,
        }));

        const filtered = mapped.filter((x: any) => {
          const byComp =
            !ano ||
            !mes ||
            x.competencia.startsWith(`${ano}-${mes}`) ||
            x.competencia.startsWith(`${ano}/${mes}`) ||
            x.competencia.endsWith(`${mes}/${ano}`);
          const byStatus = !status || (x.status || '').toLowerCase() === status.toLowerCase();
          return byComp && byStatus;
        });

        this.items.set(filtered);
        this.total.set(filtered.length);
      });
  }

  onPage(e: PageEvent): void {
    this.load(e.pageIndex);
  }

  registrarPagamento(m: any): void {
    const ref = this.dialog.open(RegistrarPagamentoDialogComponent, {
      data: { alunoNome: m.alunoNome, competencia: m.competencia, valor: m.valor },
    });
    ref.afterClosed().subscribe((res: any) => {
      if (!res) return;
      this.service
        .registrarPagamentoMensalidade(m.id, {
          meioPagamento: res.meioPagamento,
          dataPagamento: res.dataPagamento ? new Date(res.dataPagamento).toISOString() : undefined,
          observacao: res.observacao || undefined,
        })
        .subscribe({
          next: () => this.load(),
          error: () => alert('Erro ao registrar pagamento'),
        });
    });
  }

  mapStatus(s: string): string {
    const v = (s || '').toLowerCase();
    switch (v) {
      case 'emaberto':
        return 'Em aberto';
      case 'pagoparcial':
        return 'Pago parcial';
      case 'pago':
        return 'Pago';
      case 'atrasado':
        return 'Atrasado';
      case 'cancelado':
        return 'Cancelado';
      default:
        return s || '-';
    }
  }

  getStatusStyle(s: string): any {
    const key = (s || '').toLowerCase();
    const map: Record<string, { background: string; color: string }> = {
      emaberto: { background: '#fffde7', color: '#f57f17' },
      pagoparcial: { background: '#e3f2fd', color: '#1565c0' },
      pago: { background: '#e8f5e9', color: '#2e7d32' },
      atrasado: { background: '#ffebee', color: '#c62828' },
      cancelado: { background: '#eeeeee', color: '#616161' },
    };
    return map[key] || { background: '#eee', color: '#555' };
  }
}

@Component({
  selector: 'app-registrar-pagamento-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatDialogModule,
    MatDatepickerModule,
    MatNativeDateModule,
  ],
  template: `
    <h2 mat-dialog-title>Registrar Pagamento</h2>
    <div mat-dialog-content>
      <div class="resumo">
        <div><strong>Aluno:</strong> {{ data.alunoNome }}</div>
        <div><strong>Competência:</strong> {{ data.competencia }}</div>
        <div><strong>Valor:</strong> {{ data.valor | currency : 'BRL' }}</div>
      </div>
      <form [formGroup]="form" class="form-grid">
        <mat-form-field>
          <mat-label>Meio de Pagamento</mat-label>
          <mat-select formControlName="meioPagamento">
            <mat-option value="Pix">Pix</mat-option>
            <mat-option value="Dinheiro">Dinheiro</mat-option>
            <mat-option value="Cartao">Cartão</mat-option>
            <mat-option value="Transferencia">Transferência</mat-option>
            <mat-option value="Outro">Outro</mat-option>
          </mat-select>
        </mat-form-field>
        <mat-form-field>
          <mat-label>Data do Pagamento</mat-label>
          <input matInput [matDatepicker]="picker" formControlName="dataPagamento" />
          <mat-datepicker-toggle matIconSuffix [for]="picker" />
          <mat-datepicker #picker />
        </mat-form-field>
        <mat-form-field class="full-width">
          <mat-label>Observação</mat-label>
          <textarea matInput rows="2" formControlName="observacao"></textarea>
        </mat-form-field>
      </form>
    </div>
    <div mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Cancelar</button>
      <button mat-raised-button color="primary" (click)="confirmar()" [disabled]="form.invalid">
        Confirmar
      </button>
    </div>
  `,
  styles: [
    `
      .resumo {
        display: grid;
        grid-template-columns: 1fr;
        gap: 4px;
        margin-bottom: 12px;
      }
      .form-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
        gap: 12px;
      }
      .full-width {
        width: 100%;
        grid-column: 1 / -1;
      }
    `,
  ],
})
export class RegistrarPagamentoDialogComponent {
  readonly form: FormGroup;

  constructor(
    private readonly fb: FormBuilder,
    private readonly dialogRef: MatDialogRef<RegistrarPagamentoDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { alunoNome: string; competencia: string; valor: number }
  ) {
    this.form = this.fb.nonNullable.group({
      meioPagamento: ['Pix'],
      dataPagamento: [new Date()],
      observacao: [''],
    });
  }

  confirmar(): void {
    if (this.form.invalid) return;
    this.dialogRef.close(this.form.getRawValue());
  }
}
