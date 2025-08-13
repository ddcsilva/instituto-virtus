import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { debounceTime } from 'rxjs';

import { PageHeaderComponent } from '../../../../shared/ui/components/page-header/page-header.component';
import { TurmasService, TurmaFilter } from '../../data-access/turmas.service';
import { Turma, Turno } from '../../models/turma.model';
import { CursosService } from '../../../cursos/data-access/cursos.service';

@Component({
  selector: 'app-turmas-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    ReactiveFormsModule,
    MatCardModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatChipsModule,
    MatMenuModule,
    MatIconModule,
    MatSnackBarModule,
    PageHeaderComponent,
  ],
  template: `
    <app-page-header
      title="Turmas"
      subtitle="Gerencie as turmas da instituição"
      actionLabel="Nova Turma"
      actionIcon="add"
      actionRoute="/turmas/nova"
    />

    <mat-card>
      <mat-card-content>
        <div class="filters">
          <mat-form-field>
            <mat-label>Buscar</mat-label>
            <input matInput [formControl]="buscaControl" placeholder="Nome da turma" />
            <mat-icon matSuffix>search</mat-icon>
          </mat-form-field>

          <mat-form-field>
            <mat-label>Curso</mat-label>
            <mat-select [formControl]="cursoControl">
              <mat-option [value]="''">Todos</mat-option>
              @for (c of cursos(); track c.id) {
              <mat-option [value]="c.id">{{ c.nome }}</mat-option>
              }
            </mat-select>
          </mat-form-field>

          <mat-form-field>
            <mat-label>Turno</mat-label>
            <mat-select [formControl]="turnoControl">
              <mat-option [value]="''">Todos</mat-option>
              <mat-option value="Manha">Manhã</mat-option>
              <mat-option value="Tarde">Tarde</mat-option>
              <mat-option value="Noite">Noite</mat-option>
            </mat-select>
          </mat-form-field>

          <mat-form-field>
            <mat-label>Status</mat-label>
            <mat-select [formControl]="statusControl">
              <mat-option [value]="''">Todos</mat-option>
              <mat-option value="Planejada">Planejada</mat-option>
              <mat-option value="EmAndamento">Em andamento</mat-option>
              <mat-option value="Concluida">Concluída</mat-option>
              <mat-option value="Cancelada">Cancelada</mat-option>
            </mat-select>
          </mat-form-field>
        </div>

        <div class="table-container">
          <table mat-table [dataSource]="turmas()" class="full-width">
            <ng-container matColumnDef="nome">
              <th mat-header-cell *matHeaderCellDef>Turma</th>
              <td mat-cell *matCellDef="let t">
                <div class="title-cell">
                  <div class="title">{{ t.nome }}</div>
                  <div class="subtitle">
                    {{ t.curso?.nome || '-' }} — Prof. {{ t.professor?.nome || '-' }}
                  </div>
                </div>
              </td>
            </ng-container>

            <ng-container matColumnDef="turno">
              <th mat-header-cell *matHeaderCellDef>Turno</th>
              <td mat-cell *matCellDef="let t">{{ mapTurno(t.turno) }}</td>
            </ng-container>

            <ng-container matColumnDef="vagas">
              <th mat-header-cell *matHeaderCellDef>Vagas</th>
              <td mat-cell *matCellDef="let t">{{ t.vagasOcupadas }}/{{ t.vagas }}</td>
            </ng-container>

            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let t">
                <mat-chip [ngStyle]="getStatusStyle(t.status)">{{ mapStatus(t.status) }}</mat-chip>
              </td>
            </ng-container>

            <ng-container matColumnDef="acoes">
              <th mat-header-cell *matHeaderCellDef>Ações</th>
              <td mat-cell *matCellDef="let t">
                <button mat-icon-button [matMenuTriggerFor]="menu">
                  <mat-icon>more_vert</mat-icon>
                </button>
                <mat-menu #menu="matMenu">
                  <a mat-menu-item [routerLink]="['/turmas', t.id, 'editar']">
                    <mat-icon>edit</mat-icon>
                    <span>Editar</span>
                  </a>
                  <a mat-menu-item [routerLink]="['/turmas', t.id, 'alunos']">
                    <mat-icon>group</mat-icon>
                    <span>Alunos</span>
                  </a>
                  <a mat-menu-item [routerLink]="['/turmas', t.id, 'grade']">
                    <mat-icon>calendar_month</mat-icon>
                    <span>Grade</span>
                  </a>
                </mat-menu>
              </td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="displayed"></tr>
            <tr mat-row *matRowDef="let row; columns: displayed"></tr>

            <tr class="mat-row" *matNoDataRow>
              <td class="mat-cell" colspan="5">
                <div class="empty-state">
                  <mat-icon>class</mat-icon>
                  <p>Nenhuma turma encontrada</p>
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
      .title-cell .title {
        font-weight: 600;
      }
      .title-cell .subtitle {
        color: #666;
        font-size: 12px;
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
    `,
  ],
})
export class TurmasListPage implements OnInit {
  private readonly service = inject(TurmasService);
  private readonly cursosService = inject(CursosService);
  private readonly snackBar = inject(MatSnackBar);

  readonly turmas = signal<Turma[]>([]);
  readonly total = signal(0);
  readonly cursos = signal<{ id: string; nome: string }[]>([]);

  readonly buscaControl = new FormControl('');
  readonly cursoControl = new FormControl('');
  readonly turnoControl = new FormControl('');
  readonly statusControl = new FormControl('');

  readonly displayed = ['nome', 'turno', 'vagas', 'status', 'acoes'];
  readonly pageSize = 10;

  ngOnInit(): void {
    this.buscaControl.valueChanges.pipe(debounceTime(300)).subscribe(() => this.load(0));
    this.cursoControl.valueChanges.subscribe(() => this.load(0));
    this.turnoControl.valueChanges.subscribe(() => this.load(0));
    this.statusControl.valueChanges.subscribe(() => this.load(0));
    this.loadCursos();
    this.load();
  }

  load(page = 0): void {
    const filter: TurmaFilter = {
      nome: this.buscaControl.value || undefined,
      cursoId: this.cursoControl.value || undefined,
      turno: this.turnoControl.value || undefined,
      // status ainda não está no service; enviar como nome genérico se backend aceitar
      page,
      pageSize: this.pageSize,
    } as TurmaFilter;

    this.service.getAll(filter).subscribe({
      next: (resp: any) => {
        const items: Turma[] = (resp.items || resp.Items || []).map((t: any) => this.mapTurma(t));
        const total = resp.total ?? resp.TotalCount ?? items.length;
        this.turmas.set(items);
        this.total.set(total);
      },
      error: () => {
        this.snackBar.open('Erro ao carregar turmas', 'Fechar', { duration: 3000 });
      },
    });
  }

  onPage(event: PageEvent): void {
    this.load(event.pageIndex);
  }

  loadCursos(): void {
    this.cursosService.getAll({ page: 0, pageSize: 100, ativo: true }).subscribe({
      next: res => this.cursos.set(res.items || []),
      error: () => {
        this.cursos.set([]);
      },
    });
  }

  mapStatus(s: string): string {
    switch ((s || '').toLowerCase()) {
      case 'planejada':
        return 'Planejada';
      case 'emandamento':
        return 'Em andamento';
      case 'concluida':
        return 'Concluída';
      case 'cancelada':
        return 'Cancelada';
      default:
        return s || '-';
    }
  }

  getStatusStyle(s: string): any {
    const map: Record<string, { background: string; color: string }> = {
      planejada: { background: '#e3f2fd', color: '#1565c0' },
      emandamento: { background: '#e8f5e9', color: '#2e7d32' },
      concluida: { background: '#f3e5f5', color: '#6a1b9a' },
      cancelada: { background: '#ffebee', color: '#c62828' },
    };
    return map[(s || '').toLowerCase()] || { background: '#eee', color: '#555' };
  }

  mapTurno(t: Turno | string): string {
    switch ((t || '').toLowerCase()) {
      case 'manha':
        return 'Manhã';
      case 'tarde':
        return 'Tarde';
      case 'noite':
        return 'Noite';
      default:
        return t as string;
    }
  }

  private mapTurma(dto: any): Turma {
    return {
      id: dto.id ?? dto.Id,
      cursoId: dto.cursoId ?? dto.CursoId,
      nome: dto.nome ?? dto.Nome,
      professorId: dto.professorId ?? dto.ProfessorId,
      periodoInicio: dto.periodoInicio ?? dto.PeriodoInicio,
      periodoFim: dto.periodoFim ?? dto.PeriodoFim,
      vagas: dto.vagas ?? dto.Vagas ?? 0,
      vagasOcupadas: dto.vagasOcupadas ?? dto.VagasOcupadas ?? 0,
      turno: dto.turno ?? dto.Turno,
      status: dto.status ?? dto.Status ?? 'Planejada',
      horarios: (dto.horarios ?? dto.Horarios ?? []).map((h: any) => ({
        id: h.id ?? h.Id,
        diaSemana: h.diaSemana ?? h.DiaSemana,
        horaInicio: h.horaInicio ?? h.HoraInicio,
        horaFim: h.horaFim ?? h.HoraFim,
        sala: h.sala ?? h.Sala,
      })),
      curso: dto.curso ?? dto.Curso,
      professor: dto.professor ?? dto.Professor,
    } as Turma;
  }
}
