import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../../../core/config/api.config';
import { Avaliacao, Nota, LancarNotasRequest } from '../models/avaliacao.model';

@Injectable({ providedIn: 'root' })
export class AvaliacoesService {
  private readonly http = inject(HttpClient);
  private readonly apiConfig = inject(API_CONFIG);
  private readonly baseUrl = `${this.apiConfig.baseUrl}/avaliacoes`;

  // Avaliações
  getByTurma(turmaId: string): Observable<Avaliacao[]> {
    return this.http.get<Avaliacao[]>(`${this.baseUrl}/turma/${turmaId}`);
  }

  getById(id: string): Observable<Avaliacao> {
    return this.http.get<Avaliacao>(`${this.baseUrl}/${id}`);
  }

  create(avaliacao: Partial<Avaliacao>): Observable<Avaliacao> {
    const payload: any = {
      TurmaId: avaliacao.turmaId,
      Nome: avaliacao.nome,
      Peso: avaliacao.peso,
      DataAplicacao: avaliacao.dataAplicacao,
      Descricao: avaliacao.descricao,
    };
    return this.http.post<Avaliacao>(`${this.baseUrl}`, payload);
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

  lancarNotas(avaliacaoId: string, request: LancarNotasRequest): Observable<any> {
    const payload = {
      Notas: (request.notas || []).map(n => ({
        AlunoId: n.alunoId,
        Valor: n.valor,
        Observacoes: n.observacoes,
      })),
    };
    return this.http.post(`${this.baseUrl}/${avaliacaoId}/notas`, payload);
  }

  updateNota(id: string, valor: number, observacoes?: string): Observable<Nota> {
    return this.http.patch<Nota>(`${this.baseUrl}/notas/${id}`, {
      Valor: valor,
      Observacoes: observacoes,
    });
  }

  // Boletins (poderão ser reintroduzidos depois conforme backend)
}
