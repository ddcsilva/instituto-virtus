import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';

import { AuthService } from '../../../../core/auth/services/auth.service';

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
  template: `
    <div class="login-container">
      <mat-card class="login-card">
        <mat-card-header>
          <div class="logo-container">
            <mat-icon class="logo">school</mat-icon>
            <h1>Instituto Virtus</h1>
          </div>
        </mat-card-header>

        <mat-card-content>
          <form [formGroup]="loginForm" (ngSubmit)="onSubmit()">
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>E-mail</mat-label>
              <input
                matInput
                type="email"
                formControlName="email"
                placeholder="seu@email.com"
              />
              <mat-icon matSuffix>email</mat-icon>
              @if (loginForm.get('email')?.hasError('required') &&
              loginForm.get('email')?.touched) {
              <mat-error>E-mail é obrigatório</mat-error>
              } @if (loginForm.get('email')?.hasError('email') &&
              loginForm.get('email')?.touched) {
              <mat-error>E-mail inválido</mat-error>
              }
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Senha</mat-label>
              <input
                matInput
                [type]="showPassword() ? 'text' : 'password'"
                formControlName="senha"
              />
              <button
                mat-icon-button
                matSuffix
                type="button"
                (click)="showPassword.set(!showPassword())"
              >
                <mat-icon>{{
                  showPassword() ? 'visibility_off' : 'visibility'
                }}</mat-icon>
              </button>
              @if (loginForm.get('senha')?.hasError('required') &&
              loginForm.get('senha')?.touched) {
              <mat-error>Senha é obrigatória</mat-error>
              }
            </mat-form-field>

            <button
              mat-raised-button
              color="primary"
              type="submit"
              class="full-width login-button"
              [disabled]="loginForm.invalid || loading()"
            >
              @if (loading()) {
              <mat-spinner diameter="20"></mat-spinner>
              } @else { Entrar }
            </button>
          </form>
        </mat-card-content>

        <mat-card-footer>
          <p class="footer-text">
            © 2024 Instituto Virtus - Sistema de Gestão Acadêmica
          </p>
        </mat-card-footer>
      </mat-card>
    </div>
  `,
  styles: [
    `
      .login-container {
        height: 100vh;
        display: flex;
        align-items: center;
        justify-content: center;
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      }

      .login-card {
        width: 100%;
        max-width: 400px;
        padding: 24px;
      }

      .logo-container {
        display: flex;
        flex-direction: column;
        align-items: center;
        margin-bottom: 32px;
      }

      .logo {
        font-size: 64px;
        width: 64px;
        height: 64px;
        color: #667eea;
        margin-bottom: 16px;
      }

      h1 {
        margin: 0;
        font-size: 24px;
        font-weight: 500;
        color: #333;
      }

      .full-width {
        width: 100%;
      }

      mat-form-field {
        margin-bottom: 16px;
      }

      .login-button {
        height: 48px;
        font-size: 16px;
        margin-top: 16px;
      }

      .footer-text {
        text-align: center;
        color: #999;
        font-size: 12px;
        margin: 16px 0 0;
      }

      mat-spinner {
        margin: 0 auto;
      }
    `,
  ],
})
export class LoginPage {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);
  private readonly snackBar = inject(MatSnackBar);

  readonly loading = signal(false);
  readonly showPassword = signal(false);

  readonly loginForm = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    senha: ['', Validators.required],
  });

  onSubmit(): void {
    if (this.loginForm.invalid) return;

    this.loading.set(true);
    const credentials = this.loginForm.getRawValue();

    this.authService.login(credentials).subscribe({
      next: () => {
        this.snackBar.open('Login realizado com sucesso!', 'Fechar', {
          duration: 3000,
        });
        this.router.navigate(['/']);
      },
      error: (error) => {
        this.loading.set(false);
        const message =
          error.status === 401
            ? 'E-mail ou senha inválidos'
            : 'Erro ao realizar login. Tente novamente.';
        this.snackBar.open(message, 'Fechar', { duration: 5000 });
      },
    });
  }
}
