import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { PageHeaderComponent } from '../../../../shared/ui/components/page-header/page-header.component';
import { ActivatedRoute } from '@angular/router';
import { PessoasStore } from '../../data-access/pessoas.store';

@Component({
  selector: 'app-pessoa-vinculos',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    MatButtonModule,
    MatListModule,
    MatIconModule,
    PageHeaderComponent,
  ],
  providers: [PessoasStore],
  template: `
    <app-page-header title="Vínculos do Responsável" subtitle="Vincule e desvincule alunos" />

    <mat-card>
      <mat-card-content>
        <div class="actions">
          <mat-form-field>
            <mat-label>Buscar aluno</mat-label>
            <input matInput [(ngModel)]="busca" placeholder="Nome do aluno" />
          </mat-form-field>
          <button mat-raised-button color="primary" (click)="buscarAlunos()">
            <mat-icon>search</mat-icon>
            Buscar
          </button>
        </div>

        <div class="lists">
          <div class="box">
            <h3>Alunos disponíveis</h3>
            <mat-nav-list>
              @for (aluno of alunosDisponiveis(); track aluno.id) {
              <a mat-list-item (click)="vincular(aluno.id)">
                <mat-icon matListItemIcon>person_add</mat-icon>
                <span matListItemTitle>{{ aluno.nome }}</span>
              </a>
              }
            </mat-nav-list>
          </div>

          <div class="box" *ngIf="alunosVinculados | async as vinculados">
            <h3>Alunos vinculados</h3>
            <mat-nav-list>
              <a
                mat-list-item
                *ngFor="let aluno of vinculados; trackBy: trackVinculo"
                (click)="desvincular(aluno.alunoId)"
              >
                <mat-icon matListItemIcon color="warn">link_off</mat-icon>
                <span matListItemTitle>{{ aluno.aluno?.nome || aluno.alunoId }}</span>
              </a>
            </mat-nav-list>
          </div>
        </div>
      </mat-card-content>
    </mat-card>
  `,
  styles: [
    `
      .actions {
        display: flex;
        gap: 12px;
        align-items: center;
        margin-bottom: 16px;
      }
      .lists {
        display: grid;
        grid-template-columns: 1fr 1fr;
        gap: 16px;
      }
      .box {
        border: 1px solid #eee;
        border-radius: 8px;
        padding: 8px 0;
      }
      h3 {
        margin: 0 16px 8px;
        font-size: 14px;
        color: #555;
      }
    `,
  ],
})
export class PessoaVinculosPage implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly store = inject(PessoasStore);

  readonly responsavelId = signal<string>('');
  readonly busca = '';
  readonly alunosDisponiveis = signal<any[]>([]);
  readonly alunosVinculados = this.store.vinculos$;

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id') || '';
    this.responsavelId.set(id);
    this.store.loadVinculos(id);
  }

  buscarAlunos(): void {
    // reutilizar store de pessoas
    this.store.updateFilter({ nome: this.busca, tipo: 'Aluno', page: 0 });
    this.store.loadPessoas({ nome: this.busca, tipo: 'Aluno', page: 0, pageSize: 10 });
    this.store.alunos$.subscribe(a => this.alunosDisponiveis.set(a));
  }

  vincular(alunoId: string): void {
    this.store.vincularAlunos({ responsavelId: this.responsavelId(), alunoIds: [alunoId] });
  }

  desvincular(alunoId: string): void {
    this.store.desvincularAluno({ responsavelId: this.responsavelId(), alunoId });
  }

  trackVinculo = (_: number, item: any) => item.alunoId;
}
