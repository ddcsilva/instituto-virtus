import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';
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
  private readonly baseUrl = `${this.apiConfig.baseUrl}`;
  private readonly mensalidadesUrl = `${this.apiConfig.baseUrl}/mensalidades`;
  private readonly pagamentosUrl = `${this.apiConfig.baseUrl}/pagamentos`;

  // Mensalidades
  getMensalidades(
    filter?: MensalidadeFilter & { ano?: number; mes?: number; status?: string }
  ): Observable<PagedResult<Mensalidade>> {
    let params = new HttpParams();

    if (filter) {
      Object.entries(filter).forEach(([key, value]) => {
        if (value !== undefined && value !== null) {
          params = params.set(key, value.toString());
        }
      });
    }

    return this.http.get<PagedResult<Mensalidade>>(this.mensalidadesUrl, { params });
  }

  getMensalidadeById(id: string): Observable<Mensalidade> {
    return this.http.get<Mensalidade>(`${this.mensalidadesUrl}/${id}`);
  }

  getMensalidadesResponsavel(responsavelId: string): Observable<Mensalidade[]> {
    return this.http.get<Mensalidade[]>(`${this.mensalidadesUrl}/responsavel/${responsavelId}`);
  }

  // Opcional: gerar mensalidades (não há endpoint atual exposto no backend)

  cancelarMensalidadePagamento(id: string): Observable<any> {
    return this.http.put<any>(`${this.mensalidadesUrl}/${id}/cancelar-pagamento`, {});
  }

  registrarPagamentoMensalidade(
    id: string,
    payload: { meioPagamento: string; dataPagamento?: string; observacao?: string }
  ): Observable<any> {
    const body: any = {
      MeioPagamento: payload.meioPagamento,
      ...(payload.dataPagamento ? { DataPagamento: payload.dataPagamento } : {}),
      ...(payload.observacao ? { Observacao: payload.observacao } : {}),
    };
    return this.http.put<any>(`${this.mensalidadesUrl}/${id}/pagar`, body);
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

    return this.http.get<PagedResult<Pagamento>>(this.pagamentosUrl, {
      params,
    });
  }

  getPagamentoById(id: string): Observable<Pagamento> {
    return this.http.get<Pagamento>(`${this.pagamentosUrl}/${id}`);
  }

  criarPagamento(request: CreatePagamentoRequest): Observable<Pagamento> {
    return this.http.post<Pagamento>(this.pagamentosUrl, request);
  }

  estornarPagamento(id: string, motivo: string): Observable<void> {
    return this.http.post<void>(`${this.pagamentosUrl}/${id}/estornar`, {
      motivo,
    });
  }

  // Saldo
  // Stub enquanto não houver endpoint no backend
  getSaldoResponsavel(responsavelId: string): Observable<SaldoResponsavel> {
    return of({ responsavelId, saldo: 0, ultimaAtualizacao: new Date().toISOString() });
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
    // competencia no formato YYYY-MM → backend espera mes/ano separados
    const [anoStr, mesStr] = (competencia || '').split('-');
    const ano = Number(anoStr);
    const mes = Number(mesStr);
    const params = new HttpParams()
      .set('ano', String(ano || new Date().getFullYear()))
      .set('mes', String(mes || new Date().getMonth() + 1));
    return this.http.get<any[]>(`${this.apiConfig.baseUrl}/relatorios/inadimplentes`, { params });
  }

  getRecebimentosPeriodo(dataInicio: string, dataFim: string): Observable<any> {
    const params = new HttpParams().set('dataInicio', dataInicio).set('dataFim', dataFim);

    return this.http.get<any>(`${this.apiConfig.baseUrl}/relatorios/recebimentos`, {
      params,
    });
  }
}
