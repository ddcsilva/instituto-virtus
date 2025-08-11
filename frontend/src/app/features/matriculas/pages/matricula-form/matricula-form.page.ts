import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSliderModule } from '@angular/material/slider';
import { MatDividerModule } from '@angular/material/divider';
import { MatChipsModule } from '@angular/material/chips';
import { MatSnackBar } from '@angular/material/snack-bar';

import { PageHeaderComponent } from '../../../../shared/ui/components/page-header/page-header.component';
import { MatriculasService } from '../../data-access/matriculas.service';
import { PessoasStore } from '../../../pessoas/data-access/pessoas.store';
import { TurmasService } from '../../../turmas/data-access/turmas.service';
import { FinanceiroService } from '../../../financeiro/data-access/financeiro.service';
import { CreateMatriculaRequest } from '../../models/matricula.model';
import { Turma } from '../../../turmas/models/turma.model';
import { Pessoa } from '../../../pessoas/models/pessoa.model';

@Component({
  selector: 'app-matricula-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule,
    MatIconModule,
    MatCheckboxModule,
    MatSliderModule,
    MatDividerModule,
    MatChipsModule,
    CurrencyPipe,
    PageHeaderComponent,
  ],
  providers: [PessoasStore],
  template: `
    <app-page-header
      title="Nova Matrícula"
      subtitle="Matricule um aluno em uma turma"
    />

    <div class="form-container">
      <mat-card>
        <mat-card-header>
          <mat-card-title>Dados da Matrícula</mat-card-title>
        </mat-card-header>

        <mat-card-content>
          <form [formGroup]="matriculaForm" (ngSubmit)="onSubmit()">
            <!-- Seleção de Aluno -->
            <div class="form-section">
              <h3>Aluno</h3>
              <mat-form-field class="full-width">
                <mat-label>Selecione o Aluno</mat-label>
                <mat-select
                  formControlName="alunoId"
                  (selectionChange)="onAlunoChange($event.value)"
                >
                  <mat-option>
                    <ngx-mat-select-search
                      [formControl]="alunoFilterCtrl"
                      placeholderLabel="Buscar aluno..."
                      noEntriesFoundLabel="Nenhum aluno encontrado"
                    >
                    </ngx-mat-select-search>
                  </mat-option>
                  @for (aluno of filteredAlunos(); track aluno.id) {
                  <mat-option [value]="aluno.id">
                    {{ aluno.nome }} - {{ aluno.cpf }}
                  </mat-option>
                  }
                </mat-select>
                @if (matriculaForm.get('alunoId')?.hasError('required')) {
                <mat-error>Selecione um aluno</mat-error>
                }
              </mat-form-field>

              @if (selectedAluno()) {
              <div class="aluno-info">
                <mat-chip-set>
                  <mat-chip>{{ selectedAluno()!.nome }}</mat-chip>
                  <mat-chip>{{ selectedAluno()!.telefone }}</mat-chip>
                  @if (selectedAluno()!.email) {
                  <mat-chip>{{ selectedAluno()!.email }}</mat-chip>
                  }
                </mat-chip-set>
              </div>
              }
            </div>

            <!-- Seleção de Turma -->
            <div class="form-section">
              <h3>Turma</h3>
              <div class="form-grid">
                <mat-form-field>
                  <mat-label>Curso</mat-label>
                  <mat-select
                    [formControl]="cursoFilterCtrl"
                    (selectionChange)="filterTurmasByCurso($event.value)"
                  >
                    <mat-option value="">Todos</mat-option>
                    @for (curso of cursos(); track curso.id) {
                    <mat-option [value]="curso.id">{{ curso.nome }}</mat-option>
                    }
                  </mat-select>
                </mat-form-field>

                <mat-form-field>
                  <mat-label>Turno</mat-label>
                  <mat-select
                    [formControl]="turnoFilterCtrl"
                    (selectionChange)="filterTurmasByTurno($event.value)"
                  >
                    <mat-option value="">Todos</mat-option>
                    <mat-option value="Manha">Manhã</mat-option>
                    <mat-option value="Tarde">Tarde</mat-option>
                    <mat-option value="Noite">Noite</mat-option>
                  </mat-select>
                </mat-form-field>
              </div>

              <mat-form-field class="full-width">
                <mat-label>Selecione a Turma</mat-label>
                <mat-select
                  formControlName="turmaId"
                  (selectionChange)="onTurmaChange($event.value)"
                >
                  @for (turma of filteredTurmas(); track turma.id) {
                  <mat-option
                    [value]="turma.id"
                    [disabled]="turma.vagasOcupadas >= turma.vagas"
                  >
                    <div class="turma-option">
                      <span>{{ turma.nome }} - {{ turma.curso?.nome }}</span>
                      <span class="vagas-info">
                        {{ turma.vagasOcupadas }}/{{ turma.vagas }} vagas
                      </span>
                    </div>
                  </mat-option>
                  }
                </mat-select>
                @if (matriculaForm.get('turmaId')?.hasError('required')) {
                <mat-error>Selecione uma turma</mat-error>
                }
              </mat-form-field>

              @if (selectedTurma()) {
              <div class="turma-info">
                <div class="info-grid">
                  <div class="info-item">
                    <mat-icon>school</mat-icon>
                    <span>{{ selectedTurma()!.curso?.nome }}</span>
                  </div>
                  <div class="info-item">
                    <mat-icon>person</mat-icon>
                    <span>Prof. {{ selectedTurma()!.professor?.nome }}</span>
                  </div>
                  <div class="info-item">
                    <mat-icon>schedule</mat-icon>
                    <span>{{ formatHorarios(selectedTurma()!.horarios) }}</span>
                  </div>
                  <div class="info-item">
                    <mat-icon>date_range</mat-icon>
                    <span
                      >{{
                        selectedTurma()!.periodoInicio | date : 'dd/MM/yyyy'
                      }}
                      -
                      {{
                        selectedTurma()!.periodoFim | date : 'dd/MM/yyyy'
                      }}</span
                    >
                  </div>
                </div>
              </div>
              }
            </div>

            <!-- Dados Financeiros -->
            <div class="form-section">
              <h3>Informações Financeiras</h3>

              <div class="form-grid">
                <mat-form-field>
                  <mat-label>Data da Matrícula</mat-label>
                  <input
                    matInput
                    [matDatepicker]="picker"
                    formControlName="dataMatricula"
                  />
                  <mat-datepicker-toggle matIconSuffix [for]="picker" />
                  <mat-datepicker #picker />
                  @if (matriculaForm.get('dataMatricula')?.hasError('required'))
                  {
                  <mat-error>Data é obrigatória</mat-error>
                  }
                </mat-form-field>

                <mat-form-field>
                  <mat-label>Valor da Mensalidade</mat-label>
                  <input
                    matInput
                    type="number"
                    formControlName="valorMensalidade"
                  />
                  <span matPrefix>R$ </span>
                  @if
                  (matriculaForm.get('valorMensalidade')?.hasError('required'))
                  {
                  <mat-error>Valor é obrigatório</mat-error>
                  }
                </mat-form-field>
              </div>

              <div class="desconto-section">
                <mat-checkbox formControlName="aplicarDesconto">
                  Aplicar Desconto
                </mat-checkbox>

                @if (matriculaForm.get('aplicarDesconto')?.value) {
                <div class="desconto-controls">
                  <mat-slider
                    [min]="0"
                    [max]="50"
                    [step]="5"
                    [displayWith]="formatPercent"
                  >
                    <input
                      matSliderThumb
                      formControlName="descontoPercentual"
                      (valueChange)="updateDesconto($event)"
                    />
                  </mat-slider>

                  <div class="desconto-display">
                    <span
                      >{{ matriculaForm.get('descontoPercentual')?.value }}% de
                      desconto</span
                    >
                    <strong
                      >Valor final:
                      {{ valorComDesconto() | currency : 'BRL' }}</strong
                    >
                  </div>
                </div>
                }
              </div>

              <mat-form-field class="full-width">
                <mat-label>Observações</mat-label>
                <textarea
                  matInput
                  formControlName="observacao"
                  rows="3"
                ></textarea>
              </mat-form-field>
            </div>

            <!-- Geração de Mensalidades -->
            <div class="form-section">
              <h3>Geração de Mensalidades</h3>

              <mat-checkbox formControlName="gerarMensalidades">
                Gerar mensalidades automaticamente
              </mat-checkbox>

              @if (matriculaForm.get('gerarMensalidades')?.value) {
              <div class="mensalidades-config">
                <mat-form-field>
                  <mat-label>Quantidade de Meses</mat-label>
                  <input
                    matInput
                    type="number"
                    formControlName="quantidadeMeses"
                    min="1"
                    max="12"
                  />
                  <mat-hint>Número de mensalidades a gerar</mat-hint>
                </mat-form-field>

                <div class="preview-mensalidades">
                  <h4>Prévia das Mensalidades</h4>
                  <div class="mensalidades-list">
                    @for (mensalidade of previewMensalidades(); track
                    mensalidade.competencia) {
                    <div class="mensalidade-item">
                      <span>{{ mensalidade.competencia }}</span>
                      <span
                        >Vencimento:
                        {{ mensalidade.vencimento | date : 'dd/MM/yyyy' }}</span
                      >
                      <strong>{{
                        valorComDesconto() | currency : 'BRL'
                      }}</strong>
                    </div>
                    }
                  </div>
                  <div class="total-preview">
                    <span>Total:</span>
                    <strong>{{
                      totalMensalidades() | currency : 'BRL'
                    }}</strong>
                  </div>
                </div>
              </div>
              }
            </div>

            <!-- Resumo -->
            <mat-divider></mat-divider>

            <div class="resumo-section">
              <h3>Resumo da Matrícula</h3>
              <div class="resumo-grid">
                <div class="resumo-item">
                  <span>Aluno:</span>
                  <strong>{{ selectedAluno()?.nome || '-' }}</strong>
                </div>
                <div class="resumo-item">
                  <span>Turma:</span>
                  <strong>{{ selectedTurma()?.nome || '-' }}</strong>
                </div>
                <div class="resumo-item">
                  <span>Mensalidade:</span>
                  <strong>{{ valorComDesconto() | currency : 'BRL' }}</strong>
                </div>
                @if (matriculaForm.get('gerarMensalidades')?.value) {
                <div class="resumo-item">
                  <span>Mensalidades a Gerar:</span>
                  <strong
                    >{{
                      matriculaForm.get('quantidadeMeses')?.value
                    }}
                    meses</strong
                  >
                </div>
                }
              </div>
            </div>

            <div class="form-actions">
              <button mat-button type="button" (click)="cancel()">
                Cancelar
              </button>
              <button
                mat-raised-button
                color="primary"
                type="submit"
                [disabled]="matriculaForm.invalid || loading()"
              >
                <mat-icon>check</mat-icon>
                Confirmar Matrícula
              </button>
            </div>
          </form>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [
    `
      .form-container {
        max-width: 900px;
        margin: 0 auto;
      }

      .form-section {
        margin-bottom: 32px;

        h3 {
          color: #666;
          margin-bottom: 16px;
          font-size: 18px;
        }
      }

      .form-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
        gap: 16px;
      }

      .full-width {
        width: 100%;
      }

      .aluno-info,
      .turma-info {
        padding: 16px;
        background: #f5f5f5;
        border-radius: 4px;
        margin-top: 16px;
      }

      .info-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
        gap: 12px;
      }

      .info-item {
        display: flex;
        align-items: center;
        gap: 8px;

        mat-icon {
          color: #666;
          font-size: 20px;
          width: 20px;
          height: 20px;
        }
      }

      .turma-option {
        display: flex;
        justify-content: space-between;
        width: 100%;

        .vagas-info {
          color: #666;
          font-size: 12px;
        }
      }

      .desconto-section {
        margin: 24px 0;
      }

      .desconto-controls {
        margin-top: 16px;
        padding: 16px;
        background: #f5f5f5;
        border-radius: 4px;

        mat-slider {
          width: 100%;
        }
      }

      .desconto-display {
        display: flex;
        justify-content: space-between;
        margin-top: 16px;

        strong {
          color: #4caf50;
        }
      }

      .mensalidades-config {
        margin-top: 16px;
        padding: 16px;
        background: #f5f5f5;
        border-radius: 4px;
      }

      .preview-mensalidades {
        margin-top: 16px;

        h4 {
          margin-bottom: 12px;
          color: #666;
        }
      }

      .mensalidades-list {
        display: flex;
        flex-direction: column;
        gap: 8px;
        max-height: 200px;
        overflow-y: auto;
      }

      .mensalidade-item {
        display: grid;
        grid-template-columns: 1fr 1fr auto;
        gap: 16px;
        padding: 8px;
        background: white;
        border-radius: 4px;
        border: 1px solid #e0e0e0;
      }

      .total-preview {
        display: flex;
        justify-content: space-between;
        margin-top: 16px;
        padding-top: 16px;
        border-top: 1px solid #e0e0e0;
        font-size: 18px;

        strong {
          color: #1976d2;
        }
      }

      .resumo-section {
        margin: 24px 0;

        h3 {
          margin-bottom: 16px;
          color: #666;
        }
      }

      .resumo-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
        gap: 16px;
        padding: 16px;
        background: #f5f5f5;
        border-radius: 4px;
      }

      .resumo-item {
        display: flex;
        flex-direction: column;
        gap: 4px;

        span {
          color: #666;
          font-size: 14px;
        }

        strong {
          font-size: 16px;
        }
      }

      .form-actions {
        display: flex;
        justify-content: flex-end;
        gap: 8px;
        margin-top: 24px;
      }
    `,
  ],
})
export class MatriculaFormPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly snackBar = inject(MatSnackBar);
  private readonly matriculasService = inject(MatriculasService);
  private readonly turmasService = inject(TurmasService);
  private readonly financeiroService = inject(FinanceiroService);
  private readonly pessoasStore = inject(PessoasStore);

  readonly loading = signal(false);
  readonly alunos = signal<Pessoa[]>([]);
  readonly turmas = signal<Turma[]>([]);
  readonly cursos = signal<any[]>([]);
  readonly selectedAluno = signal<Pessoa | null>(null);
  readonly selectedTurma = signal<Turma | null>(null);

  readonly alunoFilterCtrl = this.fb.control('');
  readonly cursoFilterCtrl = this.fb.control('');
  readonly turnoFilterCtrl = this.fb.control('');

  readonly matriculaForm = this.fb.nonNullable.group({
    alunoId: ['', Validators.required],
    turmaId: ['', Validators.required],
    dataMatricula: [new Date(), Validators.required],
    valorMensalidade: [0, [Validators.required, Validators.min(0)]],
    aplicarDesconto: [false],
    descontoPercentual: [0, [Validators.min(0), Validators.max(50)]],
    observacao: [''],
    gerarMensalidades: [true],
    quantidadeMeses: [12, [Validators.min(1), Validators.max(12)]],
  });

  readonly filteredAlunos = computed(() => {
    const filter = this.alunoFilterCtrl.value?.toLowerCase() || '';
    return this.alunos().filter(
      (aluno) =>
        aluno.nome.toLowerCase().includes(filter) || aluno.cpf.includes(filter)
    );
  });

  readonly filteredTurmas = computed(() => {
    let turmas = this.turmas();
    const curso = this.cursoFilterCtrl.value;
    const turno = this.turnoFilterCtrl.value;

    if (curso) {
      turmas = turmas.filter((t) => t.cursoId === curso);
    }
    if (turno) {
      turmas = turmas.filter((t) => t.turno === turno);
    }

    return turmas;
  });

  readonly valorComDesconto = computed(() => {
    const valor = this.matriculaForm.get('valorMensalidade')?.value || 0;
    const desconto = this.matriculaForm.get('aplicarDesconto')?.value
      ? this.matriculaForm.get('descontoPercentual')?.value || 0
      : 0;

    return valor * (1 - desconto / 100);
  });

  readonly previewMensalidades = computed(() => {
    const quantidade = this.matriculaForm.get('quantidadeMeses')?.value || 0;
    const dataInicio = new Date(
      this.matriculaForm.get('dataMatricula')?.value || new Date()
    );
    const mensalidades = [];

    for (let i = 0; i < quantidade; i++) {
      const competenciaDate = new Date(dataInicio);
      competenciaDate.setMonth(competenciaDate.getMonth() + i);

      const vencimentoDate = new Date(competenciaDate);
      vencimentoDate.setDate(10); // Vencimento no dia 10

      mensalidades.push({
        competencia: `${
          competenciaDate.getMonth() + 1
        }/${competenciaDate.getFullYear()}`,
        vencimento: vencimentoDate,
      });
    }

    return mensalidades;
  });

  readonly totalMensalidades = computed(() => {
    const quantidade = this.matriculaForm.get('quantidadeMeses')?.value || 0;
    return this.valorComDesconto() * quantidade;
  });

  ngOnInit(): void {
    this.loadAlunos();
    this.loadTurmas();
    this.loadCursos();
  }

  loadAlunos(): void {
    this.pessoasStore.loadPessoas({ tipo: 'Aluno', ativo: true });
    this.pessoasStore.alunos$.subscribe((alunos) => this.alunos.set(alunos));
  }

  loadTurmas(): void {
    this.turmasService.getAll({ status: 'EmAndamento' }).subscribe((result) => {
      this.turmas.set(result.items || []);
    });
  }

  loadCursos(): void {
    // Mock - substituir por serviço real
    this.cursos.set([
      { id: '1', nome: 'Teologia I' },
      { id: '2', nome: 'Violão Básico' },
      { id: '3', nome: 'Piano Intermediário' },
    ]);
  }

  onAlunoChange(alunoId: string): void {
    const aluno = this.alunos().find((a) => a.id === alunoId);
    this.selectedAluno.set(aluno || null);
  }

  onTurmaChange(turmaId: string): void {
    const turma = this.turmas().find((t) => t.id === turmaId);
    this.selectedTurma.set(turma || null);

    if (turma?.curso) {
      this.matriculaForm.patchValue({
        valorMensalidade: turma.curso.valor || 0,
      });
    }
  }

  filterTurmasByCurso(cursoId: string): void {
    // Filtro reativo via computed signal
  }

  filterTurmasByTurno(turno: string): void {
    // Filtro reativo via computed signal
  }

  updateDesconto(value: number): void {
    this.matriculaForm.patchValue({ descontoPercentual: value });
  }

  formatPercent(value: number): string {
    return `${value}%`;
  }

  formatHorarios(horarios: any[]): string {
    if (!horarios || horarios.length === 0) return '-';

    return horarios
      .map((h) => `${h.diaSemana.substr(0, 3)} ${h.horaInicio}`)
      .join(', ');
  }

  onSubmit(): void {
    if (this.matriculaForm.invalid) return;

    this.loading.set(true);
    const formValue = this.matriculaForm.getRawValue();

    const request: CreateMatriculaRequest = {
      alunoId: formValue.alunoId,
      turmaId: formValue.turmaId,
      dataMatricula: formValue.dataMatricula.toISOString(),
      valorMensalidade: formValue.valorMensalidade,
      descontoPercentual: formValue.aplicarDesconto
        ? formValue.descontoPercentual
        : undefined,
      observacao: formValue.observacao || undefined,
    };

    this.matriculasService.create(request).subscribe({
      next: (matricula) => {
        if (formValue.gerarMensalidades) {
          this.financeiroService
            .gerarMensalidades(matricula.id, formValue.quantidadeMeses)
            .subscribe({
              next: () => {
                this.snackBar.open(
                  'Matrícula realizada e mensalidades geradas com sucesso!',
                  'Fechar',
                  { duration: 5000 }
                );
                this.router.navigate(['/matriculas']);
              },
              error: () => {
                this.snackBar.open(
                  'Matrícula realizada, mas erro ao gerar mensalidades',
                  'Fechar',
                  { duration: 5000 }
                );
                this.router.navigate(['/matriculas']);
              },
            });
        } else {
          this.snackBar.open('Matrícula realizada com sucesso!', 'Fechar', {
            duration: 3000,
          });
          this.router.navigate(['/matriculas']);
        }
      },
      error: () => {
        this.loading.set(false);
        this.snackBar.open('Erro ao realizar matrícula', 'Fechar', {
          duration: 5000,
        });
      },
    });
  }

  cancel(): void {
    this.router.navigate(['/matriculas']);
  }
}
