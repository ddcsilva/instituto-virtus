import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../../../core/config/api.config';
import {
  Aula,
  Presenca,
  CreatePresencaRequest,
  FrequenciaAluno,
} from '../models/presenca.model';

@Injectable({ providedIn: 'root' })
export class PresencasService {
  private readonly http = inject(HttpClient);
  private readonly apiConfig = inject(API_CONFIG);
  private readonly baseUrl = `${this.apiConfig.baseUrl}/presencas`;

  // Aulas
  getAulas(
    turmaId: string,
    dataInicio?: string,
    dataFim?: string
  ): Observable<Aula[]> {
    let params = new HttpParams().set('turmaId', turmaId);
    if (dataInicio) params = params.set('dataInicio', dataInicio);
    if (dataFim) params = params.set('dataFim', dataFim);

    return this.http.get<Aula[]>(`${this.baseUrl}/aulas`, { params });
  }

  createAula(aula: Partial<Aula>): Observable<Aula> {
    return this.http.post<Aula>(`${this.baseUrl}/aulas`, aula);
  }

  // Presenças
  getPresencasByAula(aulaId: string): Observable<Presenca[]> {
    return this.http.get<Presenca[]>(
      `${this.baseUrl}/aulas/${aulaId}/presencas`
    );
  }

  getPresencasByAluno(
    alunoId: string,
    turmaId?: string
  ): Observable<Presenca[]> {
    let params = new HttpParams();
    if (turmaId) params = params.set('turmaId', turmaId);

    return this.http.get<Presenca[]>(
      `${this.baseUrl}/alunos/${alunoId}/presencas`,
      { params }
    );
  }

  registrarPresencas(request: CreatePresencaRequest): Observable<Presenca[]> {
    return this.http.post<Presenca[]>(`${this.baseUrl}/registrar`, request);
  }

  updatePresenca(
    id: string,
    status: string,
    observacao?: string
  ): Observable<Presenca> {
    return this.http.patch<Presenca>(`${this.baseUrl}/${id}`, {
      status,
      observacao,
    });
  }

  // Frequência
  getFrequenciaTurma(turmaId: string): Observable<FrequenciaAluno[]> {
    return this.http.get<FrequenciaAluno[]>(
      `${this.baseUrl}/turmas/${turmaId}/frequencia`
    );
  }

  getFrequenciaAluno(
    alunoId: string,
    turmaId: string
  ): Observable<FrequenciaAluno> {
    return this.http.get<FrequenciaAluno>(
      `${this.baseUrl}/alunos/${alunoId}/frequencia/${turmaId}`
    );
  }
}
