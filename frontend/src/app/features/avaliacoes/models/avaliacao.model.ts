export interface Avaliacao {
  id: string;
  turmaId: string;
  turmaNome: string;
  nome: string;
  peso: number;
  dataAplicacao?: string;
  descricao?: string;
}

export interface CreateAvaliacaoRequest {
  turmaId: string;
  nome: string;
  peso: number;
  dataAplicacao?: string;
  descricao?: string;
}

export interface Nota {
  id: string;
  avaliacaoId: string;
  avaliacaoNome: string;
  alunoId: string;
  alunoNome: string;
  valor: number;
  observacoes?: string;
}

export interface LancarNotasRequest {
  notas: {
    alunoId: string;
    valor: number;
    observacoes?: string;
  }[];
}
