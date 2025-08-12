import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { API_CONFIG } from '../../../core/config/api.config';
import {
  Pessoa,
  CreatePessoaRequest,
  UpdatePessoaRequest,
  ResponsavelAluno,
  VinculoResponsavelRequest,
  TipoPessoa,
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

    return this.http.get<any>(this.baseUrl, { params }).pipe(map(resp => mapPagedResult(resp)));
  }

  getById(id: string): Observable<Pessoa> {
    return this.http.get<any>(`${this.baseUrl}/${id}`).pipe(map(dto => mapPessoa(dto)));
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
    return this.http.get<ResponsavelAluno[]>(`${this.baseUrl}/${responsavelId}/vinculos`);
  }

  vincularAlunos(request: VinculoResponsavelRequest): Observable<ResponsavelAluno[]> {
    return this.http.post<ResponsavelAluno[]>(`${this.baseUrl}/vincular`, request);
  }

  desvincularAluno(responsavelId: string, alunoId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${responsavelId}/vinculos/${alunoId}`);
  }
}

// Helpers de mapeamento para compatibilizar com o backend (.NET)
export function mapTipo(dtoTipo: string | undefined): TipoPessoa {
  if (dtoTipo === 'Aluno' || dtoTipo === 'Responsavel' || dtoTipo === 'Professor') return dtoTipo;
  return 'Aluno';
}

// Observação: o backend está configurado sem NamingPolicy, então as chaves vêm em PascalCase
export interface PessoaDtoBackend {
  Id: string;
  NomeCompleto: string;
  Telefone: string;
  Email?: string;
  DataNascimento: string;
  TipoPessoa: string;
  Observacoes?: string;
  Ativo: boolean;
}

export interface PagedResultBackend<T> {
  Items?: T[];
  items?: T[];
  TotalCount?: number;
  total?: number;
  PageNumber?: number;
  page?: number;
  PageSize?: number;
  pageSize?: number;
}

export function mapPessoa(dto: PessoaDtoBackend): Pessoa {
  return {
    id: (dto as any).id ?? dto.Id,
    nome: (dto as any).nomeCompleto ?? dto.NomeCompleto,
    cpf: '',
    dataNascimento: (dto as any).dataNascimento ?? dto.DataNascimento,
    telefone: (dto as any).telefone ?? dto.Telefone,
    email: (dto as any).email ?? dto.Email,
    tipo: mapTipo((dto as any).tipoPessoa ?? dto.TipoPessoa),
    ativo: (dto as any).ativo ?? dto.Ativo,
    createdAt: undefined,
    updatedAt: undefined,
  };
}

export function mapPagedResult(resp: PagedResultBackend<PessoaDtoBackend>): PagedResult<Pessoa> {
  const items = (resp.items ?? resp.Items ?? []).map(mapPessoa);
  const total = resp.total ?? resp.TotalCount ?? items.length;
  const page = resp.page ?? (resp.PageNumber ?? 1) - 1;
  const pageSize = resp.pageSize ?? resp.PageSize ?? items.length;
  return { items, total, page, pageSize };
}
