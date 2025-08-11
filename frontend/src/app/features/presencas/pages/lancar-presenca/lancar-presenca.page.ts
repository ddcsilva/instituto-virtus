import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  ReactiveFormsModule,
  FormsModule,
  Validators,
} from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatChipsModule } from '@angular/material/chips';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';

import { PageHeaderComponent } from '../../../../shared/ui/components/page-header/page-header.component';
import { PresencasService } from '../../data-access/presencas.service';
import { TurmasService } from '../../../turmas/data-access/turmas.service';
import {
  CreatePresencaRequest,
  StatusPresenca,
} from '../../models/presenca.model';

interface AlunoPresenca {
  alunoId: string;
  nome: string;
  status: StatusPresenca;
  observacao: string;
}

@Component({
  selector: 'app-lancar-presenca',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatCheckboxModule,
    MatButtonToggleModule,
    MatChipsModule,
    PageHeaderComponent,
  ],
  template: `
    <app-page-header
      title="Lançar Presença"
      subtitle="Registre a presença dos alunos na aula"
    />

    <mat-card>
      <mat-card-header>
        <mat-card-title>Dados da Aula</mat-card-title>
      </mat-card-header>

      <mat-card-content>
        <form [formGroup]="aulaForm">
          <div class="form-grid">
            <mat-form-field>
              <mat-label>Turma</mat-label>
              <mat-select
                formControlName="turmaId"
                (selectionChange)="onTurmaChange($event.value)"
              >
                @for (turma of turmas(); track turma.id) {
                <mat-option [value]="turma.id">
                  {{ turma.nome }} - {{ turma.curso?.nome }}
                </mat-option>
                }
              </mat-select>
              @if (aulaForm.get('turmaId')?.hasError('required')) {
              <mat-error>Selecione a turma</mat-error>
              }
            </mat-form-field>

            <mat-form-field>
              <mat-label>Data da Aula</mat-label>
              <input matInput [matDatepicker]="picker" formControlName="data" />
              <mat-datepicker-toggle matIconSuffix [for]="picker" />
              <mat-datepicker #picker />
              @if (aulaForm.get('data')?.hasError('required')) {
              <mat-error>Informe a data</mat-error>
              }
            </mat-form-field>

            <mat-form-field>
              <mat-label>Horário</mat-label>
              <mat-select formControlName="horario">
                <mat-option value="08:00">08:00 - 08:50</mat-option>
                <mat-option value="09:00">09:00 - 09:50</mat-option>
                <mat-option value="10:00">10:00 - 10:50</mat-option>
                <mat-option value="14:00">14:00 - 14:50</mat-option>
                <mat-option value="15:00">15:00 - 15:50</mat-option>
                <mat-option value="19:00">19:00 - 19:50</mat-option>
                <mat-option value="20:00">20:00 - 20:50</mat-option>
              </mat-select>
            </mat-form-field>

            <mat-form-field class="full-width">
              <mat-label>Conteúdo da Aula (opcional)</mat-label>
              <textarea matInput formControlName="conteudo" rows="2"></textarea>
            </mat-form-field>
          </div>
        </form>

        @if (alunos().length > 0) {
        <mat-card class="lista-presenca">
          <mat-card-header>
            <mat-card-title>Lista de Presença</mat-card-title>
            <div class="acoes-rapidas">
              <button mat-button (click)="marcarTodosPresente()">
                <mat-icon>check_circle</mat-icon>
                Todos Presente
              </button>
              <button mat-button (click)="marcarTodosAusente()">
                <mat-icon>cancel</mat-icon>
                Todos Ausente
              </button>
            </div>
          </mat-card-header>

          <mat-card-content>
            <table mat-table [dataSource]="alunos()" class="full-width">
              <ng-container matColumnDef="nome">
                <th mat-header-cell *matHeaderCellDef>Aluno</th>
                <td mat-cell *matCellDef="let aluno">{{ aluno.nome }}</td>
              </ng-container>

              <ng-container matColumnDef="status">
                <th mat-header-cell *matHeaderCellDef>Status</th>
                <td mat-cell *matCellDef="let aluno">
                  <mat-button-toggle-group
                    [(value)]="aluno.status"
                    appearance="legacy"
                  >
                    <mat-button-toggle value="Presente" class="status-presente">
                      <mat-icon>check</mat-icon>
                      Presente
                    </mat-button-toggle>
                    <mat-button-toggle value="Ausente" class="status-ausente">
                      <mat-icon>close</mat-icon>
                      Ausente
                    </mat-button-toggle>
                    <mat-button-toggle
                      value="Justificado"
                      class="status-justificado"
                    >
                      <mat-icon>description</mat-icon>
                      Justificado
                    </mat-button-toggle>
                  </mat-button-toggle-group>
                </td>
              </ng-container>

              <ng-container matColumnDef="observacao">
                <th mat-header-cell *matHeaderCellDef>Observação</th>
                <td mat-cell *matCellDef="let aluno">
                  <mat-form-field appearance="outline" class="obs-field">
                    <input
                      matInput
                      [(ngModel)]="aluno.observacao"
                      placeholder="Opcional"
                    />
                  </mat-form-field>
                </td>
              </ng-container>

              <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
              <tr mat-row *matRowDef="let row; columns: displayedColumns"></tr>
            </table>

            <div class="resumo-presenca">
              <mat-chip-set>
                <mat-chip color="primary">
                  <mat-icon>check</mat-icon>
                  Presentes: {{ countPresentes() }}
                </mat-chip>
                <mat-chip color="warn">
                  <mat-icon>close</mat-icon>
                  Ausentes: {{ countAusentes() }}
                </mat-chip>
                <mat-chip color="accent">
                  <mat-icon>description</mat-icon>
                  Justificados: {{ countJustificados() }}
                </mat-chip>
                <mat-chip>
                  <mat-icon>groups</mat-icon>
                  Total: {{ alunos().length }}
                </mat-chip>
              </mat-chip-set>
            </div>
          </mat-card-content>
        </mat-card>
        }
      </mat-card-content>

      <mat-card-actions align="end">
        <button mat-button (click)="cancelar()">Cancelar</button>
        <button
          mat-raised-button
          color="primary"
          [disabled]="aulaForm.invalid || alunos().length === 0"
          (click)="salvarPresencas()"
        >
          <mat-icon>save</mat-icon>
          Salvar Presenças
        </button>
      </mat-card-actions>
    </mat-card>
  `,
  styles: [
    `
      .form-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
        gap: 16px;
        margin-bottom: 24px;
      }

      .full-width {
        grid-column: 1 / -1;
        width: 100%;
      }

      .lista-presenca {
        margin-top: 24px;

        mat-card-header {
          display: flex;
          justify-content: space-between;
          align-items: center;
          margin-bottom: 16px;
        }
      }

      .acoes-rapidas {
        display: flex;
        gap: 8px;
      }

      table {
        width: 100%;
      }

      .status-presente {
        &.mat-button-toggle-checked {
          background-color: #4caf50;
          color: white;
        }
      }

      .status-ausente {
        &.mat-button-toggle-checked {
          background-color: #f44336;
          color: white;
        }
      }

      .status-justificado {
        &.mat-button-toggle-checked {
          background-color: #ff9800;
          color: white;
        }
      }

      .obs-field {
        width: 100%;

        ::ng-deep .mat-form-field-wrapper {
          padding-bottom: 0;
        }
      }

      .resumo-presenca {
        margin-top: 24px;
        padding: 16px;
        background: #f5f5f5;
        border-radius: 4px;

        mat-chip {
          mat-icon {
            margin-right: 4px;
          }
        }
      }
    `,
  ],
})
export class LancarPresencaPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly snackBar = inject(MatSnackBar);
  private readonly presencasService = inject(PresencasService);
  private readonly turmasService = inject(TurmasService);

  readonly turmas = signal<any[]>([]);
  readonly alunos = signal<AlunoPresenca[]>([]);
  readonly displayedColumns = ['nome', 'status', 'observacao'];

  readonly aulaForm = this.fb.nonNullable.group({
    turmaId: ['', Validators.required],
    data: [new Date(), Validators.required],
    horario: ['', Validators.required],
    conteudo: [''],
  });

  ngOnInit(): void {
    this.loadTurmas();
  }

  loadTurmas(): void {
    // Carregar apenas turmas do professor logado se for professor
    this.turmasService.getAll({ status: 'EmAndamento' }).subscribe((result) => {
      this.turmas.set(result.items || []);
    });
  }

  onTurmaChange(turmaId: string): void {
    if (turmaId) {
      this.loadAlunosTurma(turmaId);
    }
  }

  loadAlunosTurma(turmaId: string): void {
    this.turmasService.getAlunos(turmaId).subscribe((alunos) => {
      this.alunos.set(
        alunos.map((a) => ({
          alunoId: a.id,
          nome: a.nome,
          status: 'Presente' as StatusPresenca,
          observacao: '',
        }))
      );
    });
  }

  marcarTodosPresente(): void {
    this.alunos.update((alunos) =>
      alunos.map((a) => ({ ...a, status: 'Presente' as StatusPresenca }))
    );
  }

  marcarTodosAusente(): void {
    this.alunos.update((alunos) =>
      alunos.map((a) => ({ ...a, status: 'Ausente' as StatusPresenca }))
    );
  }

  countPresentes(): number {
    return this.alunos().filter((a) => a.status === 'Presente').length;
  }

  countAusentes(): number {
    return this.alunos().filter((a) => a.status === 'Ausente').length;
  }

  countJustificados(): number {
    return this.alunos().filter((a) => a.status === 'Justificado').length;
  }

  salvarPresencas(): void {
    if (this.aulaForm.invalid) return;

    const formValue = this.aulaForm.getRawValue();
    const [horaInicio] = formValue.horario.split(':');
    const horaFim = `${parseInt(horaInicio) + 1}:00`; // 50 minutos de aula

    // Primeiro criar a aula
    const aula = {
      turmaId: formValue.turmaId,
      data: formValue.data.toISOString(),
      horaInicio: formValue.horario,
      horaFim: horaFim,
      conteudo: formValue.conteudo || undefined,
    };

    this.presencasService.createAula(aula).subscribe({
      next: (aulaCriada) => {
        // Depois registrar as presenças
        const request: CreatePresencaRequest = {
          aulaId: aulaCriada.id,
          presencas: this.alunos().map((a) => ({
            alunoId: a.alunoId,
            status: a.status,
            observacao: a.observacao || undefined,
          })),
        };

        this.presencasService.registrarPresencas(request).subscribe({
          next: () => {
            this.snackBar.open('Presenças registradas com sucesso!', 'Fechar', {
              duration: 3000,
            });
            this.router.navigate(['/presencas']);
          },
          error: () => {
            this.snackBar.open('Erro ao registrar presenças', 'Fechar', {
              duration: 5000,
            });
          },
        });
      },
      error: () => {
        this.snackBar.open('Erro ao criar aula', 'Fechar', { duration: 5000 });
      },
    });
  }

  cancelar(): void {
    this.router.navigate(['/presencas']);
  }
}
