export interface Curso {
  id: string;
  nome: string;
  descricao?: string;
  valorMensalidade: number;
  cargaHoraria: number;
  ativo: boolean;
  createdAt?: string;
  updatedAt?: string;
}

export interface CreateCursoRequest {
  nome: string;
  descricao?: string;
  cargaHoraria: number;
  valorMensalidade: number;
}

export interface UpdateCursoRequest extends CreateCursoRequest {
  ativo: boolean;
}
