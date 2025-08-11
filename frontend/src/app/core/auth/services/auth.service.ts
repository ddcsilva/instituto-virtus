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
    this.limparDadosCorrompdios();
    this.carregarSessaoDoStorage();
  }

  private limparDadosCorrompdios(): void {
    const usuarioString = localStorage.getItem('user');
    if (usuarioString === 'undefined' || usuarioString === 'null') {
      this.limparSessao();
    }
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
    // Normaliza a resposta para lidar com PascalCase do backend
    const normalizedResponse = {
      user: (response as any).User || response.user,
      token: (response as any).Token || response.token,
      refreshToken: (response as any).RefreshToken || response.refreshToken,
      expiresIn: (response as any).ExpiresIn || response.expiresIn,
    };

    // Validação dos dados da resposta
    if (!normalizedResponse.user || !normalizedResponse.token) {
      throw new Error('Dados de login inválidos');
    }

    const session: Session = {
      user: normalizedResponse.user,
      token: normalizedResponse.token,
      refreshToken: normalizedResponse.refreshToken,
      isAuthenticated: true,
    };

    this.sessionSignal.set(session);
    this.estaAutenticado.set(true);
    this.usuarioAtual.set(normalizedResponse.user);

    // Só salva no localStorage se os dados são válidos
    localStorage.setItem('token', normalizedResponse.token);
    if (normalizedResponse.refreshToken) {
      localStorage.setItem('refreshToken', normalizedResponse.refreshToken);
    }
    localStorage.setItem('user', JSON.stringify(normalizedResponse.user));
  }

  private carregarSessaoDoStorage(): void {
    const token = localStorage.getItem('token');
    const refreshToken = localStorage.getItem('refreshToken');
    const usuarioString = localStorage.getItem('user');

    if (token && usuarioString && usuarioString !== 'undefined') {
      try {
        const usuario = JSON.parse(usuarioString) as User;

        // Normaliza o usuário para lidar com PascalCase do backend
        const normalizedUser = {
          id: (usuario as any).Id || usuario.id,
          nome: (usuario as any).Nome || usuario.nome,
          email: (usuario as any).Email || usuario.email,
          tipo: (usuario as any).Tipo || usuario.tipo,
          pessoaId: (usuario as any).PessoaId || usuario.pessoaId,
          ativo: (usuario as any).Ativo !== undefined ? (usuario as any).Ativo : usuario.ativo,
        };

        // Validação adicional do objeto usuário
        if (!normalizedUser || !normalizedUser.nome || !normalizedUser.tipo) {
          this.limparSessao();
          return;
        }

        // Verifica se o token não está expirado (validação básica)
        const tokenValido = this.isTokenValid(token);

        if (tokenValido) {
          this.sessionSignal.set({
            user: normalizedUser,
            token,
            refreshToken,
            isAuthenticated: true,
          });
          this.estaAutenticado.set(true);
          this.usuarioAtual.set(normalizedUser);
        } else {
          this.limparSessao();
        }
      } catch (error) {
        this.limparSessao();
      }
    } else {
      // Se não há token ou usuário, garante que está limpo
      this.estaAutenticado.set(false);
      this.usuarioAtual.set(null);
    }
  }

  private isTokenValid(token: string): boolean {
    try {
      // Decodifica o payload do JWT sem verificar a assinatura
      const payload = JSON.parse(atob(token.split('.')[1]));
      const now = Math.floor(Date.now() / 1000);

      // Verifica se o token não expirou
      return payload.exp && payload.exp > now;
    } catch {
      return false;
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
