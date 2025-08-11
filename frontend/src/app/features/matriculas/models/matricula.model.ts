export interface Matricula {
  id: string;
  alunoId: string;
  turmaId: string;
  dataMatricula: string;
  status: StatusMatricula;
  valorMensalidade: number;
  descontoPercentual?: number;
  valorComDesconto: number;
  observacao?: string;
  aluno?: {
    id: string;
    nome: string;
    cpf: string;
  };
  turma?: {
    id: string;
    nome: string;
    curso: {
      id: string;
      nome: string;
    };
  };
}

export type StatusMatricula = 'Ativa' | 'Trancada' | 'Cancelada' | 'Concluida';

export interface CreateMatriculaRequest {
  alunoId: string;
  turmaId: string;
  dataMatricula: string;
  valorMensalidade: number;
  descontoPercentual?: number;
  observacao?: string;
}

export interface UpdateMatriculaRequest {
  status: StatusMatricula;
  valorMensalidade?: number;
  descontoPercentual?: number;
  observacao?: string;
}
