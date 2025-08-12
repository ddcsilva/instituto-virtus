import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatChipsModule } from '@angular/material/chips';
import { MatMenuModule } from '@angular/material/menu';
import { MatDialog } from '@angular/material/dialog';
import { debounceTime, take } from 'rxjs';

import { PageHeaderComponent } from '../../../../shared/ui/components/page-header/page-header.component';
import { ConfirmDialogComponent } from '../../../../shared/ui/components/confirm-dialog/confirm-dialog.component';
import { TelefonePipe } from '../../../../shared/pipes/telefone.pipe';
import { CpfPipe } from '../../../../shared/pipes/cpf.pipe';
import { PessoasStore } from '../../data-access/pessoas.store';
import { Pessoa, TipoPessoa } from '../../models/pessoa.model';

@Component({
  selector: 'app-pessoas-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    ReactiveFormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatChipsModule,
    MatMenuModule,
    PageHeaderComponent,
    TelefonePipe,
    CpfPipe,
  ],
  providers: [PessoasStore],
  template: `
    <app-page-header
      title="Pessoas"
      subtitle="Gerencie alunos, responsáveis e professores"
      actionLabel="Nova Pessoa"
      actionIcon="add"
      actionRoute="/pessoas/novo"
    />

    <mat-card>
      <mat-card-content>
        <div class="filters">
          <mat-form-field>
            <mat-label>Buscar</mat-label>
            <input matInput [formControl]="searchControl" placeholder="Nome ou CPF" />
            <mat-icon matSuffix>search</mat-icon>
          </mat-form-field>

          <mat-form-field>
            <mat-label>Tipo</mat-label>
            <mat-select [formControl]="tipoControl">
              <mat-option value="">Todos</mat-option>
              <mat-option value="Aluno">Aluno</mat-option>
              <mat-option value="Responsavel">Responsável</mat-option>
              <mat-option value="Professor">Professor</mat-option>
            </mat-select>
          </mat-form-field>

          <mat-form-field>
            <mat-label>Status</mat-label>
            <mat-select [formControl]="statusControl">
              <mat-option [value]="null">Todos</mat-option>
              <mat-option [value]="true">Ativo</mat-option>
              <mat-option [value]="false">Inativo</mat-option>
            </mat-select>
          </mat-form-field>
        </div>

        <div class="table-container">
          <table mat-table [dataSource]="pessoas()" matSort (matSortChange)="onSort($event)">
            <ng-container matColumnDef="nome">
              <th mat-header-cell *matHeaderCellDef mat-sort-header>Nome</th>
              <td mat-cell *matCellDef="let pessoa">{{ pessoa.nome }}</td>
            </ng-container>

            <ng-container matColumnDef="cpf">
              <th mat-header-cell *matHeaderCellDef>CPF</th>
              <td mat-cell *matCellDef="let pessoa">{{ pessoa.cpf | cpf }}</td>
            </ng-container>

            <ng-container matColumnDef="telefone">
              <th mat-header-cell *matHeaderCellDef>Telefone</th>
              <td mat-cell *matCellDef="let pessoa">
                {{ pessoa.telefone | telefone }}
              </td>
            </ng-container>

            <ng-container matColumnDef="email">
              <th mat-header-cell *matHeaderCellDef>E-mail</th>
              <td mat-cell *matCellDef="let pessoa">
                {{ pessoa.email || '-' }}
              </td>
            </ng-container>

            <ng-container matColumnDef="tipo">
              <th mat-header-cell *matHeaderCellDef>Tipo</th>
              <td mat-cell *matCellDef="let pessoa">
                <mat-chip [ngStyle]="getTipoStyle(pessoa.tipo)">{{ pessoa.tipo }}</mat-chip>
              </td>
            </ng-container>

            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let pessoa">
                <mat-chip [ngStyle]="getStatusStyle(pessoa.ativo)">
                  {{ pessoa.ativo ? 'Ativo' : 'Inativo' }}
                </mat-chip>
              </td>
            </ng-container>

            <ng-container matColumnDef="acoes">
              <th mat-header-cell *matHeaderCellDef>Ações</th>
              <td mat-cell *matCellDef="let pessoa">
                <button mat-icon-button [matMenuTriggerFor]="menu">
                  <mat-icon>more_vert</mat-icon>
                </button>
                <mat-menu #menu="matMenu">
                  <a mat-menu-item [routerLink]="['/pessoas', pessoa.id, 'editar']">
                    <mat-icon>edit</mat-icon>
                    <span>Editar</span>
                  </a>
                  @if (pessoa.tipo === 'Responsavel') {
                  <a mat-menu-item [routerLink]="['/pessoas', pessoa.id, 'vinculos']">
                    <mat-icon>link</mat-icon>
                    <span>Vínculos</span>
                  </a>
                  }
                  <button mat-menu-item (click)="toggleStatus(pessoa)">
                    <mat-icon>{{ pessoa.ativo ? 'block' : 'check_circle' }}</mat-icon>
                    <span>{{ pessoa.ativo ? 'Desativar' : 'Ativar' }}</span>
                  </button>
                  <button mat-menu-item (click)="confirmDelete(pessoa)">
                    <mat-icon color="warn">delete</mat-icon>
                    <span>Excluir</span>
                  </button>
                </mat-menu>
              </td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns"></tr>

            <tr class="mat-row" *matNoDataRow>
              <td class="mat-cell" colspan="7">
                <div class="empty-state">
                  <mat-icon>people_outline</mat-icon>
                  <p>Nenhuma pessoa encontrada</p>
                </div>
              </td>
            </tr>
          </table>
        </div>

        <mat-paginator
          [length]="total()"
          [pageSize]="pageSize"
          [pageSizeOptions]="[10, 25, 50]"
          (page)="onPageChange($event)"
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

        mat-form-field {
          flex: 1;
          min-width: 200px;
        }
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

        mat-icon {
          font-size: 48px;
          width: 48px;
          height: 48px;
        }
      }

      mat-chip {
        font-size: 12px;
      }
    `,
  ],
})
export class PessoasListPage implements OnInit {
  private readonly store = inject(PessoasStore);
  private readonly dialog = inject(MatDialog);

  readonly pessoas = signal<Pessoa[]>([]);
  readonly total = signal(0);
  readonly loading = signal(false);

  readonly searchControl = new FormControl('');
  readonly tipoControl = new FormControl('');
  readonly statusControl = new FormControl<boolean | null>(null);

  readonly displayedColumns = ['nome', 'cpf', 'telefone', 'email', 'tipo', 'status', 'acoes'];
  readonly pageSize = 10;

  ngOnInit(): void {
    // Subscribe to store state
    this.store.pessoas$.subscribe(pessoas => this.pessoas.set(pessoas));
    this.store.total$.subscribe(total => this.total.set(total));
    this.store.loading$.subscribe(loading => this.loading.set(loading));

    // Setup filters
    this.searchControl.valueChanges.pipe(debounceTime(300)).subscribe(value => {
      this.store.updateFilter({ nome: value || undefined, page: 0 });
      this.loadPessoas();
    });

    this.tipoControl.valueChanges.subscribe(value => {
      this.store.updateFilter({ tipo: value || undefined, page: 0 });
      this.loadPessoas();
    });

    this.statusControl.valueChanges.subscribe(value => {
      this.store.updateFilter({ ativo: value ?? undefined, page: 0 });
      this.loadPessoas();
    });

    // Load initial data
    this.loadPessoas();
  }

  loadPessoas(): void {
    this.store.filter$.pipe(take(1)).subscribe(filter => {
      this.store.loadPessoas(filter);
    });
  }

  onPageChange(event: PageEvent): void {
    this.store.updateFilter({
      page: event.pageIndex,
      pageSize: event.pageSize,
    });
    this.loadPessoas();
  }

  onSort(sort: Sort): void {
    // Implement sorting logic if API supports it
  }

  getChipColor(tipo: TipoPessoa): 'primary' | 'accent' | 'warn' {
    switch (tipo) {
      case 'Aluno':
        return 'primary';
      case 'Responsavel':
        return 'accent';
      case 'Professor':
        return 'warn';
      default:
        return 'primary';
    }
  }

  getTipoStyle(tipo: TipoPessoa) {
    const palette: Record<TipoPessoa, { background: string; color: string; border: string }> = {
      Aluno: { background: '#E3F2FD', color: '#0D47A1', border: '#90CAF9' },
      Responsavel: { background: '#FFF3E0', color: '#E65100', border: '#FFCC80' },
      Professor: { background: '#F3E5F5', color: '#4A148C', border: '#CE93D8' },
    } as const;
    const s = palette[tipo];
    return { 'background-color': s.background, color: s.color, border: `1px solid ${s.border}` };
  }

  getStatusStyle(ativo: boolean) {
    return ativo
      ? { 'background-color': '#E8F5E9', color: '#1B5E20', border: '1px solid #A5D6A7' }
      : { 'background-color': '#FFEBEE', color: '#B71C1C', border: '1px solid #EF9A9A' };
  }

  toggleStatus(pessoa: Pessoa): void {
    this.store.toggleStatus(pessoa.id);
  }

  confirmDelete(pessoa: Pessoa): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Confirmar Exclusão',
        message: `Deseja realmente excluir ${pessoa.nome}?`,
        confirmText: 'Excluir',
        cancelText: 'Cancelar',
      },
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.store.deletePessoa(pessoa.id);
      }
    });
  }
}
