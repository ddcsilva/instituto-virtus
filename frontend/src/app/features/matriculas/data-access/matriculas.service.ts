import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../../../core/config/api.config';
import {
  Matricula,
  CreateMatriculaRequest,
  UpdateMatriculaRequest,
} from '../models/matricula.model';

export interface MatriculaFilter {
  alunoId?: string;
  turmaId?: string;
  status?: string;
  page?: number;
  pageSize?: number;
}

@Injectable({ providedIn: 'root' })
export class MatriculasService {
  private readonly http = inject(HttpClient);
  private readonly apiConfig = inject(API_CONFIG);
  private readonly baseUrl = `${this.apiConfig.baseUrl}/matriculas`;

  getAll(filter?: MatriculaFilter): Observable<any> {
    let params = new HttpParams();

    if (filter) {
      Object.entries(filter).forEach(([key, value]) => {
        if (value !== undefined && value !== null) {
          params = params.set(key, value.toString());
        }
      });
    }

    return this.http.get<any>(this.baseUrl, { params });
  }

  getById(id: string): Observable<Matricula> {
    return this.http.get<Matricula>(`${this.baseUrl}/${id}`);
  }

  getByAluno(alunoId: string): Observable<Matricula[]> {
    return this.http.get<Matricula[]>(`${this.baseUrl}/aluno/${alunoId}`);
  }

  create(matricula: CreateMatriculaRequest): Observable<Matricula> {
    // Converte para PascalCase aceito pelo backend
    const payload: any = {
      AlunoId: matricula.alunoId,
      TurmaId: matricula.turmaId,
      ...(matricula.mesesQuantidade ? { MesesQuantidade: matricula.mesesQuantidade } : {}),
      ...(matricula.diaVencimento ? { DiaVencimento: matricula.diaVencimento } : {}),
    };
    return this.http.post<Matricula>(this.baseUrl, payload);
  }

  update(id: string, matricula: UpdateMatriculaRequest): Observable<Matricula> {
    return this.http.put<Matricula>(`${this.baseUrl}/${id}`, matricula);
  }

  trancar(id: string, motivo: string): Observable<Matricula> {
    return this.http.post<Matricula>(`${this.baseUrl}/${id}/trancar`, {
      motivo,
    });
  }

  cancelar(id: string, motivo: string): Observable<Matricula> {
    return this.http.post<Matricula>(`${this.baseUrl}/${id}/cancelar`, {
      motivo,
    });
  }

  reativar(id: string): Observable<Matricula> {
    return this.http.post<Matricula>(`${this.baseUrl}/${id}/reativar`, {});
  }

  // Backend já gera mensalidades na criação da matrícula; método desnecessário
}
