export interface ConfiguracaoAvaliacao {
  id: string;
  turmaId: string;
  nome: string;
  peso: number;
  dataAvaliacao?: string;
  tipoAvaliacao: TipoAvaliacao;
  valorMaximo: number;
  ativa: boolean;
}

export type TipoAvaliacao =
  | 'Prova'
  | 'Trabalho'
  | 'Participacao'
  | 'Projeto'
  | 'Outro';

export interface Nota {
  id: string;
  avaliacaoId: string;
  alunoId: string;
  valor: number;
  observacao?: string;
  dataLancamento?: string;
  lancadoPor?: string;
  avaliacao?: ConfiguracaoAvaliacao;
  aluno?: {
    id: string;
    nome: string;
  };
}

export interface CreateNotaRequest {
  avaliacaoId: string;
  notas: {
    alunoId: string;
    valor: number;
    observacao?: string;
  }[];
}

export interface BoletimAluno {
  alunoId: string;
  alunoNome: string;
  turmaId: string;
  turmaNome: string;
  notas: NotaBoletim[];
  mediaFinal: number;
  frequencia: number;
  situacao: SituacaoAluno;
}

export interface NotaBoletim {
  avaliacaoNome: string;
  peso: number;
  valorMaximo: number;
  valorObtido: number;
  percentual: number;
}

export type SituacaoAluno =
  | 'Aprovado'
  | 'Reprovado'
  | 'EmAndamento'
  | 'Recuperacao';
