import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormControl } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { PageHeaderComponent } from '../../../../shared/ui/components/page-header/page-header.component';
import { MatDividerModule } from '@angular/material/divider';
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
    ReactiveFormsModule,
    MatButtonModule,
    MatListModule,
    MatIconModule,
    MatCheckboxModule,
    MatSelectModule,
    MatSlideToggleModule,
    MatDividerModule,
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
              <a mat-list-item>
                <mat-checkbox
                  matListItemMeta
                  [checked]="selecionados().has(aluno.id)"
                  (change)="toggleSelecionado(aluno.id, $event.checked)"
                ></mat-checkbox>
                <mat-icon matListItemIcon>person</mat-icon>
                <span matListItemTitle>{{ aluno.nome }}</span>
              </a>
              }
            </mat-nav-list>
            <mat-divider></mat-divider>
            <div class="inline-form">
              <mat-form-field>
                <mat-label>Parentesco</mat-label>
                <mat-select [formControl]="parentescoControl">
                  <mat-option value="Pai">Pai</mat-option>
                  <mat-option value="Mae">Mãe</mat-option>
                  <mat-option value="Responsavel">Responsável</mat-option>
                  <mat-option value="Outro">Outro</mat-option>
                </mat-select>
              </mat-form-field>
              <mat-slide-toggle [formControl]="principalControl">Principal</mat-slide-toggle>
              <button
                mat-raised-button
                color="primary"
                (click)="vincularSelecionados()"
                [disabled]="selecionados().size === 0"
              >
                <mat-icon>link</mat-icon>
                Vincular selecionados
              </button>
            </div>
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
      .inline-form {
        display: grid;
        grid-template-columns: minmax(220px, 300px) auto 1fr;
        align-items: center;
        gap: 16px;
        padding: 8px 16px 0;
      }
      .inline-form mat-form-field {
        width: 100%;
        max-width: 300px;
      }
      .inline-form button {
        justify-self: end;
      }
      @media (max-width: 768px) {
        .lists {
          grid-template-columns: 1fr;
        }
        .inline-form {
          grid-template-columns: 1fr;
          gap: 12px;
        }
        .inline-form button {
          justify-self: stretch;
        }
      }
    `,
  ],
})
export class PessoaVinculosPage implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly store = inject(PessoasStore);
  private readonly fb = inject(FormBuilder);

  readonly responsavelId = signal<string>('');
  readonly busca = '';
  readonly alunosDisponiveis = signal<any[]>([]);
  readonly alunosVinculados = this.store.vinculos$;
  readonly selecionados = signal<Set<string>>(new Set<string>());
  readonly parentescoControl = new FormControl('Responsavel');
  readonly principalControl = new FormControl(false);

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

  toggleSelecionado(id: string, checked: boolean): void {
    const set = new Set(this.selecionados());
    if (checked) set.add(id);
    else set.delete(id);
    this.selecionados.set(set);
  }

  vincularSelecionados(): void {
    const ids = Array.from(this.selecionados());
    if (ids.length === 0) return;
    this.store.vincularAlunos({
      responsavelId: this.responsavelId(),
      alunoIds: ids,
      parentesco: this.parentescoControl.value || 'Responsavel',
      principal: !!this.principalControl.value,
    });
    this.selecionados.set(new Set<string>());
  }
}
