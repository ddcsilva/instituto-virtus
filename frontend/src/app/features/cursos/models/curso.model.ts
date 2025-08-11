export interface Curso {
  id: string;
  nome: string;
  descricao: string;
  tipo: TipoCurso;
  duracao: number; // em meses
  cargaHoraria: number; // em horas
  valor: number;
  ativo: boolean;
  createdAt?: string;
  updatedAt?: string;
}

export type TipoCurso = 'Teologia' | 'Musica' | 'Outro';

export interface CreateCursoRequest {
  nome: string;
  descricao: string;
  tipo: TipoCurso;
  duracao: number;
  cargaHoraria: number;
  valor: number;
}

export interface UpdateCursoRequest extends CreateCursoRequest {
  ativo: boolean;
}
