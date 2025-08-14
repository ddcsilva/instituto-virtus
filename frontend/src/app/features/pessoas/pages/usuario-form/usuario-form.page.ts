import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { API_CONFIG } from '../../../../core/config/api.config';
import { PageHeaderComponent } from '../../../../shared/ui/components/page-header/page-header.component';

@Component({
  selector: 'app-usuario-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    PageHeaderComponent,
  ],
  template: `
    <app-page-header title="Novo Usuário" subtitle="Cadastro realizado pela Coordenação" />

    <mat-card>
      <mat-card-content>
        <form [formGroup]="form" class="form-grid">
          <mat-form-field class="full">
            <mat-label>Nome completo</mat-label>
            <input matInput formControlName="nomeCompleto" />
            @if (form.get('nomeCompleto')?.hasError('required')) {<mat-error
              >Informe o nome</mat-error
            >}
          </mat-form-field>

          <mat-form-field>
            <mat-label>Email</mat-label>
            <input matInput type="email" formControlName="email" />
          </mat-form-field>

          <mat-form-field>
            <mat-label>Telefone</mat-label>
            <input matInput formControlName="telefone" />
          </mat-form-field>

          <mat-form-field>
            <mat-label>CPF (opcional)</mat-label>
            <input matInput formControlName="cpf" />
          </mat-form-field>

          <mat-form-field>
            <mat-label>Data de nascimento</mat-label>
            <input matInput type="date" formControlName="dataNascimento" />
          </mat-form-field>

          <mat-form-field>
            <mat-label>Tipo de usuário</mat-label>
            <mat-select formControlName="tipoPessoa">
              <mat-option value="Responsavel">Responsável</mat-option>
              <mat-option value="Aluno">Aluno</mat-option>
              <mat-option value="Professor">Professor</mat-option>
              <mat-option value="Coordenador">Coordenador</mat-option>
            </mat-select>
          </mat-form-field>

          <mat-form-field>
            <mat-label>Senha</mat-label>
            <input matInput type="password" formControlName="senha" />
            @if (form.get('senha')?.hasError('minlength')) {<mat-error
              >Mínimo 6 caracteres</mat-error
            >}
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
        grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
        gap: 16px;
      }
      .full {
        grid-column: 1 / -1;
      }
    `,
  ],
})
export class UsuarioFormPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly http = inject(HttpClient);
  private readonly apiConfig = inject(API_CONFIG);
  private readonly router = inject(Router);
  private readonly snackBar = inject(MatSnackBar);

  form = this.fb.nonNullable.group({
    nomeCompleto: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    telefone: ['', Validators.required],
    cpf: [''],
    dataNascimento: ['', Validators.required],
    tipoPessoa: ['Responsavel', Validators.required],
    senha: ['', [Validators.required, Validators.minLength(6)]],
  });

  ngOnInit(): void {}

  salvar(): void {
    if (this.form.invalid) return;
    const v = this.form.getRawValue();
    const payload = {
      NomeCompleto: v.nomeCompleto,
      Cpf: v.cpf || undefined,
      Email: v.email,
      Senha: v.senha,
      Telefone: v.telefone.replace(/\D/g, ''),
      DataNascimento: new Date(v.dataNascimento).toISOString(),
      TipoPessoa: v.tipoPessoa,
    } as any;

    this.http.post(`${this.apiConfig.baseUrl}/auth/register`, payload).subscribe({
      next: () => {
        this.snackBar.open('Usuário criado com sucesso', 'Fechar', { duration: 3000 });
        this.router.navigate(['/pessoas']);
      },
      error: err => {
        const msg = err?.error?.message || 'Erro ao criar usuário';
        this.snackBar.open(msg, 'Fechar', { duration: 4000 });
      },
    });
  }

  cancelar(): void {
    this.router.navigate(['/pessoas']);
  }
}
