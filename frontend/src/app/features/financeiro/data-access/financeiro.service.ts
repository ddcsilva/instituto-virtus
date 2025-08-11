import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../../../core/config/api.config';
import {
  Mensalidade,
  Pagamento,
  CreatePagamentoRequest,
  SaldoResponsavel,
  ExtratoResponsavel,
  StatusMensalidade,
} from '../models/financeiro.model';

export interface MensalidadeFilter {
  responsavelId?: string;
  alunoId?: string;
  turmaId?: string;
  status?: StatusMensalidade;
  competenciaInicio?: string;
  competenciaFim?: string;
  vencimentoInicio?: string;
  vencimentoFim?: string;
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
export class FinanceiroService {
  private readonly http = inject(HttpClient);
  private readonly apiConfig = inject(API_CONFIG);
  private readonly baseUrl = `${this.apiConfig.baseUrl}/financeiro`;

  // Mensalidades
  getMensalidades(
    filter?: MensalidadeFilter
  ): Observable<PagedResult<Mensalidade>> {
    let params = new HttpParams();

    if (filter) {
      Object.entries(filter).forEach(([key, value]) => {
        if (value !== undefined && value !== null) {
          params = params.set(key, value.toString());
        }
      });
    }

    return this.http.get<PagedResult<Mensalidade>>(
      `${this.baseUrl}/mensalidades`,
      { params }
    );
  }

  getMensalidadeById(id: string): Observable<Mensalidade> {
    return this.http.get<Mensalidade>(`${this.baseUrl}/mensalidades/${id}`);
  }

  getMensalidadesResponsavel(
    responsavelId: string,
    abertas = true
  ): Observable<Mensalidade[]> {
    const params = new HttpParams().set('abertas', abertas.toString());
    return this.http.get<Mensalidade[]>(
      `${this.baseUrl}/responsaveis/${responsavelId}/mensalidades`,
      { params }
    );
  }

  gerarMensalidades(
    matriculaId: string,
    meses: number
  ): Observable<Mensalidade[]> {
    return this.http.post<Mensalidade[]>(`${this.baseUrl}/mensalidades/gerar`, {
      matriculaId,
      meses,
    });
  }

  cancelarMensalidade(id: string, motivo: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/mensalidades/${id}/cancelar`, {
      motivo,
    });
  }

  // Pagamentos
  getPagamentos(filter?: {
    responsavelId?: string;
    dataInicio?: string;
    dataFim?: string;
  }): Observable<PagedResult<Pagamento>> {
    let params = new HttpParams();

    if (filter) {
      Object.entries(filter).forEach(([key, value]) => {
        if (value) params = params.set(key, value.toString());
      });
    }

    return this.http.get<PagedResult<Pagamento>>(`${this.baseUrl}/pagamentos`, {
      params,
    });
  }

  getPagamentoById(id: string): Observable<Pagamento> {
    return this.http.get<Pagamento>(`${this.baseUrl}/pagamentos/${id}`);
  }

  criarPagamento(request: CreatePagamentoRequest): Observable<Pagamento> {
    return this.http.post<Pagamento>(`${this.baseUrl}/pagamentos`, request);
  }

  estornarPagamento(id: string, motivo: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/pagamentos/${id}/estornar`, {
      motivo,
    });
  }

  // Saldo
  getSaldoResponsavel(responsavelId: string): Observable<SaldoResponsavel> {
    return this.http.get<SaldoResponsavel>(
      `${this.baseUrl}/responsaveis/${responsavelId}/saldo`
    );
  }

  getExtratoResponsavel(
    responsavelId: string,
    dataInicio?: string,
    dataFim?: string
  ): Observable<ExtratoResponsavel> {
    let params = new HttpParams();
    if (dataInicio) params = params.set('dataInicio', dataInicio);
    if (dataFim) params = params.set('dataFim', dataFim);

    return this.http.get<ExtratoResponsavel>(
      `${this.baseUrl}/responsaveis/${responsavelId}/extrato`,
      { params }
    );
  }

  // Relatórios
  getInadimplentes(competencia: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/relatorios/inadimplentes`, {
      params: new HttpParams().set('competencia', competencia),
    });
  }

  getRecebimentosPeriodo(dataInicio: string, dataFim: string): Observable<any> {
    const params = new HttpParams()
      .set('dataInicio', dataInicio)
      .set('dataFim', dataFim);

    return this.http.get<any>(`${this.baseUrl}/relatorios/recebimentos`, {
      params,
    });
  }
}
