import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap, catchError, of } from 'rxjs';
import { toObservable } from '@angular/core/rxjs-interop';
import { API_CONFIG } from '../../config/api.config';
import {
  LoginRequest,
  LoginResponse,
  Session,
  User,
} from '../models/auth.models';

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
  readonly isAuthenticated = signal(false);
  readonly currentUser = signal<User | null>(null);

  constructor() {
    this.loadSessionFromStorage();
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${this.apiConfig.baseUrl}/auth/login`, credentials)
      .pipe(
        tap((response) => {
          this.setSession(response);
          this.router.navigate(['/']);
        }),
        catchError((error) => {
          console.error('Login error:', error);
          throw error;
        })
      );
  }

  logout(): void {
    this.clearSession();
    this.router.navigate(['/login']);
  }

  refreshToken(): Observable<LoginResponse> {
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
        tap((response) => this.setSession(response)),
        catchError(() => {
          this.logout();
          return of();
        })
      );
  }

  hasRole(roles: string[]): boolean {
    const user = this.currentUser();
    return user ? roles.includes(user.tipo) : false;
  }

  private setSession(response: LoginResponse): void {
    const session: Session = {
      user: response.user,
      token: response.token,
      refreshToken: response.refreshToken,
      isAuthenticated: true,
    };

    this.sessionSignal.set(session);
    this.isAuthenticated.set(true);
    this.currentUser.set(response.user);

    localStorage.setItem('token', response.token);
    localStorage.setItem('refreshToken', response.refreshToken);
    localStorage.setItem('user', JSON.stringify(response.user));
  }

  private loadSessionFromStorage(): void {
    const token = localStorage.getItem('token');
    const refreshToken = localStorage.getItem('refreshToken');
    const userStr = localStorage.getItem('user');

    if (token && userStr) {
      try {
        const user = JSON.parse(userStr) as User;
        this.sessionSignal.set({
          user,
          token,
          refreshToken,
          isAuthenticated: true,
        });
        this.isAuthenticated.set(true);
        this.currentUser.set(user);
      } catch {
        this.clearSession();
      }
    }
  }

  private clearSession(): void {
    this.sessionSignal.set({
      user: null,
      token: null,
      refreshToken: null,
      isAuthenticated: false,
    });
    this.isAuthenticated.set(false);
    this.currentUser.set(null);

    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
  }
}
