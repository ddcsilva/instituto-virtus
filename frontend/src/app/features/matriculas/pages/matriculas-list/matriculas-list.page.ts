import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ReactiveFormsModule, FormControl } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatChipsModule } from '@angular/material/chips';
import { PageHeaderComponent } from '../../../../shared/ui/components/page-header/page-header.component';
import { MatriculasService, MatriculaFilter } from '../../data-access/matriculas.service';

@Component({
  selector: 'app-matriculas-list',
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
    PageHeaderComponent,
  ],
  template: `
    <app-page-header
      title="Matrículas"
      subtitle="Gerencie as matrículas dos alunos"
      actionLabel="Nova Matrícula"
      actionIcon="add"
      actionRoute="/matriculas/nova"
    />

    <mat-card>
      <mat-card-content>
        <div class="filters">
          <mat-form-field>
            <mat-label>Buscar</mat-label>
            <input matInput [formControl]="searchControl" placeholder="Nome do aluno" />
            <mat-icon matSuffix>search</mat-icon>
          </mat-form-field>

          <mat-form-field>
            <mat-label>Status</mat-label>
            <mat-select [formControl]="statusControl">
              <mat-option value="">Todos</mat-option>
              <mat-option value="Ativa">Ativa</mat-option>
              <mat-option value="Trancada">Trancada</mat-option>
              <mat-option value="Cancelada">Cancelada</mat-option>
              <mat-option value="Concluida">Concluída</mat-option>
            </mat-select>
          </mat-form-field>
        </div>

        <div class="table-container">
          <table mat-table [dataSource]="items()" class="full-width">
            <ng-container matColumnDef="aluno">
              <th mat-header-cell *matHeaderCellDef>Aluno</th>
              <td mat-cell *matCellDef="let m">{{ m.alunoNome }}</td>
            </ng-container>

            <ng-container matColumnDef="turma">
              <th mat-header-cell *matHeaderCellDef>Turma</th>
              <td mat-cell *matCellDef="let m">{{ m.turmaNome }}</td>
            </ng-container>

            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let m">
                <mat-chip [ngStyle]="getStatusStyle(m.status)">{{ mapStatus(m.status) }}</mat-chip>
              </td>
            </ng-container>

            <ng-container matColumnDef="acoes">
              <th mat-header-cell *matHeaderCellDef>Ações</th>
              <td mat-cell *matCellDef="let m">
                <a mat-button [routerLink]="['/matriculas', m.id, 'detalhes']">Detalhes</a>
              </td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="displayed"></tr>
            <tr mat-row *matRowDef="let row; columns: displayed"></tr>

            <tr class="mat-row" *matNoDataRow>
              <td class="mat-cell" [attr.colspan]="displayed.length">
                <div class="empty-state">
                  <mat-icon>class</mat-icon>
                  <p>Nenhuma matrícula encontrada</p>
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
export class MatriculasListPage implements OnInit {
  private readonly service = inject(MatriculasService);

  readonly items = signal<any[]>([]);
  readonly total = signal(0);
  readonly displayed = ['aluno', 'turma', 'status', 'acoes'];
  readonly pageSize = 10;

  readonly searchControl = new FormControl('');
  readonly statusControl = new FormControl('');

  ngOnInit(): void {
    this.searchControl.valueChanges.subscribe(() => this.load(0));
    this.statusControl.valueChanges.subscribe(() => this.load(0));
    this.load();
  }

  load(page = 0): void {
    const filter: MatriculaFilter = { page, pageSize: this.pageSize } as any;
    this.service.getAll(filter).subscribe(resp => {
      const list = Array.isArray(resp) ? resp : resp.items || resp.Items || [];
      const search = (this.searchControl.value || '').toString().toLowerCase();
      const status = (this.statusControl.value || '').toString();
      const mapped = list.map((m: any) => ({
        id: m.Id ?? m.id,
        alunoNome: m.AlunoNome ?? m.alunoNome ?? '-',
        turmaNome: m.TurmaNome ?? m.turmaNome ?? '-',
        status: m.Status ?? m.status ?? '-',
      }));
      const filtered = mapped.filter((x: any) => {
        const byName = !search || x.alunoNome.toLowerCase().includes(search);
        const byStatus = !status || (x.status || '').toLowerCase() === status.toLowerCase();
        return byName && byStatus;
      });
      this.items.set(filtered);
      this.total.set(Array.isArray(resp) ? filtered.length : resp.total ?? filtered.length);
    });
  }

  onPage(e: PageEvent): void {
    this.load(e.pageIndex);
  }

  mapStatus(s: string): string {
    const v = (s || '').toLowerCase();
    switch (v) {
      case 'ativa':
        return 'Ativa';
      case 'trancada':
        return 'Trancada';
      case 'cancelada':
        return 'Cancelada';
      case 'concluida':
        return 'Concluída';
      default:
        return s || '-';
    }
  }

  getStatusStyle(s: string): any {
    const key = (s || '').toLowerCase();
    const map: Record<string, { background: string; color: string }> = {
      ativa: { background: '#e8f5e9', color: '#2e7d32' },
      trancada: { background: '#fff3e0', color: '#e65100' },
      cancelada: { background: '#ffebee', color: '#c62828' },
      concluida: { background: '#e3f2fd', color: '#1565c0' },
    };
    return map[key] || { background: '#eee', color: '#555' };
  }
}
