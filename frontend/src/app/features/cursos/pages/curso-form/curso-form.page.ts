import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import {
  ReactiveFormsModule,
  FormBuilder,
  Validators,
  AbstractControl,
  ValidationErrors,
} from '@angular/forms';
import { map } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { PageHeaderComponent } from '../../../../shared/ui/components/page-header/page-header.component';
import { CursosService } from '../../data-access/cursos.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CreateCursoRequest, UpdateCursoRequest } from '../../models/curso.model';

@Component({
  selector: 'app-curso-form',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    ReactiveFormsModule,
    PageHeaderComponent,
  ],
  template: `
    <app-page-header title="Formulário de Curso" subtitle="Cadastre ou edite um curso" />

    <mat-card>
      <mat-card-content>
        <form [formGroup]="form" (ngSubmit)="onSubmit()" class="form-grid">
          <mat-form-field>
            <mat-label>Nome</mat-label>
            <input matInput formControlName="nome" />
          </mat-form-field>

          <mat-form-field>
            <mat-label>Descrição</mat-label>
            <textarea matInput formControlName="descricao" rows="3"></textarea>
          </mat-form-field>

          <mat-form-field>
            <mat-label>Carga Horária (h)</mat-label>
            <input matInput type="number" formControlName="cargaHoraria" />
          </mat-form-field>

          <mat-form-field>
            <mat-label>Valor Mensalidade (R$)</mat-label>
            <input matInput type="number" step="0.01" formControlName="valorMensalidade" />
          </mat-form-field>

          <div class="actions">
            <button mat-raised-button color="primary" type="submit" [disabled]="form.invalid">
              Salvar
            </button>
            <button mat-button type="button" (click)="voltar()">Cancelar</button>
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
        grid-template-columns: 1fr 1fr;
      }
      .actions {
        grid-column: 1 / -1;
        display: flex;
        gap: 12px;
        justify-content: flex-end;
      }
      @media (max-width: 900px) {
        .form-grid {
          grid-template-columns: 1fr;
        }
      }
    `,
  ],
})
export class CursoFormPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly service = inject(CursosService);
  private readonly snackBar = inject(MatSnackBar);

  readonly form = this.fb.nonNullable.group({
    nome: ['', [Validators.required], [this.nomeUnicoValidator.bind(this)]],
    descricao: [''],
    cargaHoraria: [0, [Validators.required, Validators.min(0)]],
    valorMensalidade: [0, [Validators.required, Validators.min(0)]],
  });

  private id: string | null = null;
  private originalNome: string | null = null;

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id');
    if (this.id) {
      this.service.getById(this.id).subscribe(c => {
        this.originalNome = c.nome;
        this.form.patchValue(c as any);
      });
    }
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    const value = this.form.getRawValue();
    if (this.id) {
      const payload: UpdateCursoRequest = { ...value, ativo: true };
      this.service.update(this.id, payload).subscribe(() => {
        this.snackBar.open('Curso atualizado com sucesso', 'Fechar', { duration: 3000 });
        this.voltar();
      });
    } else {
      const payload: CreateCursoRequest = value;
      this.service.create(payload).subscribe(() => {
        this.snackBar.open('Curso criado com sucesso', 'Fechar', { duration: 3000 });
        this.voltar();
      });
    }
  }

  private nomeUnicoValidator(
    control: AbstractControl
  ): Promise<ValidationErrors | null> | import('rxjs').Observable<ValidationErrors | null> {
    const nome = (control?.value || '').toString().trim();
    if (!nome) return Promise.resolve(null);
    if (this.id && this.originalNome && nome.toLowerCase() === this.originalNome.toLowerCase()) {
      return Promise.resolve(null);
    }
    return this.service
      .existsByNome(nome)
      .pipe(map(exists => (exists ? { nomeJaExiste: true } : null)));
  }

  voltar(): void {
    this.router.navigate(['/cursos']);
  }
}
