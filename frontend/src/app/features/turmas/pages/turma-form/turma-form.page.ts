import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { PageHeaderComponent } from '../../../../shared/ui/components/page-header/page-header.component';
import { TurmasService } from '../../data-access/turmas.service';
import { CursosService } from '../../../cursos/data-access/cursos.service';
import { PessoasStore } from '../../../pessoas/data-access/pessoas.store';
import {
  Turma,
  CreateTurmaRequest,
  UpdateTurmaRequest,
  HorarioAula,
} from '../../models/turma.model';

@Component({
  selector: 'app-turma-form',
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
    MatDividerModule,
    PageHeaderComponent,
  ],
  providers: [PessoasStore],
  template: `
    <app-page-header
      [title]="isEdit() ? 'Editar Turma' : 'Nova Turma'"
      [subtitle]="isEdit() ? 'Atualize os dados da turma' : 'Cadastre uma nova turma'"
    />

    <mat-card>
      <mat-card-content>
        <form [formGroup]="form" (ngSubmit)="onSubmit()" class="form-grid">
          <mat-form-field>
            <mat-label>Curso</mat-label>
            <mat-select formControlName="cursoId" required>
              @for (c of cursos(); track c.id) {
              <mat-option [value]="c.id">{{ c.nome }}</mat-option>
              }
            </mat-select>
          </mat-form-field>

          <mat-form-field>
            <mat-label>Nome da Turma</mat-label>
            <input matInput formControlName="nome" />
          </mat-form-field>

          <mat-form-field>
            <mat-label>Professor</mat-label>
            <mat-select formControlName="professorId" required>
              @for (p of professores(); track p.id) {
              <mat-option [value]="p.id">{{ p.nome }}</mat-option>
              }
            </mat-select>
          </mat-form-field>

          <mat-form-field>
            <mat-label>Vagas</mat-label>
            <input matInput type="number" min="1" formControlName="vagas" />
          </mat-form-field>

          <mat-form-field>
            <mat-label>Turno</mat-label>
            <mat-select formControlName="turno">
              <mat-option value="Manha">Manhã</mat-option>
              <mat-option value="Tarde">Tarde</mat-option>
              <mat-option value="Noite">Noite</mat-option>
            </mat-select>
          </mat-form-field>

          <mat-form-field>
            <mat-label>Início (AAAA-MM-DD)</mat-label>
            <input matInput formControlName="periodoInicio" placeholder="2025-02-01" />
          </mat-form-field>

          <mat-form-field>
            <mat-label>Fim (AAAA-MM-DD)</mat-label>
            <input matInput formControlName="periodoFim" placeholder="2025-12-15" />
          </mat-form-field>

          <div class="grid-full">
            <h3>Horários</h3>
            <div class="horarios-list" formArrayName="horarios">
              @for (ctrl of horarios().controls; let i = $index; track i) {
              <div class="horario-item" [formGroupName]="i">
                <mat-form-field>
                  <mat-label>Dia</mat-label>
                  <mat-select formControlName="diaSemana">
                    <mat-option value="Segunda">Segunda</mat-option>
                    <mat-option value="Terca">Terça</mat-option>
                    <mat-option value="Quarta">Quarta</mat-option>
                    <mat-option value="Quinta">Quinta</mat-option>
                    <mat-option value="Sexta">Sexta</mat-option>
                    <mat-option value="Sabado">Sábado</mat-option>
                    <mat-option value="Domingo">Domingo</mat-option>
                  </mat-select>
                </mat-form-field>

                <mat-form-field>
                  <mat-label>Início (HH:mm)</mat-label>
                  <input matInput formControlName="horaInicio" placeholder="19:00" />
                </mat-form-field>

                <mat-form-field>
                  <mat-label>Fim (HH:mm)</mat-label>
                  <input matInput formControlName="horaFim" placeholder="19:50" />
                </mat-form-field>

                <mat-form-field>
                  <mat-label>Sala (opcional)</mat-label>
                  <input matInput formControlName="sala" />
                </mat-form-field>

                <button mat-icon-button color="warn" type="button" (click)="removeHorario(i)">
                  <mat-icon>delete</mat-icon>
                </button>
              </div>
              }
            </div>

            <button mat-stroked-button type="button" (click)="addHorario()">
              <mat-icon>add</mat-icon>
              Adicionar horário
            </button>
          </div>

          @if (isEdit()) {
          <mat-form-field class="grid-full">
            <mat-label>Status</mat-label>
            <mat-select formControlName="status">
              <mat-option value="Planejada">Planejada</mat-option>
              <mat-option value="EmAndamento">Em andamento</mat-option>
              <mat-option value="Concluida">Concluída</mat-option>
              <mat-option value="Cancelada">Cancelada</mat-option>
            </mat-select>
          </mat-form-field>
          }

          <div class="actions grid-full">
            <button mat-button type="button" (click)="voltar()">Cancelar</button>
            <button mat-raised-button color="primary" type="submit" [disabled]="form.invalid">
              Salvar
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
        gap: 16px;
        grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
      }
      .grid-full {
        grid-column: 1 / -1;
      }
      .horarios-list {
        display: flex;
        flex-direction: column;
        gap: 12px;
        margin-bottom: 12px;
      }
      .horario-item {
        display: grid;
        gap: 12px;
        grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)) auto;
        align-items: center;
      }
      h3 {
        margin: 0 0 8px;
        color: #666;
      }
      .actions {
        display: flex;
        justify-content: flex-end;
        gap: 8px;
      }
    `,
  ],
})
export class TurmaFormPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly turmasService = inject(TurmasService);
  private readonly cursosService = inject(CursosService);
  private readonly pessoasStore = inject(PessoasStore);

  readonly cursos = signal<{ id: string; nome: string }[]>([]);
  readonly professores = signal<{ id: string; nome: string }[]>([]);

  readonly form = this.fb.group({
    cursoId: ['', Validators.required],
    nome: ['', Validators.required],
    professorId: ['', Validators.required],
    vagas: [10, [Validators.required, Validators.min(1)]],
    turno: ['Manha', Validators.required],
    periodoInicio: ['', Validators.required],
    periodoFim: ['', Validators.required],
    horarios: this.fb.array<FormGroup<HorarioGroup>>([]),
    status: ['Planejada'],
  });

  ngOnInit(): void {
    this.loadCursos();
    this.loadProfessores();
    this.addHorario();

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.turmasService.getById(id).subscribe(dto => {
        const t = this.mapTurma(dto);
        this.form.patchValue({
          cursoId: t.cursoId,
          nome: t.nome,
          professorId: t.professorId,
          vagas: t.vagas,
          turno: t.turno,
          periodoInicio: t.periodoInicio,
          periodoFim: t.periodoFim,
          status: t.status,
        });
        this.clearHorarios();
        for (const h of t.horarios) this.horarios().push(this.createHorarioGroup(h));
      });
    }
  }

  isEdit(): boolean {
    return !!this.route.snapshot.paramMap.get('id');
  }

  horarios(): FormArray<FormGroup<HorarioGroup>> {
    return this.form.get('horarios') as FormArray<FormGroup<HorarioGroup>>;
  }

  addHorario(): void {
    this.horarios().push(this.createHorarioGroup());
  }

  removeHorario(index: number): void {
    this.horarios().removeAt(index);
  }

  clearHorarios(): void {
    while (this.horarios().length) this.horarios().removeAt(0);
  }

  createHorarioGroup(h?: HorarioAula): FormGroup<HorarioGroup> {
    return this.fb.group<HorarioGroup>({
      diaSemana: new FormControl(h?.diaSemana || 'Segunda', {
        nonNullable: true,
        validators: [Validators.required],
      }),
      horaInicio: new FormControl(h?.horaInicio || '', {
        nonNullable: true,
        validators: [Validators.required],
      }),
      horaFim: new FormControl(h?.horaFim || '', {
        nonNullable: true,
        validators: [Validators.required],
      }),
      sala: new FormControl(h?.sala || '', { nonNullable: true }),
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    const payload = this.toPayload();
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.turmasService.update(id, payload as UpdateTurmaRequest).subscribe(() => this.voltar());
    } else {
      this.turmasService.create(payload as CreateTurmaRequest).subscribe(() => this.voltar());
    }
  }

  voltar(): void {
    this.router.navigate(['/turmas']);
  }

  private toPayload(): CreateTurmaRequest | UpdateTurmaRequest {
    const v = this.form.value as any;
    const horarios = this.horarios().controls.map(g => ({
      diaSemana: g.get('diaSemana')!.value,
      horaInicio: g.get('horaInicio')!.value,
      horaFim: g.get('horaFim')!.value,
      sala: g.get('sala')!.value || undefined,
    }));
    return {
      cursoId: v.cursoId,
      nome: v.nome,
      professorId: v.professorId,
      vagas: Number(v.vagas) || 0,
      turno: v.turno,
      periodoInicio: v.periodoInicio,
      periodoFim: v.periodoFim,
      horarios,
      status: v.status,
    } as UpdateTurmaRequest;
  }

  private loadCursos(): void {
    this.cursosService.getAll({ page: 0, pageSize: 100, ativo: true }).subscribe(res => {
      this.cursos.set(res.items || []);
    });
  }

  private loadProfessores(): void {
    // Reutiliza PessoasStore para carregar e filtrar professores
    this.pessoasStore.updateFilter({ tipo: 'Professor', page: 0, pageSize: 100 });
    this.pessoasStore.loadPessoas(this.pessoasStore.filter$);
    this.pessoasStore.professores$.subscribe(list => {
      this.professores.set(list.map(p => ({ id: p.id, nome: p.nome })));
    });
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

type HorarioGroup = {
  diaSemana: FormControl<string>;
  horaInicio: FormControl<string>;
  horaFim: FormControl<string>;
  sala: FormControl<string>;
};
