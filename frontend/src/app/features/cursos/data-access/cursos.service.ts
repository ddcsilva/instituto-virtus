import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../../../core/config/api.config';
import {
  Curso,
  CreateCursoRequest,
  UpdateCursoRequest,
} from '../models/curso.model';

export interface CursoFilter {
  nome?: string;
  tipo?: string;
  ativo?: boolean;
  page?: number;
  pageSize?: number;
}

export interface PagedResult<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}

@Injectable({ providedIn: 'root' })
export class CursosService {
  private readonly http = inject(HttpClient);
  private readonly apiConfig = inject(API_CONFIG);
  private readonly baseUrl = `${this.apiConfig.baseUrl}/cursos`;

  getAll(filter?: CursoFilter): Observable<PagedResult<Curso>> {
    let params = new HttpParams();

    if (filter) {
      Object.entries(filter).forEach(([key, value]) => {
        if (value !== undefined && value !== null) {
          params = params.set(key, value.toString());
        }
      });
    }

    return this.http.get<PagedResult<Curso>>(this.baseUrl, { params });
  }

  getById(id: string): Observable<Curso> {
    return this.http.get<Curso>(`${this.baseUrl}/${id}`);
  }

  create(curso: CreateCursoRequest): Observable<Curso> {
    return this.http.post<Curso>(this.baseUrl, curso);
  }

  update(id: string, curso: UpdateCursoRequest): Observable<Curso> {
    return this.http.put<Curso>(`${this.baseUrl}/${id}`, curso);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  toggleStatus(id: string): Observable<Curso> {
    return this.http.patch<Curso>(`${this.baseUrl}/${id}/toggle-status`, {});
  }
}
