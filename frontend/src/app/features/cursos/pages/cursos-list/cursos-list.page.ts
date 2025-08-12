import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { PageHeaderComponent } from '../../../../shared/ui/components/page-header/page-header.component';
import { CursosService, PagedResult } from '../../data-access/cursos.service';
import { Curso } from '../../models/curso.model';
import { ConfirmDialogComponent } from '../../../../shared/ui/components/confirm-dialog/confirm-dialog.component';
import { MatSnackBar } from '@angular/material/snack-bar';
import { debounceTime } from 'rxjs';

@Component({
  selector: 'app-cursos-list',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    ReactiveFormsModule,
    MatChipsModule,
    MatDialogModule,
    MatMenuModule,
    MatIconModule,
    RouterLink,
    PageHeaderComponent,
  ],
  template: `
    <app-page-header
      title="Cursos"
      subtitle="Gerencie os cursos da instituição"
      actionLabel="Novo Curso"
      actionIcon="add"
      actionRoute="/cursos/novo"
    />

    <mat-card>
      <mat-card-content>
        <div class="filters">
          <mat-form-field>
            <mat-label>Buscar</mat-label>
            <input matInput [formControl]="buscaControl" placeholder="Nome" />
            <mat-icon matSuffix>search</mat-icon>
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
          <table mat-table [dataSource]="cursos()" matSort class="full-width">
            <ng-container matColumnDef="nome">
              <th mat-header-cell *matHeaderCellDef mat-sort-header>Nome</th>
              <td mat-cell *matCellDef="let c">{{ c.nome }}</td>
            </ng-container>
            <ng-container matColumnDef="valor">
              <th mat-header-cell *matHeaderCellDef>Mensalidade</th>
              <td mat-cell *matCellDef="let c">
                {{ c.valorMensalidade | currency : 'BRL' : 'symbol-narrow' : '1.2-2' : 'pt-BR' }}
              </td>
            </ng-container>
            <ng-container matColumnDef="carga">
              <th mat-header-cell *matHeaderCellDef>Carga Horária</th>
              <td mat-cell *matCellDef="let c">{{ c.cargaHoraria }}h</td>
            </ng-container>
            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let c">
                <mat-chip [ngStyle]="getStatusStyle(c.ativo)">{{
                  c.ativo ? 'Ativo' : 'Inativo'
                }}</mat-chip>
              </td>
            </ng-container>
            <ng-container matColumnDef="acoes">
              <th mat-header-cell *matHeaderCellDef>Ações</th>
              <td mat-cell *matCellDef="let c">
                <button mat-icon-button [matMenuTriggerFor]="menu">
                  <mat-icon>more_vert</mat-icon>
                </button>
                <mat-menu #menu="matMenu">
                  <a mat-menu-item [routerLink]="['/cursos', c.id, 'editar']">
                    <mat-icon>edit</mat-icon>
                    <span>Editar</span>
                  </a>
                  <button mat-menu-item (click)="confirmToggle(c)">
                    <mat-icon>{{ c.ativo ? 'block' : 'check_circle' }}</mat-icon>
                    <span>{{ c.ativo ? 'Desativar' : 'Ativar' }}</span>
                  </button>
                </mat-menu>
              </td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="displayed"></tr>
            <tr mat-row *matRowDef="let row; columns: displayed"></tr>

            <tr class="mat-row" *matNoDataRow>
              <td class="mat-cell" colspan="5">
                <div class="empty-state">
                  <mat-icon>menu_book</mat-icon>
                  <p>Nenhum curso encontrado</p>
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
      }
      .empty-state mat-icon {
        font-size: 48px;
        width: 48px;
        height: 48px;
      }
      .full-width {
        width: 100%;
      }
    `,
  ],
})
export class CursosListPage implements OnInit {
  private readonly service = inject(CursosService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  readonly cursos = signal<Curso[]>([]);
  readonly total = signal(0);
  readonly buscaControl = new FormControl('');
  readonly statusControl = new FormControl<boolean | null>(null);
  readonly displayed = ['nome', 'valor', 'carga', 'status', 'acoes'];
  readonly pageSize = 10;

  ngOnInit(): void {
    // reatividade dos filtros para seguir padrão de Pessoas
    this.buscaControl.valueChanges.pipe(debounceTime(300)).subscribe(() => this.load(0));
    this.statusControl.valueChanges.subscribe(() => this.load(0));
    this.load();
  }

  load(page = 0): void {
    this.service
      .getAll({
        nome: this.buscaControl.value || undefined,
        ativo: this.statusControl.value ?? undefined,
        page,
        pageSize: this.pageSize,
      })
      .subscribe((res: PagedResult<Curso>) => {
        this.cursos.set(res.items);
        this.total.set(res.total);
      });
  }

  onPage(e: PageEvent): void {
    this.load(e.pageIndex);
  }

  toggle(curso: Curso): void {
    this.service.toggleStatus(curso.id).subscribe(updated => {
      this.cursos.set(this.cursos().map(c => (c.id === updated.id ? updated : c)));
      const status = updated.ativo ? 'ativado' : 'desativado';
      this.snackBar.open(`Curso ${status} com sucesso`, 'Fechar', { duration: 3000 });
    });
  }

  confirmToggle(curso: Curso): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: curso.ativo ? 'Desativar Curso' : 'Ativar Curso',
        message: `Confirmar ${curso.ativo ? 'desativação' : 'ativação'} de ${curso.nome}?`,
        confirmText: curso.ativo ? 'Desativar' : 'Ativar',
        cancelText: 'Cancelar',
      },
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) this.toggle(curso);
    });
  }

  getStatusStyle(ativo: boolean) {
    return ativo
      ? { 'background-color': '#E8F5E9', color: '#1B5E20', border: '1px solid #A5D6A7' }
      : { 'background-color': '#FFEBEE', color: '#B71C1C', border: '1px solid #EF9A9A' };
  }
}
