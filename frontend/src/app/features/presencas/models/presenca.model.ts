export interface Aula {
  id: string;
  turmaId: string;
  data: string;
  horaInicio: string;
  horaFim: string;
  conteudo?: string;
  observacao?: string;
  turma?: {
    id: string;
    nome: string;
    curso: {
      id: string;
      nome: string;
    };
  };
}

export interface Presenca {
  id: string;
  aulaId: string;
  alunoId: string;
  status: StatusPresenca;
  observacao?: string;
  registradoPor?: string;
  dataRegistro?: string;
  aula?: Aula;
  aluno?: {
    id: string;
    nome: string;
  };
}

export type StatusPresenca = 'Presente' | 'Ausente' | 'Justificado';

export interface CreatePresencaRequest {
  aulaId: string;
  presencas: {
    alunoId: string;
    status: StatusPresenca;
    observacao?: string;
  }[];
}

export interface FrequenciaAluno {
  alunoId: string;
  alunoNome: string;
  totalAulas: number;
  presencas: number;
  ausencias: number;
  justificadas: number;
  percentualFrequencia: number;
}
