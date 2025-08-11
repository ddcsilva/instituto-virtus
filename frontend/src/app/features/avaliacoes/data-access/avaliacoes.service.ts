import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../../../core/config/api.config';
import {
  ConfiguracaoAvaliacao,
  Nota,
  CreateNotaRequest,
  BoletimAluno,
} from '../models/avaliacao.model';

@Injectable({ providedIn: 'root' })
export class AvaliacoesService {
  private readonly http = inject(HttpClient);
  private readonly apiConfig = inject(API_CONFIG);
  private readonly baseUrl = `${this.apiConfig.baseUrl}/avaliacoes`;

  // Configuração de Avaliações
  getConfiguracoesByTurma(
    turmaId: string
  ): Observable<ConfiguracaoAvaliacao[]> {
    return this.http.get<ConfiguracaoAvaliacao[]>(
      `${this.baseUrl}/turmas/${turmaId}/configuracoes`
    );
  }

  createConfiguracao(
    config: Partial<ConfiguracaoAvaliacao>
  ): Observable<ConfiguracaoAvaliacao> {
    return this.http.post<ConfiguracaoAvaliacao>(
      `${this.baseUrl}/configuracoes`,
      config
    );
  }

  updateConfiguracao(
    id: string,
    config: Partial<ConfiguracaoAvaliacao>
  ): Observable<ConfiguracaoAvaliacao> {
    return this.http.put<ConfiguracaoAvaliacao>(
      `${this.baseUrl}/configuracoes/${id}`,
      config
    );
  }

  // Notas
  getNotasByAvaliacao(avaliacaoId: string): Observable<Nota[]> {
    return this.http.get<Nota[]>(`${this.baseUrl}/${avaliacaoId}/notas`);
  }

  getNotasByAluno(alunoId: string, turmaId?: string): Observable<Nota[]> {
    let params = new HttpParams();
    if (turmaId) params = params.set('turmaId', turmaId);

    return this.http.get<Nota[]>(`${this.baseUrl}/alunos/${alunoId}/notas`, {
      params,
    });
  }

  lancarNotas(request: CreateNotaRequest): Observable<Nota[]> {
    return this.http.post<Nota[]>(`${this.baseUrl}/lancar`, request);
  }

  updateNota(id: string, valor: number, observacao?: string): Observable<Nota> {
    return this.http.patch<Nota>(`${this.baseUrl}/notas/${id}`, {
      valor,
      observacao,
    });
  }

  // Boletim
  getBoletimAluno(alunoId: string, turmaId: string): Observable<BoletimAluno> {
    return this.http.get<BoletimAluno>(
      `${this.baseUrl}/boletim/${alunoId}/${turmaId}`
    );
  }

  getBoletinsTurma(turmaId: string): Observable<BoletimAluno[]> {
    return this.http.get<BoletimAluno[]>(
      `${this.baseUrl}/turmas/${turmaId}/boletins`
    );
  }
}
