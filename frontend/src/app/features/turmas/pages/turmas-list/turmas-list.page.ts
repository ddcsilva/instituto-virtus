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

          <mat-form-field>
            <mat-label>Ano letivo</mat-label>
            <input matInput type="number" [formControl]="anoControl" placeholder="2025" />
          </mat-form-field>

          <mat-form-field>
            <mat-label>Semestre</mat-label>
            <mat-select [formControl]="periodoControl">
              <mat-option [value]="null">Todos</mat-option>
              <mat-option [value]="1">1º</mat-option>
              <mat-option [value]="2">2º</mat-option>
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
                    {{ t.cursoNome || t.curso?.nome || '-' }} — Prof.
                    {{ t.professorNome || t.professor?.nome || '-' }}
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

            <ng-container matColumnDef="sala">
              <th mat-header-cell *matHeaderCellDef>Sala</th>
              <td mat-cell *matCellDef="let t">{{ t.sala || '-' }}</td>
            </ng-container>

            <ng-container matColumnDef="capacidade">
              <th mat-header-cell *matHeaderCellDef>Capacidade</th>
              <td mat-cell *matCellDef="let t">{{ t.capacidade ?? '-' }}</td>
            </ng-container>

            <ng-container matColumnDef="periodoAno">
              <th mat-header-cell *matHeaderCellDef>Período/Ano</th>
              <td mat-cell *matCellDef="let t">
                {{ t.periodo || '-' }} / {{ t.anoLetivo || '-' }}
              </td>
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
                  <button mat-menu-item (click)="confirmToggle(t)">
                    <mat-icon>{{
                      (t.status || '').toLowerCase() === 'ativa' ? 'block' : 'check_circle'
                    }}</mat-icon>
                    <span>{{
                      (t.status || '').toLowerCase() === 'ativa' ? 'Desativar' : 'Ativar'
                    }}</span>
                  </button>
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
              <td class="mat-cell" [attr.colspan]="displayed.length">
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
  readonly anoControl = new FormControl<number | null>(new Date().getFullYear());
  readonly periodoControl = new FormControl<number | null>(null);

  readonly displayed = [
    'nome',
    'turno',
    'vagas',
    'sala',
    'capacidade',
    'periodoAno',
    'status',
    'acoes',
  ];
  readonly pageSize = 10;

  ngOnInit(): void {
    this.buscaControl.valueChanges.pipe(debounceTime(300)).subscribe(() => this.load(0));
    this.cursoControl.valueChanges.subscribe(() => this.load(0));
    this.turnoControl.valueChanges.subscribe(() => this.load(0));
    this.statusControl.valueChanges.subscribe(() => this.load(0));
    this.anoControl.valueChanges.subscribe(() => this.load(0));
    this.periodoControl.valueChanges.subscribe(() => this.load(0));
    this.loadCursos();
    this.load();
  }

  load(page = 0): void {
    const filter: TurmaFilter = {
      nome: this.buscaControl.value || undefined,
      cursoId: this.cursoControl.value || undefined,
      turno: this.turnoControl.value || undefined,
      anoLetivo: this.anoControl.value ?? new Date().getFullYear(),
      periodo: this.periodoControl.value ?? undefined,
      // status ainda não está no service; enviar como nome genérico se backend aceitar
      page,
      pageSize: this.pageSize,
    } as TurmaFilter;

    this.service.getAll(filter).subscribe({
      next: (resp: any) => {
        const list = Array.isArray(resp) ? resp : resp.items || resp.Items || [];
        const items: Turma[] = list.map((t: any) => this.mapTurma(t));
        const total = Array.isArray(resp)
          ? items.length
          : resp.total ?? resp.TotalCount ?? items.length;
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

  confirmToggle(t: Turma): void {
    this.service.toggleStatus(t.id).subscribe({
      next: (dto: any) => {
        // Simplificado: recarrega a lista para refletir status
        this.snackBar.open('Status da turma alterado', 'Fechar', { duration: 2500 });
        this.load();
      },
      error: () => this.snackBar.open('Erro ao alterar status', 'Fechar', { duration: 3000 }),
    });
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
    const v = (s || '').toLowerCase();
    if (['planejada', 'emandamento', 'concluida', 'cancelada'].includes(v)) {
      return v === 'emandamento' ? 'Em andamento' : v.charAt(0).toUpperCase() + v.slice(1);
    }
    if (v === 'true') return 'Ativa';
    if (v === 'false') return 'Inativa';
    return s || '-';
  }

  getStatusStyle(s: string): any {
    const key = (s || '').toLowerCase();
    const map: Record<string, { background: string; color: string }> = {
      planejada: { background: '#e3f2fd', color: '#1565c0' },
      emandamento: { background: '#e8f5e9', color: '#2e7d32' },
      concluida: { background: '#f3e5f5', color: '#6a1b9a' },
      cancelada: { background: '#ffebee', color: '#c62828' },
      ativa: { background: '#e8f5e9', color: '#2e7d32' },
      inativa: { background: '#ffebee', color: '#c62828' },
    };
    return map[key] || { background: '#eee', color: '#555' };
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
    // Backend atual retorna TurmaDto com propriedades PascalCase
    const nome = dto.Nome ?? dto.nome ?? dto.CursoNome ?? dto.cursoNome ?? 'Turma';
    const horarioInicio = dto.HorarioInicio ?? dto.horarioInicio;
    const hour =
      typeof horarioInicio === 'string' ? parseInt(horarioInicio.split(':')[0] || '0', 10) : 0;
    const turnoDerivado: Turno = hour < 12 ? 'Manha' : hour < 18 ? 'Tarde' : 'Noite';
    const ativo = dto.Ativo ?? dto.ativo;
    const mapped: any = {
      id: dto.Id ?? dto.id,
      cursoId: dto.CursoId ?? dto.cursoId,
      nome,
      professorId: dto.ProfessorId ?? dto.professorId,
      cursoNome: dto.CursoNome ?? dto.cursoNome,
      professorNome: dto.ProfessorNome ?? dto.professorNome,
      capacidade: dto.Capacidade ?? dto.capacidade,
      sala: dto.Sala ?? dto.sala,
      anoLetivo: dto.AnoLetivo ?? dto.anoLetivo,
      periodo: dto.Periodo ?? dto.periodo,
      periodoInicio: dto.PeriodoInicio ?? dto.periodoInicio ?? '',
      periodoFim: dto.PeriodoFim ?? dto.periodoFim ?? '',
      vagas: dto.Capacidade ?? dto.vagas ?? 0,
      vagasOcupadas: dto.AlunosMatriculados ?? dto.vagasOcupadas ?? 0,
      turno: (dto.Turno ?? dto.turno ?? turnoDerivado) as Turno,
      status:
        typeof ativo === 'boolean'
          ? ativo
            ? 'Ativa'
            : 'Inativa'
          : dto.Status ?? dto.status ?? 'Ativa',
      horarios: [],
      curso: { id: dto.CursoId ?? dto.cursoId, nome: dto.CursoNome ?? dto.cursoNome ?? '' },
      professor: dto.ProfessorNome
        ? { id: dto.ProfessorId ?? dto.professorId, nome: dto.ProfessorNome }
        : undefined,
    };
    return mapped as unknown as Turma;
  }
}
