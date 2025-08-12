import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';

import { AuthService } from '../../../../core/auth/services/auth.service';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './login.page.html',
  styleUrls: ['./login.page.scss'],
})
export class LoginPage {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly snackBar = inject(MatSnackBar);

  readonly carregando = signal(false);
  readonly mostrarSenha = signal(false);
  readonly anoAtual = new Date().getFullYear();

  readonly loginForm = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    senha: ['', Validators.required],
  });

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.carregando.set(true);
    const credenciais = this.loginForm.getRawValue();

    this.authService.login(credenciais).subscribe({
      next: () => {
        this.carregando.set(false);
        this.snackBar.open('Login realizado com sucesso!', 'Fechar', {
          duration: 3000,
        });
      },
      error: (error: HttpErrorResponse) => {
        this.carregando.set(false);
        const mensagem =
          error.status === 401
            ? 'E-mail ou senha inválidos'
            : 'Erro ao realizar login. Tente novamente.';
        this.snackBar.open(mensagem, 'Fechar', { duration: 5000 });
      },
    });
  }
}
