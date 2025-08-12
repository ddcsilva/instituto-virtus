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
    const body = toBackendCreatePayload(pessoa);
    return this.http.post<any>(this.baseUrl, body).pipe(map(dto => mapPessoa(dto)));
  }

  update(id: string, pessoa: UpdatePessoaRequest): Observable<Pessoa> {
    const body = toBackendUpdatePayload(pessoa);
    return this.http.put<any>(`${this.baseUrl}/${id}`, body).pipe(map(dto => mapPessoa(dto)));
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  toggleStatus(id: string): Observable<Pessoa> {
    return this.http
      .patch<any>(`${this.baseUrl}/${id}/toggle-status`, {})
      .pipe(map(dto => mapPessoa(dto)));
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
  Cpf?: string;
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
    cpf: (dto as any).cpf ?? dto.Cpf ?? '',
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

// Payload mappers (front -> backend PascalCase)
function digitsOnly(value: string | undefined): string | undefined {
  return value ? value.replace(/\D/g, '') : undefined;
}

function toISODate(value: string | Date): string {
  const d = value instanceof Date ? value : new Date(value);
  // yyyy-MM-dd para evitar timezone shift no backend
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(
    d.getDate()
  ).padStart(2, '0')}`;
}

function toBackendCreatePayload(p: CreatePessoaRequest): any {
  return {
    NomeCompleto: (p as any).nome ?? (p as any).NomeCompleto ?? '',
    Cpf: digitsOnly((p as any).cpf ?? (p as any).Cpf),
    Telefone: digitsOnly((p as any).telefone ?? (p as any).Telefone) ?? '',
    Email: (p as any).email ?? (p as any).Email,
    DataNascimento: toISODate((p as any).dataNascimento ?? (p as any).DataNascimento),
    TipoPessoa: (p as any).tipo ?? (p as any).TipoPessoa,
    Observacoes: (p as any).observacoes ?? (p as any).Observacoes,
  };
}

function toBackendUpdatePayload(p: UpdatePessoaRequest): any {
  const base = toBackendCreatePayload(p as any);
  return {
    ...base,
    Ativo: (p as any).ativo ?? (p as any).Ativo,
  };
}
