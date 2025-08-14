export interface Pessoa {
  id: string;
  nome: string;
  cpf: string;
  dataNascimento: string;
  telefone: string;
  email?: string;
  tipo: TipoPessoa;
  ativo: boolean;
  temAcesso?: boolean;
  createdAt?: string;
  updatedAt?: string;
}

export type TipoPessoa = 'Aluno' | 'Responsavel' | 'Professor';

export interface CreatePessoaRequest {
  nome: string;
  cpf: string;
  dataNascimento: string;
  telefone: string;
  email?: string;
  tipo: TipoPessoa;
}

export interface UpdatePessoaRequest extends CreatePessoaRequest {
  ativo: boolean;
}

export interface ResponsavelAluno {
  id: string;
  responsavelId: string;
  alunoId: string;
  responsavel?: Pessoa;
  aluno?: Pessoa;
}

export interface VinculoResponsavelRequest {
  responsavelId: string;
  alunoIds: string[];
  parentesco?: string;
  principal?: boolean;
}
