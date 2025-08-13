import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../../../core/config/api.config';
import { Aula, Presenca, CreatePresencaRequest, FrequenciaAluno } from '../models/presenca.model';

@Injectable({ providedIn: 'root' })
export class PresencasService {
  private readonly http = inject(HttpClient);
  private readonly apiConfig = inject(API_CONFIG);
  private readonly baseUrl = `${this.apiConfig.baseUrl}/presencas`;
  private readonly aulasUrl = `${this.apiConfig.baseUrl}/aulas`;

  // Aulas
  getAulas(turmaId: string, dataInicio?: string, dataFim?: string): Observable<Aula[]> {
    let params = new HttpParams();
    if (dataInicio) params = params.set('inicio', dataInicio);
    if (dataFim) params = params.set('fim', dataFim);
    return this.http.get<Aula[]>(`${this.aulasUrl}/turma/${turmaId}`, { params });
  }

  createAula(aula: Partial<Aula>): Observable<Aula> {
    const payload = {
      TurmaId: aula.turmaId,
      Data: aula.data,
      Conteudo: aula.conteudo,
    } as any;
    return this.http.post<Aula>(`${this.aulasUrl}`, payload);
  }

  // Presenças
  getPresencasByAula(aulaId: string): Observable<Presenca[]> {
    return this.http.get<Presenca[]>(`${this.baseUrl}/aula/${aulaId}`);
  }

  getPresencasByAluno(alunoId: string, turmaId?: string): Observable<Presenca[]> {
    let params = new HttpParams();
    if (turmaId) params = params.set('turmaId', turmaId);

    return this.http.get<Presenca[]>(`${this.baseUrl}/alunos/${alunoId}/presencas`, { params });
  }

  registrarPresencas(request: CreatePresencaRequest): Observable<Presenca[]> {
    return this.http.post<Presenca[]>(`${this.baseUrl}/registrar`, request);
  }

  updatePresenca(id: string, status: string, observacao?: string): Observable<Presenca> {
    return this.http.patch<Presenca>(`${this.baseUrl}/${id}`, {
      status,
      observacao,
    });
  }

  // Frequência
  getFrequenciaTurma(turmaId: string): Observable<FrequenciaAluno[]> {
    return this.http.get<FrequenciaAluno[]>(`${this.baseUrl}/turmas/${turmaId}/frequencia`);
  }

  getFrequenciaAluno(alunoId: string, turmaId: string): Observable<FrequenciaAluno> {
    return this.http.get<FrequenciaAluno>(
      `${this.baseUrl}/alunos/${alunoId}/frequencia/${turmaId}`
    );
  }
}
