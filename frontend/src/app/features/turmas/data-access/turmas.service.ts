import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../../../core/config/api.config';
import { Turma, CreateTurmaRequest, UpdateTurmaRequest } from '../models/turma.model';

export interface TurmaFilter {
  nome?: string;
  cursoId?: string;
  professorId?: string;
  status?: string;
  turno?: string;
  anoLetivo?: number;
  periodo?: number;
  page?: number;
  pageSize?: number;
}

@Injectable({ providedIn: 'root' })
export class TurmasService {
  private readonly http = inject(HttpClient);
  private readonly apiConfig = inject(API_CONFIG);
  private readonly baseUrl = `${this.apiConfig.baseUrl}/turmas`;

  getAll(filter?: TurmaFilter): Observable<any> {
    let params = new HttpParams();

    if (filter) {
      Object.entries(filter).forEach(([key, value]) => {
        if (value !== undefined && value !== null) {
          params = params.set(key, value.toString());
        }
      });
    }

    // Backend exige ao menos o ano letivo; define padrão para o ano corrente
    if (!params.has('anoLetivo')) {
      params = params.set('anoLetivo', new Date().getFullYear().toString());
    }

    return this.http.get<any>(this.baseUrl, { params });
  }

  getById(id: string): Observable<Turma> {
    return this.http.get<Turma>(`${this.baseUrl}/${id}`);
  }

  getByProfessor(professorId: string): Observable<Turma[]> {
    return this.http.get<Turma[]>(`${this.baseUrl}/professor/${professorId}`);
  }

  create(turma: CreateTurmaRequest): Observable<Turma> {
    return this.http.post<Turma>(this.baseUrl, turma);
  }

  update(id: string, turma: UpdateTurmaRequest): Observable<Turma> {
    return this.http.put<Turma>(`${this.baseUrl}/${id}`, turma);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  getAlunos(turmaId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/${turmaId}/alunos`);
  }

  getGradeHorarios(turmaId: string): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/${turmaId}/grade-horarios`);
  }

  toggleStatus(id: string): Observable<Turma> {
    return this.http.patch<Turma>(`${this.baseUrl}/${id}/toggle-status`, {});
  }

  confereConflito(
    professorId: string,
    diaSemana: string,
    horaInicioHHmm: string
  ): Observable<{ conflito: boolean }> {
    // Normaliza para HH:mm:ss
    const hhmmss = /:\\d{2}$/.test(horaInicioHHmm) ? horaInicioHHmm : `${horaInicioHHmm}:00`;
    const params = new HttpParams()
      .set('professorId', professorId)
      .set('diaSemana', diaSemana)
      .set('horaInicio', hhmmss);
    return this.http.get<{ conflito: boolean }>(`${this.baseUrl}/confere-conflito`, { params });
  }
}
