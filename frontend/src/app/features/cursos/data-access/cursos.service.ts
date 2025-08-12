import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { API_CONFIG } from '../../../core/config/api.config';
import { Curso, CreateCursoRequest, UpdateCursoRequest } from '../models/curso.model';

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

    return this.http.get<any>(this.baseUrl, { params }).pipe(map(mapPaged));
  }

  getById(id: string): Observable<Curso> {
    return this.http.get<any>(`${this.baseUrl}/${id}`).pipe(map(mapCurso));
  }

  create(curso: CreateCursoRequest): Observable<Curso> {
    const payload = toBackend(curso);
    return this.http.post<any>(this.baseUrl, payload).pipe(map(mapCurso));
  }

  update(id: string, curso: UpdateCursoRequest): Observable<Curso> {
    const payload = toBackend(curso);
    return this.http.put<any>(`${this.baseUrl}/${id}`, payload).pipe(map(mapCurso));
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  toggleStatus(id: string): Observable<Curso> {
    return this.http.patch<any>(`${this.baseUrl}/${id}/toggle-status`, {}).pipe(map(mapCurso));
  }

  existsByNome(nome: string): Observable<boolean> {
    return this.getAll({ nome, page: 0, pageSize: 1 }).pipe(
      map(res => !!res.items[0] && res.items[0].nome.toLowerCase() === nome.toLowerCase())
    );
  }
}

// Mappers backend <-> frontend
function mapCurso(dto: any): Curso {
  return {
    id: dto.Id ?? dto.id,
    nome: dto.Nome ?? dto.nome,
    descricao: dto.Descricao ?? dto.descricao,
    valorMensalidade: dto.ValorMensalidade ?? dto.valorMensalidade,
    cargaHoraria: dto.CargaHoraria ?? dto.cargaHoraria ?? 0,
    ativo: dto.Ativo ?? dto.ativo ?? true,
    createdAt: dto.CreatedAt ?? dto.createdAt,
    updatedAt: dto.UpdatedAt ?? dto.updatedAt,
  } as Curso;
}

function mapPaged(resp: any): PagedResult<Curso> {
  const items = (resp.Items ?? resp.items ?? []).map(mapCurso);
  return {
    items,
    total: resp.TotalCount ?? resp.total ?? items.length,
    page: resp.Page ?? resp.page ?? 0,
    pageSize: resp.PageSize ?? resp.pageSize ?? items.length,
  } as PagedResult<Curso>;
}

function toBackend(data: any): any {
  return {
    Nome: data.nome ?? data.Nome,
    Descricao: data.descricao ?? data.Descricao,
    ValorMensalidade: data.valorMensalidade ?? data.ValorMensalidade,
    CargaHoraria: data.cargaHoraria ?? data.CargaHoraria,
    Ativo: data.ativo ?? data.Ativo,
  };
}
