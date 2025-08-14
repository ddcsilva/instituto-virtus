import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { PageHeaderComponent } from '../../../../shared/ui/components/page-header/page-header.component';
import { AvaliacoesService } from '../../data-access/avaliacoes.service';
import { TurmasService } from '../../../turmas/data-access/turmas.service';

@Component({
  selector: 'app-lancar-notas',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatCardModule,
    MatTableModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    PageHeaderComponent,
  ],
  template: `
    <app-page-header title="Lançar Notas" subtitle="Registre as notas dos alunos" />

    <mat-card>
      <mat-card-content>
        <div class="info">
          <div><strong>Avaliação:</strong> {{ avaliacao()?.nome }}</div>
          <div><strong>Turma:</strong> {{ avaliacao()?.turmaNome }}</div>
          <div><strong>Peso:</strong> {{ avaliacao()?.peso }}</div>
          <div><strong>Data:</strong> {{ avaliacao()?.dataAplicacao | date : 'dd/MM/yyyy' }}</div>
        </div>

        <table mat-table [dataSource]="alunos()" class="full-width">
          <ng-container matColumnDef="aluno">
            <th mat-header-cell *matHeaderCellDef>Aluno</th>
            <td mat-cell *matCellDef="let a">{{ a.nome }}</td>
          </ng-container>

          <ng-container matColumnDef="nota">
            <th mat-header-cell *matHeaderCellDef>Nota (0-10)</th>
            <td mat-cell *matCellDef="let a">
              <input matInput type="number" min="0" max="10" step="0.1" [(ngModel)]="a.nota" />
            </td>
          </ng-container>

          <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
          <tr mat-row *matRowDef="let row; columns: displayedColumns"></tr>
        </table>
      </mat-card-content>

      <mat-card-actions align="end">
        <button mat-button (click)="preencherZero()">Preencher 0</button>
        <button mat-button (click)="limparNotas()">Limpar</button>
        <button mat-raised-button color="primary" (click)="salvar()">Salvar</button>
      </mat-card-actions>
    </mat-card>
  `,
  styles: [
    `
      .info {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
        gap: 8px;
        margin-bottom: 12px;
      }
      table {
        width: 100%;
      }
      td input {
        max-width: 100px;
      }
    `,
  ],
})
export class LancarNotasPage implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly snackBar = inject(MatSnackBar);
  private readonly avaliacoesService = inject(AvaliacoesService);
  private readonly turmasService = inject(TurmasService);

  avaliacaoId = '';
  readonly avaliacao = signal<any | null>(null);
  readonly alunos = signal<{ alunoId: string; nome: string; nota: number | null }[]>([]);
  readonly displayedColumns = ['aluno', 'nota'];

  ngOnInit(): void {
    this.avaliacaoId = this.route.snapshot.paramMap.get('avaliacaoId') || '';
    if (!this.avaliacaoId) {
      this.router.navigate(['/avaliacoes']);
      return;
    }

    // Carrega avaliação e alunos da turma + notas existentes
    this.avaliacoesService.getById(this.avaliacaoId).subscribe({
      next: (a: any) => {
        const mapped = {
          id: a.Id ?? a.id,
          turmaId: a.TurmaId ?? a.turmaId,
          turmaNome: a.TurmaNome ?? a.turmaNome ?? '',
          nome: a.Nome ?? a.nome,
          peso: a.Peso ?? a.peso,
          dataAplicacao: a.DataAplicacao ?? a.dataAplicacao,
          descricao: a.Descricao ?? a.descricao,
        };
        this.avaliacao.set(mapped);

        this.turmasService.getAlunos(mapped.turmaId).subscribe((list: any[]) => {
          const alunosBase = (list || []).map((al: any) => ({
            alunoId: al.Id ?? al.id,
            nome: al.Nome ?? al.nome,
            nota: null as number | null,
          }));

          this.avaliacoesService.getNotasByAvaliacao(this.avaliacaoId).subscribe((notas: any[]) => {
            const mapNotas = new Map<string, any>();
            (notas || []).forEach(n => mapNotas.set((n.AlunoId ?? n.alunoId) as string, n));
            const merged = alunosBase.map(a => {
              const n = mapNotas.get(a.alunoId);
              return n ? { ...a, nota: Number(n.Valor ?? n.valor) } : a;
            });
            this.alunos.set(merged);
          });
        });
      },
      error: () => {
        this.snackBar.open('Avaliação não encontrada', 'Fechar', { duration: 3000 });
        this.router.navigate(['/avaliacoes']);
      },
    });
  }

  preencherZero(): void {
    this.alunos.update(list => list.map(a => ({ ...a, nota: 0 })));
  }

  limparNotas(): void {
    this.alunos.update(list => list.map(a => ({ ...a, nota: null })));
  }

  salvar(): void {
    const notasValidas = this.alunos()
      .filter(a => a.nota !== null && a.nota !== undefined)
      .map(a => ({ alunoId: a.alunoId, valor: Number(a.nota) }));

    this.avaliacoesService.lancarNotas(this.avaliacaoId, { notas: notasValidas }).subscribe({
      next: () => {
        this.snackBar.open('Notas lançadas com sucesso', 'Fechar', { duration: 3000 });
        this.router.navigate(['/avaliacoes']);
      },
      error: () => this.snackBar.open('Erro ao lançar notas', 'Fechar', { duration: 4000 }),
    });
  }
}
