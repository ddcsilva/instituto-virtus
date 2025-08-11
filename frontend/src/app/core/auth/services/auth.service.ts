import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap, catchError, of } from 'rxjs';
import { toObservable } from '@angular/core/rxjs-interop';
import { API_CONFIG } from '../../config/api.config';
import { LoginRequest, LoginResponse, Session, User } from '../models/auth.models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly apiConfig = inject(API_CONFIG);

  private readonly sessionSignal = signal<Session>({
    user: null,
    token: null,
    refreshToken: null,
    isAuthenticated: false,
  });

  readonly session = this.sessionSignal.asReadonly();
  readonly session$ = toObservable(this.session);
  readonly estaAutenticado = signal(false);
  readonly usuarioAtual = signal<User | null>(null);

  constructor() {
    this.carregarSessaoDoStorage();
  }

  login(credenciais: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiConfig.baseUrl}/auth/login`, credenciais).pipe(
      tap(response => {
        this.definirSessao(response);
        this.router.navigate(['/']);
      }),
      catchError(error => {
        console.error('Erro ao realizar login:', error);
        throw error;
      })
    );
  }

  logout(): void {
    this.limparSessao();
    this.router.navigate(['/login']);
  }

  atualizarToken(): Observable<LoginResponse> {
    const refreshToken = this.session().refreshToken;
    if (!refreshToken) {
      this.logout();
      return of();
    }

    return this.http
      .post<LoginResponse>(`${this.apiConfig.baseUrl}/auth/refresh`, {
        refreshToken,
      })
      .pipe(
        tap(response => this.definirSessao(response)),
        catchError(() => {
          this.logout();
          return of();
        })
      );
  }

  possuiPermissao(permissoes: string[]): boolean {
    const usuario = this.usuarioAtual();
    return usuario ? permissoes.includes(usuario.tipo) : false;
  }

  private definirSessao(response: LoginResponse): void {
    const session: Session = {
      user: response.user,
      token: response.token,
      refreshToken: response.refreshToken,
      isAuthenticated: true,
    };

    this.sessionSignal.set(session);
    this.estaAutenticado.set(true);
    this.usuarioAtual.set(response.user);

    localStorage.setItem('token', response.token);
    localStorage.setItem('refreshToken', response.refreshToken);
    localStorage.setItem('user', JSON.stringify(response.user));
  }

  private carregarSessaoDoStorage(): void {
    const token = localStorage.getItem('token');
    const refreshToken = localStorage.getItem('refreshToken');
    const usuarioString = localStorage.getItem('user');

    if (token && usuarioString) {
      try {
        const usuario = JSON.parse(usuarioString) as User;
        this.sessionSignal.set({
          user: usuario,
          token,
          refreshToken,
          isAuthenticated: true,
        });
        this.estaAutenticado.set(true);
        this.usuarioAtual.set(usuario);
      } catch {
        this.limparSessao();
      }
    }
  }

  private limparSessao(): void {
    this.sessionSignal.set({
      user: null,
      token: null,
      refreshToken: null,
      isAuthenticated: false,
    });
    this.estaAutenticado.set(false);
    this.usuarioAtual.set(null);

    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
  }
}
