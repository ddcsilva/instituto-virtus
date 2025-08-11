import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../../../core/config/api.config';
import {
  Pessoa,
  CreatePessoaRequest,
  UpdatePessoaRequest,
  ResponsavelAluno,
  VinculoResponsavelRequest,
} from '../models/pessoa.model';

export interface PessoaFilter {
  nome?: string;
  cpf?: string;
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
export class PessoasService {
  private readonly http = inject(HttpClient);
  private readonly apiConfig = inject(API_CONFIG);
  private readonly baseUrl = `${this.apiConfig.baseUrl}/pessoas`;

  getAll(filter?: PessoaFilter): Observable<PagedResult<Pessoa>> {
    let params = new HttpParams();

    if (filter) {
      Object.entries(filter).forEach(([key, value]) => {
        if (value !== undefined && value !== null) {
          params = params.set(key, value.toString());
        }
      });
    }

    return this.http.get<PagedResult<Pessoa>>(this.baseUrl, { params });
  }

  getById(id: string): Observable<Pessoa> {
    return this.http.get<Pessoa>(`${this.baseUrl}/${id}`);
  }

  create(pessoa: CreatePessoaRequest): Observable<Pessoa> {
    return this.http.post<Pessoa>(this.baseUrl, pessoa);
  }

  update(id: string, pessoa: UpdatePessoaRequest): Observable<Pessoa> {
    return this.http.put<Pessoa>(`${this.baseUrl}/${id}`, pessoa);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  toggleStatus(id: string): Observable<Pessoa> {
    return this.http.patch<Pessoa>(`${this.baseUrl}/${id}/toggle-status`, {});
  }

  getVinculos(responsavelId: string): Observable<ResponsavelAluno[]> {
    return this.http.get<ResponsavelAluno[]>(
      `${this.baseUrl}/${responsavelId}/vinculos`
    );
  }

  vincularAlunos(
    request: VinculoResponsavelRequest
  ): Observable<ResponsavelAluno[]> {
    return this.http.post<ResponsavelAluno[]>(
      `${this.baseUrl}/vincular`,
      request
    );
  }

  desvincularAluno(responsavelId: string, alunoId: string): Observable<void> {
    return this.http.delete<void>(
      `${this.baseUrl}/${responsavelId}/vinculos/${alunoId}`
    );
  }
}
