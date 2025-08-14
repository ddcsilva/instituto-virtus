import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { PageHeaderComponent } from '../../../../shared/ui/components/page-header/page-header.component';
import { AvaliacoesService } from '../../data-access/avaliacoes.service';

@Component({
  selector: 'app-configurar-avaliacao',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule,
    PageHeaderComponent,
  ],
  template: `
    <app-page-header title="Nova Avaliação" subtitle="Defina os dados da avaliação" />

    <mat-card>
      <mat-card-content>
        <form [formGroup]="form" class="form-grid">
          <mat-form-field>
            <mat-label>Nome</mat-label>
            <input matInput formControlName="nome" />
            @if (form.get('nome')?.hasError('required')) {<mat-error>Informe o nome</mat-error>}
          </mat-form-field>

          <mat-form-field>
            <mat-label>Peso</mat-label>
            <input matInput type="number" formControlName="peso" min="1" max="3" step="0.5" />
          </mat-form-field>

          <mat-form-field>
            <mat-label>Data de aplicação (opcional)</mat-label>
            <input matInput [matDatepicker]="picker" formControlName="dataAplicacao" />
            <mat-datepicker-toggle matIconSuffix [for]="picker" />
            <mat-datepicker #picker />
          </mat-form-field>

          <mat-form-field class="full-width">
            <mat-label>Descrição (opcional)</mat-label>
            <textarea matInput rows="3" formControlName="descricao"></textarea>
          </mat-form-field>
        </form>
      </mat-card-content>

      <mat-card-actions align="end">
        <button mat-button (click)="cancelar()">Cancelar</button>
        <button mat-raised-button color="primary" [disabled]="form.invalid" (click)="salvar()">
          Salvar
        </button>
      </mat-card-actions>
    </mat-card>
  `,
  styles: [
    `
      .form-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
        gap: 16px;
      }
      .full-width {
        grid-column: 1 / -1;
      }
    `,
  ],
})
export class ConfigurarAvaliacaoPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly snackBar = inject(MatSnackBar);
  private readonly avaliacoesService = inject(AvaliacoesService);

  turmaId = '';
  form = this.fb.nonNullable.group({
    nome: ['', Validators.required],
    peso: [1, Validators.required],
    dataAplicacao: [null as Date | null],
    descricao: [''],
  });

  ngOnInit(): void {
    this.turmaId = this.route.snapshot.paramMap.get('turmaId') || '';
  }

  salvar(): void {
    if (!this.turmaId || this.form.invalid) return;
    const v = this.form.getRawValue();
    this.avaliacoesService
      .create({
        turmaId: this.turmaId,
        nome: v.nome,
        peso: Number(v.peso),
        dataAplicacao: v.dataAplicacao ? v.dataAplicacao.toISOString() : undefined,
        descricao: v.descricao || undefined,
      })
      .subscribe({
        next: () => {
          this.snackBar.open('Avaliação criada', 'Fechar', { duration: 3000 });
          this.router.navigate(['/avaliacoes'], { queryParams: { turmaId: this.turmaId } });
        },
        error: () => this.snackBar.open('Erro ao criar avaliação', 'Fechar', { duration: 4000 }),
      });
  }

  cancelar(): void {
    this.router.navigate(['/avaliacoes']);
  }
}
