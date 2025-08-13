export interface Matricula {
  id: string;
  alunoId: string;
  turmaId: string;
  dataMatricula: string;
  status: StatusMatricula;
  // Campos opcionais conforme disponibilidade do backend
  dataTrancamento?: string | null;
  dataConclusao?: string | null;
  motivoSaida?: string | null;
  aluno?: {
    id: string;
    nome: string;
    cpf?: string;
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
  mesesQuantidade?: number; // default 12 no backend
  diaVencimento?: number; // default 10 no backend
}

export interface UpdateMatriculaRequest {
  status: StatusMatricula;
  valorMensalidade?: number;
  descontoPercentual?: number;
  observacao?: string;
}
