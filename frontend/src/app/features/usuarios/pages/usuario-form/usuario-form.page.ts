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
import { API_CONFIG, ApiConfig } from '../../../../core/config/api.config';
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
            <mat-label>Tipo de usuário</mat-label>
            <mat-select formControlName="tipoPessoa">
              <mat-option value="Coordenacao">Coordenação</mat-option>
              <mat-option value="Professor">Professor</mat-option>
              <mat-option value="Responsavel">Responsável</mat-option>
            </mat-select>
          </mat-form-field>

          <mat-form-field class="full">
            <mat-label>Senha provisória</mat-label>
            <input matInput type="password" formControlName="senha" />
            @if (form.get('senha')?.hasError('required')) {<mat-error
              >Informe uma senha</mat-error
            >}
          </mat-form-field>
        </form>
      </mat-card-content>

      <mat-card-actions>
        <button mat-button type="button" (click)="voltar()">Cancelar</button>
        <button mat-raised-button color="primary" (click)="salvar()" [disabled]="form.invalid">
          Cadastrar Usuário
        </button>
      </mat-card-actions>
    </mat-card>
  `,
  styles: [`
    .form-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 16px;
      margin: 16px 0;
    }

    .full {
      grid-column: 1 / -1;
    }

    mat-card-actions {
      padding: 16px;
      display: flex;
      gap: 8px;
      justify-content: flex-end;
    }
  `]
})
export class UsuarioFormPage implements OnInit {
  private fb = inject(FormBuilder);
  private http = inject(HttpClient);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);
  private apiConfig = inject(API_CONFIG);

  loading = signal(false);

  form = this.fb.group({
    nomeCompleto: ['', [Validators.required, Validators.minLength(3)]],
    email: ['', [Validators.email]],
    telefone: ['', [Validators.required]],
    tipoPessoa: ['', [Validators.required]],
    senha: ['', [Validators.required, Validators.minLength(6)]]
  });

  ngOnInit() {
    // Initialize form if needed
  }

  async salvar() {
    if (this.form.invalid) return;

    this.loading.set(true);
    
    try {
      const formValue = this.form.value;
      
      await this.http.post(`${this.apiConfig.baseUrl}/pessoas`, {
        nomeCompleto: formValue.nomeCompleto,
        email: formValue.email,
        telefone: formValue.telefone,
        tipoPessoa: formValue.tipoPessoa,
        senha: formValue.senha,
        dataNascimento: new Date().toISOString() // Default value
      }).toPromise();

      this.snackBar.open('Usuário cadastrado com sucesso!', 'OK', { duration: 3000 });
      this.voltar();
    } catch (error) {
      this.snackBar.open('Erro ao cadastrar usuário', 'OK', { duration: 3000 });
      console.error('Erro:', error);
    } finally {
      this.loading.set(false);
    }
  }

  voltar() {
    this.router.navigate(['/usuarios']);
  }
}