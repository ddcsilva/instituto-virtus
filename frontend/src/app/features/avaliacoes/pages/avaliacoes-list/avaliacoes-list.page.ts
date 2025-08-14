import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router, ActivatedRoute } from '@angular/router';
import { PageHeaderComponent } from '../../../../shared/ui/components/page-header/page-header.component';
import { AvaliacoesService } from '../../data-access/avaliacoes.service';
import { TurmasService } from '../../../turmas/data-access/turmas.service';
import { Avaliacao } from '../../models/avaliacao.model';

@Component({
  selector: 'app-avaliacoes-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    PageHeaderComponent,
  ],
  template: `
    <app-page-header title="Avaliações" subtitle="Gerencie as avaliações das turmas" />

    <mat-card>
      <mat-card-content>
        <form [formGroup]="filterForm" class="filters">
          <mat-form-field>
            <mat-label>Turma</mat-label>
            <mat-select formControlName="turmaId" (selectionChange)="onTurmaChange($event.value)">
              @for (turma of turmas(); track turma.id) {
              <mat-option [value]="turma.id">{{ turma.nome }} - {{ turma.curso?.nome }}</mat-option>
              }
            </mat-select>
          </mat-form-field>

          <span class="spacer"></span>

          <button mat-raised-button color="primary" (click)="novaAvaliacao()">
            <mat-icon>add</mat-icon>
            Nova avaliação
          </button>
        </form>

        <table mat-table [dataSource]="avaliacoes()" class="full-width">
          <ng-container matColumnDef="nome">
            <th mat-header-cell *matHeaderCellDef>Nome</th>
            <td mat-cell *matCellDef="let a">{{ a.nome }}</td>
          </ng-container>

          <ng-container matColumnDef="data">
            <th mat-header-cell *matHeaderCellDef>Data</th>
            <td mat-cell *matCellDef="let a">{{ a.dataAplicacao | date : 'dd/MM/yyyy' }}</td>
          </ng-container>

          <ng-container matColumnDef="peso">
            <th mat-header-cell *matHeaderCellDef>Peso</th>
            <td mat-cell *matCellDef="let a">{{ a.peso }}</td>
          </ng-container>

          <ng-container matColumnDef="acoes">
            <th mat-header-cell *matHeaderCellDef class="col-acoes">Ações</th>
            <td mat-cell *matCellDef="let a" class="col-acoes">
              <button mat-button color="primary" (click)="lancarNotas(a)">
                <mat-icon>assignment</mat-icon>
                Lançar notas
              </button>
            </td>
          </ng-container>

          <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
          <tr mat-row *matRowDef="let row; columns: displayedColumns"></tr>
        </table>
      </mat-card-content>
    </mat-card>
  `,
  styles: [
    `
      .filters {
        display: flex;
        align-items: center;
        gap: 12px;
        margin-bottom: 12px;
      }
      .spacer {
        flex: 1 1 auto;
      }
      .col-acoes {
        width: 200px;
        text-align: right;
      }
      table {
        width: 100%;
      }
    `,
  ],
})
export class AvaliacoesListPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly snackBar = inject(MatSnackBar);
  private readonly avaliacoesService = inject(AvaliacoesService);
  private readonly turmasService = inject(TurmasService);

  readonly turmas = signal<any[]>([]);
  readonly avaliacoes = signal<Avaliacao[]>([]);
  readonly displayedColumns = ['nome', 'data', 'peso', 'acoes'];

  readonly filterForm = this.fb.nonNullable.group({
    turmaId: [''],
  });

  ngOnInit(): void {
    this.loadTurmas();
    const turmaId = this.route.snapshot.queryParamMap.get('turmaId');
    if (turmaId) {
      this.filterForm.patchValue({ turmaId });
      this.onTurmaChange(turmaId);
    }
  }

  loadTurmas(): void {
    this.turmasService.getAll({}).subscribe({
      next: (result: any) => {
        const list = Array.isArray(result) ? result : result.items || result.Items || [];
        const mapped = list.map((t: any) => ({
          id: t.Id ?? t.id,
          nome: t.Nome ?? t.nome ?? t.CursoNome ?? t.cursoNome ?? 'Turma',
          curso: { id: t.CursoId ?? t.cursoId, nome: t.CursoNome ?? t.cursoNome ?? '' },
        }));
        this.turmas.set(mapped);
      },
      error: () => this.turmas.set([]),
    });
  }

  onTurmaChange(turmaId: string): void {
    if (!turmaId) {
      this.avaliacoes.set([]);
      return;
    }
    this.avaliacoesService.getByTurma(turmaId).subscribe({
      next: list => {
        const mapped = (list || []).map((a: any) => ({
          id: a.Id ?? a.id,
          turmaId: a.TurmaId ?? a.turmaId,
          turmaNome: a.TurmaNome ?? a.turmaNome ?? '',
          nome: a.Nome ?? a.nome,
          peso: a.Peso ?? a.peso,
          dataAplicacao: a.DataAplicacao ?? a.dataAplicacao,
          descricao: a.Descricao ?? a.descricao,
        }));
        this.avaliacoes.set(mapped);
      },
      error: () => this.snackBar.open('Erro ao carregar avaliações', 'Fechar', { duration: 3000 }),
    });
  }

  novaAvaliacao(): void {
    const turmaId = this.filterForm.get('turmaId')?.value as string;
    if (!turmaId) {
      this.snackBar.open('Selecione uma turma', 'Fechar', { duration: 3000 });
      return;
    }
    this.router.navigate([`/avaliacoes/configurar/${turmaId}`]);
  }

  lancarNotas(a: Avaliacao): void {
    this.router.navigate([`/avaliacoes/lancar/${a.id}`]);
  }
}
